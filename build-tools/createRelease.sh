INSTALL_BUILDER_VERSION="15.1.0"

if [ "$(uname)" == "Darwin" ]; then
  INSTALLBUILDERCLI="/Applications/Bitrock InstallBuilder Enterprise ${INSTALL_BUILDER_VERSION}/bin/Builder.app/Contents/MacOS/installbuilder.sh"
elif [ "$(expr substr $(uname -s) 1 5)" == "Linux" ]; then
  if [[ $WERCKER = true ]]; then
    INSTALLBUILDERCLI="/opt/installbuilder-${INSTALL_BUILDER_VERSION}/bin/builder"
  else
    INSTALLBUILDERCLI="${HOME}/installbuilder-${INSTALL_BUILDER_VERSION}/bin/builder"
  fi
fi
PATHTOBUILDFILE="flickrdownloadr.xml"
PATHTOLICENSEFILE="flickrdownloadrlicense.xml"

PLATFORM="mac"
if [ $# = 1 ]; then
  PLATFORM=$1
fi

PACKVARIABLE="pack_${PLATFORM}_platform_files"

if [ -f $PATHTOLICENSEFILE ]; then
  EXECCOMMAND="'$INSTALLBUILDERCLI' build $PATHTOBUILDFILE $PLATFORM --license $PATHTOLICENSEFILE --setvars project.version=$BUILDNUMBER $PACKVARIABLE=true"
else
  EXECCOMMAND="'$INSTALLBUILDERCLI' build $PATHTOBUILDFILE $PLATFORM --setvars project.version=$BUILDNUMBER $PACKVARIABLE=true"
fi

echo "About to execute: $EXECCOMMAND"

eval $EXECCOMMAND
