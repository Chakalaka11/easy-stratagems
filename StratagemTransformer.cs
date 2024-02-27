
using System.Runtime.InteropServices;

namespace EasyStrategems
{
    public class StratagemTransformer
    {
        #region Keys

        const int W_KEY = 0x57; 
        const int A_KEY = 0x41;
        const int S_KEY = 0x53;
        const int D_KEY = 0x44;
        const int X_KEY = 0x58;
        const int VK_LCONTROL = 0xA2;
        const int VK_CAPITAL = 0x14;
        const int KEYEVENTF_KEYUP = 0x0002;

        #endregion

        void PressKey(Keyboard.ScanCodeShort Key)
        {
            Keyboard.Send(Key);
        }

        private KeyboardInput _keyboardInput;
        private MouseInput _mouseInput;
        private const int StratagemButton = VK_CAPITAL;
        private const int CountedDifference = 250;
        private bool isStratagemButtonHeld = false;

        private List<Point> _mouseMovementPoints;

        public StratagemTransformer()
        {
            _keyboardInput = new KeyboardInput();
            _mouseInput = new MouseInput();
            _keyboardInput.KeyPressed += KeyPressedEventHandler;
            _mouseInput.MouseMoved += MouseMovedEventHandler;
            _mouseMovementPoints = new List<Point>();
        }

        public void StartTransform()
        {
            Console.WriteLine("Reading keyboard...");
            _keyboardInput.StartListening();

            Console.WriteLine("Reading mouse...");
            _mouseInput.StartListening();
        }
        public void EndTransform()
        {
            _keyboardInput.StopListening();
            _mouseInput.StopListening();
        }

        private void KeyPressedEventHandler(object? s, KeyPressedEventArgs args)
        {
            if (args.KeyCode == StratagemButton)
            {
                if (args.IsKeyPressed && !isStratagemButtonHeld)
                {
                    isStratagemButtonHeld = true;
                    Console.WriteLine("Stratagem button pressed!");
                }
                if (args.IsKeyReleased && isStratagemButtonHeld)
                {
                    isStratagemButtonHeld = false;
                    Console.WriteLine("Stratagem button released!");
                }
            }
        }

        private void MouseMovedEventHandler(object? s, MouseMovedEventArgs args)
        {
            if (isStratagemButtonHeld)
            {
                // Calculate distance
                _mouseMovementPoints.Add(args.Location);
                var firstItem = _mouseMovementPoints.First();
                var lastItem = _mouseMovementPoints.Last();
                var horizontalDifference = firstItem.x - lastItem.x;
                var verticalDifference = firstItem.y - lastItem.y;
                // Console.WriteLine($"{horizontalDifference} {verticalDifference}");
                if (Math.Abs(horizontalDifference) > CountedDifference)
                {
                    if (horizontalDifference < 0)
                    {
                        Console.WriteLine("Pressed right");
                        PressKey(Keyboard.ScanCodeShort.KEY_D);
                    }
                    if (horizontalDifference > 0)
                    {
                        Console.WriteLine("Pressed left");
                        PressKey(Keyboard.ScanCodeShort.KEY_A);
                    }
                    _mouseMovementPoints.Clear();
                }
                if (Math.Abs(verticalDifference) > CountedDifference)
                {
                    if (verticalDifference < 0)
                    {
                        Console.WriteLine("Pressed down");
                        PressKey(Keyboard.ScanCodeShort.KEY_S);
                    }
                    if (verticalDifference > 0)
                    {
                        Console.WriteLine("Pressed up");
                        PressKey(Keyboard.ScanCodeShort.KEY_W);
                    }

                    _mouseMovementPoints.Clear();
                }
            }
        }
    }
}