using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Windows.Forms.Integration;

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance { get { return _instance ?? new MainWindow(); } }

        private static FileSystemWatcher _contentWatcher;

        const string SettingsPath = "/Config/Settings.xml";

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;

            string path = AppDomain.CurrentDomain.BaseDirectory + SettingsPath;
            EditorSettings settings;
            if (!File.Exists(SettingsPath))
            {
                settings = new EditorSettings();
                settings.SaveXML(path);
            }
            else
                settings = EditorSettings.FromXML(path);

            if (!String.IsNullOrEmpty(settings._contentMonitorPath) && Directory.Exists(settings._contentMonitorPath))
            {
                _contentWatcher = new FileSystemWatcher()
                {
                    Filter = FileExtensionManager.GetListFilter(),
                    EnableRaisingEvents = true,
                    IncludeSubdirectories = true,
                    Path = settings._contentMonitorPath,
                };
                _contentWatcher.Changed += _contentWatcher_Changed;
                _contentWatcher.Created += _contentWatcher_Created;
                _contentWatcher.Deleted += _contentWatcher_Deleted;
            }
        }

        private void _contentWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            
        }

        private void _contentWatcher_Created(object sender, FileSystemEventArgs e)
        {
            
        }

        private void _contentWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowsFormsHost host = new WindowsFormsHost();
            RenderPanel renderPanel = new RenderPanel();
            host.Child = renderPanel;
            this.grid1.Children.Add(host);
        }
    }
}
