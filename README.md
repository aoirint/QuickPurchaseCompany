# QuickPurchaseCompany

Lethal Company Mod to make the item purchase process faster in a fair way.

## Features

- TODO: Directly spawn the purchased items on the ship.
  - The first day orbit
  - Landed on the Company
  - The next day orbit after landing on the company and before routing to a moon

## Development

Create a new directory `lib` at the repository root.
Copy a DLL file into `lib/` from `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company`.

- `Lethal Company_Data/Managed/Assembly-CSharp.dll`

## Build

```powershell
# Debug build
DOTNET_CLI_UI_LANGUAGE=en dotnet build

# Release build
DOTNET_CLI_UI_LANGUAGE=en dotnet build --configuration Release
```

## Install

### Manual

1. Install BeplnEx: https://docs.bepinex.dev/articles/user_guide/installation/index.html
2. Copy the DLL file into `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/BepInEx/plugins/` from `bin/Release/netstandard2.1/`.

### r2modman

1. Open `Settings > Import local mod`.
2. Select the DLL file from `bin/Release/netstandard2.1/`.
