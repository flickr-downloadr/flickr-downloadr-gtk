$installBuilderCli = "C:\Program Files (x86)\BitRock InstallBuilder Enterprise 8.6.0\bin\builder-cli.exe"
$pathToBuildFile = "flickrdownloadr.xml"
$pathToLicenseFile = "flickrdownloadrlicense.xml"

& $installBuilderCli build $pathToBuildFile --license $pathToLicenseFile
