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
using System.Collections;
using System.Collections.Concurrent;

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

        private object _subObject;
        private object SubObject
        {
            get => _subObject;
            set
            {
                _subObject = value;
                LoadProperties(_subObject);
            }
        }

        private object _targetObject;
        public object TargetObject
        {
            get => _targetObject;
            set
            {
                _targetObject = value;

                if (_targetObject == null)
                {
                    pnlProps.Controls.Clear();
                    foreach (var category in _categories.Values)
                        category.DestroyProperties();
                    _categories.Clear();

                    lblObjectName.Visible = false;
                    return;
                }

                lblObjectName.Text = _targetObject.ToString() + " [" + _targetObject.GetType().GetFriendlyName() + "]";
                lblObjectName.Visible = true;

                if (_targetObject is IActor actor)
                {
                    treeViewSceneComps.Nodes.Clear();
                    PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor.RootComponent);
                    PopulateLogicComponentList(actor.LogicComponents);

                    lblProperties.Visible = true;
                    lblSceneComps.Visible = true;
                    treeViewSceneComps.Visible = true;
                    
                    //treeViewSceneComps.SelectedNode = treeViewSceneComps.Nodes[0];
                }
                else
                {
                    lblLogicComps.Visible = false;
                    lblSceneComps.Visible = false;
                    lblProperties.Visible = false;
                    treeViewSceneComps.Visible = false;
                    lstLogicComps.Visible = false;

                    SubObject = value;
                }
            }
        }

        private void PopulateLogicComponentList(MonitoredList<LogicComponent> logicComponents)
        {
            if (lstLogicComps.Visible = lblLogicComps.Visible = 
                logicComponents != null && logicComponents.Count > 0)
            {
                lstLogicComps.DataSource = logicComponents;
                //lstLogicComps.Items.AddRange(logicComponents.ToArray());
            }
        }

        private class PropertyData
        {
            public Deque<Type> ControlTypes { get; set; }
            public PropertyInfo Property { get; set; }
            public object[] Attribs { get; set; }
        }
        
        private async void LoadProperties(object obj)
        {
            pnlProps.SuspendLayout();

            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();

            Type targetObjectType = obj.GetType();
            PropertyInfo[] props = targetObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            MethodInfo[] methods = targetObjectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            
            ConcurrentDictionary<int, PropertyData> info = new ConcurrentDictionary<int, PropertyData>();
            await Task.Run(() => Parallel.For(0, props.Length, i =>
            {
                PropertyInfo prop = props[i];
                var indexParams = prop.GetIndexParameters();
                if (indexParams != null && indexParams.Length > 0)
                    return;

                Type subType = prop.PropertyType;
                var attribs = prop.GetCustomAttributes(true);
                if (attribs.FirstOrDefault(x => x is BrowsableAttribute) is BrowsableAttribute browsable && !browsable.Browsable)
                    return;

                PropertyData p = new PropertyData()
                {
                    ControlTypes = GetControlTypes(subType),
                    Property = prop,
                    Attribs = attribs,
                };

                //BeginInvoke((Action)(() => CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs)));

                info.TryAdd(i, p);
            }));

            for (int i = 0; i < props.Length; ++i)
            {
                if (!info.ContainsKey(i))
                    continue;
                PropertyData p = info[i];
                CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs);
            }

            for (int i = 0; i < methods.Length; ++i)
            {
                MethodInfo p = methods[i];
                //CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs);
            }

            if (_categories.Count == 1 && _categories.ContainsKey(MiscName))
                _categories[MiscName].CategoryName = null;

            pnlProps.ResumeLayout(true);
        }

        public static Deque<Type> GetControlTypes(Type propertyType)
        {
            Type mainControlType = null;
            Type subType = propertyType;
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
                Engine.Log("Unable to find control for " + propertyType == null ? "null" : propertyType.GetFriendlyName());
                controlTypes.PushBack(typeof(PropGridText));
            }
            return controlTypes;
        }

        public static List<PropGridItem> CreateControls(Deque<Type> controlTypes, PropertyInfo prop, object obj)
        {
            return controlTypes.Select(x =>
            {
                PropGridItem control = Activator.CreateInstance(x) as PropGridItem;
                control.SetProperty(prop, obj);
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                control.Show();
                return control;
            }).ToList();
        }
        public static List<PropGridItem> CreateControls(Deque<Type> controlTypes, IList list, int listIndex)
        {
            Type elementType = list.DetermineElementType();
            return controlTypes.Select(x =>
            {
                var control = Activator.CreateInstance(x) as PropGridItem;
                control.SetIListOwner(list, elementType, listIndex);
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                control.Show();
                return control;
            }).ToList();
        }
        public static void CreateControls(
            Deque<Type> controlTypes,
            PropertyInfo prop,
            Panel panel,
            Dictionary<string, PropGridCategory> categories,
            object obj,
            object[] attribs)
        {
            var controls = CreateControls(controlTypes, prop, obj);
            
            var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
            string catName = category == null ? MiscName : category.Category;
            if (categories.ContainsKey(catName))
                categories[catName].AddProperty(controls, attribs);
            else
            {
                PropGridCategory misc = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                misc.AddProperty(controls, attribs);
                categories.Add(catName, misc);
                panel.Controls.Add(misc);
            }
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
            SubObject = treeViewSceneComps.SelectedNode.Tag;
        }

        private void lstLogicComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            SubObject = lstLogicComps.SelectedItem;
        }
    }
}
