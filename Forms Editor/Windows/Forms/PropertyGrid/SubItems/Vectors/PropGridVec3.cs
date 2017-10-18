using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridItem(typeof(Vec3))]
    public partial class PropGridVec3 : PropGridItem
    {
        public PropGridVec3() => InitializeComponent();
        
        protected override void UpdateDisplayInternal()
        {
            object value = GetPropertyValue();
            if (value is Vec3 vec3Val)
            {
                numericInputBoxX.Value = vec3Val.X;
                numericInputBoxY.Value = vec3Val.Y;
                numericInputBoxZ.Value = vec3Val.Z;
            }
            else
                throw new Exception(Property.PropertyType.GetFriendlyName() + " is not a Vec3 type.");
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
                var anim = new AnimationContainer("NewAnimVec3", Property.Name, false, new PropAnimVec3(0.0f, true, true));
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
        
        private void numericInputBoxX_ValueChanged(Single? previous, Single? current)
            => UpdatePropertyValue(new Vec3(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value));
        private void numericInputBoxY_ValueChanged(Single? previous, Single? current)
            => UpdatePropertyValue(new Vec3(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value));
        private void numericInputBoxZ_ValueChanged(Single? previous, Single? current)
            => UpdatePropertyValue(new Vec3(
                numericInputBoxX.Value.Value,
                numericInputBoxY.Value.Value,
                numericInputBoxZ.Value.Value));
    }
}
