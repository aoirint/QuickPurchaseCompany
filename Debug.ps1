$ErrorActionPreference = 'Stop'

$ProfileContainerDir = Join-Path $PSScriptRoot "profiles"
$DebugBuildModFile = Join-Path $PSScriptRoot "SkipDropshipCompany\bin\Debug\netstandard2.1\com.aoirint.SkipDropshipCompany.dll"

$GameDir = "C:\Program Files (x86)\Steam\steamapps\common\Lethal Company"
$GameExeName = "Lethal Company.exe"

$GameExePath = Join-Path $GameDir $GameExeName

Add-Type @"
using System;
using System.Runtime.InteropServices;
using System.Text;

public static class Win32 {
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern bool SetWindowText(IntPtr hWnd, string lpString);

    public delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll")]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    // Check if UnityWndClass window exists for the given PID
    public static bool IsUnityReady(int pid)
    {
        bool isUnityReady = false;

        // Loop through all windows
        EnumWindows((hWnd, lParam) =>
        {
            int winPid;
            GetWindowThreadProcessId(hWnd, out winPid);
            if (winPid != pid || !IsWindowVisible(hWnd))
                return true; // Continue enumeration

            var sb = new StringBuilder(256);
            GetClassName(hWnd, sb, sb.Capacity);
            var cls = sb.ToString();

            if (cls == "UnityWndClass") {
                isUnityReady = true;
                return false; // Stop enumeration
            }

            return true; // Continue enumeration
        }, IntPtr.Zero);

      return isUnityReady;
    }

    // Set window titles for all windows with the given PID
    public static bool SetAllWindowTitles(int pid, string title)
    {
        EnumWindows((hWnd, lParam) =>
        {
            int winPid;
            GetWindowThreadProcessId(hWnd, out winPid);
            if (winPid != pid || !IsWindowVisible(hWnd))
                return true;

            SetWindowText(hWnd, title);
            return true;
        }, IntPtr.Zero);

        return true;
    }
}
"@

function Set-WindowTitleSafely {
    param(
        [System.Diagnostics.Process]$Proc,
        [string]$Title
    )

    for ($i = 0; $i -lt 50; $i++) {
        $Proc.Refresh()

        if ($Proc.HasExited) {
            Write-Warning "Process already exited: PID $($Proc.Id)"
            return $false
        }

        # Wait until Unity window is ready
        if (-not [Win32]::IsUnityReady($Proc.Id)) {
            Start-Sleep -Milliseconds 200
            continue
        }

        # Set window title for all windows of the process
        [Win32]::SetAllWindowTitles($Proc.Id, $Title)
        return $true
    }

    Write-Warning "Failed to find Unity window for PID $($Proc.Id)"
    return $false
}

function Enable-BepInExConsole-If-Exist {
    param(
        [string]$ProfileDir
    )

    $ConfigFile = Join-Path $ProfileDir "BepInEx\config\BepInEx.cfg"

    if (-not (Test-Path $ConfigFile)) {
        Write-Warning "BepInEx config not found yet: $ConfigFile"
        return
    }

    $lines = [System.Collections.Generic.List[string]](Get-Content $ConfigFile)

    # Find [Logging.Console] section
    $sectionStart = $null
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\s*\[Logging\.Console\]\s*$') {
            $sectionStart = $i
            break
        }
    }

    if ($null -eq $sectionStart) {
        Write-Warning "[Logging.Console] section not found in $ConfigFile"
        return
    }

    # Find section end
    $sectionEnd = $lines.Count
    for ($j = $sectionStart + 1; $j -lt $lines.Count; $j++) {
        if ($lines[$j] -match '^\s*\[') {
            $sectionEnd = $j
            break
        }
    }

    # Set Enabled = true
    $enabledIndex = $null
    for ($k = $sectionStart + 1; $k -lt $sectionEnd; $k++) {
        if ($lines[$k] -match '^\s*Enabled\s*=') {
            $enabledIndex = $k
            break
        }
    }

    if ($null -ne $enabledIndex) {
        $lines[$enabledIndex] = 'Enabled = true'
    }
    else {
        $lines.Insert($sectionStart + 1, 'Enabled = true')
        $sectionEnd++
    }

    # Set LogLevels = All
    $logLevelIndex = $null
    for ($k = $sectionStart + 1; $k -lt $sectionEnd; $k++) {
        if ($lines[$k] -match '^\s*LogLevels\s*=') {
            $logLevelIndex = $k
            break
        }
    }

    if ($null -ne $logLevelIndex) {
        $lines[$logLevelIndex] = 'LogLevels = All'
    }
    else {
        $lines.Insert($sectionStart + 2, 'LogLevels = All')
    }

    $lines | Set-Content -Path $ConfigFile -Encoding UTF8

    Write-Host "Enabled console logging + LogLevels=All in $ConfigFile"
}

# Debug build
dotnet build --configuration Debug

if (-not (Test-Path $DebugBuildModFile)) {
  Write-Error "Debug build mod file not found: $DebugBuildModFile"
  exit 1
}

# Start game processes
$GameProcesses = @()
for ($i = 1; $i -le 2; $i++) {
  $ProfileDir = Join-Path $ProfileContainerDir ("profile_" + $i)

  # Enable BepInEx console after second run
  Enable-BepInExConsole-If-Exist -ProfileDir $ProfileDir

  $BepInExPluginDir = Join-Path $ProfileDir "BepInEx\plugins"

  # Install debug build mod
  Copy-Item -Path $DebugBuildModFile -Destination $BepInExPluginDir -Force

  if ($i -eq 1) {
    Copy-Item -Path (Join-Path $ProfileDir "winhttp.dll") -Destination $GameDir -Force
    Copy-Item -Path (Join-Path $ProfileDir "doorstop_config.ini") -Destination $GameDir -Force
  }

  $BepInExPreloaderDllFile = Join-Path $ProfileDir "BepInEx\core\BepInEx.Preloader.dll"
  if (-not (Test-Path $BepInExPreloaderDllFile)) {
    Write-Error "BepInEx Preloader DLL not found in profile directory: $BepInExPreloaderDllFile"
    exit 1
  }

  $Args = @(
    '-screen-width'
    '1280'
    '-screen-height'
    '720'
    # Doorstop v3 for BepInEx 5.4.21
    '--doorstop-enable'
    'true'
    '--doorstop-target'
    $BepInExPreloaderDllFile
  )

  $DebuggerPort = 55555 + ($i - 1)
  $EnvDict = @{
    "MONO_ENV_OPTIONS" = "--debugger-agent=transport=dt_socket,server=y,address=127.0.0.1:${DebuggerPort},embedding=1,defer=y"
  }

  try {
    $GameProcess = Start-Process `
      -FilePath $GameExePath `
      -ArgumentList $Args `
      -WorkingDirectory $GameDir `
      -Environment $EnvDict `
      -PassThru
  } catch {
    Write-Error "Failed to start game process for profile $_"
    exit 1
  }

  $GameProcesses += $GameProcess

  Set-WindowTitleSafely -Proc $GameProcess -Title ("Lethal Company - Profile $i") | Out-Null

  # Wait to prevent conflicting initial file access
  Start-Sleep -Seconds 1
}

# Wait until all game processes exit
[System.Diagnostics.Debug]::WriteLine("Waiting for game processes to exit...")
try {
  while ($true) {
    $Alive = $GameProcesses | Where-Object { $_ -and -not $_.HasExited }

    Write-Host ("Alive processes: " + $Alive.Count)

    if (-not $Alive) {
      break
    }

    Start-Sleep -Milliseconds 100
  }
}
finally {
  # Stop all game processes on exit
  foreach ($GameProcess in $GameProcesses) {
    if ($null -ne $GameProcess -and -not $GameProcess.HasExited) {
      try {
        $null = $GameProcess.CloseMainWindow()
        if (-not $GameProcess.WaitForExit(2000)) {
          $GameProcess.Kill()
          $null = $GameProcess.WaitForExit(3000)
        }
      } catch {
        # ignore errors
      }
    }
  }
}
