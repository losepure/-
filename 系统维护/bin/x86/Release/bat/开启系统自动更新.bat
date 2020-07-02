echo off
echo 1.关闭 windows update 服务
net stop wuauserv >nul 2>nul 
echo.
echo 2.清空历史更新文件
rd /q /s "C:\Windows\SoftwareDistribution">nul 2>nul
sc config wuauserv start= Delayed-auto>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\wuauserv" /v Start /t REG_DWORD /d 2 /f
echo.
echo 3.设置启动类型为“自动”（延时启动）
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\wuauserv" /v DelayedAutoStart /t REG_DWORD /d 1 /f
echo.
echo 4.再次开启 windows update 服务
net start wuauserv>nul 2>nul
echo.
echo 5.更新设置为“自动安装更新（推荐）”
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v AUOptions /t REG_DWORD /d 4 /f
echo.
echo 6.更新安装时间设置为“每天9：00”
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v ScheduledInstallDay /t REG_DWORD /d 0 /f
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v ScheduledInstallTime /t REG_DWORD /d 9 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v IncludeRecommendedUpdates /t REG_DWORD /d 1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\WindowsUpdate\Auto Update" /v ElevateNonAdmins /t REG_DWORD /d 1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\WinDefend" /v Start /t REG_DWORD /d 4 /f>nul 2>nul
echo.
echo 7.更新服务器指向更换为局集团公司服务器
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate" /v WUServer /t REG_SZ /d http://10.208.4.229:8530 /f
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate /v WUStatusServer /t REG_SZ /d http://10.208.4.229:8530 /f>nul 2>nul
reg add HKEY_LOCAL_MACHINE\SOFTWARE\Policies\Microsoft\Windows\WindowsUpdate\AU /v UseWUServer /t REG_DWORD /d 1 /f>nul 2>nul
gpupdate /force>nul 2>nul

echo 按任意键返回。
