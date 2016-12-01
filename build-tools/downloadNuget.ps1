if (Test-Path ../source/.nuget/NuGet.exe) {
  Remove-Item ../source/.nuget/NuGet.exe
}
(new-object System.Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe','../source/.nuget/NuGet.exe')
cd ../source
./.nuget/NuGet.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
