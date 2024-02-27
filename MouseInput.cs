using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasyStrategems
{
    public class MouseInput : IDisposable
    {
        public event EventHandler<MouseMovedEventArgs> MouseMoved;
        private WindowsHookHelper.HookDelegate _mouseDelegate;
        private IntPtr _hookId;
        private MessageLoop _loop;
        private const Int32 WH_MOUSE_LL = 14;
        private const Int32 WH_MOUSE = 7;

        private bool disposed;

        public MouseInput()
        {
            _mouseDelegate = MouseHook;
            _loop = new MessageLoop(() =>
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookId = WindowsHookHelper.SetWindowsHookEx(WH_MOUSE_LL, _mouseDelegate, WindowsHookHelper.GetModuleHandle(curModule.ModuleName), 0);
                }
            });
        }

        public void StartListening()
        {
            _loop.Start();
        }

        public void StopListening()
        {
            _loop.Stop();
        }

        private IntPtr MouseHook(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return WindowsHookHelper.CallNextHookEx(_hookId, nCode, wParam, lParam);


            if (MouseMoved != null)
            {
                var mouseStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                var mouseMovedEventArgs = new MouseMovedEventArgs()
                {
                   Location = mouseStruct.pt
                };
                MouseMoved(this, mouseMovedEventArgs);
            }

            return WindowsHookHelper.CallNextHookEx(_hookId, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (_hookId != IntPtr.Zero)
                    WindowsHookHelper.UnhookWindowsHookEx(_hookId);

                disposed = true;
            }
        }

        ~MouseInput()
        {
            Dispose(false);
        }
    }

    public class MouseMovedEventArgs
    {
        public Point Location { get; set; }
    }

    public struct MSLLHOOKSTRUCT
    {
        public Point pt;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int x;
        public int y;
    }
}
