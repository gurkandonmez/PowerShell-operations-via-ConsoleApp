using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        InstallAndConfigurePowerShell();
    }

    static void InstallAndConfigurePowerShell()
    {
        // URLs for PowerShell installers
        string pwshUrl = "https://github.com/PowerShell/PowerShell/releases/download/v7.3.1/PowerShell-7.3.1-win-x64.msi";
        string pwshInstallerPath = Path.Combine(Path.GetTempPath(), "PowerShell-7.3.1-win-x64.msi");

        // Check if PowerShell 7 is installed
        if (!IsPowerShellInstalled("pwsh.exe"))
        {
            Console.WriteLine("PowerShell 7 not found. Downloading and installing...");
            DownloadFile(pwshUrl, pwshInstallerPath);
            InstallMsi(pwshInstallerPath);
        }

        // Add PowerShell 7 to PATH
        string pwshPath = @"C:\Program Files\PowerShell\7";
        AddToPath(pwshPath);

        // Check if Windows PowerShell is installed
        if (!IsPowerShellInstalled("powershell.exe"))
        {
            Console.WriteLine("Windows PowerShell not found. It should be pre-installed on Windows.");
        }
        else
        {
            // Add Windows PowerShell to PATH
            string powershellPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "WindowsPowerShell\\v1.0");
            AddToPath(powershellPath);
        }

        Console.WriteLine("PowerShell configuration completed.");
    }

    static bool IsPowerShellInstalled(string executableName)
    {
        string[] paths = Environment.GetEnvironmentVariable("PATH").Split(';');
        foreach (string path in paths)
        {
            if (File.Exists(Path.Combine(path, executableName)))
            {
                return true;
            }
        }
        return false;
    }

    static void DownloadFile(string url, string outputPath)
    {
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(url, outputPath);
        }
    }

    static void InstallMsi(string msiPath)
    {
        Process installerProcess = Process.Start("msiexec", $"/i \"{msiPath}\" /quiet /norestart");
        installerProcess.WaitForExit();
    }

    static void AddToPath(string newPath)
    {
        string currentPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
        if (!currentPath.Contains(newPath))
        {
            string updatedPath = $"{currentPath};{newPath}";
            Environment.SetEnvironmentVariable("PATH", updatedPath, EnvironmentVariableTarget.Machine);
            Console.WriteLine($"Added {newPath} to PATH.");
        }
    }
}
