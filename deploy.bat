

set H=R:\KSP_1.3.0_dev
rem set H=R:\KSP_1.2.2_dev
echo %H%

copy CCTV\bin\Debug\CCTV.dll GameData\CCTV\Plugins

xcopy GameData\CCTV %H%\GameData\CCTV /E /Y
