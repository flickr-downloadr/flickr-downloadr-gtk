#!/bin/bash

# Thanks to http://docs.travis-ci.com/user/common-build-problems/#Mac%3A-Code-Signing-Errors

KEY_CHAIN="flickr-downloadr.keychain"
# Make the keychain the default so identities are found
security default-keychain -s ${KEY_CHAIN}
# Unlock the keychain
security unlock-keychain -p ${TRAVIS_COMMIT} ${KEY_CHAIN}
# Set keychain locking timeout to 3600 seconds
security set-keychain-settings -t 3600 -u ${KEY_CHAIN}
