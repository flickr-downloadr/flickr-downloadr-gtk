#!/bin/bash

echo "Installing dependencies for flickr downloadr..."
echo "Packages to be installed are: ${1}"

gnome-terminal -e "sudo apt-get -y install ${1}"
wait
mozroots --import --ask-remove
