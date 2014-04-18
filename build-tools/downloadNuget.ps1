Remove-Item ../source/.nuget/NuGet.exe
(new-object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe','../source/.nuget/NuGet.exe')
cd ../source
../build/nuget.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
