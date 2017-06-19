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
            Game game = new Game()
            {
                OpeningWorld = typeof(TestWorld),
            };
            Engine.Initialize(game);
            _panel.RegisterTick();
        }
    }
}
