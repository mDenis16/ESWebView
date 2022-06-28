using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using ESWebViewInternal.Configuration.Attributes;
using Newtonsoft;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace ESWebViewInternal.Configuration
{

    //$baseurl?userid=$userid&version=$appversion&tz=$currenttimezone
    public class ConfigData
    {

        [JsonRequired]
        [ConfigItem("Base Url", Access.URL_ITEM | Access.PUBLIC)]
        public string BaseUrl { get; set; }

        [JsonIgnore]
        [ConfigItem("User Id", Access.URL_ITEM | Access.INTERNAL)]
        public string UserId { get; set; }
        
        [JsonIgnore]
        [ConfigItem("App Version", Access.URL_ITEM | Access.INTERNAL)]
        public string AppVersion { get; set; }

        [JsonIgnore]
        [ConfigItem("Current timezone", Access.URL_ITEM | Access.INTERNAL)]
        public string CurrentTimeZone { get; set; }

        [JsonRequired]
        [ConfigItem("Load Url")]
        public string loadURL { get; set; }

        
        [JsonRequired]
        [ConfigItem("Auto update")]
        public bool autoUpdate { get; set; }

        [JsonRequired]
        [ConfigItem("Success Sound Path")]
        public string SuccessSoundPath { get; set; }

        [JsonRequired]
        [ConfigItem("Fail Sound Path")]
        public string FailSoundPath { get; set; }
    }
    public class Config
    {
       
        private string _ConfigPath { get; set; }
        
        public Config(DataDirectory _directory, string ConfigFileName)
        {
            _ConfigPath = _directory.Path + "//" +  ConfigFileName;
            data = new ConfigData();
        }

        public ConfigData data { get; set; }



        public enum ConfigVerifyResult
        {
            VALID_CONFIGURATION,
            CREATED_CONFIGURATION,
            INVALID_CONFIGURATION
        }
       

        public Tuple<bool, string> LoadConfiguration()
        {
            try
            {
                var jsonText = File.ReadAllText(_ConfigPath);
                this.data = JsonConvert.DeserializeObject<ConfigData>(jsonText);
            }
            catch(Exception ex) {
                return new Tuple<bool, string>(false, ex.Message);
            }
           
            if (data == null)
                return new Tuple<bool, string>(false, "Configuration file is invalid.");

            return new Tuple<bool, string>(true, "Configuration file is valid.");
        }

      
        public string GetConfigWebComp()
        {
          
            List<Dictionary<string, string>> configWeb = new List<Dictionary<string, string>>();

            var props = typeof(ConfigData).GetProperties();
            foreach (var prop in props)
            {
                if (!prop.IsDefined(typeof(ConfigItemAttribute)))
                    continue;

               
                ConfigItemAttribute? itemData = prop.GetCustomAttribute<ConfigItemAttribute>();
                if (itemData is null)
                    continue;


                Dictionary<string, string> curProp = new Dictionary<string, string>();

                curProp["displayName"] = itemData.DisplayName;
                curProp["propertyName"] = prop.Name;
                curProp["type"] = prop.PropertyType.Name.ToLower();
                curProp["accessType"] = ((int)itemData.AccessType).ToString();

                var val = prop.GetValue(data);
                if (val is not null)
                    curProp["value"] = val.ToString();
                else
                    curProp["value"] = "";

                configWeb.Add(curProp);
            }

            return JsonConvert.SerializeObject(configWeb);
        }
        public bool DoesConfigurationExist()
        {
            return File.Exists(_ConfigPath);
        }

        public void SaveConfig()
        {
            var serializedConfig = JsonConvert.SerializeObject(this.data);
            File.WriteAllText(_ConfigPath, serializedConfig);
        }

        public void UpdateConfig()
        {
            try
            {

            }
            catch(Exception ex)
            {

            }
        }
    }
}
  
