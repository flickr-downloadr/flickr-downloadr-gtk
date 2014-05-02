#!/usr/bin/env bash

# Copied from the MonoDevelop Launch Script

#Workaround for Unity gnome shell
export UBUNTU_MENUPROXY=0

# Ubuntu overlay scrollbars are not working well with the 1px boundaries in the monodevelop shell
export LIBOVERLAY_SCROLLBAR=0

# The Oxygen GTK theme crashes unless this is set
export OXYGEN_DISABLE_INNER_SHADOWS_HACK=1

#this script should be in $PREFIX/bin
MONO_EXEC="exec -a 'flickr downloadr' mono-sgen"
EXE_PATH="${0%%/flickrdownloadr}/bin/flickr-downloadr.exe"

_FD_REDIRECT_LOG="${FD_REDIRECT_LOG:-${XDG_CONFIG_HOME:-$HOME/.config}/flickr-downloadr/log}"

if [ -n "$_FD_REDIRECT_LOG" ]; then
	mkdir -p `dirname "$_FD_REDIRECT_LOG"`
	$MONO_EXEC $MONO_OPTIONS "$EXE_PATH" $* 2>&1 | tee "$_FD_REDIRECT_LOG"
else
	$MONO_EXEC $MONO_OPTIONS "$EXE_PATH" $*
fi
