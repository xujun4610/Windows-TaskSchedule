@echo off
%1 mshta vbscript:CreateObject("Shell.Application").ShellExecute("cmd.exe","/c %~s0 ::","","runas",1)(window.close)&&exit
cd /d "%~dp0"
:main
cls

echo.--------------------------------------------------------
echo.
echo. Niko.Xu ������ Windows Service ��װ�ű� copyright 2018
echo.
echo.--------------------------------------------------------
echo.����360�����ԹܼҵȰ�ȫ������ѣ��빴ѡ��������Ͳ������ѣ�
echo.���߸ɴ�رհɣ�Ȩ�޲��������ˡ�
echo.
echo.���µ�ַ��https://github.com/xujun4610/Windows-TaskSchedule
echo.ʮ�ָ�л leleroyn ��ԭ��Ŀ��https://github.com/leleroyn/Windows-TaskSchedule��
echo.--------------------------------------------------------
echo.��ѡ��ʹ�ã�
echo.
echo. 1.��װ���񣨼�����������1��
echo.
echo. 2.ж�ط��񣨼�����������2��
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
..\Windows.TaskSchedule.exe install
REM ��װ�߼�����
echo.-----------------------------------------------------------
echo. Windows ����װ����
echo.-----------------------------------------------------------
goto end

:uninstall-svc
REM ж���߼�
..\Windows.TaskSchedule.exe uninstall
echo.-----------------------------------------------------------
echo. Windows ����ж������
echo.-----------------------------------------------------------
REM ж���߼�����
goto end

:end
echo �밴������˳���
@Pause>nul