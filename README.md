# FreeFire PC Bridge

A lightweight Windows companion for mirroring and controlling an Android phone over USB or Wi-Fi using the official open-source scrcpy/ADB tools.

## Status

Early prototype. This project does not run Free Fire natively on Windows; the game runs on your Android phone and the PC displays the phone screen. Keyboard compatibility depends on the game and scrcpy input support. It does not bypass anti-cheat or game protections.

## Requirements

- Windows 10 x64
- .NET 8 Desktop Runtime (or use the self-contained release build)
- Official scrcpy release for Windows, including `scrcpy.exe` and `adb.exe`
- Android phone with USB debugging enabled for USB setup

## Build

```powershell
dotnet publish .\FreeFirePCBridge\FreeFirePCBridge.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

See the app README for setup and connection steps. Download scrcpy only from https://github.com/Genymobile/scrcpy/releases.
