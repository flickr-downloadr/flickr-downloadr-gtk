#!/bin/sh

# Thanks to http://stackoverflow.com/a/27778139/218882

KEY_CHAIN="flickr-downloadr.keychain"
security create-keychain -p travis ${KEY_CHAIN}
# Make the keychain the default so identities are found
security default-keychain -s ${KEY_CHAIN}
# Unlock the keychain
security unlock-keychain -p travis ${KEY_CHAIN}
# Set keychain locking timeout to 3600 seconds
security set-keychain-settings -t 3600 -u ${KEY_CHAIN}

# Add certificates to keychain and allow codesign to access them
security import "./X47E7X2X6J-code-signing-certificate.p12" -k ${KEY_CHAIN} -P ${FD_CERT_PWD} -T /usr/bin/codesign
