using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Worlds.Actors.Components;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class TheraPropertyGrid : UserControl
    {
        public static Dictionary<Type, Type> InPlaceEditorTypes = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> FullEditorTypes = new Dictionary<Type, Type>();
        static TheraPropertyGrid()
        {
            var propControls = Engine.FindTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(PropGridItem)));
            foreach (var propControlType in propControls)
            {
                var attribs = propControlType.GetCustomAttributesExt<PropGridControlForAttribute>();
                if (attribs.Length > 0)
                {
                    PropGridControlForAttribute a = attribs[0];
                    foreach (Type varType in a.Types)
                    {
                        if (InPlaceEditorTypes.ContainsKey(varType))
                            throw new Exception("Type " + varType.GetFriendlyName() + " already has control " + propControlType.GetFriendlyName() + " associated with it.");
                        InPlaceEditorTypes.Add(varType, propControlType);
                    }
                }
            }
            var fullEditors = Engine.FindTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(Form)) && x.GetCustomAttribute<EditorForAttribute>() != null);
            foreach (var editorType in fullEditors)
            {
                var attrib = editorType.GetCustomAttribute<EditorForAttribute>();
                foreach (Type varType in attrib.DataTypes)
                {
                    if (FullEditorTypes.ContainsKey(varType))
                        throw new Exception("Type " + varType.GetFriendlyName() + " already has editor " + editorType.GetFriendlyName() + " associated with it.");
                    FullEditorTypes.Add(varType, editorType);
                }
            }
        }
        public TheraPropertyGrid()
        {
            InitializeComponent();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            PropGridItem.UpdateTimer.StartMultiFire(PropGridItem.UpdateVisibleItems, Editor.Settings.File.PropertyGrid.File.UpdateRateInSeconds);
            base.OnHandleCreated(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            PropGridItem.UpdateTimer.Stop();
            base.OnHandleDestroyed(e);
        }

        private const string MiscName = "Miscellaneous";
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();

        private object _subObject;
        private object SubObject
        {
            get => _subObject;
            set
            {
                //Do nothing if target object is the same
                if (_subObject == value)
                    return;

                //Destroy old properties
                if (_subObject != null)
                    LoadProperties(null);

                _subObject = value;

                if (_subObject is TObject obj)
                    obj.EditorState.Selected = true;

                //If scene component, select it in the scene
                if (Engine.LocalPlayers.Count > 0 && 
                    Engine.LocalPlayers[0]?.ControlledPawn?.HUD is EditorHud hud)
                {
                    if (_subObject is SceneComponent sceneComp)
                        hud.SelectedComponent = sceneComp;
                    else
                        hud.SelectedComponent = null;
                }
                
                //Load the properties of the object
                LoadProperties(_subObject);
            }
        }

        private bool _updating;
        private object _targetObject;
        public object TargetObject
        {
            get => _targetObject;
            set
            {
                if (_targetObject == value)
                    return;

                if (_targetObject != null)
                {
                    pnlProps.Controls.Clear();
                    foreach (var category in _categories.Values)
                        category.DestroyProperties();
                    _categories.Clear();
                }

                _targetObject = value;
                  
                lblObjectName.Visible = Enabled = _targetObject != null;
                if (!Enabled)
                    return;

                lblObjectName.Text = string.Format("{0} [{1}]",
                    _targetObject.ToString(), 
                    _targetObject.GetType().GetFriendlyName());

                lblObjectName.Visible = true;

                if (_targetObject is IActor actor)
                {
                    _updating = true;
                    treeViewSceneComps.Nodes.Clear();
                    PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor.RootComponent);
                    PopulateLogicComponentList(actor.LogicComponents);

                    lblProperties.Visible = true;
                    lblSceneComps.Visible = true;
                    treeViewSceneComps.Visible = true;
                    _updating = false;

                    //treeViewSceneComps.SelectedNode = treeViewSceneComps.Nodes[0];
                }
                else
                {
                    _updating = true;
                    lblLogicComps.Visible = false;
                    lblSceneComps.Visible = false;
                    lblProperties.Visible = false;
                    treeViewSceneComps.Visible = false;
                    lstLogicComps.Visible = false;
                    _updating = false;
                }
                SubObject = value;
            }
        }

        private void PopulateLogicComponentList(EventList<LogicComponent> logicComponents)
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
            public bool ReadOnly { get; set; }
        }
        
        private void LoadProperties(object obj)
        {
            if (Disposing || IsDisposed)
                return;

            //pnlProps.SuspendLayout();

            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();

            if (obj == null)
            {
                //pnlProps.ResumeLayout(true);
                return;
            }

            PropertyInfo[] props = null;
            MethodInfo[] methods = null;
            ConcurrentDictionary<int, PropertyData> info = new ConcurrentDictionary<int, PropertyData>();
            Task.Run(() =>
            {
                Type targetObjectType = obj.GetType();
                props = targetObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                methods = targetObjectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                Parallel.For(0, props.Length, i =>
                {
                    PropertyInfo prop = props[i];
                    var indexParams = prop.GetIndexParameters();
                    if (indexParams != null && indexParams.Length > 0)
                        return;

                    object propObj = prop.GetValue(obj);
                    Type subType = propObj?.GetType() ?? prop.PropertyType;
                    var attribs = prop.GetCustomAttributes(true);
                    bool readOnly = false;

                    foreach (var attrib in attribs)
                    {
                        if (attrib is BrowsableAttribute browsable && !browsable.Browsable)
                            return;
                        if (attrib is BrowsableIf browsableIf && !browsableIf.Evaluate(obj))
                            return;
                        if (attrib is ReadOnlyAttribute readOnlyAttrib)
                            readOnly = readOnlyAttrib.IsReadOnly;
                    }

                    PropertyData p = new PropertyData()
                    {
                        ControlTypes = GetControlTypes(subType),
                        Property = prop,
                        Attribs = attribs,
                        ReadOnly = readOnly,
                    };

                    info.TryAdd(i, p);
                });
            }).ContinueWith(t =>
            {
                if (!Disposing && !IsDisposed)
                    Invoke((Action)(() =>
                    {
                        for (int i = 0; i < props.Length; ++i)
                        {
                            if (!info.ContainsKey(i))
                                continue;
                            PropertyData p = info[i];
                            CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs, p.ReadOnly);
                        }

                        for (int i = 0; i < methods.Length; ++i)
                        {
                            MethodInfo p = methods[i];
                            //CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs);
                        }

                        if (Editor.Settings.File.PropertyGrid.File.IgnoreLoneSubCategories && _categories.Count == 1)
                            _categories.Values.ToArray()[0].CategoryName = null;

                        //pnlProps.ResumeLayout(true);
                    }));
            });
        }

        public static Deque<Type> GetControlTypes(Type propertyType)
        {
            Type mainControlType = null;
            Type subType = propertyType;
            Deque<Type> controlTypes = new Deque<Type>();
            while (subType != null)
            {
                if (mainControlType == null && InPlaceEditorTypes.ContainsKey(subType))
                {
                    mainControlType = InPlaceEditorTypes[subType];
                    if (!controlTypes.Contains(mainControlType))
                        controlTypes.PushFront(mainControlType);
                }
                Type[] interfaces = subType.GetInterfaces();
                foreach (Type i in interfaces)
                    if (InPlaceEditorTypes.ContainsKey(i))
                    {
                        Type controlType = InPlaceEditorTypes[i];
                        if (!controlTypes.Contains(controlType))
                            controlTypes.PushBack(controlType);
                    }

                subType = subType.BaseType;
            }
            if (controlTypes.Count == 0)
            {
                Engine.LogWarning("Unable to find control for " + propertyType == null ? "null" : propertyType.GetFriendlyName());
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
            object[] attribs,
            bool readOnly)
        {
            var controls = CreateControls(controlTypes, prop, obj);
            
            var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
            string catName = category == null ? MiscName : category.Category;
            if (categories.ContainsKey(catName))
                categories[catName].AddProperty(controls, attribs, readOnly);
            else
            {
                PropGridCategory misc = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                misc.AddProperty(controls, attribs, readOnly);
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
            if (lstLogicComps.Visible)
                lblProperties.MouseMove += MouseMoveLogicComps;
            else
                lblProperties.MouseMove += MouseMoveSceneComps;
        }
        private void lblLogicComps_MouseUp(object sender, MouseEventArgs e)
        {
            lblLogicComps.MouseMove -= MouseMoveSceneComps;
        }
        private void lblProperties_MouseUp(object sender, MouseEventArgs e)
        {
            if (lstLogicComps.Visible)
                lblProperties.MouseMove -= MouseMoveLogicComps;
            else
                lblProperties.MouseMove -= MouseMoveSceneComps;
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

        private void lstLogicComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            SubObject = lstLogicComps.SelectedItem;
        }

        private void lblObjectName_MouseEnter(object sender, EventArgs e)
        {
            lblObjectName.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void lblObjectName_MouseLeave(object sender, EventArgs e)
        {
            lblObjectName.BackColor = Color.FromArgb(55, 55, 60);
        }

        private void lblObjectName_Click(object sender, EventArgs e)
        {
            SubObject = _targetObject;
        }
        
        private void treeViewSceneComps_MouseDown(object sender, MouseEventArgs e)
        {
            if (_updating)
                return;
            TreeNode node = treeViewSceneComps.GetNodeAt(e.Location);
            if (node != null)
            {
                Rectangle r = node.Bounds;
                r.X -= 25; r.Width += 25;
                if (r.Contains(e.Location))
                    SubObject = node.Tag;
            }
        }

        private void treeViewSceneComps_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_updating || e.Action == TreeViewAction.ByMouse)
                return;
            TreeNode node = e.Node;
            if (node != null)
            {
                SceneComponent s = node.Tag as SceneComponent;
                SubObject = node.Tag;
            }
        }

        //protected override void OnMouseEnter(EventArgs e)
        //{
        //    PropGridItem.UpdateTimer.Stop();
        //    base.OnMouseEnter(e);
        //}

        //protected override void OnMouseLeave(EventArgs e)
        //{
        //    base.OnMouseLeave(e);
        //}

        //protected override void OnGotFocus(EventArgs e)
        //{
        //    base.OnGotFocus(e);
        //}

        //protected override void OnLostFocus(EventArgs e)
        //{
        //    base.OnLostFocus(e);
        //}
    }
}
