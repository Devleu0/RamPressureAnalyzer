# RamPressureAnalyzer

![License](https://img.shields.io/badge/license-MIT-blue) ![Platform](https://img.shields.io/badge/platform-Windows_x64-lightgrey)

A tool that answers the question **"Do I need 32GB of RAM?"** with data, not guesses.

This is not a "Game Booster" or a "Memory Cleaner." It is a lightweight GUI wrapper for Windows native performance tools (`logman`, `perfmon`). It determines whether your system is genuinely struggling with memory pressure or simply caching data efficiently.

## Background

* **Task Manager is misleading:** Seeing 80% RAM usage often causes unnecessary panic. High usage is normal; unused RAM is wasted RAM.
* **The real bottleneck:** Performance degradation (stuttering) occurs during **Hard Faults**—when the system is forced to swap data between Disk and RAM actively.
* **Data-driven decision:** This tool records background performance metrics while you game or work, then analyzes the logs to give a definitive "Upgrade" or "Don't Upgrade" verdict.

## Architecture

No black-box magic. I value transparency. Feel free to inspect the `scripts` folder.

1.  **UI (WPF)**: Acts as a remote control. It simply triggers the batch files.
2.  **Collector (Logman)**: Uses the native `logman.exe` to save CSV logs. (Negligible system resource usage).
3.  **Analyzer (PowerShell)**: Parses the CSV logs after the session to generate a report.

## Usage

Portable. No installation required. Just unzip and run.

1.  **Run**: Open `RamPressureAnalyzer.exe`.
    (Requires Administrator privileges to access `logman`.)
2.  **Start Logging**: Click the [Start Logging] button.
3.  **Work/Game**: Perform your heavy tasks (Gaming, Rendering, Compiling, etc.). A minimum of 10 minutes is recommended.
4.  **Stop & Analyze**: Click [Stop & Analyze] when finished.
5.  **Check Verdict**:
    * **NORMAL**: RAM is sufficient. Save your money.
    * **WARNING**: High dependency on virtual memory. Consider upgrading if budget allows.
    * **CRITICAL**: Performance loss due to RAM shortage confirmed. Upgrade recommended.

## FAQ

**Q. I get a "Windows protected your PC" (SmartScreen) warning.**
As an individual developer, I did not purchase an expensive Code Signing Certificate. This is not malware. Click [More Info] -> [Run anyway]. If you are concerned, feel free to build the code yourself from the source.

**Q. Why does it require Administrator privileges?**
Admin rights are mandatory to access Windows Performance Counters. This application does not modify any other system settings.

## File Structure

RamPressureAnalyzer/
├── RamPressureAnalyzer.exe  # Main executable
└── scripts/                 # Core logic (Editable)
    ├── start_log.bat        # Controls logman
    ├── stop_log.bat
    ├── analyze.ps1          # Analysis logic
    └── counters.txt         # List of performance counters

## License
MIT License. Free to use, modify, and distribute.
