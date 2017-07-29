try {
	$buildToolsDir = "C:\projects\flickr-downloadr-gtk\build-tools\"

	$gtkInstaller = "gtk-sharp-2.12.38.msi"
	$gtkInstallerMsi = "https://download.xamarin.com/GTKforWindows/Windows/$($gtkInstaller)"
	$gtkInstallerMsiLocal = "$($buildToolsDir)$($gtkInstaller)"
	$gtkInstallerInstallLog = "$($buildToolsDir)gtkInstallerLog.log"

	$installBuilder = "installbuilder-enterprise-17.7.0-windows-installer.exe"
	$installBuilderExe = "https://installbuilder.bitrock.com/$($installBuilder)"
	$installBuilderExeLocal = "$($buildToolsDir)$($installBuilder)"
	$installBuilderInstallLog = "$($buildToolsDir)installbuilder.log"

	(new-object System.Net.WebClient).DownloadFile($gtkInstallerMsi,$gtkInstallerMsiLocal)
	msiexec /i $gtkInstallerMsiLocal /quiet /qn /norestart /log $gtkInstallerInstallLog

	(new-object System.Net.WebClient).DownloadFile($installBuilderExe,$installBuilderExeLocal)
	& $installBuilderExeLocal --mode unattended --unattendedmodeui none --debuglevel 4 --debugtrace $installBuilderInstallLog
}
catch {
	throw
}
