using System.Collections.Generic;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

namespace TheraEditor.Windows.Forms
{
    public abstract class BaseEditorRenderHandler<TScene, TPawn, TGameMode> :
        RenderHandler<TScene>, IEditorRenderHandler
        where TScene : class, IScene
        where TPawn : class, IPawn
        where TGameMode : class, IGameMode
    {
        public BaseEditorRenderHandler(ELocalPlayerIndex playerIndex) : base()
        {
            PlayerIndex = playerIndex;
            ValidPlayerIndices = new List<ELocalPlayerIndex>() { PlayerIndex };
        }

        public ELocalPlayerIndex PlayerIndex { get; private set; } = ELocalPlayerIndex.One;
        public abstract IWorld World { get; }
        public virtual TGameMode GameMode => World?.CurrentGameMode as TGameMode;

        //protected override TScene GetScene(Viewport v) => World?.Scene as TScene;
        public abstract TPawn EditorPawn { get; }

        ELocalPlayerIndex IEditorRenderHandler.PlayerIndex => PlayerIndex;
        RenderContext IEditorRenderHandler.RenderPanel => Context;
        IPawn IEditorRenderHandler.EditorPawn => EditorPawn;
        IGameMode IEditorRenderHandler.GameMode => GameMode;
        IWorld IEditorRenderHandler.World => World;

        public override RenderContext Context
        {
            get => base.Context;
            set
            {
                if (base.Context != null)
                {
                    base.Context.Disposing -= Disposing;
                }
                base.Context = value;
                if (value != null)
                {
                    value.VSyncMode = EVSyncMode.Adaptive;
                    value.Disposing += Disposing;
                }
            }
        }
        private void Disposing()
        {
            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }
        public override void GotFocus()
            => Editor.SetActiveEditorControl(this);
    }
}
