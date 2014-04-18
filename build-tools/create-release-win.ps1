$installBuilderExe = "http://installbuilder.bitrock.com/installbuilder-enterprise-8.6.0-windows-installer.exe"
$installBuilderExeLocal = "C:\projects\flickr-downloadr-gtk\installbuilder-enterprise-8.6.0-windows-installer.exe"
$installBuilderInstallLog = "C:\projects\flickr-downloadr-gtk\installbuilder.log"
$installBuilderCli = "C:\Program Files (x86)\BitRock InstallBuilder Enterprise 8.6.0\bin\builder-cli.exe"
$pathToBuildFile = "flickrdownloadr.xml"


(new-object System.Net.WebClient).DownloadFile($installBuilderExe,$installBuilderExeLocal)
& $installBuilderExeLocal --mode unattended --unattendedmodeui none --debuglevel 4 --debugtrace $installBuilderInstallLog
type $installBuilderInstallLog
& $installBuilderCli build $pathToBuildFile
