@echo off
setlocal enabledelayedexpansion

REM Solution file name
SET solution_name=Theraot.sln

REM build configurations, space separated
SET build_configuration=netcoreapp1.0
REM assembly to recover
SET assemblies=Theraot.Core.dll
REM path to recover from
SET recovery_path=bin

REM Store path of the bat
SET mypath=%~dp0

echo ____________________________
echo Running from %mypath:~0,-1%\build_net.bat
echo ____________________________

echo Looking for dotnet in PATH

REM look for dotnet in PATH environment variable
for %%X in (dotnet.exe) do (SET FOUND_A=%%~$PATH:X)
REM if found
if defined FOUND_A (
	echo dotnet.exe is in PATH
	SET build="%FOUND_A%"
) ELSE (
	echo dotnet.exe is in not in PATH
	if EXIST "%ProgramFiles%\dotnet\dotnet.exe" (
		SET build="%ProgramFiles%\dotnet\dotnet.exe"
	)
)

if NOT EXIST %build% (
	echo dotnet not found
	exit /b
)
echo dotnet found.
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
	
	REM call dotnet with each build configuration
	@echo on
	@for %%A in (%build_configuration%) do (
		%build% build %solution% --configuration %%A --output %mypath:~0,-1%\%recovery_path%\%%A\
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

echo done with dotnet

popd

exit /b