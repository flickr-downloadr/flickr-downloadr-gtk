(new-object System.Net.WebClient).DownloadFile('http://flickrdownloadr.com/installer/win/dependencies/gtk-sharp-2.12.25.msi','C:\projects\flickr-downloadr-gtk\gtk-sharp-2.12.25.msi')
msiexec /i "C:\projects\flickr-downloadr-gtk\gtk-sharp-2.12.25.msi" /quiet /qn /norestart /log install.log
