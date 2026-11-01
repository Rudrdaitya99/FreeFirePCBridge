using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace FreeFirePCBridge;

public partial class MainWindow : Window
{
    private Process? _session;
    private string? _scrcpyFolder;

    public MainWindow() => InitializeComponent();

    private void SetStatus(string message) => StatusText.Text = "Status: " + message;

    private bool ValidateTools()
    {
        _scrcpyFolder = ScrcpyFolderBox.Text;
        if (string.IsNullOrWhiteSpace(_scrcpyFolder) ||
            !File.Exists(Path.Combine(_scrcpyFolder, "scrcpy.exe")) ||
            !File.Exists(Path.Combine(_scrcpyFolder, "adb.exe")))
        {
            MessageBox.Show("Select the official scrcpy folder containing scrcpy.exe and adb.exe.",
                "scrcpy folder required", MessageBoxButton.OK, MessageBoxImage.Information);
            return false;
        }
        return true;
    }

    private string[] BuildScrcpyArgs()
    {
        int size = SizeBox.SelectedIndex switch { 1 => 1280, 2 => 1600, _ => 1024 };
        int fps = FpsBox.SelectedIndex switch { 1 => 45, 2 => 60, _ => 30 };
        var args = new List<string> { "--fullscreen", $"--max-size={size}", $"--max-fps={fps}", "--video-bit-rate=4M", "--keyboard=uhid" };
        if (AudioBox.IsChecked != true) args.Add("--no-audio");
        return args.ToArray();
    }

    private void LaunchScrcpy(IEnumerable<string> extraArgs)
    {
        if (!ValidateTools()) return;
        StopSession();
        var psi = new ProcessStartInfo
        {
            FileName = Path.Combine(_scrcpyFolder!, "scrcpy.exe"),
            WorkingDirectory = _scrcpyFolder!,
            UseShellExecute = false
        };
        foreach (string arg in BuildScrcpyArgs().Concat(extraArgs)) psi.ArgumentList.Add(arg);
        try
        {
            _session = Process.Start(psi);
            SetStatus("scrcpy launched. Approve the USB debugging prompt on your phone if shown.");
        }
        catch (Exception ex) { SetStatus("Could not start scrcpy: " + ex.Message); }
    }

    private void BrowseButton_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog { Title = "Select scrcpy.exe", Filter = "scrcpy.exe|scrcpy.exe", CheckFileExists = true };
        if (dialog.ShowDialog() == true)
        {
            ScrcpyFolderBox.Text = Path.GetDirectoryName(dialog.FileName) ?? "";
            SetStatus("scrcpy folder selected.");
        }
    }

    private void UsbButton_Click(object sender, RoutedEventArgs e) => LaunchScrcpy(Array.Empty<string>());
    private void WifiSetupButton_Click(object sender, RoutedEventArgs e) => LaunchScrcpy(new[] { "--tcpip" });

    private async void WifiButton_Click(object sender, RoutedEventArgs e)
    {
        if (!ValidateTools()) return;
        string address = WifiAddressBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(address) || address.Contains(' '))
        {
            SetStatus("Enter the phone's ADB IP address and port, for example 192.168.1.25:5555.");
            return;
        }
        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = Path.Combine(_scrcpyFolder!, "adb.exe"),
                WorkingDirectory = _scrcpyFolder!,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            psi.ArgumentList.Add("connect");
            psi.ArgumentList.Add(address);
            using var process = Process.Start(psi) ?? throw new InvalidOperationException("Could not start ADB.");
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            await process.WaitForExitAsync();
            SetStatus((output + " " + error).Trim());
            if (process.ExitCode == 0) LaunchScrcpy(new[] { $"--serial={address}" });
        }
        catch (Exception ex) { SetStatus("Wireless connection failed: " + ex.Message); }
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopSession();
        SetStatus("Bridge session stopped.");
    }

    private void StopSession()
    {
        try { if (_session is { HasExited: false }) _session.Kill(entireProcessTree: true); }
        catch { /* Process may already have exited. */ }
        _session = null;
    }

    protected override void OnClosed(EventArgs e)
    {
        StopSession();
        base.OnClosed(e);
    }
}
