@echo off
setlocal enabledelayedexpansion

REM nuspec file name
SET spec_file=Theraot.nuspec

echo.
echo Starting.
echo.

REM Store path of the bat
SET mypath=%~dp0

echo ____________________________
echo Running from %mypath:~0,-1%\package_final.bat
echo ____________________________

if not EXIST %mypath:~0,-1%\%spec_file% (
	popd
	exit
)

REM enter the path of the bat
pushd %~dp0
cd /D %mypath:~0,3%
cd %mypath%

	REM copy the nuspec file to the package folder
	copy %spec_file% package\%spec_file%

	echo Looking for NuGet in PATH

	REM look for nuget.exe in the PATH environment variable
	for %%X in (NuGet.exe) do (SET FOUND_B=%%~$PATH:X)
	REM if found
	if defined FOUND_B (
		echo NuGet.exe is in PATH
		SET nuget="%FOUND_B%"
	) ELSE (
		echo NuGet.exe is in not in PATH
		echo Fallback to NuGet.CommandLine
		REM get nuget from .\packages\NuGet.CommandLine.*\tools\NuGet.exe
		pushd %~dp0
		cd /D %mypath:~0,3%
		cd %mypath:~0,-1%\packages\
		for /D %%s in (.\*) do (
			SET folder=%%s
			SET folder=!folder:~2,17!
			if "!folder!" == "NuGet.CommandLine" (
				SET folder=%%s
				SET nuget="%mypath:~0,-1%\packages\!folder:~2!\tools\NuGet.exe"
				SET dst_folder=%mypath:~0,-1%\packages\!folder:~2!\tools\
			)
		)
		popd
		echo Looking for NuGet.exe in: !nuget!
	)

	REM get the version number form version.txt if it exists

	if EXIST version.txt (
		SET /P version=<version.txt
		for /f "skip=1" %%x in ('wmic os get localdatetime') do if not defined MyDate set MyDate=%%x
		set today=!MyDate:~0,4!!MyDate:~4,2!!MyDate:~6,2!
		SET version=-Version !version!
	)

REM leave the path
popd

REM if NuGet exists use it to create the package
if EXIST !nuget:~1,-1! (
	pushd %~dp0
	if defined dst_folder (
		cd !dst_folder!
		.\NuGet.exe pack %mypath:~0,-1%\package\%spec_file% -OutputDirectory %mypath:~0,-1%\package\ !version!
	) ELSE (
		NuGet.exe pack %mypath:~0,-1%\package\%spec_file% -OutputDirectory %mypath:~0,-1%\package\ !version!
	)
)

popd

exit /b