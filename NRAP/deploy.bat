

set H=R:\KSP_1.3.0_dev
echo %H%

copy bin\Debug\NRAP.dll ..\Output\GameData\NRAP\Plugins

xcopy /y/s ..\Output\GameData\NRAP %H%\GameData\NRAP
