@echo off
IF dummy==dummy%2 (
nant\nant-0.92\bin\NAnt.exe -buildfile:FlickrDownloadr.build %1 -D:project.build.type=Debug
) ELSE (
nant\nant-0.92\bin\NAnt.exe -buildfile:FlickrDownloadr.build %1 -D:project.build.type=%2
)
date /t && time /t
