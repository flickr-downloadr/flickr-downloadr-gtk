#!/bin/bash
rm -f nuget.exe
wget http://nuget.org/nuget.exe
cp nuget.exe ../source/.nuget/NuGet.exe
chmod a+x ../source/.nuget/NuGet.exe

