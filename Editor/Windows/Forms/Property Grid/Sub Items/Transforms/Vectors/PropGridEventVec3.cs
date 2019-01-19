using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(EventVec3))]
    public partial class PropGridEventVec3 : PropGridItem
    {
        public PropGridEventVec3()
        {
            InitializeComponent();

            numericInputBoxX.Tag = nameof(EventVec3.X);
            numericInputBoxY.Tag = nameof(EventVec3.Y);
            numericInputBoxZ.Tag = nameof(EventVec3.Z);

            numericInputBoxX.GotFocus += InputGotFocus;
            numericInputBoxY.GotFocus += InputGotFocus;
            numericInputBoxZ.GotFocus += InputGotFocus;

            numericInputBoxX.LostFocus += InputLostFocus;
            numericInputBoxY.LostFocus += InputLostFocus;
            numericInputBoxZ.LostFocus += InputLostFocus;
        }
        protected override object RefObject => _eventVec3;

        public EventVec3 _eventVec3;
        protected override void UpdateDisplayInternal(object value)
        {
            _eventVec3 = value as EventVec3;
            bool notNull = _eventVec3 != null;
            bool editable = IsEditable();

            if (notNull)
            {
                numericInputBoxX.Value = _eventVec3.X;
                numericInputBoxY.Value = _eventVec3.Y;
                numericInputBoxZ.Value = _eventVec3.Z;
            }
            else
            {
                numericInputBoxX.Value = null;
                numericInputBoxY.Value = null;
                numericInputBoxZ.Value = null;
            }

            chkNull.Checked = !notNull;
            chkNull.Enabled = editable;
            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = editable && notNull;
        }

        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.X = current.Value;
                OnValueChanged();
            }
        }

        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.Y = current.Value;
                OnValueChanged();
            }
        }

        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
        {
            if (_eventVec3 != null && !_updating)
            {
                _eventVec3.Z = current.Value;
                OnValueChanged();
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
        protected override void OnLabelSet()
        {
            //Label.MouseDown += LabelMouseDown;
            //Label.MouseUp += LabelMouseUp;

            PropGridItemRefPropertyInfo propInfo = GetParentInfo<PropGridItemRefPropertyInfo>();
            if (propInfo.GetOwner() is TObject obj)
            {
                var anims = obj.Animations?.
                    Where(x => x.RootMember?.MemberName == propInfo.Property.Name && x.RootMember?.Animation.File != null).
                    Select(x => new ToolStripMenuItem(x.Name, null, EditAnimation) { Tag = x }).
                    ToArray();

                ContextMenuStrip strip = new ContextMenuStrip();
                if (anims != null && anims.Length > 0)
                {
                    ToolStripMenuItem animsBtn = new ToolStripMenuItem("Animations...");
                    animsBtn.DropDownItems.AddRange(anims);
                    ToolStripMenuItem[] m = new ToolStripMenuItem[]
                    {
                        new ToolStripMenuItem("New Animation", null, CreateAnimation),
                        animsBtn,
                    };
                    strip.Items.AddRange(m);
                }
                else
                {
                    strip.Items.Add(new ToolStripMenuItem("New Animation", null, CreateAnimation));
                }

                strip.RenderMode = ToolStripRenderMode.Professional;
                strip.Renderer = new TheraToolstripRenderer();
                Label.ContextMenuStrip = strip;
            }
        }

        private void EditAnimation(object sender, EventArgs e)
        {
            if (sender is MenuItem item && item.Tag is AnimationTree anim)
            {

            }
        }
        private void CreateAnimation(object sender, EventArgs e)
        {
            PropGridItemRefPropertyInfo propInfo = GetParentInfo<PropGridItemRefPropertyInfo>();
            if (propInfo.GetOwner() is TObject obj)
            {
                string memberName = propInfo.Property.Name;
                var anim = new AnimationTree(memberName, memberName + ".Raw", EAnimationMemberType.Property, new PropAnimVec3(0.0f, true, true));
                
                if (obj.Animations == null)
                    obj.Animations = new EventList<AnimationTree>();
                obj.Animations.Add(anim);

                var menu = Label.ContextMenuStrip.Items;
                var menuItem = new ToolStripMenuItem(anim.Name, null, EditAnimation) { Tag = anim };
                if (menu.Count == 1)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem("Animations...");
                    item.DropDownItems.Add(menuItem);
                    menu.Add(item);
                }
                else
                    ((ToolStripMenuItem)menu[1]).DropDownItems.Add(menuItem);
            }
        }
        private void LabelMouseDown(object sender, MouseEventArgs e)
        {

        }
    }
}
