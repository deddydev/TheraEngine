using Extensions;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Core.Reflection;

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
        public GenericsSelector(MethodInfoProxy methodType) : base()
        {
            InitializeComponent();

            OriginalMethodType = methodType.GetGenericMethodDefinition();
            lblClassName.Text = OriginalMethodType.GetFriendlyName();

            SetArgumentTypes(OriginalMethodType.GetGenericArguments());
        }
        public GenericsSelector(TypeProxy classType) : base()
        {
            InitializeComponent();

            OriginalClassType = classType.GetGenericTypeDefinition();
            lblClassName.Text = OriginalClassType.GetFriendlyName();
            
            SetArgumentTypes(OriginalClassType.GetGenericArguments());
        }
        //public GenericsSelector(Type classTypeDefinition, Type[] genericArguments) : base()
        //{
        //    InitializeComponent();

        //    OriginalClassType = classTypeDefinition;
        //    lblClassName.Text = classTypeDefinition.MakeGenericType(genericArguments).GetFriendlyName();

        //    SetArgumentTypes(OriginalClassType.GetGenericArguments());
        //}
        private void SetArgumentTypes(TypeProxy[] args)
        {
            SelectedTypes = new TypeProxy[args.Length];
            for (int i = 0; i < args.Length; ++i)
            {
                SelectedTypes[i] = null;
                TypeProxy genArg = args[i];

                genArg.GetGenericParameterConstraints(out EGenericVarianceFlag gvf, out ETypeConstraintFlag tcf);
                TypeProxy baseType = genArg.BaseType;

                string genericName = genArg.Name;
                bool hasTC = tcf != ETypeConstraintFlag.None;
                bool hasBT = baseType != null;
                var interfaces = genArg.GetInterfaces().ToList();
                for (int x = 0; x < interfaces.Count; ++x)
                {
                    TypeProxy t = interfaces[x];
                    if (baseType != null && t.IsAssignableFrom(baseType))
                    {
                        interfaces.RemoveAt(x--);
                        continue;
                    }
                    for (int j = x + 1; j < interfaces.Count; j++)
                    {
                        TypeProxy t2 = interfaces[j];
                        if (t.IsAssignableFrom(t2))
                        {
                            interfaces.RemoveAt(x--);
                            break;
                        }
                    }
                }

                if (hasBT || gvf != EGenericVarianceFlag.None || hasTC)
                {
                    genericName = "where " + genericName + " : ";
                    if (hasBT)
                    {
                        genericName += baseType.GetFriendlyName();
                        if (interfaces.Count > 0)
                            genericName += ", ";
                    }
                    bool first = true;
                    foreach (Type iType in interfaces)
                    {
                        if (first)
                            first = false;
                        else
                            genericName += ", ";
                        genericName += iType.GetFriendlyName();
                    }
                    if (hasTC)
                    {
                        if (hasBT || interfaces.Count > 0)
                            genericName += ", ";
                        switch (tcf)
                        {
                            case ETypeConstraintFlag.Struct:
                                genericName += "struct";
                                break;
                            case ETypeConstraintFlag.Class:
                                genericName += "class";
                                break;
                            case ETypeConstraintFlag.NewClass:
                                genericName += "class, new()";
                                break;
                            case ETypeConstraintFlag.NewStructOrClass:
                                genericName += "new()";
                                break;
                        }
                    }
                    if (gvf != EGenericVarianceFlag.None)
                    {
                        //throw new Exception();
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

                ToolStripMenuItem root = new ToolStripMenuItem("Select a type...") { Tag = i };

                bool test(TypeProxy type)
                {
                    return !(
                    
                    //Base type isn't requested base type?
                    (baseType != null && !type.IsAssignableTo(baseType)) ||

                    !interfaces.All(x => type.IsAssignableTo(x)) ||

                    //Doesn't fit constraints?
                    !type.FitsConstraints(gvf, tcf)// ||

                    //Has no default constructor?
                    //type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) is null
                    );
                }

                void onClick(object sender, EventArgs e)
                {
                    if (sender is ToolStripMenuItem button)
                    {
                        TypeProxy f = button.Tag as TypeProxy;
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

                        btnOkay.Enabled = !SelectedTypes.Any(x => x is null);
                    }
                }

                Program.PopulateMenuDropDown(root, onClick, test);

                menu.Items.Add(root);
                box.Controls.Add(menu);
                BodyPanel.Controls.Add(box);
            }
        }

        public TypeProxy OriginalClassType { get; private set; }
        public TypeProxy FinalClassType { get; private set; }
        
        public MethodInfoProxy OriginalMethodType { get; private set; }
        public MethodInfoProxy FinalMethodType { get; private set; }

        public TypeProxy[] SelectedTypes { get; private set; }

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
