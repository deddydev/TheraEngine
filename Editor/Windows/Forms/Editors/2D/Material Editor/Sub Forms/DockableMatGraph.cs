//using ComponentOwl.BetterListView;
using System.Collections.Generic;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Input;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public class DockableMatGraph : DockableRenderableFileEditor<TMaterial, MatGraphRenderHandler>
    {
        public override bool ShouldHideCursor => true;
        protected override bool TrySetFile(TMaterial file)
        {
            if (!base.TrySetFile(file))
                return false;

            RenderPanel.RenderHandler.UI.TargetMaterial = file;
            return true;
        }
    }
    public class MaterialEditorController : LocalPlayerController
    {
        public MaterialEditorController(ELocalPlayerIndex index) : this(index, null) { }
        public MaterialEditorController(ELocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue) => InheritControlledPawnCamera = InheritControlledPawnHUD = false;
    }
    public class MatGraphRenderHandler : UIRenderHandler<MaterialEditorUI, MaterialEditorGameMode, MaterialEditorController> { }
    public class MaterialEditorGameMode : UIGameMode<MaterialEditorUI, MaterialEditorController> { }
}
