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
using ESWebViewInternal.Bridge;
using ESWebViewInternal.Configuration;
using System.Reflection;
using System.Diagnostics;
using System.IO;

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

            WebViewComponent.CoreWebView2.ExecuteScriptAsync($"populateSettings(`{jsonConfig}`)");
        }

        private void CoreWebView2WebMessageReceivedEventArgs(object sender, CoreWebView2WebMessageReceivedEventArgs ev)
        {
            WebMessage? WebMessage = JsonConvert.DeserializeObject<WebMessage>(ev.WebMessageAsJson); if (WebMessage is null) return;
            app.WebMessageReceived(WebMessage);
        }
    }
}