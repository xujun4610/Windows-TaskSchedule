@echo off
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
cd /d "%~dp0"
:main
cls
REM title
echo.*************************************************************************
echo.
echo. Niko.Xu ������ Windows Service ��װ�ű����汾��1
echo. �������ڣ�2018-05-12
echo. 
echo. �����ڰ�װ���������
echo.     Windows-TaskSchedule
echo.
echo. ����Ʒʹ�� Apache License 2.0 ���֤
echo. ? %date:~0,4%  Niko Xu Production, All rights reserved.
echo.
echo.*************************************************************************
echo.����360�����ԹܼҵȰ�ȫ������ѣ��빴ѡ��������Ͳ������ѣ�
echo.���߸ɴ�رհɣ�PS:��װʱ��Ҫ����ԱȨ����Ȩ��
echo.
echo.���µ�ַ��https://github.com/xujun4610/Windows-TaskSchedule
echo.ʮ�ָ�л leleroyn ��ԭ��Ŀ��https://github.com/leleroyn/Windows-TaskSchedule��
echo.--------------------------------------------------------
echo.��ѡ��ʹ�ã�
echo.
echo. 1.��װ�������·���1��
echo.
echo. 2.ж�ط������·���2��
echo.--------------------------------------------------------

if exist "%SystemRoot%\System32\choice.exe" goto NT6Choice

set /p choice=���������ֲ����س���ȷ��:

echo.
if %choice%==1 goto install-svc
if %choice%==2 goto uninstall-svc
cls
"set choice="
echo ����������������ѡ��

goto main

:NT6Choice
choice /c 12 /n /m "��������Ӧ���֣�"
if errorlevel 2 goto uninstall-svc
if errorlevel 1 goto install-svc
cls
goto main

:install-svc
REM ��װ�߼�
color 2E
echo.-----------------------------------------------------------
echo. ����ʾ�� ���ڰ�װ����......
echo.-----------------------------------------------------------
echo Installing...
..\Windows.TaskSchedule.exe install
REM ��װ�߼�����
echo.-----------------------------------------------------------
echo. ����ʾ�� Windows Service ��װ��ɣ�
echo.-----------------------------------------------------------
goto end

:uninstall-svc
REM ж���߼�
color 70
echo.-----------------------------------------------------------
echo. ����ʾ�� ����ж�ط���......
echo.-----------------------------------------------------------
echo Uninstalling...
..\Windows.TaskSchedule.exe uninstall
echo.-----------------------------------------------------------
echo. ����ʾ�� Windows Service ж����ɣ�
echo.-----------------------------------------------------------
REM ж���߼�����
goto end

:end

echo �밴������˳���
@Pause>nul
color 07