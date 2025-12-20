using System.Text.Json.Serialization;

namespace RamPressureAnalyzer.Models
{
    // JSON 구조:
    // { "TargetFile": "...", "Stats": { ... }, "Result": { ... } }

    public class AnalysisResult
    {
        [JsonPropertyName("TargetFile")]
        public string TargetFile { get; set; }

        [JsonPropertyName("TotalSamples")]
        public int TotalSamples { get; set; }

        [JsonPropertyName("Stats")]
        public StatsData Stats { get; set; }

        [JsonPropertyName("Result")]
        public VerdictData Result { get; set; }

        [JsonPropertyName("error")]
        public string ErrorMessage { get; set; }
    }

    public class StatsData
    {
        public double AvgAvailableMB { get; set; }
        public double MinAvailableMB { get; set; }
        public double MaxPageFileUsage { get; set; }
        public double AvgPagesPerSec { get; set; }
    }

    public class VerdictData
    {
        public string Status { get; set; }   // NORMAL, WARNING, CRITICAL
        public string Message { get; set; }
    }

}