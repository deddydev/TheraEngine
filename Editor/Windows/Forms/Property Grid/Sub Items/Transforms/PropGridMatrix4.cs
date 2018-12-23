using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Matrix4))]
    public partial class PropGridMatrix4 : PropGridItem
    {
        public PropGridMatrix4()
        {
            InitializeComponent();
            _boxes = new NumericInputBoxSingle[,]
            {
                { r0c0, r0c1, r0c2, r0c3 },
                { r1c0, r1c1, r1c2, r1c3 },
                { r2c0, r2c1, r2c2, r2c3 },
                { r3c0, r3c1, r3c2, r3c3 },
            };
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c)
                {
                    NumericInputBoxSingle box = _boxes[r, c];
                    box.ValueChanged += PropGridMatrix4_ValueChanged;
                    box.GotFocus += Box_GotFocus;
                    box.LostFocus += Box_LostFocus;
                }
        }

        private void Box_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void Box_GotFocus(object sender, EventArgs e) => IsEditing = true;

        private void PropGridMatrix4_ValueChanged(NumericInputBoxBase<float> box, float? previous, float? current)
            => UpdateValue(new Matrix4(
                r0c0.Value.Value, r0c1.Value.Value, r0c2.Value.Value, r0c3.Value.Value,
                r1c0.Value.Value, r1c1.Value.Value, r1c2.Value.Value, r1c3.Value.Value,
                r2c0.Value.Value, r2c1.Value.Value, r2c2.Value.Value, r2c3.Value.Value,
                r3c0.Value.Value, r3c1.Value.Value, r3c2.Value.Value, r3c3.Value.Value), false);

        private NumericInputBoxSingle[,] _boxes;

        protected override void UpdateDisplayInternal(object value)
        {
            bool editable = IsEditable();
            Matrix4 m = (Matrix4)value;
            for (int r = 0; r < 4; ++r)
                for (int c = 0; c < 4; ++c)
                {
                    var box = _boxes[r, c];
                    box.Value = m[r, c];
                    box.Enabled = editable;
                }
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
                var anim = new AnimationTree("NewAnimMatrix4", propInfo.Property.Name, false, new PropAnimMatrix4(0.0f, true, true));
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
    }
}
