<#
.SYNOPSIS
    Analyzes the latest logman CSV file and outputs system memory status in JSON.
    (English Output Version to prevent encoding issues)
#>

param (
    [string]$LogDir = ".\logs" 
)

# 1. Find the latest CSV file
$latestCsv = Get-ChildItem -Path $LogDir -Filter "*.csv" | Sort-Object LastWriteTime -Descending | Select-Object -First 1

if ($null -eq $latestCsv) {
    Write-Output "{ ""error"": ""No log files found."" }"
    exit
}

# 2. Load CSV data
$data = Import-Csv -Path $latestCsv.FullName

if ($data.Count -eq 0) {
    Write-Output "{ ""error"": ""Log file is empty."" }"
    exit
}

# 3. Dynamic Header Mapping
# Finds headers that contain specific keywords (handling \\COMPUTERNAME prefix)
$headers = $data[0].PSObject.Properties.Name

$colAvail = $headers | Where-Object { $_ -like "*Available MBytes" } | Select-Object -First 1
$colPage  = $headers | Where-Object { $_ -like "*Paging File*% Usage" } | Select-Object -First 1
$colFault = $headers | Where-Object { $_ -like "*Pages/sec" } | Select-Object -First 1

# 4. Helper Function: Clean and Measure Data
function Get-Stat ($columnName) {
    if ([string]::IsNullOrEmpty($columnName)) { return $null }
    
    # Force convert to Double, ignoring empty/null values
    $values = $data | ForEach-Object { 
        $val = $_."$columnName"
        if ($val -ne $null -and $val -ne "") {
            try { [double]$val } catch { 0 } 
        }
    }
    
    if ($values) {
        return $values | Measure-Object -Average -Maximum -Minimum
    }
    return $null
}

# Calculate Stats
$statAvail = Get-Stat $colAvail
$statPage  = Get-Stat $colPage
$statFault = Get-Stat $colFault

# Defaults to 0 if data is missing
$avgAvailMB    = if ($statAvail) { $statAvail.Average } else { 0 }
$minAvailMB    = if ($statAvail) { $statAvail.Minimum } else { 0 }
$maxPageFile   = if ($statPage)  { $statPage.Maximum }  else { 0 }
$avgPageFaults = if ($statFault) { $statFault.Average } else { 0 }


# 5. Verdict Logic (Thresholds)
$verdict = "NORMAL"
$recommendation = "Memory is sufficient."

# Condition A: Available RAM average is below 500MB
if ($avgAvailMB -lt 500) {
    $verdict = "CRITICAL"
    $recommendation = "Physical memory is critically low. Upgrade required."
}
# Condition B: Page File usage peak is above 80%
elseif ($maxPageFile -gt 80) {
    $verdict = "WARNING"
    $recommendation = "High virtual memory usage. Consider upgrading."
}
# Condition C: Hard Faults are frequent (> 50/sec)
elseif ($avgPageFaults -gt 50) {
    $verdict = "LAGGY"
    $recommendation = "Performance degradation detected due to swapping."
}

# 6. JSON Output Construction
$output = [PSCustomObject]@{
    TargetFile      = $latestCsv.Name
    TotalSamples    = $data.Count
    Stats = @{
        AvgAvailableMB   = [Math]::Round($avgAvailMB, 1)
        MinAvailableMB   = [Math]::Round($minAvailMB, 1)
        MaxPageFileUsage = [Math]::Round($maxPageFile, 1)
        AvgPagesPerSec   = [Math]::Round($avgPageFaults, 1)
    }
    Result = @{
        Status  = $verdict
        Message = $recommendation
    }
}

# Output valid JSON
$json = $output | ConvertTo-Json -Depth 3
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Write-Output $json