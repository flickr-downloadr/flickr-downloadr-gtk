Remove-Item nuget.exe
(new-object System.Net.WebClient).DownloadFile('http://nuget.org/nuget.exe','nuget.exe')
cd ../source
../build/nuget.exe restore "FloydPink.Flickr.Downloadr.sln" -verbosity detailed
