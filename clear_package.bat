@echo off
setlocal enabledelayedexpansion

REM Store path of the bat
SET mypath=%~dp0

echo ____________________________
echo Running from %mypath:~0,-1%\clear_package.bat
echo ____________________________

REM enter the path of the bat
pushd %~dp0
cd /D %mypath:~0,3%
cd %mypath%

	REM reset the package folder
	rmdir /S /Q package
	mkdir package
	mkdir package\lib\
	
REM leave the path
popd

popd