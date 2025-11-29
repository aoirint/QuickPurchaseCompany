# SkipDropshipCompany

A Lethal Company mod that skips the dropship when you're on the Company or in other limited situations.

- [User guide](./assets/README.md)

## Development

Install .NET SDK 10.0 or later.

- <https://dotnet.microsoft.com/en-us/download/dotnet/10.0>

Restore NuGet packages.

```powershell
dotnet restore --locked-mode
```

## Code format

- Language version: [C# 13.0](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-13)
- Target framework: [.NET standard 2.1](https://learn.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-1)

```powershell
dotnet format
```

## Package management

To update the lock file after modifying your package references, run:

```powershell
dotnet restore --use-lock-file
```

## GitHub Actions management

The repository uses GitHub Actions for CI.

The version of the actions are pinned with [pinact](https://github.com/suzuki-shunsuke/pinact).

```powershell
# Pin
pinact run

# Update
pinact run --update
```

## Build

```powershell
# Debug build
DOTNET_CLI_UI_LANGUAGE=en dotnet build

# Release build
DOTNET_CLI_UI_LANGUAGE=en dotnet build --configuration Release
```

## Release

1. Replace version in `SkipDropshipCompany/SkipDropshipCompany.csproj` as semver format, e.g. `1.2.3`.
2. Commit and push the changes.
3. CI will create a GitHub Release automatically.
4. Download the release artifact from the GitHub Release page.
5. Upload the artifact to Thunderstore. **NOTE: prerelease version is not supported, e.g. `1.2.3-beta.1`.**

## Debugging

### r2modman

1. Open r2modman.
2. Open `Config editor`.
3. Open `BepInEx\config\BepInEx.cfg` in the config list.
4. Set `Logging.Console.LogLevels` to `All`.
5. Open `Settings > Import local mod`.
6. Select the DLL file from `bin/Debug/netstandard2.1/`.
7. Click `Start modded`.

### Manual

1. Install BepInEx: https://docs.bepinex.dev/articles/user_guide/installation/index.html
2. Launch `Lethal Company.exe` and exit to generate the BepInEx config files.
3. Open `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/BepInEx/config/BepInEx.cfg`.
4. Copy the DLL file into `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/BepInEx/plugins/` from `bin/Debug/netstandard2.1/`.
5. Set `Logging.Console.Enabled` to `true`.
6. Set `Logging.Console.LogLevels` to `All`.
7. Launch `Lethal Company.exe` again.

## Test Scenarios

TODO: Add test scenarios for the configuration options.

### 1. Purchase on the first day orbit

1. Start a new game.
2. Open the terminal.
3. Issue a command: `wal`.
4. Confirm that a walkie is spawned on the ship.

### 2. Purchase on the first day orbit after routed

1. Start a new game.
2. Open the terminal.
3. Issue a command: `ass`.
4. Issue a command: `wal`.
5. Confirm that a walkie is spawned on the ship.

### 3. Purchase on the first day company

1. Start a new game.
2. Open the terminal.
3. Issue a command: `comp`.
4. Land on `Gordion`.
5. Open the terminal.
6. Issue a command: `wal`.
7. Confirm that a walkie is spawned on the ship.

### 4. Purchase on the first day moon

1. Start a new game.
2. Land on `Experimentation`.
3. Open the terminal.
4. Issue a command: `wal`.
5. Issue a command: `help`.
6. Confirm that the terminal shows `1 purchased items on route.`.

### 5. Purchase on the second day orbit after landing on the company

1. Start a new game.
2. Open the terminal.
3. Issue a command: `comp`.
4. Land on `Gordion`.
5. Take off.
6. Open the terminal.
7. Issue a command: `wal`.
8. Confirm that a walkie is spawned on the ship.

### 6. Purchase on the second day orbit after landing on a moon

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `wal`.
6. Issue a command: `help`.
7. Confirm that the terminal shows `1 purchased items on route.`.

### 7. Purchase on the third day orbit after landing on the company

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `comp`.
6. Land on `Gordion`.
7. Take off.
8. Open the terminal.
9. Issue a command: `wal`.
10. Confirm that a walkie is spawned on the ship.

### 8. Purchase on the third day orbit after landing on the company and routing to a moon

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `comp`.
6. Land on `Gordion`.
7. Take off.
8. Open the terminal.
9. Issue a command: `exp`.
10. Issue a command: `wal`.
11. Issue a command: `help`.
12. Confirm that the terminal shows `1 purchased items on route.`.

### 9. Purchase on the first day after ejected

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `eject`.
6. Wait until ejection is completed.
7. Open the terminal.
8. Issue a command: `wal`.
9. Confirm that a walkie is spawned on the ship.

### 10. Purchase on the second day orbit then land on the company

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `wal`.
6. Issue a command: `help`.
7. Confirm that the terminal shows `1 purchased items on route.`.
8. Issue a command: `comp`.
9. Land on `Gordion`.
10. Confirm that the dropship arrives and contains a walkie.

### 11. Purchase on the second day orbit then land and purchase on the company

1. Start a new game.
2. Land on `Experimentation`.
3. Take off.
4. Open the terminal.
5. Issue a command: `wal`.
6. Issue a command: `help`.
7. Confirm that the terminal shows `1 purchased items on route.`.
8. Issue a command: `comp`.
9. Pull the lever to land on `Gordion`.
10. Open the terminal and Issue a command `wal` before the dropship arrives.
11. Issue a command: `help`.
12. Confirm that the terminal does not show `purchased items on route.`.
13. Confirm that two walkies are spawned on the ship.
