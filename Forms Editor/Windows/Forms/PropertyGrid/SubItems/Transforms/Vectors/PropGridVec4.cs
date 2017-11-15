using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(Vec4))]
    public partial class PropGridVec4 : PropGridItem
    {
        public PropGridVec4() => InitializeComponent();
        
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            if (value is Vec4 Vec4Val)
            {
                numericInputBoxX.Value = Vec4Val.X;
                numericInputBoxY.Value = Vec4Val.Y;
                numericInputBoxZ.Value = Vec4Val.Z;
            }
            else
                throw new Exception(DataType.GetFriendlyName() + " is not a Vec4 type.");
        }

        protected override void OnLabelSet()
        {
            //Label.MouseDown += LabelMouseDown;
            //Label.MouseUp += LabelMouseUp;

            if (PropertyOwner is ObjectBase obj)
            {
                var anims = obj.Animations?.
                    Where(x => x.RootFolder?._propertyName == Property.Name && x.RootFolder?._animation != null).
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
            if (sender is MenuItem item && item.Tag is AnimationContainer anim)
            {

            }
        }
        private void CreateAnimation(object sender, EventArgs e)
        {
            if (PropertyOwner is ObjectBase obj)
            {
                var anim = new AnimationContainer("NewAnimVec4", Property.Name, false, new PropAnimVec4(0.0f, true, true));
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
        
        private void numericInputBoxX_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value));
        private void numericInputBoxY_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value));
        private void numericInputBoxZ_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value));
        private void numericInputBoxW_ValueChanged(NumericInputBoxBase<Single> box, Single? previous, Single? current)
            => UpdateValue(new Vec4(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value,
                numericInputBoxW.Value.Value));
    }
}
