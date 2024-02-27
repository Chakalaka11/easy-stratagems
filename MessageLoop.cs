using System.Runtime.InteropServices;
using EasyStrategems;

public class MessageLoop
{    
    private Action InitialAction { get; }
    private Thread? Thread { get; set; }
    
    public bool IsRunning { get; private set; }

    public MessageLoop(Action initialAction)
    {
        InitialAction = initialAction;
    }

    public void Start()
    {
        IsRunning = true;
        
        Thread = new Thread(() =>
        {
            InitialAction.Invoke();
            
            while (IsRunning)
            {
                var result = WindowsHookHelper.GetMessage(out var message, IntPtr.Zero, 0, 0);

                if (result <= 0)
                {
                    Stop();
                    
                    continue;
                }

                WindowsHookHelper.TranslateMessage(ref message);
                WindowsHookHelper.DispatchMessage(ref message);
            }
        });
        
        Thread.Start();
    }

    public void Stop()
    {
        IsRunning = false;
    }
}