@echo off
setlocal enabledelayedexpansion

SET mypath=%~dp0

echo ____________________________
echo Running from %mypath:~0,-1%\build_final.bat
echo ____________________________

REM enter the path of the bat
pushd %~dp0
cd /D %mypath:~0,3%
cd %mypath%

	call clear_package.bat
	call build_net.bat
	call build_netcf.bat
	call package_final.bat

popd

popd