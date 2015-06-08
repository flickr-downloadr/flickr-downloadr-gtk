#!/usr/bin/env bash

# Adapted from the MonoDevelop Launch Script

# Workaround for Unity gnome shell
export UBUNTU_MENUPROXY=0

# Ubuntu overlay scrollbars are not working well with the 1px boundaries in the monodevelop shell
export LIBOVERLAY_SCROLLBAR=0

# The Oxygen GTK theme crashes unless this is set
export OXYGEN_DISABLE_INNER_SHADOWS_HACK=1

APPNAME="flickr downloadr"

# mono version check
REQUIRED_MAJOR=3
REQUIRED_MINOR=2

VERSION_TITLE="Cannot launch $APPNAME"
VERSION_MSG="$APPNAME requires the Mono Framework version $REQUIRED_MAJOR.$REQUIRED_MINOR or later."
DOWNLOAD_URL="http://www.mono-project.com/Release_Notes_Mono_3.2#Installing_Mono_3.2"

MONO_VERSION="$(mono --version | grep 'Mono JIT compiler version ' |  cut -f5 -d\ )"
MONO_VERSION_MAJOR="$(echo $MONO_VERSION | cut -f1 -d.)"
MONO_VERSION_MINOR="$(echo $MONO_VERSION | cut -f2 -d.)"
if [ -z "$MONO_VERSION" ] \
	|| [ $MONO_VERSION_MAJOR -lt $REQUIRED_MAJOR ] \
	|| [ $MONO_VERSION_MAJOR -eq $REQUIRED_MAJOR -a $MONO_VERSION_MINOR -lt $REQUIRED_MINOR ]
then
    if zenity --question --text="$VERSION_MSG" --title="$VERSION_TITLE" --ok-label="Download..." --cancel-label="Cancel"; then
        if command -v xdg-open 2>/dev/null; then
            xdg-open $DOWNLOAD_URL
        elif command -v gnome-open 2>/dev/null; then
            gnome-open $DOWNLOAD_URL
        else
            zenity --warning --text="Please click <a href=\"$DOWNLOAD_URL\">here</a> to download the Mono framework" --title="Can't open browser"
        fi
    fi
	echo "$VERSION_TITLE"
	echo "$VERSION_MSG"
	exit 1
fi

# If this is the first run, ensure that the `mozroots --import --sync` is run prior to launching the app
FD_FIRST_RUN="${XDG_CONFIG_HOME:-$HOME/.config}/flickr-downloadr/firstrun"
if [ ! -f "$FD_FIRST_RUN" ]; then
  mkdir -p `dirname "$FD_FIRST_RUN"`
  echo $(date +"%Y-%m-%d_%H-%M-%S") > ${FD_FIRST_RUN}
  mozroots --import --sync
fi

# this script should be in $PREFIX/bin
MONO_EXEC="exec -a flickrdownloadr mono-sgen"
EXE_PATH="${0%%/flickrdownloadr}/bin/flickr-downloadr.exe"

_FD_REDIRECT_LOG="${FD_REDIRECT_LOG:-${XDG_CONFIG_HOME:-$HOME/.config}/flickr-downloadr/log}"

if [ -n "$_FD_REDIRECT_LOG" ]; then
	mkdir -p `dirname "$_FD_REDIRECT_LOG"`
	$MONO_EXEC $MONO_OPTIONS "$EXE_PATH" $* 2>&1 | tee "$_FD_REDIRECT_LOG"
else
	$MONO_EXEC $MONO_OPTIONS "$EXE_PATH" $*
fi
