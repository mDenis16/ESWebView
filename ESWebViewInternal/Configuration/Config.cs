using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Newtonsoft;
using Newtonsoft.Json;

namespace ESWebViewInternal.Configuration
{
    public class ConfigData
    {
        [JsonRequired]
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
  
