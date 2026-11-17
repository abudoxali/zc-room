$ErrorActionPreference = 'Stop'
$csc = Join-Path $env:WINDIR 'Microsoft.NET\Framework\v4.0.30319\csc.exe'
& $csc /nologo /target:winexe /platform:anycpu /optimize+ /out:ZewailCityLauncher.exe /r:System.Windows.Forms.dll /r:System.Drawing.dll /r:System.IO.Compression.dll /r:System.IO.Compression.FileSystem.dll /r:Microsoft.CSharp.dll Launcher.cs
if ($LASTEXITCODE -ne 0) { throw 'Launcher build failed' }
