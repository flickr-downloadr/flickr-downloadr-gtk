if (Test-Path ../source/.nuget/NuGet.exe) {
  Remove-Item ../source/.nuget/NuGet.exe
}
(new-object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe','../source/.nuget/NuGet.exe')
cd ../source
./.nuget/NuGet.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
