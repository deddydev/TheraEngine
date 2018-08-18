using System;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Core.Extensions;

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
        public GenericsSelector(MethodInfo methodType) : base()
        {
            InitializeComponent();

            OriginalMethodType = methodType.GetGenericMethodDefinition();
            lblClassName.Text = OriginalMethodType.GetFriendlyName();

            SetArgumentTypes(OriginalMethodType.GetGenericArguments());
        }
        public GenericsSelector(Type classType) : base()
        {
            InitializeComponent();

            OriginalClassType = classType.GetGenericTypeDefinition();
            lblClassName.Text = OriginalClassType.GetFriendlyName();
            
            SetArgumentTypes(OriginalClassType.GetGenericArguments());
        }
        private void SetArgumentTypes(Type[] args)
        {
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
                    !type.FitsConstraints(gvf, tcf)// ||

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
                            using (GenericsSelector gs = new GenericsSelector(f))
                            {
                                if (gs.ShowDialog(this) == DialogResult.OK)
                                    f = gs.FinalClassType;
                                else
                                    return;
                            }
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
        }

        public Type OriginalClassType { get; private set; }
        public Type FinalClassType { get; private set; }
        
        public MethodInfo OriginalMethodType { get; private set; }
        public MethodInfo FinalMethodType { get; private set; }

        public Type[] SelectedTypes { get; private set; }

        private void btnOkay_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            FinalClassType = OriginalClassType?.MakeGenericType(SelectedTypes);
            FinalMethodType = OriginalMethodType?.MakeGenericMethod(SelectedTypes);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
