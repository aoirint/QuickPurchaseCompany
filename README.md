# QuickPurchaseCompany

A Lethal Company mod that speeds up item purchases when you're on the Company or in other limited situations only.

## Features

### Instant Item Delivery

Purchased items are spawned directly on the ship.

#### When It Activates

This feature activates when any of these conditions are met:

- In orbit on the first day
- While landed on the Company
- In orbit on the next day after landing on the Company, still routing to the Company

## Development

Create a new directory `lib` at the repository root.
Copy a DLL file into `lib/` from `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company`.

- `Lethal Company_Data/Managed/Assembly-CSharp.dll`
- `Lethal Company_Data/Managed/Unity.Mathematics.dll`
- `Lethal Company_Data/Managed/Unity.Netcode.Runtime.dll`

## Build

```powershell
# Debug build
DOTNET_CLI_UI_LANGUAGE=en dotnet build

# Release build
DOTNET_CLI_UI_LANGUAGE=en dotnet build --configuration Release
```

## Debugging

### r2modman

1. Open r2modman.
2. Open `Config editor`.
3. Open `BeplnEx\config\BepInEx.cfg` in the config list.
4. Set `Logging.Console.LogLevels` to `All`.
5. Open `Settings > Import local mod`.
6. Select the DLL file from `bin/Debug/netstandard2.1/`.
7. Click `Start modded`.

### Manual

1. Install BeplnEx: https://docs.bepinex.dev/articles/user_guide/installation/index.html
2. Launch `Lethal Company.exe` and exit to generate the BeplnEx config files.
3. Open `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/BeplnEx/config/BepInEx.cfg`.
4. Copy the DLL file into `C:/Program Files (x86)/Steam/steamapps/common/Lethal Company/BepInEx/plugins/` from `bin/Debug/netstandard2.1/`.
5. Set `Logging.Console.Enabled` to `true`.
6. Set `Logging.Console.LogLevels` to `All`.
7. Launch `Lethal Company.exe` again.

## Credit

Inspired by the following projects:

- [InstantBuy by nexor-source](https://github.com/nexor-source/InstantBuy)

## Test Scenarios

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
