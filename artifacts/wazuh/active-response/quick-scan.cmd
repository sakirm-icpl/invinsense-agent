@echo off
ECHO.

rem Set the location of the Windows Defender executable
set DEFENDER="%ProgramFiles%\Windows Defender\MpCmdRun.exe"

rem Set the location of the log file
set LOGFILE="C:\Program Files (x86)\ossec-agent\active-response\active-responses.log"

rem Get the current date and time in the desired format
for /f "tokens=2 delims==" %%a in ('wmic OS Get localdatetime /value') do set "dt=%%a"
set "year=%dt:~0,4%"
set "month=%dt:~4,2%"
set "day=%dt:~6,2%"
set "hour=%dt:~8,2%"
set "minute=%dt:~10,2%"
set "second=%dt:~12,2%"
set "DATETIME=%year%/%month%/%day% %hour%:%minute%:%second%"

rem Log the scan start time
echo %DATETIME% active-response/bin/quick-scan.exe: Scan starting... >> %LOGFILE%

rem Run the command and log the output with timestamps, excluding scan start logs
for /f "tokens=*" %%a in ('%DEFENDER% -Scan -ScanType 1 2^>^&1') do (
    echo %%a | findstr /C:"Scan starting..." > nul || (
        echo %DATETIME% active-response/bin/quick-scan.exe: %%a >> %LOGFILE%
    )
)
