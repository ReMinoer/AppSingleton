using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppSingleton
{
    static internal class Program
    {
        private const uint Restore = 9;
        private const uint ForceMinimize = 11;

        private const int AltKeyCode = 0xA4;
        private const int ExtendedKey = 0x1;
        private const int KeyUp = 0x2;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static private extern bool ShowWindow(IntPtr hwnd, uint nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static private extern bool SetForegroundWindow(IntPtr hwnd);

        [DllImport("user32.dll")]
        static private extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        static private extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        static private extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        static private void Main(string[] args)
        {
            if (args.Length < 1)
            {
                MessageBox.Show("Invalid parameters !\nUsage : AppSingleton.exe fullAppPath arg1 arg2 ...",
                    "AppSingleton", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Process[] processes = Process.GetProcesses();

            foreach (Process process in processes)
            {
                try
                {
                    // Find the process by executable filename
                    if (process.MainModule.FileName != args[0])
                        continue;
                }
                catch (Win32Exception) // Handle unauthorized access
                {
                    continue;
                }

                // If window already has focus, minimize it
                if (process.MainWindowHandle == GetForegroundWindow())
                {
                    ShowWindow(process.MainWindowHandle, ForceMinimize);
                    return;
                }

                // If window minimized, show it
                if (IsIconic(process.MainWindowHandle))
                    ShowWindow(process.MainWindowHandle, Restore);

                // Hack for SetForegroundWindow : press Alt before
                keybd_event(AltKeyCode, 0x45, ExtendedKey | 0, 0);
                keybd_event(AltKeyCode, 0x45, ExtendedKey | KeyUp, 0);

                // Set the targeted process window to foreground
                SetForegroundWindow(process.MainWindowHandle);
                return;
            }

            // If process is not currently running, start it
            string arguments = "";
            if (args.Length > 1)
            {
                arguments = args[1];
                for (int i = 2; i < args.Length; i++)
                    arguments += " " + args[i];
            }

            Process.Start(args[0], arguments);
        }
    }
}