$solutionDir = "..\source"                      # this is where my code lives
$buildOutputDir = "..\source\bin\Release" # this is where the build output lives
$releasesDir = ".\Release"          # publish to a folder outside the repository

$script =  Get-ChildItem "$solutionDir\packages\\" `
                -Filter "Create-Release.ps1" `
                -Recurse | Select-Object -first 1

. $script.FullName -SolutionDir $solutionDir -BuildDir $buildOutputDir ` 
                   -ReleasesDir $releasesDir
