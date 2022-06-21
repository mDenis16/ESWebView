using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using ESWebViewWin;
using ESWebViewWin.NativeMethods;

namespace ESWebView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        WebView webView;
        Mutex mutex;
        public void Application_Startup(object sender, StartupEventArgs e)
        {

            mutex = new Mutex(false, Environment.UserName + Process.GetCurrentProcess().ProcessName);

            if (!mutex.WaitOne(TimeSpan.FromMilliseconds(10), false))
            {
                string processName = Process.GetCurrentProcess().ProcessName;
                BringOldInstanceToFront(processName);
                Environment.Exit(3);
            }
            else
            {
                GC.Collect();
                webView = new WebView();
                webView.Show();
                webView.ShowInTaskbar = true;
            }
        }



        private static void BringOldInstanceToFront(string processName)
        {
            Process[] RunningProcesses = Process.GetProcessesByName(processName);
            if (RunningProcesses.Length > 0)
            {
                Process runningProcess = RunningProcesses[0];
                if (runningProcess != null)
                {
                    IntPtr mainWindowHandle = runningProcess.MainWindowHandle;
                    NativeMethods.ShowWindowAsync(mainWindowHandle, 1);
                  
                }
            }
        }
    }
}