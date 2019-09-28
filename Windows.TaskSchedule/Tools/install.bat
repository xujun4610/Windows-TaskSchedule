@echo off
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
cd /d "%~dp0"
:main
cls
REM title
echo.*************************************************************************
echo.
echo. Niko.Xu 制作的 Windows Service 安装脚本，版本：1
echo. 制作日期：2018-05-12
echo. 
echo. 适用于安装如下软件：
echo.     Windows-TaskSchedule
echo.
echo. 本产品使用 Apache License 2.0 许可证
echo. ? %date:~0,4%  Niko Xu Production, All rights reserved.
echo.
echo.*************************************************************************
echo.如有360、电脑管家等安全软件提醒，请勾选信任允许和不再提醒！
echo.或者干脆关闭吧！PS:安装时需要管理员权限授权！
echo.
echo.更新地址：https://github.com/xujun4610/Windows-TaskSchedule
echo.十分感谢 leleroyn 的原项目（https://github.com/leleroyn/Windows-TaskSchedule）
echo.--------------------------------------------------------
echo.请选择使用：
echo.
echo. 1.安装服务，请下方打【1】
echo.
echo. 2.卸载服务，请下方打【2】
echo.--------------------------------------------------------

if exist "%SystemRoot%\System32\choice.exe" goto NT6Choice

set /p choice=请输入数字并按回车键确认:

echo.
if %choice%==1 goto install-svc
if %choice%==2 goto uninstall-svc
cls
"set choice="
echo 您输入有误，请重新选择。

goto main

:NT6Choice
choice /c 12 /n /m "请输入相应数字："
if errorlevel 2 goto uninstall-svc
if errorlevel 1 goto install-svc
cls
goto main

:install-svc
REM 安装逻辑
color 2E
echo.-----------------------------------------------------------
echo. 【提示】 正在安装服务......
echo.-----------------------------------------------------------
echo Installing...
..\Windows.TaskSchedule.exe install
REM 安装逻辑结束
echo.-----------------------------------------------------------
echo. 【提示】 Windows Service 安装完成！
echo.-----------------------------------------------------------
goto end

:uninstall-svc
REM 卸载逻辑
color 70
echo.-----------------------------------------------------------
echo. 【提示】 正在卸载服务......
echo.-----------------------------------------------------------
echo Uninstalling...
..\Windows.TaskSchedule.exe uninstall
echo.-----------------------------------------------------------
echo. 【提示】 Windows Service 卸载完成！
echo.-----------------------------------------------------------
REM 卸载逻辑结束
goto end

:end

echo 请按任意键退出。
@Pause>nul
color 07