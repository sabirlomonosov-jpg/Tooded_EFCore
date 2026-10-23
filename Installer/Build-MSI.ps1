# Käivita Windowsi PowerShellis, kui .NET 8 SDK ja WiX Toolset 3.11 on paigaldatud.
# SQL Server Express LocalDB peab olema sihtarvutisse eraldi paigaldatud.
$ErrorActionPreference = "Stop"

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$project = Join-Path $root "Tooded_EFCore\Tooded_EFCore.csproj"
$publish = Join-Path $PSScriptRoot "publish"
$objDir = Join-Path $PSScriptRoot "obj"
$outDir = Join-Path $PSScriptRoot "Release"
New-Item -ItemType Directory -Force -Path $publish, $objDir, $outDir | Out-Null

$wix = if ($env:WIX) { Join-Path $env:WIX 'bin' } else { 'C:\Program Files (x86)\WiX Toolset v3.11\bin' }
$heat = Join-Path $wix 'heat.exe'
$candle = Join-Path $wix 'candle.exe'
$light = Join-Path $wix 'light.exe'
foreach ($file in @($heat, $candle, $light)) {
    if (!(Test-Path $file)) { throw "WiX Toolset 3.11 puudub: $file" }
}

dotnet publish $project -c Release -r win-x64 --self-contained true -p:PublishSingleFile=false -o $publish
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish ebaõnnestus' }

$harvest = Join-Path $objDir 'PublishedFiles.wxs'
& $heat dir $publish -cg PublishedFiles -dr INSTALLFOLDER -srd -scom -sreg -gg -var var.PublishDir -out $harvest -nologo
if ($LASTEXITCODE -ne 0) { throw 'WiX heat ebaõnnestus' }

$main = Join-Path $PSScriptRoot 'ToodedInstaller.wxs'
& $candle -nologo -arch x64 "-dPublishDir=$publish" "-out" "$objDir\" $main $harvest
if ($LASTEXITCODE -ne 0) { throw 'WiX candle ebaõnnestus' }

& $light -nologo -out (Join-Path $outDir 'ToodedInstaller.msi') `
    (Join-Path $objDir 'ToodedInstaller.wixobj') (Join-Path $objDir 'PublishedFiles.wixobj')
if ($LASTEXITCODE -ne 0) { throw 'WiX light ebaõnnestus' }

Write-Host "MSI valmis: $(Join-Path $outDir 'ToodedInstaller.msi')"
Write-Warning 'MSI ei installi SQL Server Express LocalDB-d. Paigalda LocalDB sihtarvutisse enne rakendust.'
