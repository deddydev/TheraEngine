using System.Windows.Controls;
using CustomEngine;
using CustomEngine.Worlds;

namespace Editor.Controls
{
    /// <summary>
    /// Interaction logic for RenderPanel.xaml
    /// </summary>
    public partial class EditorRenderPanel : UserControl
    {
        RenderPanel _panel;
        public EditorRenderPanel()
        {
            InitializeComponent();
            FormsHost.Child = _panel = new RenderPanel();
            EngineSettings settings = new EngineSettings();
            settings.OpeningWorld = typeof(TestWorld);
            Engine._engineSettings.SetFile(settings, false);
            _panel.AttachToEngine();
        }
    }
}
