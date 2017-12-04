using System;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimFloat))]
    public partial class PropAnimFloatEditor : DockContent
    {
        private UIMaterialEditor _hud;
        public PropAnimFloatEditor()
        {
            InitializeComponent();
        }
        public PropAnimFloatEditor(PropAnimFloat anim) : this()
        {
            Animation = anim;
        }
        private PropAnimFloat _animation;
        public PropAnimFloat Animation
        {
            get => _animation;
            set
            {
                _animation = value;
            }
        }
    }
}
