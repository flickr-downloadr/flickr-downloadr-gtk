$installBuilderCli = "C:\Program Files (x86)\BitRock InstallBuilder Enterprise 8.6.0\bin\builder-cli.exe"
$pathToBuildFile = "flickrdownloadr.xml"
$pathToLicenseFile = "flickrdownloadrlicense.xml"
$additionalVars = "--setvars project.version=$env:BUILDNUMBER"

$platform = "windows"
If($args.Length -eq 1)
 {
   $platform = $args[0]
 }

If(Test-Path $pathToLicenseFile)
 {
   Write-Host "About to run: $installBuilderCli build $pathToBuildFile $platform $additionalVars --license $pathToLicenseFile"
   & $installBuilderCli build $pathToBuildFile $platform $additionalVars --license $pathToLicenseFile
 }
Else
 {
   Write-Host "About to run: $installBuilderCli build $pathToBuildFile $platform $additionalVars"
   & $installBuilderCli build $pathToBuildFile $platform $additionalVars
 }
