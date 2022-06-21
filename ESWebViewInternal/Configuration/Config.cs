using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ESWebViewInternal.Configuration
{
    public class Config
    {
        private string _ConfigPath { get; set; }
        
        public Config(DataDirectory _directory, string ConfigFileName)
        {
            _ConfigPath = _directory.Path + "//" +  ConfigFileName;
            data = new XmlConfig();
        }

        public XmlConfig data { get; set; }

        public enum ConfigVerifyResult
        {
            VALID_CONFIGURATION,
            CREATED_CONFIGURATION,
            INVALID_CONFIGURATION
        }
        public Tuple<ConfigVerifyResult, string> VerifyConfiguration()
        {
            if (!File.Exists(_ConfigPath))
            {
                CreateDefaultConfiguration();
                return new Tuple<ConfigVerifyResult, string>(ConfigVerifyResult.CREATED_CONFIGURATION, "Default configuration has been created.");
            }

            if (new FileInfo(_ConfigPath).Length == 0)
                return new Tuple<ConfigVerifyResult, string>(ConfigVerifyResult.INVALID_CONFIGURATION, "Configuration file exists but is empty.");

            var ConfigTest = IsXmlConfigValid();
            if (!ConfigTest.Item1)
                return new Tuple<ConfigVerifyResult, string>(ConfigVerifyResult.INVALID_CONFIGURATION, ConfigTest.Item2);

            return new Tuple<ConfigVerifyResult, string>(ConfigVerifyResult.VALID_CONFIGURATION, "Configuration file is valid.");
        }

        private Tuple<bool, string> IsXmlConfigValid()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(_ConfigPath);

            if (xmlDoc is null) return new Tuple<bool, string>(false, "Configuration is null!");
            XmlNode? root = xmlDoc.DocumentElement;

            if (root is null) return new Tuple<bool, string>(false, "Root node configuration is null!");

            XmlNode? settings = root.SelectSingleNode("/Settings");

            if (settings is null) return new Tuple<bool, string>(false, "Settings node configuration is null!");



            var XmlClassProprieties = typeof(XmlConfig).GetProperties();

            foreach (var prop in XmlClassProprieties)
            {
                XmlNode? propNode = settings.SelectSingleNode(prop.Name);

                if (propNode is null) return new Tuple<bool, string>(false, $"Config item {prop.Name} is null!");

                if (propNode.InnerText.Length == 0) return new Tuple<bool, string>(false, $"Config item {prop.Name} is empty!");

                prop.SetValue(data, propNode.InnerText);
            }

            return new Tuple<bool, string>(true, "Configuration file is valid.");
        }

        private void CreateDefaultConfiguration()
        {
            var XmlClassProprieties = typeof(XmlConfig).GetProperties();
            var settings = new XElement("Settings");
            XDocument doc = new XDocument(settings);

            foreach (var prop in XmlClassProprieties)
                settings.Add(new XElement(prop.Name, string.Empty));
        
            doc.Save(_ConfigPath);
        }

        public void SaveConfig(IDictionary<string, string> data)
        {
            var settings = new XElement("Settings");
            XDocument doc = new XDocument(settings);

            
            foreach (var prop in data)
                settings.Add(new XElement(prop.Key, prop.Value));

            doc.Save(_ConfigPath);
        }
    }
}
  
