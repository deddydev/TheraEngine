using CustomEngine.World;
using OpenTK;

namespace CustomEngine
{
    public partial class CustomGameForm : GameWindow
    {
        public static CustomGameForm Instance { get { return _instance ?? new CustomGameForm("ERROR"); } }
        private static CustomGameForm _instance;

        public WorldBase _currentWorld = null;

        public CustomGameForm(string title)
        {
            Title = title;
            _instance = this;
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _currentWorld.Tick(e.Time);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            _currentWorld.RenderTick(e.Time);
        }

        public void LoadWorld(WorldBase world)
        {
            if (_currentWorld != null)
                _currentWorld.OnUnload();

            _currentWorld = world;

            if (_currentWorld != null)
                _currentWorld.OnLoad();
        }
    }
}
