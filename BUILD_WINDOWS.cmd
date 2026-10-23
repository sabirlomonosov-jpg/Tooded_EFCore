@echo off
setlocal
cd /d "%~dp0"
where dotnet >nul 2>nul
if errorlevel 1 (
  echo NET 8 SDK not found. Install .NET 8 SDK and Visual Studio 2022.
  pause
  exit /b 1
)

dotnet restore "Tooded_EFCore.sln"
if errorlevel 1 goto fail

dotnet build "Tooded_EFCore.sln" -c Release --no-restore
if errorlevel 1 goto fail

echo Build completed. Start from Visual Studio (F5) to test LocalDB and Windows Forms.
pause
exit /b 0

:fail
echo BUILD FAILED. Open Visual Studio Error List and README_RU.md.
pause
exit /b 1
