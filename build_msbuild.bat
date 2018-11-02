@echo off
setlocal enabledelayedexpansion

REM Solution file name
SET solution_name=Theraot.sln

REM build configurations, space separated
SET build_configuration=NET20 NET30 NET35 NET40 NET45
REM assembly to recover
SET assemblies=Theraot.Core.dll
REM path to recover from
SET recovery_path=bin

REM path for msbuild
REM USE QUOTES ALWAYS <----
SET fallback="C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe"

REM Store path of the bat
SET mypath=%~dp0

echo ____________________________
echo Running from %mypath:~0,-1%\build_net.bat
echo ____________________________

echo Looking for MSBuild in PATH

REM look for msbuild in PATH environment variable
for %%X in (MSBuild.exe) do (SET FOUND_A=%%~$PATH:X)
REM if found
if defined FOUND_A (
	echo MSBuild.exe is in PATH
	SET build="%FOUND_A%"
) ELSE (
	echo MSBuild.exe is in not in PATH
	if EXIST "%SystemRoot%\Microsoft.NET\Framework64\" (
		echo Looking for MSBuild in "%SystemRoot%\Microsoft.NET\Framework64\"
		for /D %%V in ("%SystemRoot%\Microsoft.NET\Framework64\v*") do (
			if EXIST %%V\MSBuild.exe (
				SET build=%%V\MSBuild.exe
			)
		)
	) ELSE (
		echo Looking for MSBuild in "%SystemRoot%\Microsoft.NET\Framework\"
		for /D %%V in ("%SystemRoot%\Microsoft.NET\Framework\v*") do (
			if EXIST %%V\MSBuild.exe (
				SET build=%%V\MSBuild.exe
			)
		)
	)
)

if NOT EXIST %build% (
	echo Fallback to default MSBuild.exe location
	SET build=%fallback%
	echo Looking for MSBuild in: %fallback%
)

if NOT EXIST %build% (
	echo MSBuild not found
	exit /b
)
echo MSBuild found.
echo %build%

REM build path to solution from the solution file name and the path of the bat
SET solution=%mypath:~0,-1%\%solution_name%

echo Looking Solution file: %solution%

REM if the solution file exists...
IF EXIST %solution% (
	echo Solution file found.

	echo.
	echo Building.
	echo.
	
	REM call msbuild with each build configuration
	@echo on
	@for %%A in (%build_configuration%) do (
		%build% %solution% /m /p:Configuration=%%A
	)
	@echo off

	echo.
	echo Recovering assemblies.
	echo.

	@echo on

	REM enter the path of the bat
	pushd %~dp0
	cd /D %mypath:~0,3%
	cd %mypath%

		REM copy the assamblies to the package folder
		@for %%A in (%build_configuration%) do (
			mkdir package\lib\%%A\
			@for %%B in (%assemblies%) do (
				copy %recovery_path%\%%A\%%B package\lib\%%A\%%B
			)
		)
		@echo off
		
	REM leave the path
	popd
	
	echo done with solution %solution%
)

echo done with MSBuild

popd

exit /b