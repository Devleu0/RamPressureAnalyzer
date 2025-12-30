@echo off
setlocal

:: --- 설정 ---
set SESSION_NAME=RPA_Session
set LOG_DIR=%~dp0logs
set COUNTER_FILE=%~dp0counters.txt

:: 로그 폴더 생성
if not exist "%LOG_DIR%" mkdir "%LOG_DIR%"

:: 혹시 모를 이전 세션 정리 (안전장치)
logman stop %SESSION_NAME% >nul 2>&1
logman delete %SESSION_NAME% >nul 2>&1

:: --- logman 세션 생성 및 시작 ---
:: -f csv: CSV 형식 저장
:: -si 1: 1초 간격 (게임/작업 중 스파이크 캐치용)
:: -v mmddhhmm: 파일명에 날짜시간 자동 붙임
:: -o: 출력 경로 지정
echo [Info] Creating data collector set...
logman create counter %SESSION_NAME% -cf "%COUNTER_FILE%" -f csv -si 1 -v mmddhhmm -o "%LOG_DIR%\memory_log"

echo [Info] Starting collection...
logman start %SESSION_NAME%

echo [Success] Logging started.