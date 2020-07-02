@echo off
if exist %windir%\SysWOW64 (
 user64.reg
)else (
 user.reg
)
