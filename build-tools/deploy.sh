SOURCE_BRANCH="source"

REPO="https://github.com/flickr-downloadr/flickr-downloadr.github.io.git"
SOURCEREPO="https://github.com/flickr-downloadr/flickr-downloadr-gtk.git"

CIENGINE="appveyor"
if [[ $TRAVIS = true ]]
then
  echo "The TRAVIS_COMMIT variable has a value of - ${TRAVIS_COMMIT}"
  CIENGINE="travis"
  brew update 2>&1 1> "brewupdate.log"
  brew install jq 2>&1 1> "brewinstall.log"
  echo "About to run: curl https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/${TRAVIS_COMMIT} | jq -r '.commit.message'"
  APPVEYOR_REPO_COMMIT_MESSAGE=$(curl -u ${GH_TOKEN}:x-oauth-basic https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/$TRAVIS_COMMIT | jq -r '.commit.message')
elif [[ $WERCKER = true ]]
then
  echo "The WERCKER_GIT_COMMIT variable has a value of - ${WERCKER_GIT_COMMIT}"
  CIENGINE="wercker"
  wget "http://stedolan.github.io/jq/download/linux64/jq"
  chmod +x jq
  echo "About to run: curl https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/${WERCKER_GIT_COMMIT} | jq -r '.commit.message'"
  APPVEYOR_REPO_COMMIT_MESSAGE=$(curl -u ${GH_TOKEN}:x-oauth-basic https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/$WERCKER_GIT_COMMIT | ./jq -r '.commit.message')
fi

echo "CI Server      : ${CIENGINE}."
echo "Commit Message : '${APPVEYOR_REPO_COMMIT_MESSAGE}'"

if [[ $APPVEYOR_REPO_COMMIT_MESSAGE != *\[deploy\]* ]]
then
  echo 'There is nothing to deploy here. Moving on!';
  exit
fi

echo "Beginning Deploy..."

git config --global user.name "The CI Bot"
git config --global user.email "contact.us@flickrdownloadr.com"

VERSION="v${BUILDNUMBER}"
DEPLOYVERSION="deploy-${VERSION}"

# wercker seems to be cloning to /pipeline/build
THISREPOCLONEDIR="flickr-downloadr-gtk"
if [[ $WERCKER = true ]]
then
  THISREPOCLONEDIR="build"
fi

cd ../..
git clone -b $SOURCE_BRANCH $REPO
cd flickr-downloadr.github.io
git config credential.helper "store --file=.git/fd-credentials"
echo "https://${GH_TOKEN}:@github.com" > .git/fd-credentials
git config push.default tracking
git checkout -b ${DEPLOYVERSION}
rm -rf "../${THISREPOCLONEDIR}/dist/osx/Install flickr downloadr (${VERSION}).app"
cp -r ../${THISREPOCLONEDIR}/dist/* ./app/installer
git add -f .
git commit -m "created release ${VERSION} ($CIENGINE) [skip ci]" -s
git ls-remote --heads origin | grep ${DEPLOYVERSION} && git pull --rebase origin ${DEPLOYVERSION}
git ls-remote --heads origin | grep ${DEPLOYVERSION} && git push origin ${DEPLOYVERSION} || git push -u origin ${DEPLOYVERSION}

# Do the below script only from Travis - updates source to mark the current released version
if [[ $TRAVIS = true ]]
then
  cd ..
  git clone -b master $SOURCEREPO flickr-downloadr-gtk-new
  cd flickr-downloadr-gtk-new
  git config credential.helper "store --file=.git/fd-credentials"
  echo "https://${GH_TOKEN}:@github.com" > .git/fd-credentials
  git config push.default tracking
  cp -f ../flickr-downloadr-gtk/build-tools/build.number ./build-tools/
  cp -f ../flickr-downloadr-gtk/source/CommonAssemblyInfo.cs ./source/
  git add -f .
  git commit -m "Released ${VERSION} [skip ci]" -s
  git tag -a ${VERSION} -m "Creating release ${VERSION}"
  git push --tags origin master
fi

echo "Deployed $VERSION successfully"
exit
