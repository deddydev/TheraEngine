using TheraEngine.Files;
using System;
using System.IO;
using System.Windows;
using Xceed.Wpf.AvalonDock.Layout;

namespace TheraEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static MainWindow _instance;
        public static MainWindow Instance => _instance ?? new MainWindow();

        private SingleFileRef<Project> _project;

        public Project Project
        {
            get => _project;
            set => _project = value;
        }

        public MainWindow()
        {
            InitializeComponent();
            _instance = this;
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
