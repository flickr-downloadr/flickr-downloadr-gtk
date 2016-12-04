@echo off
IF dummy==dummy%2 (
  nant\nant-0.93-nightly-2015-02-12\bin\NAnt.exe -buildfile:windows.build %1 -D:project.build.type=Debug
) ELSE (
  nant\nant-0.93-nightly-2015-02-12\bin\NAnt.exe -buildfile:windows.build %1 -D:project.build.type=%2
)

if %errorlevel% neq 0 exit /b %errorlevel%

date /t && time /t
