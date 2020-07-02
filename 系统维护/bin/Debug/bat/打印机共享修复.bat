echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.
echo                    开始修复打印机共享......
net user guest /active:no>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa" /v forceguest /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa\MSV1_0" /v NtlmMinClientSec /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa\MSV1_0" /v NtlmMinServerSec /t REG_DWORD /d 0x0 /f>nul 2>nul
goto sc_main
:sc_main
sc config LanmanWorkstation start= auto>nul 2>nul
sc config LanmanServer start= auto>nul 2>nul
sc config Winmgmt start= auto>nul 2>nul
sc config RpcSs start= auto>nul 2>nul
sc config Netman start= auto>nul 2>nul
sc config RasMan start= demand>nul 2>nul
sc config SSDPSRV start= auto>nul 2>nul
sc config BFE start= auto>nul 2>nul
sc config ALG start= demand>nul 2>nul
sc config SharedAccess start= auto>nul 2>nul
net start SharedAccess /y>nul 2>nul
sc config Browser start= auto>nul 2>nul
net start Browser /y>nul 2>nul
sc config Dnscache start= auto>nul 2>nul
net start Dnscache /y>nul 2>nul
sc config Dhcp start= auto>nul 2>nul
net start Dhcp /y>nul 2>nul
sc config lmhosts start= auto>nul 2>nul
net start lmhosts /y>nul 2>nul
sc config Spooler start= auto>nul 2>nul
net start Spooler /y>nul 2>nul
sc config upnphost start= demand>nul 2>nul
net start upnphost /y>nul 2>nul
reg query "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\Netlogon" /v Start|findstr "0x4">nul 2>nul && sc config Netlogon start= demand>nul 2>nul
reg add "HKLM\SYSTEM\CurrentControlSet\services\NetBT\Parameters" /v TransportBindName /t REG_SZ /d \Device\ /f>nul 2>nul
reg delete "HKLM\SYSTEM\CurrentControlSet\services\LanmanServer\Parameters" /v SMB1 /F>nul 2>nul
reg delete "HKLM\SYSTEM\CurrentControlSet\services\LanmanServer\Parameters" /v SMB2 /F>nul 2>nul
reg delete "HKLM\SYSTEM\CurrentControlSet\services\NetBT\Parameters" /v SMBDeviceEnabled /F>nul 2>nul
sc config netbt start= system>nul 2>nul
net start netbt>nul 2>nul
if %bulid%==nt5 (goto sc_sernt5) else (goto sc_sernt6)
:sc_sernt5
sc config PlugPlay start= auto>nul 2>nul
sc config TapiSrv start= auto>nul 2>nul
sc config Nla start= auto>nul 2>nul
net start Nla /y>nul 2>nul
netsh firewall set opmode mode=disable>nul 2>nul
goto sc_main2_etc
:sc_sernt6
sc config DcomLaunch start= auto>nul 2>nul
sc config RpcEptMapper start= auto>nul 2>nul
sc config SamSs start= auto>nul 2>nul
sc config nsi start= auto>nul 2>nul
sc config SstpSvc start= demand>nul 2>nul
sc config MpsSvc start= auto>nul 2>nul
net start MpsSvc /y>nul 2>nul
sc config NlaSvc start= auto>nul 2>nul
sc config netprofm start= auto>nul 2>nul
sc config fdPHost start= auto>nul 2>nul
net start fdPHost /y>nul 2>nul
sc config FDResPub start= auto>nul 2>nul
net start FDResPub /y>nul 2>nul
sc config HomeGroupListener start= auto>nul 2>nul
sc config WMPNetworkSvc start= auto>nul 2>nul
net start WMPNetworkSvc /y>nul 2>nul
sc config HomeGroupProvider start= auto>nul 2>nul
net start HomeGroupProvider /y>nul 2>nul
netsh advfirewall set allprofiles state off>nul 2>nul
:sc_main2_etc
(echo [Unicode]
echo Unicode=yes
echo [Version]
echo signature="$CHICAGO$"
echo Revision=1
echo [Privilege Rights]
echo sedenynetworklogonright = 
echo senetworklogonright = Everyone,Administrators,Users,Power Users,Backup Operators,guest)>>%usertemp%zcl.inf
secedit /configure /db %usertemp%zcl.sdb /cfg %usertemp%zcl.inf /log %usertemp%zcl.log /quiet
del /q %usertemp%zcl.*>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa\MSV1_0" /v LmCompatibilityLevel /t REG_DWORD /d 0x1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa" /v restrictanonymous /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa" /v restrictanonymoussam /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa" /v everyoneincludesanonymous /t REG_DWORD /d 0x1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Lsa" /v NoLmHash /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\LanmanServer\Parameters" /v AutoShareServer /t REG_DWORD /d 0x1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\LanmanServer\Parameters" /v AutoShareWks /t REG_DWORD /d 0x1 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\LanmanServer\Parameters" /v restrictnullsessaccess /t REG_DWORD /d 0x0 /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Browser\Parameters" /v MaintainServerList /t REG_SZ /d Auto /f>nul 2>nul
reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\Browser\Parameters" /v IsDomainMaster /t REG_SZ /d FALSE /f>nul 2>nul
for /f "delims=" %%a in ('reg query "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\NetBT\Parameters\Interfaces" /s /e /f "0x2"^|findstr "\Tcpip_"') do reg add "%%a" /v NetbiosOptions /t REG_DWORD /d 0x0 /f>nul 2>nul
reg query "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\NetBT\Parameters" /v EnableLMHOSTS|findstr "0x0">nul 2>nul && reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\services\NetBT\Parameters" /v EnableLMHOSTS /t REG_DWORD /d 0x1 /f>nul 2>nul
reg delete "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\RemoteComputer\NameSpace\{D6277990-4C6A-11CF-8D87-00AA0060F5BF}" /f>nul 2>nul
net use * /del /y>nul 2>nul
net config server /hidden:no>nul 2>nul
net share ipc$>nul 2>nul
if %bulid%==nt10 powershell -NonInteractive "Enable-WindowsOptionalFeature -Online -FeatureName SMB1Protocol">nul 2>nul && reg add "HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\LanmanWorkstation\Parameters" /v AllowInsecureGuestAuth /t REG_DWORD /d 0x1 /f>nul 2>nul
cls
echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.&echo.
echo   修复已完成，请手动重启计算机！！！