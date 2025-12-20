using System.Windows.Input;
using RamPressureAnalyzer.Services;
using RamPressureAnalyzer.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace RamPressureAnalyzer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ScriptRunner _runner = new ScriptRunner();

        [ObservableProperty]
        private string _statusMessage = "Ready to analyze.";

        [ObservableProperty]
        private string _analysisReport = "";

        [ObservableProperty]
        private bool _isRecording = false;

        [RelayCommand]
        public void StartLog()
        {
            try
            {
                _runner.StartLogging();
                IsRecording = true;
                StatusMessage = "🔴 Recording memory usage... (Do your work!)";
                AnalysisReport = "";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Error: {ex.Message}";
            }
        }

        [RelayCommand]
        public async Task StopAndAnalyze()
        {
            IsRecording = false;
            StatusMessage = "🟡 Stopping and Analyzing...";

            // 1. 로그 중지
            await Task.Run(() => _runner.StopLogging());

            // 2. 분석 실행
            var result = await _runner.AnalyzeAsync();

            if (result == null || result.Stats == null || result.Result == null)
            {
                StatusMessage = "❌ Analysis failed.";
                AnalysisReport = "로그 데이터가 없거나 분석에 실패했습니다.\n관리자 권한을 확인해주세요.";
                return;
            }

            StatusMessage = "🟢 Analysis Complete.";

            // 3. 한국어 번역 가져오기
            var (korStatus, korMsg) = TranslateResult(result.Result.Status);

            // 4. 결과 리포트 작성 (한/영 병기)
            AnalysisReport = $"""
                [판정 결과 (Verdict)]
                {result.Result.Status} / {korStatus}
                -------------------------------------
                {result.Result.Message}
                ({korMsg})

                [세부 데이터 (Details)]
                - 평균 가용 램 (Avg Available RAM): {result.Stats.AvgAvailableMB} MB
                - 최대 스왑 사용 (Max Swap Usage): {result.Stats.MaxPageFileUsage} %
                - 렉 유발 빈도 (Hard Faults): {result.Stats.AvgPagesPerSec} /sec
                """;
        }

        // 번역 헬퍼 메서드 (C# 내부에서 처리하므로 인코딩 문제 없음)
        private (string Status, string Message) TranslateResult(string englishStatus)
        {
            return englishStatus switch
            {
                "NORMAL" => ("정상", "메모리가 충분합니다. 현재 상태를 유지하세요."),

                "WARNING" => ("주의", "가상 메모리 의존도가 높습니다. 여유가 된다면 업그레이드를 고려하세요."),

                "LAGGY" => ("버벅임 감지", "잦은 스와핑(하드 폴트)으로 인해 성능 저하가 발생하고 있습니다."),

                "CRITICAL" => ("심각", "물리 메모리가 매우 부족합니다. 램 증설이 필수적입니다."),

                _ => ("알 수 없음", "분석 결과를 해석할 수 없습니다.")
            };
        }
    }
}