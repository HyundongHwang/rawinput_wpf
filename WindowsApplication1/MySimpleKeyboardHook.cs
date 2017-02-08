using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace WPFRawInput
{
    public class MySimpleKeyboardHook
    {
        public enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        private HookProc _hookProc = null;

        private IntPtr _hHook = IntPtr.Zero;

        public MySimpleKeyboardHook()
        {
            // initialize our delegate
            _hookProc = new HookProc(MyCallbackFunction);

            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                _hHook = SetWindowsHookEx((int)HookType.WH_KEYBOARD_LL, _hookProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);


        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        public bool BlockOnce { get; internal set; }

        private IntPtr MyCallbackFunction(int code, IntPtr wParam, IntPtr lParam)
        {
            Trace.TraceInformation("MyCallbackFunction code : " + code);
            Trace.TraceInformation("MyCallbackFunction wParam : " + wParam);

            if (code >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Trace.TraceInformation("MyCallbackFunction (Keys)vkCode : " + (Keys)vkCode);
            }

            if (this.BlockOnce)
            {
                if (wParam == (IntPtr)WM_KEYUP)
                    this.BlockOnce = false;

                Trace.TraceInformation("MyCallbackFunction block ...");
                return new IntPtr(1);
            }

            return CallNextHookEx(_hHook, code, wParam, lParam);
        }
    }
}
