$installBuilderCli = "C:\Program Files (x86)\BitRock InstallBuilder Enterprise 8.6.0\bin\builder-cli.exe"
$pathToBuildFile = "flickrdownloadr.xml"
$pathToLicenseFile = "flickrdownloadrlicense.xml"

$platform = "windows"
If($args.Length -eq 1)
 {
   $platform = $args[0]
 }

& $installBuilderCli build $pathToBuildFile --license $pathToLicenseFile
