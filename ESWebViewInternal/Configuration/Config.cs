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

namespace ESWebViewInternal.Configuration
{
    public class ConfigData
    {
        [JsonRequired]
        [ConfigItem("Base Url")]
        public string baseURL { get; set; }
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
            //[{"propertyName":"folderPath","name":"Folder path","type":"text","value":"localhost","ref":null},{"propertyName":"autoStart","name":"Autostart","type":"bool","value":false,"ref":null}]
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
  
