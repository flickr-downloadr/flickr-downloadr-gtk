#!/bin/bash

echo "Installing dependencies for flickr downloadr..."
echo "Packages to be installed are: ${1}"
sudo apt-get -y install ${1}
mozroots --import --ask-remove
