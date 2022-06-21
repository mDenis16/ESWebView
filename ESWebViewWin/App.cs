﻿using ESWebViewInternal;
using ESWebViewInternal.Configuration;
using System.Reflection;
using System.Windows;
using static ESWebViewInternal.Configuration.Config;
using System.Diagnostics;

namespace ESWebViewWin
{
    public class WinWebViewApp : InternalApp
    {
        public Config Config { get; set; }
        public DataDirectory Directory { get; set; }

        public WinWebViewApp()
        {
            Directory = new DataDirectory("ESData");
            Config = new Config(Directory, "AppConfig.xml");
        }
        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public bool Startup()
        {
            var directoryResult = Directory.CheckIfDirectoryExists();
            if (!directoryResult.Item1)
            {
                MessageBox.Show(directoryResult.Item2);
                return false;
            }

            var configResult = Config.VerifyConfiguration();
            if (configResult.Item1 == ConfigVerifyResult.CREATED_CONFIGURATION || configResult.Item1 == ConfigVerifyResult.INVALID_CONFIGURATION)
            {
                MessageBox.Show(configResult.Item2);
                return false;
            }

            if (IsAppAlreadyRunning())
            {
                MessageBox.Show("An instance of this app is already running.");
                return false;
            }
            
            return true;
        }
        public bool IsAppAlreadyRunning()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1;
        }

        public string GenerateLoadUrl()
        {
            string userId = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TimeZone localZone = TimeZone.CurrentTimeZone;
            return Config.data.baseURL + $"/userId={userId}&version={appVersion}&tz={localZone.ToString()}";
        }
    }
}