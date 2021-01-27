using System;
using System.Xml;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;

class StartUpOptions
{
    private static string RawPath = AppDomain.CurrentDomain.BaseDirectory;
    private static string AppName = "TerminalHotkey";

    public static string GetPath()
    {
        string Path = RawPath.Replace(@"\", "/");
        return Path;
    }
    public static void SetStartUp(bool startUpState)
    {
        RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        if (startUpState == true)
        {
            rk.SetValue(AppName, Application.ExecutablePath);
        } else {
            rk.DeleteValue(AppName, false);
        }
    }
}

class InterceptKeys
{
    private const int WH_KEYBOARD_LL = 13;
    private const int WM_KEYDOWN = 0x0100;
    private static LowLevelKeyboardProc _proc = HookCallback;
    public bool UseShellExecute { get; set; }
    private static IntPtr _hookID = IntPtr.Zero;

    public static void Main()
    {
        _hookID = SetHook(_proc);
        Application.Run();
        UnhookWindowsHookEx(_hookID);
    }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                GetModuleHandle(curModule.ModuleName), 0);
        }
    }

    public static IntPtr FindWindow(string windowName)
    {
        var hWnd = FindWindow(windowName, string.Empty);
        return hWnd;
    }

    private delegate IntPtr LowLevelKeyboardProc(
        int nCode, IntPtr wParam, IntPtr lParam);

    private static IntPtr HookCallback(
        int nCode, IntPtr wParam, IntPtr lParam)
    {
        StartUpOptions.SetStartUp(true);
        if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            if (Keys.T == (Keys)vkCode && (Keys.Control | Keys.Alt) == Control.ModifierKeys)
            {
                using (Process compiler = new Process())
                {
                    var _windowHandle = FindWindow("CASCADIA_HOSTING_WINDOW_CLASS", null);
                    if (_windowHandle == IntPtr.Zero)
                    {
                        string Path = StartUpOptions.GetPath();
                        compiler.StartInfo.FileName = "wt";
                        //compiler.StartInfo.Arguments = "-d \"" +Path +"\"";
                        compiler.StartInfo.UseShellExecute = true;
                        compiler.Start();
                    }
                    else
                    {
                        SetForegroundWindow(_windowHandle);
                    }
                }
            } else
            {
                if (Keys.T == (Keys)vkCode && (Keys.Control | Keys.Alt | Keys.Shift) == Control.ModifierKeys)
                {
                    DialogResult dialogresult = MessageBox.Show("Deseja fechar o Terminal HTK Assistant?", "Warning", MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    if (dialogresult == DialogResult.Yes)
                    {
                        StartUpOptions.SetStartUp(false);
                        Application.Exit();
                    }
                }
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", SetLastError = true)]
    static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);
}