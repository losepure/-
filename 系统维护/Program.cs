using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace 系统维护
{
    static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            bool isNewInstance;
            string appName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            Mutex mtx = new Mutex(true, appName, out isNewInstance);
            if (!isNewInstance)
            {
                Process[] myProcess = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(appName));
                if (myProcess.FirstOrDefault()!=null) 
                {
                    ShowWindow(myProcess.FirstOrDefault().MainWindowHandle, 1);
                }
                MessageBox.Show("已有实例正在运行！");
                return;
            }


            if (args.Length == 0)
            {
                Application.Run(new MainFrm());
            }
            else
            {
                Application.Run(new MainFrm(args));
            }
        }
    }
}
