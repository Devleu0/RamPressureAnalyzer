@echo off
setlocal

set SESSION_NAME=RPA_Session

echo [Info] Stopping collection...
logman stop %SESSION_NAME%

echo [Info] Deleting session configuration...
logman delete %SESSION_NAME%

echo [Success] Logging stopped.