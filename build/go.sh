nant="nant/nant-0.92/bin/NAnt.exe"
buildfile="FlickrDownloadr.build"

if [ "dummy" = "dummy$2" ]; then
    mono $nant -buildfile:$buildfile $1 -D:project.build.type=Debug
else
    mono $nant -buildfile:$buildfile $1 -D:project.build.type=$2
fi

date

