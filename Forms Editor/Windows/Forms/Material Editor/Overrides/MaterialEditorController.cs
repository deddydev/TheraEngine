using System.Collections.Generic;
using TheraEngine.Actors;
using TheraEngine.Input;

namespace TheraEditor.Windows.Forms
{
    public class MaterialEditorController : LocalPlayerController
    {
        public MaterialEditorController(LocalPlayerIndex index) : base(index)
        {
            SetViewportCamera = false;
            SetViewportHUD = false;
        }
        public MaterialEditorController(LocalPlayerIndex index, Queue<IPawn> possessionQueue = null)
            : base(index, possessionQueue)
        {
            SetViewportCamera = false;
            SetViewportHUD = false;
        }
    }
}
