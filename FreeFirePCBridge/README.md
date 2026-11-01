# FreeFire PC Bridge

## What it does
This Windows WPF companion launches the official scrcpy and ADB tools. Your Android phone runs the game; the PC mirrors the phone display and forwards supported physical keyboard input.

**Important:** scrcpy keyboard forwarding is not the same as a game-specific WASD-to-touch joystick mapper. Free Fire may not accept all forwarded keys. This project does not modify the game, inject macros, or bypass anti-cheat/protections.

## Requirements
- Windows 10 x64
- Android phone and a data-capable USB cable
- Official Windows scrcpy package, containing `scrcpy.exe` and `adb.exe`
- USB debugging enabled for initial USB authorization

Download scrcpy from the official project releases only: https://github.com/Genymobile/scrcpy/releases

## USB setup
1. On your phone, open Settings > About device and tap Build number repeatedly until Developer options are enabled (the exact menu wording can vary).
2. Open Developer options and enable USB debugging.
3. Connect the phone to the PC with the USB data cable and unlock the phone.
4. Approve the USB debugging authorization prompt for your own PC.
5. Launch FreeFire PC Bridge, select the extracted scrcpy folder using Browse, then click **Start USB Mirror**.

If Windows does not detect the phone, try another data-capable cable or USB port. Do not approve debugging prompts on computers you do not trust.

## Wi-Fi
1. Establish and authorize a USB connection first.
2. Click **Enable Wi-Fi from USB**.
3. Find the ADB IP:port shown by your phone or ADB setup and enter it in the address field, then click **Connect Wi-Fi**.
4. Keep both devices on the same trusted local network.

Android Wireless debugging can use a pairing code and a dynamic port. This prototype's simple IP:5555 field does not implement the full pairing-code flow, so it may not work with every Android 16 configuration.

## Performance
The default settings are intentionally conservative for low-memory PCs:
- 1024-pixel maximum video size
- 30 FPS
- 4 Mbps video bitrate

If the mirror stutters, turn off audio forwarding and keep the 1024/30 settings. Full-screen is enabled by default.

## Build
From the repository root with .NET 8 SDK installed:

```powershell
dotnet publish .\FreeFirePCBridge\FreeFirePCBridge.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o publish
```

A GitHub Actions workflow also builds a self-contained Windows x64 artifact on pushes to `main`. The artifact contains this companion app; scrcpy is downloaded separately from its official release page.
