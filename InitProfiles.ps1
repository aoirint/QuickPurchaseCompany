# Initialize BeplnEx profiles

$ErrorActionPreference = 'Stop'

$ProfileContainerDir = Join-Path $PSScriptRoot "profiles"

$BepInExVersion = "5.4.21"
$BepInExAssetName = "BepInEx_x64_5.4.21.0.zip"
$BepInExSha256Expected = "2af69fe0aaf821e95c4cd3e4d53860e667c54648f97dca6f971a5bcd3c22aa34"

# Thunderstore mods to install
$mods = @(
  @{
    Name = 'LCBetterSaves'
    Id = 'Pooble-LCBetterSaves'
    Version = '1.7.3'
    Sha256 = '502c75b79c3a89ccce484893df020adcdb8eade9d3a10ea39f74110efe77b5a6'
    CopyPaths = @('LCBetterSaves.dll')
  },
  @{
    Name = 'LethalCompanyInputUtils'
    Id = 'Rune580-LethalCompany_InputUtils'
    Version = '0.7.12'
    Sha256 = 'c185134830c1bffd47a30872b1b48bc727dc64cb728c9192bb7fdc88bcdbda20'
    CopyPaths = @('plugins/LethalCompanyInputUtils')
  },
  @{
    Name = 'OdinSerializer'
    Id = 'Lordfirespeed-OdinSerializer'
    Version = '2024.2.2700'
    Sha256 = '302446d2191906a0a98b4179aeb8d11d9db03617b61cac35886b03f8afea1273'
    CopyPaths = @('BepInEx/core/OdinSerializer')
  }
  @{
    Name = 'LethalNetworkAPI'
    Id = 'xilophor-LethalNetworkAPI'
    Version = '3.3.2'
    Sha256 = '0b4368904b719577c52a3189c35ac4dc6d6d8dd93409643237a8c5f01516a6c1'
    CopyPaths = @('BepInEx/plugins/LethalNetworkAPI/LethalNetworkAPI.dll')
  }
  @{
    Name = 'Imperium'
    Id = 'giosuel-Imperium'
    Version = '1.1.1'
    Sha256 = '27378e9b0f854829aff91a925648440399e7d2dd598b549ded4ccdcbb61f2b17'
    CopyPaths = @('giosuel.Imperium.dll')
  }
)

function Download-And-ExtractMod($mod) {
  $url = "https://gcdn.thunderstore.io/live/repository/packages/$($mod.Id)-$($mod.Version).zip"
  $zipPath = Join-Path $env:TEMP ("mod_zip_" + [guid]::NewGuid().ToString() + ".zip")
  try {
    Invoke-WebRequest -Uri $url -OutFile $zipPath
  } catch {
    Write-Error "Failed to download $($mod.Name): $_"
    exit 1
  }

  $hash = (Get-FileHash -Path $zipPath -Algorithm SHA256).Hash.ToLower()
  if ($hash -ne $mod.Sha256.ToLower()) {
    Write-Error "$($mod.Name) hash mismatch. Expected: $($mod.Sha256), Actual: $hash"
    Remove-Item -Path $zipPath -Force
    exit 1
  }

  $extractDir = Join-Path $env:TEMP ("mod_" + [guid]::NewGuid().ToString())
  try {
    Expand-Archive -Path $zipPath -DestinationPath $extractDir -Force
  } catch {
    Write-Error "Failed to extract $($mod.Name): $_"
    exit 1
  }
  Remove-Item -Path $zipPath -Force
  $mod.ExtractedDir = $extractDir
}

# Download BepInEx
$BepInExUrl = "https://github.com/BepInEx/BepInEx/releases/download/v${BepInExVersion}/${BepInExAssetName}"
$TempBepInExZipFile = Join-Path $env:TEMP ("BepInEx_zip_" + [guid]::NewGuid().ToString() + ".zip")
try {
  Invoke-WebRequest -Uri $BepInExUrl -OutFile $TempBepInExZipFile
} catch {
  Write-Error "Failed to download BepInEx: $_"
  exit 1
}

$BepInExHash = (Get-FileHash -Path $TempBepInExZipFile -Algorithm SHA256).Hash.ToLower()
if ($BepInExHash -ne $BepInExSha256Expected.ToLower()) {
  Write-Error "BepInEx hash mismatch. Expected: $BepInExSha256Expected, Actual: $BepInExHash"
  Remove-Item -Path $TempBepInExZipFile -Force
  exit 1
}

$TempBepInExDir = Join-Path $env:TEMP  ("BepInEx_" + [guid]::NewGuid().ToString())
try {
  Expand-Archive -Path $TempBepInExZipFile -DestinationPath $TempBepInExDir -Force
} catch {
  Write-Error "Failed to extract BepInEx: $_"
  exit 1
}
Remove-Item -Path $TempBepInExZipFile -Force

# Initialize profile directories
if (-not (Test-Path $ProfileContainerDir)) {
  New-Item -ItemType Directory -Path $ProfileContainerDir
}
for ($i = 1; $i -le 2; $i++) {
  $ProfileDir = Join-Path $ProfileContainerDir ("profile_" + $i)

  if (Test-Path $ProfileDir) {
    Remove-Item -Path $ProfileDir -Recurse -Force
  }

  New-Item -ItemType Directory -Path $ProfileDir | Out-Null
  Copy-Item -Path (Join-Path $TempBepInExDir "*") -Destination $ProfileDir -Recurse
}
Remove-Item -Path $TempBepInExDir -Recurse -Force

# Download and extract mods
foreach ($m in $mods) {
  Download-And-ExtractMod -mod $m
}

# Install mods
for ($i = 1; $i -le 2; $i++) {
  $ProfileDir = Join-Path $ProfileContainerDir ("profile_" + $i)
  $PluginsDir = Join-Path $ProfileDir "BepInEx\plugins"

  if (-not (Test-Path $PluginsDir)) {
    New-Item -ItemType Directory -Path $PluginsDir | Out-Null
  }

  foreach ($m in $mods) {
    foreach ($rel in $m.CopyPaths) {
      $src = Join-Path $m.ExtractedDir $rel
      if (Test-Path $src) {
        Copy-Item -Path $src -Destination $PluginsDir -Recurse -Force
      } else {
        Write-Warning "Source not found for $($m.Name): $src"
      }
    }
  }
}

# Remove temporary mod extraction directories
foreach ($m in $mods) {
  if ($m.ExtractedDir -and (Test-Path $m.ExtractedDir)) {
    Remove-Item -Path $m.ExtractedDir -Recurse -Force
  }
}
