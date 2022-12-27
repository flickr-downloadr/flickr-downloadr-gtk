### Automated Build Process

Every commit to the `main` branch kicks off CI builds (that builds and runs all the unit tests) in three different CI services:

 - [CircleCI](https://circleci.com/gh/flickr-downloadr/flickr-downloadr-gtk/tree/main) for Linux
 - [GitHub Actions](https://github.com/flickr-downloadr/flickr-downloadr-gtk/actions/workflows/ci.yml) for macOS
 - [AppVeyor](https://ci.appveyor.com/project/floydpink/flickr-downloadr-gtk) for Windows

Any commit that has the string `[deploy]` in the commit message will also build the installers for all three supported platforms, using [BitRock InstallBuilder](http://installbuilder.bitrock.com/).

The installers will be committed separately from the CI builds to a new branch named `deploy-v<VERSION>` (as an example, for the version`v1.0.2.1`, the new branch will be `deploy-v1.0.2.1`) on the [`flickr-downloadr/flickr-downloadr.github.io`](https://github.com/flickr-downloadr/flickr-downloadr.github.io) repository.

[A custom webhook](https://github.com/flickr-downloadr/github-webhook) on the `flickr-downloadr/flickr-downloadr.github.io` repo that runs on Heroku ensures that installers for all three platforms have been built successfully and then makes a commit with the name of the new branch, updated into the `branch` file on the [`flickr-downloadr/releases`](https://github.com/flickr-downloadr/releases) repository (`deploy-v1.0.2.1`, in the example above) and sends an email a deployment will be ready to be pushed to the website soon [[see here](https://github.com/flickr-downloadr/github-webhook/blob/c88f106965878d62992db286fcdbca02385def1a/deploy/index.js#L59)].

In the event that any of the CI builds fail to create the installer, after at least one installer gets successfully built and committed, the custom webhook waits for 30 minutes from the first installer getting committed and sends an email notifying of a build failure [[see here](https://github.com/flickr-downloadr/github-webhook/blob/c88f106965878d62992db286fcdbca02385def1a/helpers/index.js#L68)].

Yet another webhook on the `flickr-downloadr/releases` repo kicks off another build on [CircleCI](https://circleci.com/gh/flickr-downloadr/flickr-downloadr-gtk/tree/main) to merge the new branch with all three installers (`deploy-v1.0.2.1` branch) into the `source` branch on `flickr-downloadr/flickr-downloadr.github.io` to make it ready for push into the `master` branch that runs [the main website](http://flickrdownloadr.com). This CI job does a few other things like archiving this version to [SourceForge](http://sourceforge.net/projects/flickr-downloadr/files/) etc., as can be seen [here](https://github.com/flickr-downloadr/releases/blob/master/wercker.yml).

The final push to the website is manual and can be done by running `grunt deploy` on the latest, merged version of the `source` branch and this will make the latest version installers current on the website.
