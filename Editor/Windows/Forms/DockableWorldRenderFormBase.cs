using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Actors.Types.Pawns;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public class DockableWorldRenderFormBase<T> : DockContent, IEditorRenderableControl where T : BaseRenderPanel, new()
    {
        public DockableWorldRenderFormBase(ELocalPlayerIndex playerIndex, int formIndex)
        {
            FormIndex = formIndex;
            PlayerIndex = playerIndex;
            RenderPanel = new T
            {
                Dock = DockStyle.Fill,
                Location = new System.Drawing.Point(0, 0),
                Margin = new Padding(3, 2, 3, 2),
                Name = "RenderPanel",
                Size = new System.Drawing.Size(650, 410),
                TabIndex = 0,
                VsyncMode = EVSyncMode.Disabled,
                ValidPlayerIndices = new List<ELocalPlayerIndex>() { playerIndex }
            };
            RenderPanel.GotFocus += RenderPanel_GotFocus;

            var hud = Engine.DomainProxy.CreateInstance<EditorUI3D>((Vec2)RenderPanel.ClientSize);
            AppDomainHelper.Sponsor(hud);

            EditorPawn = new EditorCameraPawn(PlayerIndex)
            {
                HUD = hud,
                Name = $"Viewport{(FormIndex + 1).ToString()}_EditorCamera"
            };
            var vp = RenderPanel.AddViewport(playerIndex);
            vp.Camera = EditorPawn.Camera;

            Engine.Instance.ProxySet += Instance_ProxySet;
            Engine.Instance.ProxyUnset += Instance_ProxyUnset;
        }

        public T RenderPanel { get; private set; }

        public int FormIndex { get; private set; }
        public ELocalPlayerIndex PlayerIndex { get; private set; } = ELocalPlayerIndex.One;
        public EditorCameraPawn EditorPawn { get; private set; }

        protected virtual IWorld World => Engine.World;
        protected virtual IGameMode GameMode => World?.CurrentGameMode;

        ELocalPlayerIndex IEditorRenderableControl.PlayerIndex => PlayerIndex;
        BaseRenderPanel IEditorRenderableControl.RenderPanel => RenderPanel;
        IPawn IEditorRenderableControl.EditorPawn => EditorPawn;
        IGameMode IEditorRenderableControl.GameMode => GameMode;
        IWorld IEditorRenderableControl.World => World;

        protected virtual void RenderPanel_GotFocus(object sender, EventArgs e)
        {
            Editor.SetActiveEditorControl(this);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);

            if (Editor.ActiveRenderForm == this)
                Editor.SetActiveEditorControl(null);
        }

        protected override void OnShown(EventArgs e)
        {
            World?.SpawnActor(EditorPawn);
            base.OnShown(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            World?.DespawnActor(EditorPawn);
            base.OnClosed(e);
        }
        private void Instance_ProxyUnset(EngineDomainProxy obj)
        {
            EditorPawn.HUD?.Sponsor?.Release();
            EditorPawn.HUD = null;
        }
        private void Instance_ProxySet(EngineDomainProxy obj)
        {
            var hud = Engine.DomainProxy.CreateInstance<EditorUI3D>((Vec2)RenderPanel.ClientSize);
            AppDomainHelper.Sponsor(hud);

            EditorPawn.HUD = hud;
        }
        public void AlignView(BoundingBox aabb)
        {
            //Get aspect of the front plane of the aabb
            aabb.GetCorners(out _, out _, out Vec3 TFL, out Vec3 TFR, out _, out _, out _, out Vec3 BFR);
            float w = Math.Abs(TFR.X - TFL.X);
            float h = Math.Abs(TFR.Y - BFR.Y);
            float boxAspect = w / h;

            //Start the camera off at the center of the aabb
            Vec3 pos = aabb.Translation;

            //Move the camera back to start at the front plane of the aabb
            pos.Z += aabb.HalfExtents.Z;

            //Get the aspect of the camera
            var cam = EditorPawn.Camera;
            if (cam is PerspectiveCamera pcam)
            {
                float camAspect = pcam.Width / pcam.Height;

                //Move the camera back to show all of the front plane rectangle
                pos.Z += boxAspect > camAspect ? pcam.FrustumDistanceAtWidth(w) : pcam.FrustumDistanceAtHeight(h);
            }
            else if (cam is OrthographicCamera ocam)
            {

            }

            EditorPawn.RootComponent.Translation = pos;
        }
        protected override string GetPersistString()
            => GetType().ToString() + "," + FormIndex;
    }
}
