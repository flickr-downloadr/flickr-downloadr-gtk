#!/bin/bash
set -e

nant="nant/nant-0.93-custom-2016-12-05/bin/NAnt.exe"
buildfile="posix.build"

export PKG_CONFIG_PATH=/opt/local/lib/pkgconfig:/Library/Frameworks/Mono.framework/Versions/Current/lib/pkgconfig

if [ "dummy" = "dummy$2" ]; then
    mono $nant -buildfile:$buildfile $1 -D:project.build.type=Debug
else
    mono $nant -buildfile:$buildfile $1 -D:project.build.type=$2
fi

date

