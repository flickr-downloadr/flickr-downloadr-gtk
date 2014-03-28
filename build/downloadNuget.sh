#!/bin/bash
rm -f nuget.exe
wget http://nuget.org/nuget.exe
chmod a+x .nuget.exe
cd ../source
mono ../build/nuget.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
