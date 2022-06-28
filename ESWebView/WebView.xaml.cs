using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
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
using System.Windows.Threading;

using ESWebViewWin;
using ESWebViewInternal.Bridge;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using Newtonsoft.Json;
using ESWebViewInternal;

namespace ESWebView
{
    /// <summary>
    /// Interaction logic for WebView.xaml
    /// </summary>
    public partial class WebView : Window
    {
        public WinWebViewApp app { get; set; }
        public string LoginId { get; set; }
       
        public StartupResult StartupResult { get; set; }
        public ICommand OpenConfigCommand { get; set; }
       
        public WebView(WinWebViewApp _app, StartupResult StartupResult)
        {
            app = _app;
            this.StartupResult = StartupResult;

            InitializeComponent();
        }
        private void DoSmth(object obj)
        {
            Console.WriteLine("mearsa");
        }

        private bool CanDoSmth(object obj)
        {
            //you could implement your logic here. But by default it should be  
            //set to true
            return true;
        }

        
        private async void Configuraton_Click(object sender, RoutedEventArgs e)
        {
          
            WebViewComponent.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
            this.WebViewComponent.CoreWebView2.Navigate("http://localhost:5500");
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoginId = WindowsIdentity.GetCurrent().Name;
           

            DispatcherTimer timer = new DispatcherTimer(TimeSpan.FromMilliseconds(30), DispatcherPriority.Normal, (object s, EventArgs ev) =>
            {
                NodaTime.ZonedDateTime hereAndNow = NodaTime.SystemClock.Instance.GetCurrentInstant().InZone(
    NodaTime.DateTimeZoneProviders.Tzdb.GetSystemDefault());

                System.TimeSpan zoneOffset = hereAndNow.ToDateTimeOffset().Offset;

                string sTimeDisplay = string.Format("{0} (UTC{2}{3:hh\\:mm})",
                    DateTime.Now.ToShortTimeString(),
                    hereAndNow.Zone.GetZoneInterval(hereAndNow.ToInstant()).Name,
                    zoneOffset < TimeSpan.Zero ? "-" : "+",
                    zoneOffset,
                    hereAndNow.Zone.Id);

                lblCurrentTime.Text = "TIME " + sTimeDisplay;
            }, this.Dispatcher);
            timer.Start();

         
        }
        private async void WebView_Loaded(object sender, RoutedEventArgs e)
        {
            WebViewComponent.WebMessageReceived += WebView_CoreWebView2WebMessageReceived;
            WebViewComponent.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
            
            await WebViewComponent.EnsureCoreWebView2Async( );
           
        }
        private void WebView_CoreWebView2PermissionRequested(object sender, CoreWebView2PermissionRequestedEventArgs e)
        {
            e.State = CoreWebView2PermissionState.Allow;
        }
        private void WebView_CoreWebView2NavigationCompleted(object sender, CoreWebView2NavigationCompletedEventArgs e)
        { 
            if (e.IsSuccess)
            {
                System.Media.SystemSounds.Beep.Play();
                /*play success sound*/
            }
            else
            {
                System.Media.SystemSounds.Exclamation.Play();
            }
        }
        private void WebView_CoreWebView2InitializationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            var webview = sender as WebView2;
            if (webview != null)
            {

                webview.CoreWebView2.NavigationCompleted += WebView_CoreWebView2NavigationCompleted;
                webview.CoreWebView2.PermissionRequested += WebView_CoreWebView2PermissionRequested;
               
                //https://www.moneycontrol.com/
                if (StartupResult == StartupResult.OPEN_NORMAL)
                {
                    webview.CoreWebView2.DOMContentLoaded -= CoreWebView2_DOMContentLoaded;
                    webview.CoreWebView2.Navigate(app.BuidLoadUrl());
                }
                else
                {
                    webview.CoreWebView2.DOMContentLoaded += CoreWebView2_DOMContentLoaded;
                    webview.CoreWebView2.Navigate("http://localhost:5500/");
                }


               
            }

        }
        private void CoreWebView2_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            var jsonConfig = app.Config.GetConfigWebComp();

            WebViewComponent.CoreWebView2.ExecuteScriptAsync($"populateSettings(`{jsonConfig}`)");
        }

        private void WebView_CoreWebView2WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs ev)
        {
            WebMessage? WebMessage = JsonConvert.DeserializeObject<WebMessage>(ev.WebMessageAsJson); if (WebMessage is null) return;
            app.WebMessageReceived(WebMessage);
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
            label.Text = LoginId;
        }
       


        private void View_Click(object sender, RoutedEventArgs e)
        {
            this.WebViewComponent.CoreWebView2.Navigate(app.BuidLoadUrl());
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {

        }

        private void About_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
