$ErrorActionPreference = 'Stop'
Set-Location (Join-Path $PSScriptRoot '..')
dotnet run --project tests/Tests.csproj -c Release
if ($LASTEXITCODE) { throw 'Fallaron las pruebas del motor' }
dotnet publish src/App/App.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o artifacts/publish
if ($LASTEXITCODE) { throw 'Falló la publicación' }
$exe = Join-Path $PWD 'artifacts/publish/FestivosColombia.exe'
if (!(Test-Path $exe)) { throw 'No se generó el ejecutable' }
$stage = Join-Path $PWD 'artifacts/entrega'
if (Test-Path $stage) { Remove-Item $stage -Recurse -Force }
New-Item $stage -ItemType Directory | Out-Null
Copy-Item $exe $stage
Copy-Item README.md,VALIDACION.md,global.json $stage
Copy-Item src,tests,scripts,.github $stage -Recurse
Get-ChildItem $stage -Directory -Recurse | Where-Object { $_.Name -in @('bin','obj') } | Remove-Item -Recurse -Force
Compress-Archive -Path "$stage/*" -DestinationPath artifacts/FestivosColombia.zip -Force
Get-FileHash $exe -Algorithm SHA256 | Format-List
