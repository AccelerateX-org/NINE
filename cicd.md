
# Local Build

Das folgende Kommando geht in der Developer PowerShell aus Visual Studio heraus.
In einer "normalen" PowerShell und/oder CommandLine geht es nicht, weil der Pfad auf MSbuild fehlt.

msbuild /p:Configuration=Release ./Sources/MyStik.TimeTable.sln

"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\MSBuild.exe"
"C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin\amd64\MSBuild.exe"


zuerst den Pfad setzen, dann klappt das Kommando

$env:PATH += ";C:\Program Files (x86)\Microsoft Visual Studio\2019\BuildTools\MSBuild\Current\Bin"

damit lässt sich msbuild ausführen
es gibt dann aber probleme mit dem import variables in den csprojs