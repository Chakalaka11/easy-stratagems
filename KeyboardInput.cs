using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace EasyStrategems
{
    public class KeyboardInput : IDisposable
    {
        public event EventHandler<KeyPressedEventArgs> KeyPressed;
        private WindowsHookHelper.HookDelegate _keyboardDelegate;
        private IntPtr _hookId;
        private MessageLoop _loop;
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_KEYBOARD = 2;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;

        private bool disposed;

        public KeyboardInput()
        {
            _keyboardDelegate = KeyboardHook;
            _loop = new MessageLoop(() =>
            {
                using (Process curProcess = Process.GetCurrentProcess())
                using (ProcessModule curModule = curProcess.MainModule)
                {
                    _hookId = WindowsHookHelper.SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardDelegate, WindowsHookHelper.GetModuleHandle(curModule.ModuleName), 0);
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

        private IntPtr KeyboardHook(Int32 nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var keyStruct = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                // Console.WriteLine($"{keyStruct.vkCode}");

                if (KeyPressed != null)
                {
                    var keyPressedEventArgs = new KeyPressedEventArgs()
                    {
                        IsKeyPressed = wParam == (IntPtr)WM_KEYDOWN,
                        IsKeyReleased = wParam == (IntPtr)WM_KEYUP,
                        KeyCode = keyStruct.vkCode
                    };
                    KeyPressed(this, keyPressedEventArgs);
                }
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

        ~KeyboardInput()
        {
            Dispose(false);
        }
    }

    public class KeyPressedEventArgs
    {
        public uint KeyCode { get; set; }
        public bool IsKeyPressed { get; set; }
        public bool IsKeyReleased { get; set; }
    }

    public struct KBDLLHOOKSTRUCT{
        public uint vkCode { get; set; }
        public uint scanCode { get; set; }
        public uint flags { get; set; }
        public uint time { get; set; }
    }
}
