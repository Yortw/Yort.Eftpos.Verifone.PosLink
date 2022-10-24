@echo off
echo Press any key to publish
pause
".nuget\NuGet.exe" push Yort.Eftpos.Verifone.PosLink.1.0.4.nupkg -Source https://www.nuget.org/api/v2/package
pause