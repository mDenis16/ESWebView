using ESWebViewInternal;
using ESWebViewInternal.Configuration;
using System.Reflection;
using System.Windows;
using static ESWebViewInternal.Configuration.Config;
using System.Diagnostics;
using System.Security.Principal;
using ESWebViewInternal.Configuration.Attributes;
using ESWebViewInternal.Bridge;
using Newtonsoft.Json;

namespace ESWebViewWin
{
    

    public class WinWebViewApp : InternalApp
    {
       
        public DataDirectory Directory { get; set; }
        public Mutex mutex;
        public Config Config { get; set; }


        public WinWebViewApp()
        {
            Directory = new DataDirectory("ESData");
            Config = new Config(Directory, "AppConfig.json");


        }
        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public void SetInternalConfigData()
        {
            Config.data.AppVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            Config.data.UserId = WindowsIdentity.GetCurrent().Name;
            Config.data.CurrentTimeZone = GetTimeZone();
        }
        public StartupResult Startup()
        {
            if (IsAppAlreadyRunning())
            {
                MessageBox.Show("An instance of this app is already running.");
                return StartupResult.CLOSE_APPLICATION;
            }


            var directoryResult = Directory.CheckIfDirectoryExists();
            if (!directoryResult.Item1)
            {
                MessageBox.Show(directoryResult.Item2);
                return StartupResult.CLOSE_APPLICATION;
            }

            
            if (!Config.DoesConfigurationExist())
            {
                MessageBox.Show("Configuration doesn't exist. Please configure it.");
                return StartupResult.OPEN_CONFIG_WINDOW;
            }
            var configResult = Config.LoadConfiguration();
            if (!configResult.Item1)
            {
                MessageBox.Show(configResult.Item2);
                return StartupResult.CLOSE_APPLICATION;
            }
            
            SetInternalConfigData();


            return StartupResult.OPEN_NORMAL;
        }
        public bool IsAppAlreadyRunning()
        {
            return Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1;
        }

        public string GetTimeZone()
        {
            NodaTime.ZonedDateTime hereAndNow = NodaTime.SystemClock.Instance.GetCurrentInstant().InZone(NodaTime.DateTimeZoneProviders.Tzdb.GetSystemDefault());

            System.TimeSpan zoneOffset = hereAndNow.ToDateTimeOffset().Offset;

            return "UTC" + (zoneOffset < TimeSpan.Zero ? "-" : "+") + zoneOffset;
        }

        public void WebMessageReceived(WebMessage webMessage)
        {
            if (webMessage.Type == MessageType.SAVE_SETTINGS)
            {
                var settingsObj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(webMessage.PayLoad);
                if (settingsObj is null) return;

                var configProps = typeof(ConfigData).GetProperties();

                if (configProps is null) return;

                foreach (var setting in settingsObj)
                {
                    string? propertyName = setting["propertyName"] as string;
                    string? type = setting["type"] as string;
                    object? value = setting["value"];

                    if (value is null || type is null || propertyName is null) return;

                    PropertyInfo? prop = configProps.FirstOrDefault(a => a.Name == propertyName);

                    if (prop is null) continue;

                    if (type == "bool")
                        prop.SetValue(Config.data, bool.Parse(value as string));
                    else if (type == "string")
                        prop.SetValue(Config.data, value as string);
                }

                Config.SaveConfig();
            }
        }

        public string BuidLoadUrl()
        {

            string userId = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value;
            string appVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            TimeZone localZone = TimeZone.CurrentTimeZone;

            string loadURL = Config.data.loadURL;

            var props = typeof(ConfigData).GetProperties().Where(a => a.GetCustomAttribute<ConfigItemAttribute>() != null && a.GetCustomAttribute<ConfigItemAttribute>().AccessType.HasFlag(Access.URL_ITEM));
            foreach (var prop in props)
                loadURL = loadURL.Replace("{" + prop.Name + "}", prop.GetValue(Config.data).ToString());

            string url = $@"$baseurl?userid=$userid&version=$appversion&tz=$currenttimezone";
            return loadURL;
        }
    }
}
