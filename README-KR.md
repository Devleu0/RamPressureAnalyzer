# RamPressureAnalyzer

[![Download](https://img.shields.io/badge/Download-Latest_Release-blue?style=for-the-badge&logo=github)](https://github.com/Devleu0/RamPressureAnalyzer/releases/latest)

![License](https://img.shields.io/badge/license-MIT-blue) ![Platform](https://img.shields.io/badge/platform-Windows_x64-lightgrey)

"지금 16GB 사용 중인데, 램 32GB로 업그레이드할까요?"라는 질문에 **데이터**로 답해주는 도구입니다.

"게임 부스터"나 "메모리 클리너"가 아닙니다. 윈도우에 내장된 성능 측정 도구(logman, perfmon)를 편하게 쓸 수 있도록 UI만 입혔습니다. 시스템이 실제로 메모리 부족을 겪고 있는지, 아니면 단순히 캐싱 중인지 숫자로 확인하세요.

## 개발 배경

* 작업 관리자만 보고 "램 사용량 80%네? 큰일 났다"라고 오해하는 경우가 많습니다.
* 실제 성능 저하는 램 용량 그 자체가 아니라, 디스크와 램 사이에서 데이터를 강제로 교환하는 **페이지 폴트(Page Faults)**가 발생할 때 일어납니다.
* Ram Pressure Analyzer는 게임이나 작업을 하는 동안 백그라운드에서 진짜 성능 지표를 기록하고, 종료 후 로그를 분석해 "업그레이드가 필요한지" 판단합니다.

## 작동 원리

1. **UI (WPF)**: 버튼을 누르면 배치 파일(.bat)을 실행하는 역할만 수행합니다.
2. **수집 (Logman)**: 윈도우 기본앱인 logman.exe를 호출해 CSV 로그를 남깁니다. (시스템 리소스 점유율 거의 없음)
3. **분석 (PowerShell)**: 로그 수집이 끝나면 analyze.ps1 스크립트가 CSV를 파싱해서 결과를 보여줍니다.

## 사용법

설치 없이 압축을 풀고 바로 사용하는 포터블(Portable) 방식입니다.

<div align="center">
  <img src="/UI.png" width="500" alt="Main Dashboard">
  <p><em>RamPressureAnalyzer Main Dashboard View</em></p>
</div>

1. **실행**: RamPressureAnalyzer.exe를 실행합니다.
   (logman 접근을 위해 관리자 권한을 요청합니다.)
2. **기록 시작**: [Start Logging] 버튼을 누릅니다.
3. **작업 수행**: 평소 하던 게임, 렌더링, 개발 작업을 수행합니다. (최소 10분 이상 권장, 게임의 경우 로딩이후에 기록하는게 정확합니다.)
4. **종료 및 분석**: 작업이 끝나면 [Stop & Analyze]를 누릅니다.
5. **결과 확인**
   - NORMAL: 램 충분함. 업그레이드 불필요.
   - WARNING: 가상 메모리 의존도 높음. 여유 되면 업그레이드 고려.
   - CRITICAL: 램 부족으로 인한 성능 저하 확인됨. 업그레이드 필수.

## 자주 묻는 질문 (FAQ)
**Q. 왜 관리자 권한을 요구하나요?**
윈도우 시스템 성능 카운터(Performance Counters)에 접근하려면 관리자 권한이 필수입니다. 이 앱은 로깅 외의 시스템 설정은 건드리지 않습니다.

## 파일 구조
```
RamPressureAnalyzer/
├── RamPressureAnalyzer.exe  # 메인 실행 파일
└── scripts/                 # 핵심 로직 (수정 가능)
    ├── start_log.bat        # logman 제어
    ├── stop_log.bat
    ├── analyze.ps1          # 분석 스크립트
    └── counters.txt         # 수집 카운터 목록
```
## 라이선스
MIT License. 자유롭게 수정 및 배포 가능합니다.
