if [[ $TRAVIS = true ]]
then
  brew update > brewinstall
  brew install jq
  APPVEYOR_REPO_COMMIT_MESSAGE=$(curl https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/$TRAVIS_COMMIT | jq -r '.commit.message')
fi

if [[ $APPVEYOR_REPO_COMMIT_MESSAGE != *\[deploy\]* ]]
  then
    echo 'There is nothing to deploy here. Moving on!';
    exit
fi

git config --global user.name "The CI Bot"
git config --global user.email "contact.us@flickrdownloadr.com"

VERSION="v${BUILDNUMBER}"
DEPLOYVERSION="deploy-${VERSION}"

cd ../..
git clone -b master $REPO
cd flickr-downloadr.github.io
git config credential.helper "store --file=.git/fd-credentials"
echo "https://${GH_TOKEN}:@github.com" > .git/fd-credentials
git config push.default tracking
git checkout -b ${DEPLOYVERSION}
cp -r ../flickr-downloadr-gtk/dist/* ./installer
# TODO: Do the build.number later
# cp ../flickr-downloadr-gtk/build-tools/build.number .
git add -f .
git commit -m "created release ${VERSION} (travis) [ci skip]" -s
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
  git commit -m "Released ${VERSION} [ci skip]" -s
  git tag -a ${VERSION} -m "Creating release ${VERSION}"
  git push --tags origin master
fi

echo "Deployed $VERSION successfully"
exit
