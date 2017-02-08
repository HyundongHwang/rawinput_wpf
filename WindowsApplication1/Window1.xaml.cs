using Newtonsoft.Json;
using RawStuff;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using WPFRawInput;
using Application = System.Windows.Application;

namespace WindowsApplication1
{
    public partial class Window1
    {
        private RawStuff.InputDevice _inputdevice;
        private MySimpleKeyboardHook _keyboardHook;
        private string _blockHidKeyword = "";


        public Window1()
        {
            this.InitializeComponent();
            this.Loaded += _This_Loaded;
            this.BtnClear.Click += BtnClear_Click;
            this.CbCaptureKeyboard.Click += _Cb_Click;
            this.CbCaptureMouse.Click += _Cb_Click;
            this.BtnBlock.Click += BtnBlock_Click;
        }

        private void BtnBlock_Click(object sender, RoutedEventArgs e)
        {
            _blockHidKeyword = this.TbBlockHidKeyword.Text;
        }

        private void _This_Loaded(object sender, RoutedEventArgs e)
        {
            //_keyboardHook = new MySimpleKeyboardHook();
            this._Init();
        }

        private void _Cb_Click(object sender, RoutedEventArgs e)
        {
            _inputdevice.RegisterRawInputDevices(
                (bool)this.CbCaptureKeyboard.IsChecked,
                (bool)this.CbCaptureMouse.IsChecked);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            this.TbLog.Clear();
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (_inputdevice != null)
            {
                // I could have done one of two things here.
                // 1. Use a Message as it was used before.
                // 2. Changes the ProcessMessage method to handle all of these parameters(more work).
                //    I opted for the easy way.

                //Note: Depending on your application you may or may not want to set the handled param.

                var message = new Message();
                message.HWnd = hwnd;
                message.Msg = msg;
                message.LParam = lParam;
                message.WParam = wParam;

                _inputdevice.ProcessMessage(message);
            }
            return IntPtr.Zero;
        }

        private void _Init()
        {
            var hwnd = IntPtr.Zero;
            var myWin = Application.Current.MainWindow;

            try
            {
                hwnd = new WindowInteropHelper(myWin).Handle;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            //Get the Hwnd source   
            var source = HwndSource.FromHwnd(hwnd);
            //Win32 queue sink
            source.AddHook(new HwndSourceHook(WndProc));

            _inputdevice = new RawStuff.InputDevice(hwnd);
            _inputdevice.RAWINPUT_EventCalled += _inputdevice_RAWINPUT_EventCalled;

            _inputdevice.RegisterRawInputDevices(
                (bool)this.CbCaptureKeyboard.IsChecked,
                (bool)this.CbCaptureMouse.IsChecked);
        }

        private void _inputdevice_RAWINPUT_EventCalled(RawStuff.InputDevice.DeviceInfo dinfo, RawStuff.InputDevice.RAWINPUT raw)
        {
            object logObj = null;

            try
            {
                if (dinfo.deviceType == "KEYBOARD")
                {
                    if (!string.IsNullOrWhiteSpace(_blockHidKeyword) && 
                        dinfo.deviceName.Contains(_blockHidKeyword))
                    {
                        //_keyboardHook.BlockOnce = true;
                    }

                    logObj = new
                    {
                        Name = dinfo.Name,
                        HID = dinfo.deviceName,
                        VKEY = Enum.GetName(typeof(Keys), raw.keyboard.VKey),
                    };
                }
                else if (dinfo.deviceType == "MOUSE")
                {
                    logObj = new
                    {
                        Name = dinfo.Name,
                        HID = dinfo.deviceName,
                        x = raw.mouse.lLastX,
                        y = raw.mouse.lLastY,
                        btn = raw.mouse.ulButtons,
                        btn2 = raw.mouse.ulRawButtons,
                    };
                }

                _WriteLog(JsonConvert.SerializeObject(logObj, Formatting.Indented));
            }
            catch (Exception ex)
            {
                _WriteLog(ex.ToString());
            }
        }

        private void _WriteLog(string log)
        {
            Trace.TraceInformation(log);
            this.TbLog.Text += log + "\n";
        }
    }
}