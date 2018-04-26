#!/bin/bash
rm -f ../source/.nuget/NuGet.exe
wget https://dist.nuget.org/win-x86-commandline/latest/nuget.exe
chmod a+x nuget.exe
mv nuget.exe ../source/.nuget/NuGet.exe
cd ../source
SUDO_CMD="sudo"
if [[ $WERCKER = true ]]; then
  SUDO_CMD=""
fi
$SUDO_CMD mono ./.nuget/NuGet.exe restore -configFile ../build-tools/NuGet.Config  -verbosity detailed "FloydPink.Flickr.Downloadr.sln"
