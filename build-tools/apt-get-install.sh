#!/bin/bash
echo "Packages to be installed are: ${1}"
sudo apt-get -y install $1
