using System.Windows.Controls;
using TheraEngine;
using TheraEngine.Tests;
using TheraEngine.Worlds;

namespace TheraEditor.Controls
{
    /// <summary>
    /// Interaction logic for RenderPanel.xaml
    /// </summary>
    public partial class EditorRenderPanel : UserControl
    {
        RenderPanel _panel;
        public EditorRenderPanel()
        {
            Game game = new Game()
            {
                OpeningWorld = typeof(TestWorld),
            };
            Engine.SetGame(game);
            InitializeComponent();
            FormsHost.Child = _panel = new RenderPanel();
            Engine.Initialize(_panel);
            Engine.Run();
        }
    }
}
