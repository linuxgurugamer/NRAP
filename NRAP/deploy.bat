

set H=R:\KSP_1.3.1_dev
echo %H%

copy bin\Debug\NRAP.dll ..\Output\GameData\NRAP\Plugins

xcopy /y/s/i ..\Output\GameData\NRAP %H%\GameData\NRAP
