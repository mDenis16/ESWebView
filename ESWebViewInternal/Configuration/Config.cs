using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ESWebViewInternal.Configuration
{
    internal class Config
    {
        internal string _ConfigPath { get; set; }

        public Config(string ConfigPath)
        {
            _ConfigPath = ConfigPath;
        }

        public XmlConfig data { get; set; }

        internal enum ConfigVerifyResult
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
            }

            return new Tuple<bool, string>(true, "Configuration file is valid.");
        }

        public void CreateDefaultConfiguration()
        {
            XmlDocument doc = new XmlDocument();


            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            XmlElement root = doc.DocumentElement;
            doc.InsertBefore(xmlDeclaration, root);

            XmlElement Settings = doc.CreateElement(string.Empty, "Settings", string.Empty);
            doc.AppendChild(Settings);
            {
                doc.CreateElement(string.Empty, "AppName", string.Empty).AppendChild(Settings);
                doc.CreateElement(string.Empty, "baseURL", string.Empty).AppendChild(Settings);
            }

            doc.Save(_ConfigPath);
        }
    }
}
  
