Remove-Item ../source/.nuget/NuGet.exe
(new-object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe','../source/.nuget/NuGet.exe')