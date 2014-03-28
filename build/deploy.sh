PATH=$HOME/bin:$ADDPATH:$PATH

if [[ $APPVEYOR_REPO_COMMIT_MESSAGE != *\[deploy\]* ]]
then
  echo 'There is nothing to deploy here. Moving on!';
  exit
fi

git config --global user.email "contact.us@flickrdownloadr.com"
git config --global user.name "The CI Bot"

cd ../source/bin/Release
REPO=https://$OAUTH_TOKEN:x-oauth-basic@github.com/flickr-downloadr/flickr-downloadr.git
VERSION="v${BUILDNUMBER}"
MSG="application ($VERSION)"

#clone repo in a tmp dir
echo 'Cloning gh-pages branch'
mkdir tmp-gh-pages
cd tmp-gh-pages
git clone -b gh-pages $REPO
cd flickr-downloadr
git config push.default tracking

#remove all files except index.html in downloads/latest
echo 'Deleting the previous version artifacts'
mv downloads/latest/index.html .
cd downloads/latest/
git rm -r .
cd ../..
mv index.html downloads/latest

#add published files & build.number to gh-pages; commit; push
echo 'Creating the correct changeset from built artifacts'
cp -r ../../Deploy/* ./downloads/latest
cp ../../../../../build/build.number .
git add -f .
git commit -m "deploying $MSG" -s
git push

#checkout master to add the modified build.number and CommonAssemblyInfo; commit; push
echo 'Check out master branch and commit the changed Assembly Info and build.number'
git checkout master
cp ../../../../../build/build.number ./build
cp ../../../../CommonAssemblyInfo.cs ./source
git commit -a -m "deploying $MSG [ci skip]" -s
git tag -a $VERSION -m "tagging version $VERSION"
git push --tags origin master

#remove the tmp dir
echo 'Cleaning up...'
cd ../..
rm -rf tmp-gh-pages

#reset the modified build.number and CommonAssemblyInfo in the main (outer) repo
cd ../../..
git checkout -- build/build.number source/CommonAssemblyInfo.cs

# done
echo "deployed $MSG successfully"
exit
