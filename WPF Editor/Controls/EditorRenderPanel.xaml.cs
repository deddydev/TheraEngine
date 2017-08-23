using System.Windows.Controls;
using TheraEngine;
using TheraEngine.Tests;

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
            Engine.SetGamePanel(RenderPanel, false);
            Engine.Initialize(_panel);
            Engine.Run();
        }
    }
}
