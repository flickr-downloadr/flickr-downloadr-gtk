try {
	$buildToolsDir = "C:\projects\flickr-downloadr-gtk\build-tools\"

	$gtkInstaller = "gtk-sharp-2.12.45.msi"
	$gtkInstallerMsi = "https://github.com/mono/gtk-sharp/releases/download/2.12.45/$($gtkInstaller)"
	$gtkInstallerMsiLocal = "$($buildToolsDir)$($gtkInstaller)"
	$gtkInstallerInstallLog = "$($buildToolsDir)gtkInstallerLog.log"

	$installBuilder = "installbuilder-enterprise-23.11.0-windows-x64-installer.exe"
	$installBuilderExe = "https://releases.installbuilder.com/installbuilder/$($installBuilder)"
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
