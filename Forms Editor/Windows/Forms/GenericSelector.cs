using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public partial class GenericsSelector : TheraForm
    {
        public static GenericsSelector Current = null;
        protected override void OnActivated(EventArgs e)
        {
            Current = this;
            base.OnActivated(e);
        }
        protected override void OnDeactivate(EventArgs e)
        {
            Current = null;
            base.OnDeactivate(e);
        }
        public GenericsSelector(Type fileType) : base()
        {
            InitializeComponent();

            OriginalType = fileType.GetGenericTypeDefinition();

            Type[] args = OriginalType.GetGenericArguments();
            SelectedTypes = new Type[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                SelectedTypes[i] = null;
                Type genArg = args[i];

                genArg.GetGenericParameterConstraints(out GenericVarianceFlag gvf, out TypeConstraintFlag tcf);
                Type baseType = genArg.BaseType;

                string genericName = genArg.Name;
                bool hasTC = tcf != TypeConstraintFlag.None;
                bool hasBT = baseType != null;

                if (hasBT || gvf != GenericVarianceFlag.None || hasTC)
                {
                    genericName = "where " + genericName + " : ";
                    if (hasBT)
                    {
                        genericName += baseType.GetFriendlyName();
                    }
                    if (hasTC)
                    {
                        if (hasBT)
                            genericName += ", ";
                        switch (tcf)
                        {
                            case TypeConstraintFlag.Struct:
                                genericName += "struct";
                                break;
                            case TypeConstraintFlag.Class:
                                genericName += "class";
                                break;
                            case TypeConstraintFlag.NewClass:
                                genericName += "class, new()";
                                break;
                            case TypeConstraintFlag.NewStructOrClass:
                                genericName += "new()";
                                break;
                        }
                    }
                    if (gvf != GenericVarianceFlag.None)
                    {
                        throw new Exception();
                    }
                }

                GroupBox box = new GroupBox()
                {
                    ForeColor = Color.FromArgb(224, 224, 224),
                    Text = genericName,
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink,
                    Dock = DockStyle.Top,
                };
                MenuStrip menu = new MenuStrip()
                {
                    ForeColor = Color.FromArgb(224, 224, 224),
                    Dock = DockStyle.Top,
                    RenderMode = ToolStripRenderMode.Professional,
                    Renderer = new TheraToolstripRenderer()
                };

                ToolStripDropDownButton root = new ToolStripDropDownButton("Select a type...") { Tag = i };

                Predicate<Type> test = type => 
                {
                    return !(

                    //Base type isn't requested base type?
                    (baseType != null && !baseType.IsAssignableFrom(type)) || 

                    //Doesn't fit constraints?
                    !TypeFitsConstraints(type, gvf, tcf)// ||

                    //Has no default constructor?
                    //type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null
                    );
                };

                EventHandler onClick = (sender, e) =>
                {
                    if (sender is ToolStripDropDownButton button)
                    {
                        Type f = button.Tag as Type;
                        if (f.ContainsGenericParameters)
                        {
                            GenericsSelector gs = new GenericsSelector(f);
                            if (gs.ShowDialog() == DialogResult.OK)
                                f = gs.FinalType;
                            else
                                return;
                        }
                        
                        root.Text = f.GetFriendlyName();
                        SelectedTypes[(int)root.Tag] = f;

                        btnOkay.Enabled = !SelectedTypes.Any(x => x == null);
                    }
                };

                Program.PopulateMenuDropDown(root, onClick, test);

                menu.Items.Add(root);
                box.Controls.Add(menu);
                BodyPanel.Controls.Add(box);
            }
            lblClassName.Text = OriginalType.GetFriendlyName();
        }
        private bool TypeFitsConstraints(Type t, GenericVarianceFlag gvf, TypeConstraintFlag tcf)
        {
            if (gvf != GenericVarianceFlag.None)
                throw new Exception();

            switch (tcf)
            {
                case TypeConstraintFlag.Class:
                    return t.IsClass;
                case TypeConstraintFlag.NewClass:
                    return t.IsClass && t.GetConstructor(new Type[0]) != null;
                case TypeConstraintFlag.NewStructOrClass:
                    return t.GetConstructor(new Type[0]) != null;
                case TypeConstraintFlag.Struct:
                    return t.IsValueType;
            }
            return true;
        }

        public Type OriginalType { get; private set; }
        public Type[] SelectedTypes { get; private set; }
        public Type FinalType { get; private set; }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            FinalType = OriginalType.MakeGenericType(SelectedTypes);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
