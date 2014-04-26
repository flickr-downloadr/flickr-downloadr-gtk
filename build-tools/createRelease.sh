INSTALLBUILDERCLI="/Applications/Bitrock InstallBuilder Enterprise 8.6.0/bin/Builder.app/Contents/MacOS/installbuilder.sh"
PATHTOBUILDFILE="flickrdownloadr.xml"
PATHTOLICENSEFILE="flickrdownloadrlicense.xml"

PLATFORM="mac"
if [ $# = 1 ]; then
  PLATFORM=$1
fi

if [ -f $PATHTOLICENSEFILE ]; then
  eval "'$INSTALLBUILDERCLI' build $PATHTOBUILDFILE $PLATFORM --license $PATHTOLICENSEFILE"
else
  eval "'$INSTALLBUILDERCLI' build $PATHTOBUILDFILE $PLATFORM"
fi
