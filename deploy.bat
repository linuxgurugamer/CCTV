

set H=R:\KSP_1.3.1_dev
rem set H=R:\KSP_1.2.2_dev
echo %H%

copy CCTV\bin\Debug\CCTV.dll GameData\CCTV\Plugins

xcopy  /E /y /I GameData\CCTV %H%\GameData\CCTV 
