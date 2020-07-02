@echo off 
if not exist "%~dp0newtstop.dll" goto error:
if not exist "%~dp0nts64helper.dll" goto error:

set rdir=n
if exist "%windir%\System32\rserver30\rserver3.exe" set rdir=%windir%\System32\rserver30
if exist "%windir%\SysWOW64\rserver30\rserver3.exe" set rdir=%windir%\SysWOW64\rserver30
if %rdir%==n goto rerror:

echo>"%rdir%\ntstest"
if not exist "%rdir%\ntstest" goto rights:
del /f /q "%rdir%\ntstest"

cls
echo.
echo ===================================================
echo.
echo  NewTrialStop v2.3 for Famatech Radmin Server v3.4
echo.
echo ===================== INSTALL =====================
echo.

net stop rserver3
if exist "%rdir%\newtstop.dll" goto old:
:install
copy /y "%~dp0newtstop.dll" "%rdir%\wsock32.dll"
if exist "%rdir%\fam64helper.exe" copy /y "%~dp0nts64helper.dll" "%rdir%"
echo.
net start rserver3
rundll32 "%rdir%\wsock32.dll",ntskd
goto end:

:error
echo Installation files not found.
goto end:

:rerror
echo Cannot find Radmin Server.
goto end:

:old
regsvr32 /s /u "%rdir%\newtstop.dll"
del /f "%rdir%\newtstop.dll"
del /f "%rdir%\newtstop.ini"
goto install:

:rights
cls
echo Administrator rights are required.
goto end:

:end
