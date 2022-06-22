using ESWebViewWin;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ESWebView.Bridge;
using ESWebViewInternal.Configuration;
using System.Reflection;

namespace ESWebView
{
    /// <summary>
    /// Interaction logic for ConfigurationWebWindow.xaml
    /// </summary>
    public partial class ConfigurationWebWindow : Window
    {
        public WinWebViewApp app { get; set; }
        public ConfigurationWebWindow(WinWebViewApp _app)
        {
            app = _app;
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WebViewComponent.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            WebViewComponent.WebMessageReceived += CoreWebView2WebMessageReceivedEventArgs;
          
            await WebViewComponent.EnsureCoreWebView2Async(null);

           
        }
        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var webview = sender as WebView2;
            webview.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;

            webview.CoreWebView2.Navigate("http://127.0.0.1:5500/index.html");
        }

        private void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            var jsonConfig = app.Config.GetConfigWebComp();

            WebViewComponent.CoreWebView2.ExecuteScriptAsync("setInterval(() =>{console.log('mata'); populateSettings(${jsonConfig})},500);");
        }

        private void CoreWebView2WebMessageReceivedEventArgs(object sender, CoreWebView2WebMessageReceivedEventArgs ev)
        {
            WebMessage? WebMessage = JsonConvert.DeserializeObject<WebMessage>(ev.WebMessageAsJson); if (WebMessage is null) return;
            //[{"propertyName":"folderPath","name":"Folder path","type":"text","value":"localhost","ref":null},{"propertyName":"autoStart","name":"Autostart","type":"bool","value":false,"ref":null}]
            if (WebMessage.Type == MessageType.SAVE_SETTINGS)
            {
                var settingsObj = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(WebMessage.PayLoad);
                if (settingsObj is null) return;

                var configProps = typeof(ConfigData).GetProperties();

                if (configProps is null) return;

                foreach(var setting in settingsObj)
                {
                    string? propertyName = setting["propertyName"] as string;
                    string? type = setting["type"] as string;
                    object? value = setting["value"];

                    if (value is null || type is null  || propertyName is null) return;

                    PropertyInfo? prop = configProps.FirstOrDefault(a => a.Name == propertyName);

                    if (prop is null) continue;

                    if (type == "bool")
                        prop.SetValue(app.Config.data, bool.Parse(value as string));
                    else if (type == "string")
                        prop.SetValue(app.Config.data, value as string);
                }

                app.Config.SaveConfig();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var jsonConfig = app.Config.GetConfigWebComp();
            WebViewComponent.CoreWebView2.ExecuteScriptAsync($"populateSettings('{jsonConfig}')");
        }
    }
}
