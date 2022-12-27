SOURCE_BRANCH="source"

REPO="https://github.com/flickr-downloadr/flickr-downloadr.github.io.git"
SOURCEREPO="https://github.com/flickr-downloadr/flickr-downloadr-gtk.git"

CIENGINE="appveyor"
if [[ $GITHUB_WORKFLOW = 'ci cd' ]]
then
  echo "The GITHUB_COMMIT_MESSAGE variable has a value of - ${GITHUB_COMMIT_MESSAGE}"
  CIENGINE="github"
  APPVEYOR_REPO_COMMIT_MESSAGE=${GITHUB_COMMIT_MESSAGE}
elif [[ $CIRCLECI = true ]]
then
  echo "The CIRCLE_SHA1 variable has a value of - ${CIRCLE_SHA1}"
  CIENGINE="circleci"
  wget "http://stedolan.github.io/jq/download/linux64/jq"
  chmod +x jq
  echo "About to run: curl https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/${CIRCLE_SHA1} | ./jq -r '.commit.message'"
  APPVEYOR_REPO_COMMIT_MESSAGE=$(curl -u ${GH_TOKEN}:x-oauth-basic https://api.github.com/repos/flickr-downloadr/flickr-downloadr-gtk/commits/$CIRCLE_SHA1 | ./jq -r '.commit.message')
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

# circleci seems to be cloning to /root/project
THISREPOCLONEDIR="flickr-downloadr-gtk"
if [[ $CIRCLECI = true ]]
then
  THISREPOCLONEDIR="project"
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

# circleci throws 'fatal: could not read from remote repository' error
if [[ $CIRCLECI = true ]]
then
  git remote set-url origin "https://${GH_TOKEN}:@github.com/flickr-downloadr/flickr-downloadr.github.io.git"
fi

echo "Pulling/Rebasing with the remote branch..."
git ls-remote --heads origin | grep ${DEPLOYVERSION} && git pull --rebase origin ${DEPLOYVERSION}
echo "Pushing the branch to remote..."
git ls-remote --heads origin | grep ${DEPLOYVERSION} && git push origin ${DEPLOYVERSION} || git push -u origin ${DEPLOYVERSION}

# Do the below script only from GitHub - updates source to mark the current released version
if [[ $CIENGINE = 'github' ]]
then
  cd ..
  git clone -b main $SOURCEREPO flickr-downloadr-gtk-new
  cd flickr-downloadr-gtk-new
  git config credential.helper "store --file=.git/fd-credentials"
  echo "https://${GH_TOKEN}:@github.com" > .git/fd-credentials
  git config push.default tracking
  cp -f ../flickr-downloadr-gtk/build-tools/build.number ./build-tools/
  cp -f ../flickr-downloadr-gtk/source/CommonAssemblyInfo.cs ./source/
  git add -f .
  git commit -m "Released ${VERSION} [skip ci]" -s
  git tag -a ${VERSION} -m "Creating release ${VERSION}"
  git push --tags origin main
fi

echo "Deployed $VERSION successfully"
exit
