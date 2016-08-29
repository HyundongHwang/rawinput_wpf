using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Application=System.Windows.Application;

namespace WindowsApplication1
{
    public partial class Window1
    {
       RawStuff.InputDevice id;
       int NumberOfKeyboards;
       Message message = new Message();

        public Window1()
        {
           Activate();
        }

        private void _KeyPressed(object sender, RawStuff.InputDevice.KeyControlEventArgs e)
        {
            string[] tokens = e.Keyboard.Name.Split(';');
            string token = tokens[1];

            lbHandle.Content = e.Keyboard.deviceHandle.ToString();
            lbType.Content   = e.Keyboard.deviceType;
            lbName.Content   = e.Keyboard.deviceName;
            lbKey.Content    = e.Keyboard.key.ToString();
            lbVKey.Content   = e.Keyboard.vKey;
            lbDescription.Content  = token; 
            lbNumKeyboards.Content = NumberOfKeyboards.ToString();
        }

        public IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (id != null)
            {
                // I could have done one of two things here.
                // 1. Use a Message as it was used before.
                // 2. Changes the ProcessMessage method to handle all of these parameters(more work).
                //    I opted for the easy way.

                //Note: Depending on your application you may or may not want to set the handled param.

                message.HWnd   = hwnd;
                message.Msg    = msg;
                message.LParam = lParam;
                message.WParam = wParam;
                
                id.ProcessMessage(message);
            }
            return IntPtr.Zero;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            // I am new to WPF and I don't know where else to call this function.
            // It has to be called after the window is created or the handle won't
            // exist yet and the function will throw an exception.
            StartWndProcHandler();
            
            base.OnSourceInitialized(e);
        }
            
        void StartWndProcHandler()
        {
            IntPtr hwnd = IntPtr.Zero;
            Window myWin = Application.Current.MainWindow;

            try
            {
                hwnd = new WindowInteropHelper(myWin).Handle;
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex);
            }

            //Get the Hwnd source   
            HwndSource source = HwndSource.FromHwnd(hwnd);
            //Win32 queue sink
            source.AddHook(new HwndSourceHook(WndProc));

            id = new RawStuff.InputDevice(source.Handle);
            NumberOfKeyboards = id.EnumerateDevices();
            id.KeyPressed += new RawStuff.InputDevice.DeviceEventHandler(_KeyPressed);
        }

        void CloseMe(object sender, RoutedEventArgs e)
        {
            Close();
        }
  }
}