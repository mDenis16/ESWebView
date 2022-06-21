using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using ESWebViewWin;
using Microsoft.Web.WebView2.Wpf;

namespace ESWebView
{
    /// <summary>
    /// Interaction logic for WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        public WinWebViewApp app { get; set; }
    
        public WebView()
        {
            app = new WinWebViewApp();
            if (!app.Startup())
                Environment.Exit(0);
            
            InitializeComponent();
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WebViewComponent.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            await WebViewComponent.EnsureCoreWebView2Async(null);

        }
        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var webview = sender as WebView2;
            webview.CoreWebView2.Navigate(app.GenerateLoadUrl());
        }

        private void lblAppVersion_Loaded(object sender, RoutedEventArgs e)
        {
            var label = sender as TextBlock;
            // ... Set date in content.
            label.Text = "version " + Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
        private void lblUsername_Loaded(object sender, RoutedEventArgs e)
        {
            var label = sender as TextBlock;
            // ... Set date in content.
            label.Text = "User "  + Environment.UserName;
        }
       

        private void WebView2_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
