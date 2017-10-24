@echo off & setlocal

set BuildMode=%1
set BuildConfiguration=%2

echo ###########################
echo #### RPS - Local Build ####
echo ###########################
echo.

echo ARG Build Mode: %BuildMode%
echo ARG Build Configuration: %BuildConfiguration%
echo.

IF [%BuildMode%]==[] set /p BuildMode="Enter Build Mode: "
IF [%BuildConfiguration%]==[] set /p BuildConfiguration="Enter Configuration (Debug/Release): "
echo.

powershell -ExecutionPolicy ByPass -File build.ps1 -target %BuildMode% -configuration %BuildConfiguration%

REM IF [%1]==[] pause
pause