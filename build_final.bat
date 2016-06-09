@echo off
setlocal enabledelayedexpansion

SET solution_name=Theraot.sln

SET build_configuration=NET20 NET30 NET35 NET40 NET45
SET assemblies=Theraot.Core.dll
SET recovery_path=Core\bin
SET spec_file=Theraot.nuspec

REM USE QUOTES ALWAYS <----
SET msbuild_fallback="C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe"

echo.
echo Starting.
echo.

SET mypath=%~dp0
echo Running build.bat from %mypath:~0,-1%

echo Looking for MSBuild in PATH

for %%X in (MSBuild.exe) do (SET FOUND_A=%%~$PATH:X)
if defined FOUND_A (
	echo MSBuild.exe is in PATH
	SET msbuild="%FOUND_A%"
) ELSE (
	echo MSBuild.exe is in not in PATH
	echo Fallback to default MSBuild.exe location
	SET msbuild=%msbuild_fallback%
	echo Looking for MSBuild in: %msbuild_fallback%
)

if EXIST %msbuild:~1,-1% (
	echo MSBuild found.

	SET solution=%mypath:~0,-1%\%solution_name%
	echo Looking Solution file: %solution%

	IF EXIST %solution% (
		echo Solution file found.

		echo.
		echo Building.
		echo.
		@echo on
		@for %%A in (%build_configuration%) do (
			%msbuild% %solution% /m /p:Configuration=%%A
		)
		@echo off

		if not EXIST %mypath:~0,-1%\%spec_file% (
			popd
			exit
		)

		echo.
		echo Recovering assemblies.
		echo.
	
		@echo on

		pushd %~dp0
		cd /D %mypath:~0,3%
		cd %mypath%

		rmdir /S /Q package
		mkdir package
		mkdir package\lib\

		copy %spec_file% package\%spec_file%
		
		@for %%A in (%build_configuration%) do (
			mkdir package\lib\%%A\
			@for %%B in (%assemblies%) do (
				copy %recovery_path%\%%A\%%B package\lib\%%A\%%B
			)
		)
		@echo off

		echo Looking for NuGet in PATH

		for %%X in (NuGet.exe) do (SET FOUND_B=%%~$PATH:X)
		if defined FOUND_B (
			echo NuGet.exe is in PATH
			SET nuget="%FOUND_B%"
		) ELSE (
			echo NuGet.exe is in not in PATH
			echo Fallback to NuGet.CommandLine
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

		if EXIST version.txt (
			SET /P version=<version.txt
			for /f "skip=1" %%x in ('wmic os get localdatetime') do if not defined MyDate set MyDate=%%x
			set today=!MyDate:~0,4!!MyDate:~4,2!!MyDate:~6,2!
			SET version=!version!
			SET version=-Version !version!
		)

		popd

		if EXIST !nuget:~1,-1! (
			pushd %~dp0
			if defined dst_folder (
				cd !dst_folder!
				.\NuGet.exe pack %mypath:~0,-1%\package\%spec_file% -OutputDirectory %mypath:~0,-1%\package\ !version!
			) ELSE (
				NuGet.exe pack %mypath:~0,-1%\package\%spec_file% -OutputDirectory %mypath:~0,-1%\package\ !version!
			)
			popd
			exit
		) ELSE (
			echo NuGet.exe not found.
			exit
		)
	) ELSE (
		echo Solution file not found.
		exit
	)
) ELSE (
	echo MSBuild.exe not found.
	exit
)
