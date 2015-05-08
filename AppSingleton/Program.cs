using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AppSingleton
{
    static internal class Program
    {
        private const int SwShownormal = 1;

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static private extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        static private extern bool SetForegroundWindow(IntPtr hwnd);

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
                    if (process.MainModule.FileName != args[0]) continue;

                    ShowWindow(process.MainWindowHandle, SwShownormal);
                    SetForegroundWindow(process.MainWindowHandle);
                    return;
                }
                catch (Win32Exception)
                {
                }
            }

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