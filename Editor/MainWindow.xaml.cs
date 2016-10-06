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

namespace Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static FileSystemWatcher _contentWatcher;

        public MainWindow()
        {
            InitializeComponent();

            //EditorSettings s = EditorSettings.FromXML("");
            //_contentWatcher = new FileSystemWatcher()
            //{
            //    Filter = FileExtensionManager.GetListFilter(),
            //    EnableRaisingEvents = true,
            //    IncludeSubdirectories = true,
            //    Path = s._contentMonitorPath,
            //};
            //_contentWatcher.Changed += _contentWatcher_Changed;
            //_contentWatcher.Created += _contentWatcher_Created;
            //_contentWatcher.Deleted += _contentWatcher_Deleted;
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
    }
}
