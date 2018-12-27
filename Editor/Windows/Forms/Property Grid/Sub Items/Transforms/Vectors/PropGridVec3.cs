using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Vec3))]
    public partial class PropGridVec3 : PropGridItem
    {
        public PropGridVec3()
        {
            InitializeComponent();
            numericInputBoxX.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxY.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxZ.GotFocus += NumericInputBoxX_GotFocus;
            numericInputBoxX.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxY.LostFocus += NumericInputBoxX_LostFocus;
            numericInputBoxZ.LostFocus += NumericInputBoxX_LostFocus;
        }
        private void NumericInputBoxX_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void NumericInputBoxX_GotFocus(object sender, EventArgs e) => IsEditing = true;

        protected override void UpdateDisplayInternal(object value)
        {
            Vec3 v = (Vec3)value;
            numericInputBoxX.Value = v.X;
            numericInputBoxY.Value = v.Y;
            numericInputBoxZ.Value = v.Z;

            numericInputBoxX.Enabled = numericInputBoxY.Enabled = numericInputBoxZ.Enabled = IsEditable();
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
                    Select(x => new MenuItem(x.Name, EditAnimation) { Tag = x }).
                    ToArray();

                if (anims != null && anims.Length > 0)
                {
                    MenuItem[] m = new MenuItem[]
                    {
                        new MenuItem("New Animation", CreateAnimation),
                        new MenuItem("Animations...", anims),
                    };
                    Label.ContextMenu = new ContextMenu(m);
                }
                else
                {
                    MenuItem[] m = new MenuItem[]
                    {
                        new MenuItem("New Animation", CreateAnimation),
                    };
                    Label.ContextMenu = new ContextMenu(m);
                }
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
                var anim = new AnimationTree("NewAnimVec3", propInfo.Property.Name, EAnimationMemberType.Property, new PropAnimVec3(0.0f, true, true));
                obj.AddAnimation(anim);
                var menu = Label.ContextMenu.MenuItems;
                var menuItem = new MenuItem(anim.Name, EditAnimation) { Tag = anim };
                if (menu.Count == 1)
                    menu.Add(new MenuItem("Animations...", new MenuItem[] { menuItem }));
                else
                    menu[1].MenuItems.Add(menuItem);
            }
        }
        
        private void LabelMouseDown(object sender, MouseEventArgs e)
        {

        }
        
        private void numericInputBox_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec3(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value), false);
    }
}
