using ESWebViewInternal.Configuration;
using ESWebViewWin;
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


namespace ESWebView
{
    /// <summary>
    /// Interaction logic for Configuration.xaml
    /// </summary>
    public partial class ConfigurationWindow : Window
    {
        public WinWebViewApp app { get; set; }

        public ConfigurationWindow(WinWebViewApp _app)
        {
            app = _app;

            InitializeComponent();
        }
        private void ConfigurationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //  <TextBlock Text="baseURL" Grid.Row="0" Grid.Column="0" FontSize="16" VerticalAlignment="Center" HorizontalAlignment="Center"></TextBlock>
            // <TextBlock Grid.Row="0" Grid.Column="1" FontSize="16" VerticalAlignment="Center" HorizontalAlignment="Center"><TextBox Grid.Column="1" HorizontalAlignment="Center" TextWrapping="Wrap" Text="TextBox" VerticalAlignment="Center" Width="238"/></TextBlock>
            if (GridConfig is not null)
            {
                var props = typeof(ConfigData).GetProperties();


                var leftCollumn = new ColumnDefinition();
                leftCollumn.Width = new GridLength(30, GridUnitType.Star);
                GridConfig.ColumnDefinitions.Add(leftCollumn);

                var rightCollumn = new ColumnDefinition();
                rightCollumn.Width = new GridLength(70, GridUnitType.Star);
                GridConfig.ColumnDefinitions.Add(rightCollumn);


                foreach (var prop in props)
                {
                    var def = new RowDefinition();
                    def.Height = new GridLength(30, GridUnitType.Pixel);

                    GridConfig.RowDefinitions.Add(def);



                    var TextBlock = new TextBlock();
                    TextBlock.Text = prop.Name;
                    TextBlock.SetValue(Grid.RowProperty, GridConfig.RowDefinitions.Count - 1);
                    TextBlock.SetValue(Grid.ColumnProperty, 0);

                    var TextBox = new TextBox();
                    TextBox.Text = (string)prop.GetValue(app.Config.data);
                    TextBox.SetValue(Grid.RowProperty, GridConfig.RowDefinitions.Count - 1);
                    TextBox.SetValue(Grid.ColumnProperty, 1);

                    TextBox.HorizontalAlignment = HorizontalAlignment.Center;
                    TextBox.VerticalAlignment = VerticalAlignment.Center;
                    TextBox.TextWrapping = TextWrapping.Wrap;


                    TextBox.Width = 200;

                    GridConfig.Children.Add(TextBlock);
                    GridConfig.Children.Add(TextBox);
                }

            }
        }

        private void ConfigGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = sender as DataGrid;

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            IDictionary<string, string> configDict = new Dictionary<string, string>();

            var classProps = typeof(ConfigData).GetProperties();
            for (int i = 0; i < GridConfig.Children.Count; i += 2)
            {
                TextBlock? textblock = GridConfig.Children[i] as TextBlock;
                if (textblock is null)
                    continue;

                TextBox? textbox = GridConfig.Children[i + 1] as TextBox;
                if (textbox is null)
                    continue;

                var prop = classProps.Where(x => x.Name == textblock.Text).FirstOrDefault();
                if (prop is not  null) prop.SetValue(app.Config.data, textbox.Text);
            }

            app.Config.SaveConfig();
            ReinitializeWebView();
        }

        private void ReinitializeWebView()
        {
            var WebView = new WebView(app);

            Application.Current.MainWindow = WebView;
            WebView.Show();

            this.Close();
        }

        public static implicit operator ConfigurationWindow(ConfigurationWebWindow v)
        {
            throw new NotImplementedException();
        }
    }
}

