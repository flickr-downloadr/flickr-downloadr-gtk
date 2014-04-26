INSTALLBUILDERCLI = "'/Applications/Bitrock InstallBuilder Enterprise 8.6.0/bin/Builder.app/Contents/MacOS/installbuilder.sh'"
PATHTOBUILDFILE = "flickrdownloadr.xml"
PATHTOLICENSEFILE = "flickrdownloadrlicense.xml"

PLATFORM = "mac"
if [ $# = 1 ]; then
  PLATFORM = $1
fi

EXECCOMMAND = $INSTALLBUILDERCLI build $PATHTOBUILDFILE $PLATFORM

if [ -f $PATHTOLICENSEFILE ]; then
  EXECCOMMAND = $EXECCOMMAND $PLATFORM --license $PATHTOLICENSEFILE
fi

$EXECCOMMAND
