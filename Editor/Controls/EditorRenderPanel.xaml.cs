using System.Windows.Controls;
using TheraEngine;
using TheraEngine.Tests;
using TheraEngine.Worlds;

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
            EngineSettings settings = new EngineSettings()
            {
                OpeningWorld = typeof(TestWorld)
            };
            Engine.Settings = settings;
            Engine.Initialize();
            _panel.RegisterTick();
        }
    }
}
