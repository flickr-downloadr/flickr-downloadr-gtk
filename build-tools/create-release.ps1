$installBuilderCli = "C:\Program Files (x86)\VMware InstallBuilder Enterprise 20.12.0\bin\builder-cli.exe"
$pathToBuildFile = "flickrdownloadr.xml"
$pathToLicenseFile = "flickrdownloadrlicense.xml"

$platform = "windows"
If($args.Length -eq 1)
 {
   $platform = $args[0]
 }

$packVariable = "pack_$($platform)_platform_files"

If(Test-Path $pathToLicenseFile)
 {
   Write-Host "About to execute: $installBuilderCli build $pathToBuildFile $platform --license $pathToLicenseFile --setvars project.version=$env:BUILDNUMBER $packVariable=true runSignTool=0"
   & $installBuilderCli build $pathToBuildFile $platform --license $pathToLicenseFile --setvars project.version=$env:BUILDNUMBER $packVariable=true runSignTool=0
 }
Else
 {
   Write-Host "About to execute: $installBuilderCli build $pathToBuildFile $platform --setvars project.version=$env:BUILDNUMBER  $packVariable=true runSignTool=0"
   & $installBuilderCli build $pathToBuildFile $platform --setvars project.version=$env:BUILDNUMBER  $packVariable=true runSignTool=0
 }
