using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using TheraEngine;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class TheraPropertyGrid : UserControl
    {
        public static Dictionary<Type, Type> SubItemControlTypes = new Dictionary<Type, Type>();
        static TheraPropertyGrid()
        {
            var propControls = Program.FindPublicTypes(x => x.IsSubclassOf(typeof(PropGridItem)));
            foreach (var propControlType in propControls)
            {
                var attribs = propControlType.GetCustomAttributesExt<PropGridItemAttribute>();
                if (attribs.Length > 0)
                {
                    PropGridItemAttribute a = attribs[0];
                    foreach (Type varType in a.Types)
                    {
                        if (SubItemControlTypes.ContainsKey(varType))
                            throw new Exception("Type " + varType.GetFriendlyName() + " already has control " + propControlType.GetFriendlyName() + " associated with it.");
                        SubItemControlTypes.Add(varType, propControlType);
                    }
                }
            }
        }
        public TheraPropertyGrid()
        {
            InitializeComponent();
        }

        private const string MiscName = "Miscellaneous";
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();

        private object _targetObject;
        public object TargetObject
        {
            get => _targetObject;
            set
            {
                _targetObject = value;
                if (_targetObject == null)
                    return;
                if (_targetObject is IActor actor)
                {
                    treeViewSceneComps.Nodes.Clear();
                    PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor.RootComponent);

                    lblLogicComps.Visible = true;
                    lblSceneComps.Visible = true;
                    treeViewSceneComps.Visible = true;
                    lstLogicComps.Visible = true;
                }
                else
                {
                    lblLogicComps.Visible = false;
                    lblSceneComps.Visible = false;
                    treeViewSceneComps.Visible = false;
                    lstLogicComps.Visible = false;

                    LoadProperties(TargetObject);
                }
            }
        }

        private void LoadProperties(object obj)
        {
            pnlProps.SuspendLayout();
            Type targetObjectType = obj.GetType();
            PropertyInfo[] props = targetObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();

            foreach (PropertyInfo prop in props)
            {
                var indexParams = prop.GetIndexParameters();
                if (indexParams != null && indexParams.Length > 0)
                    continue;

                Type subType = prop.PropertyType;
                var attribs = prop.GetCustomAttributes(true);
                if (attribs.FirstOrDefault(x => x is BrowsableAttribute) is BrowsableAttribute browsable && !browsable.Browsable)
                    continue;

                Type mainControlType = null;
                Deque<Type> controlTypes = new Deque<Type>();
                while (subType != null)
                {
                    if (mainControlType == null && SubItemControlTypes.ContainsKey(subType))
                    {
                        mainControlType = SubItemControlTypes[subType];
                        if (!controlTypes.Contains(mainControlType))
                            controlTypes.PushFront(mainControlType);
                    }
                    Type[] interfaces = subType.GetInterfaces();
                    foreach (Type i in interfaces)
                        if (SubItemControlTypes.ContainsKey(i))
                        {
                            Type controlType = SubItemControlTypes[i];
                            if (!controlTypes.Contains(controlType))
                                controlTypes.PushBack(controlType);
                        }
                    
                    subType = subType.BaseType;
                }

                if (controlTypes.Count == 0)
                {
                    Engine.PrintLine("Unable to find control for " + prop.PropertyType.GetFriendlyName());
                    controlTypes.PushBack(typeof(PropGridText));
                }
                CreateControl(controlTypes, prop, obj, attribs);
            }
            pnlProps.ResumeLayout(true);
        }

        private void CreateControl(Deque<Type> controlTypes, PropertyInfo prop, object obj, object[] attribs)
        {
            var controls = controlTypes.Select(x =>
            {
                var control = Activator.CreateInstance(x) as PropGridItem;
                control.SetProperty(prop, obj);
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                control.Show();
                return control;
            }).ToList();
            
            var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
            string catName = category == null ? MiscName : category.Category;
            if (_categories.ContainsKey(catName))
                _categories[catName].AddProperty(controls, attribs);
            else
            {
                PropGridCategory misc = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                misc.AddProperty(controls, attribs);
                _categories.Add(catName, misc);
                pnlProps.Controls.Add(misc);
            }
            //}
            //else
            //    Engine.PrintLine("Unable to find control for " + subType);
        }

        private void PopulateSceneComponentTree(TreeNodeCollection nodes, SceneComponent currentSceneComp)
        {
            TreeNode s = new TreeNode(currentSceneComp.Name) { Tag = currentSceneComp };
            foreach (SceneComponent childSceneComp in currentSceneComp.ChildComponents)
            {
                PopulateSceneComponentTree(s.Nodes, childSceneComp);
            }
            nodes.Add(s);
        }

        int _y = 0;
        private void lblLogicComps_MouseDown(object sender, MouseEventArgs e)
        {
            _y = e.Y;
            lblLogicComps.MouseMove += MouseMoveSceneComps;
        }

        private void lblProperties_MouseDown(object sender, MouseEventArgs e)
        {
            _y = e.Y;
            lblProperties.MouseMove += MouseMoveLogicComps;
        }
        private void lblLogicComps_MouseUp(object sender, MouseEventArgs e)
        {
            lblLogicComps.MouseMove -= MouseMoveSceneComps;
        }
        private void lblProperties_MouseUp(object sender, MouseEventArgs e)
        {
            lblProperties.MouseMove -= MouseMoveLogicComps;
        }
        private void MouseMoveSceneComps(object sender, MouseEventArgs e)
        {
            int diff = e.Y - _y;
            treeViewSceneComps.Height += diff;
        }
        private void MouseMoveLogicComps(object sender, MouseEventArgs e)
        {
            int diff = e.Y - _y;
            lstLogicComps.Height += diff;
        }

        private void treeViewSceneComps_AfterSelect(object sender, TreeViewEventArgs e)
        {
            LoadProperties(treeViewSceneComps.SelectedNode.Tag);
        }

        private void lstLogicComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProperties(lstLogicComps.SelectedItem);
        }
    }
}
