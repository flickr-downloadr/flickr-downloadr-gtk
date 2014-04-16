#!/bin/bash
rm -f ../source/.nuget/NuGet.exe
wget http://nuget.org/nuget.exe
chmod a+x nuget.exe
mv nuget.exe ../source/.nuget/NuGet.exe
cd ../source
mono ./.nuget/NuGet.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
