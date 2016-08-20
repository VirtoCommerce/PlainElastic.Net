IF NOT DEFINED ProgramFiles(x86) SET ProgramFiles(x86)=%ProgramFiles%
IF NOT DEFINED MSBUILD_PATH IF EXIST "%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe" SET MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\14.0\Bin\MSBuild.exe
IF NOT DEFINED MSBUILD_PATH IF EXIST "%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe" SET MSBUILD_PATH=%ProgramFiles(x86)%\MSBuild\12.0\Bin\MSBuild.exe
IF NOT DEFINED MSBUILD_PATH SET MSBUILD_PATH=%WINDIR%\Microsoft.NET\Framework\v4.0.30319\msbuild.exe

@set NUGET="packages\NuGet.CommandLine.2.8.3\tools\NuGet.exe"

@echo ==========================
@echo Building PlainElastic.Net.
@rmdir src\PlainElastic.Net\bin /s /q
%MSBUILD_PATH% "PlainElastic.Net.sln" /nologo /verbosity:m /t:Build /p:Configuration=Release;Platform="Any CPU"
@if errorlevel 1 goto error

@echo ==========================
@echo Copying PlainElastic.Net assemblies.
@rmdir NuGet\lib /s /q
@mkdir NuGet\lib
xcopy src\PlainElastic.Net\bin\Release NuGet\lib /s /y
@if errorlevel 0 goto nuget
@goto error

:nuget
@echo ==========================
@echo NuGet package creation.
@%NUGET% pack NuGet\plainelastic.net.nuspec -basePath NuGet -o NuGet
@if not errorlevel 0 goto error

@echo PlainElastic.Net build sucessfull.
@goto end

:error
@echo Error occured during PlainElastic.Net build.
@pause

:end
@pause