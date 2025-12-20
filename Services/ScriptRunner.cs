using System.Diagnostics;
using System.IO;
using System.Text.Json;
using RamPressureAnalyzer.Models;

namespace RamPressureAnalyzer.Services
{
    public class ScriptRunner
    {
        // 스크립트 폴더 경로
        private readonly string _scriptDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "scripts");

        public void StartLogging()
        {
            // ... (기존과 동일)
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(_scriptDir, "start_log.bat"),
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                Process.Start(psi);
                Debug.WriteLine($"[ScriptRunner] StartLogging executed: {psi.FileName}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ScriptRunner] StartLogging Failed: {ex.Message}");
            }
        }

        public void StopLogging()
        {
            // ... (기존과 동일)
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(_scriptDir, "stop_log.bat"),
                UseShellExecute = true,
                Verb = "runas",
                WindowStyle = ProcessWindowStyle.Hidden
            };
            try
            {
                Process.Start(psi)?.WaitForExit();
                Debug.WriteLine($"[ScriptRunner] StopLogging executed.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ScriptRunner] StopLogging Failed: {ex.Message}");
            }
        }

        public async Task<AnalysisResult?> AnalyzeAsync()
        {
            var psScript = Path.Combine(_scriptDir, "analyze.ps1");
            var logDir = Path.Combine(_scriptDir, "logs"); // 로그 폴더 절대 경로 계산

            // 디버깅: 경로 확인
            Debug.WriteLine($"[ScriptRunner] Script Path: {psScript}");
            Debug.WriteLine($"[ScriptRunner] Log Path: {logDir}");

            // 로그 폴더가 실제로 있는지 확인
            if (!Directory.Exists(logDir))
            {
                Debug.WriteLine("[Error] Log directory does not exist yet.");
                return null;
            }

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                // [수정 핵심 1] -LogDir 파라미터로 절대 경로를 직접 전달합니다.
                Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"{psScript}\" -LogDir \"{logDir}\"",

                // [수정 핵심 2] 작업 디렉터리를 scripts 폴더로 강제 고정합니다.
                WorkingDirectory = _scriptDir,

                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                StandardOutputEncoding = System.Text.Encoding.UTF8
            };

            try
            {
                using var process = Process.Start(psi);
                if (process == null) return null;

                var jsonOutputTask = process.StandardOutput.ReadToEndAsync();
                var errorOutputTask = process.StandardError.ReadToEndAsync();

                await Task.WhenAll(jsonOutputTask, errorOutputTask);
                await process.WaitForExitAsync();

                var jsonOutput = jsonOutputTask.Result;
                var errorOutput = errorOutputTask.Result;

                // 에러가 있다면 디버그 창에 출력
                if (!string.IsNullOrWhiteSpace(errorOutput))
                {
                    Debug.WriteLine($"[PowerShell Error] {errorOutput}");
                }

                Debug.WriteLine($"[PowerShell Output] {jsonOutput}");

                if (string.IsNullOrWhiteSpace(jsonOutput)) return null;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<AnalysisResult>(jsonOutput, options);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[Exception] {ex.Message}");
                return null;
            }
        }
    }
}