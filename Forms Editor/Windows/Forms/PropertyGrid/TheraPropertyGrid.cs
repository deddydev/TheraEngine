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
using TheraEngine.Files;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class TheraPropertyGrid : UserControl, IDataChangeHandler
    {
        public static Dictionary<Type, Type> InPlaceEditorTypes = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> FullEditorTypes = new Dictionary<Type, Type>();
        static TheraPropertyGrid()
        {
            if (Engine.DesignMode)
                return;

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

        public TheraPropertyGrid() => InitializeComponent();
        
        internal static GameTimer UpdateTimer = new GameTimer();
        protected override void OnHandleCreated(EventArgs e)
        {
            if (!Engine.DesignMode)
                UpdateTimer.StartMultiFire(PropGridItem.UpdateVisibleItems, Editor.GetSettings().PropertyGridRef.File.UpdateRateInSeconds);
            base.OnHandleCreated(e);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!Engine.DesignMode)
                UpdateTimer.Stop();
            base.OnHandleDestroyed(e);
        }

        private const string MiscName = "Miscellaneous";
        private const string MethodName = "Methods";
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();

        private object _subObject;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object SubObject
        {
            get => _subObject;
            set
            {
                //Do nothing if target object is the same
                if (_subObject == value)
                    return;

                if (_subObject != null)
                {
                    pnlProps.Controls.Clear();
                    foreach (var category in _categories.Values)
                        category.DestroyProperties();
                    _categories.Clear();
                }

                _subObject = value;
                
                if (_subObject is TObject obj)
                    obj.EditorState.Selected = true;
                
                //Load the properties of the object
                LoadProperties(_subObject);
            }
        }

        private bool _updating;
        private IFileObject _targetObject;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFileObject TargetObject
        {
            get => _targetObject;
            set
            {
                if (InvokeRequired)
                {
                    Invoke((Action)(() => TargetObject = value));
                    return;
                }

                if (_targetObject == value)
                    return;
                
                _targetObject = value;

                lblObjectName.Visible = Enabled = _targetObject != null;
                if (Enabled)
                {
                    btnSave.Visible = _targetObject.EditorState.IsDirty;

                    lblObjectName.Text = string.Format("{0} [{1}]",
                        _targetObject.ToString(),
                        _targetObject.GetType().GetFriendlyName());
                    
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
        private class MethodData
        {
            public MethodInfo Method { get; set; }
            public object[] Attribs { get; set; }
            public string DisplayName { get; set; }
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
            ConcurrentDictionary<int, PropertyData> propInfo = new ConcurrentDictionary<int, PropertyData>();
            ConcurrentDictionary<int, MethodData> methodInfo = new ConcurrentDictionary<int, MethodData>();
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

                    propInfo.TryAdd(i, p);
                });
                Parallel.For(0, methods.Length, i =>
                {
                    MethodInfo method = methods[i];
                    if (!method.IsSpecialName && (method.GetCustomAttribute<GridCallable>(true)?.Evaluate(obj) ?? false))
                    {
                        object[] attribs = method.GetCustomAttributes(true);
                        MethodData m = new MethodData()
                        {
                            Method = method,
                            Attribs = attribs,
                            DisplayName = (attribs.FirstOrDefault(x => x is GridCallable) as GridCallable)?.DisplayName ?? method.Name,
                        };

                        methodInfo.TryAdd(i, m);
                    }
                });
            }).ContinueWith(t =>
            {
                if (!Disposing && !IsDisposed && IsHandleCreated)
                {
                    Invoke((Action)(() =>
                    {
                        DateTime startTime = DateTime.Now;
                        for (int i = 0; i < props.Length; ++i)
                        {
                            if (!propInfo.ContainsKey(i))
                                continue;
                            PropertyData p = propInfo[i];
                            CreateControls(p.ControlTypes, p.Property, pnlProps, _categories, obj, p.Attribs, p.ReadOnly, this);
                        }
                        TimeSpan elapsed = DateTime.Now - startTime;
                        Engine.PrintLine("Initializing controls took {0} seconds.", elapsed.TotalSeconds.ToString());

                        for (int i = 0; i < methods.Length; ++i)
                        {
                            if (!methodInfo.ContainsKey(i))
                                continue;
                            MethodData m = methodInfo[i];
                            CreateMethodControl(m.Method, m.DisplayName, m.Attribs, pnlProps, _categories, obj);
                        }

                        if (Editor.Instance.Project.EditorSettings.PropertyGrid.IgnoreLoneSubCategories && _categories.Count == 1)
                            _categories.Values.ToArray()[0].CategoryName = null;
                        
                        Engine.PrintLine("Loaded properties for " + _subObject.GetType().GetFriendlyName());
                        //pnlProps.ResumeLayout(true);
                    }));
                }
            });
        }

        /// <summary>
        /// Returns a deque of all control types that can edit the given class type.
        /// </summary>
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
                Engine.LogWarning("Unable to find control for " + (propertyType == null ? "null" : propertyType.GetFriendlyName()));
                controlTypes.PushBack(typeof(PropGridText));
            }
            else if (controlTypes.Count > 1)
                if (mainControlType == typeof(PropGridObject))
                    controlTypes.PopFront();

            return controlTypes;
        }

        /// <summary>
        /// Instantiates the given PropGridItem-derived control types for the given property.
        /// </summary>
        /// <param name="controlTypes">The controls to create. All must derive from <see cref="PropGridItem"/>.</param>
        /// <param name="prop">The info of the property that will be modified.</param>
        /// <param name="propertyOwner">The object that owns the property.</param>
        /// <param name="stateChangeMethod">The method that will be called upon a change in value.</param>
        /// <returns></returns>
        public static List<PropGridItem> InstantiatePropertyEditors(Deque<Type> controlTypes, PropertyInfo prop, object propertyOwner, IDataChangeHandler dataChangeHandler)
            => controlTypes.Select(x => InstantiatePropertyEditor(x, prop, propertyOwner, dataChangeHandler)).ToList();
        /// <summary>
        /// Instantiates the given PropGridItem-derived control type for the given property.
        /// </summary>
        /// <param name="controlType">The control to create. Must derive from <see cref="PropGridItem"/>.</param>
        /// <param name="prop">The info of the property that will be modified.</param>
        /// <param name="propertyOwner">The object that owns the property.</param>
        /// <param name="stateChangeMethod">The method that will be called upon a change in value.</param>
        /// <returns></returns>
        public static PropGridItem InstantiatePropertyEditor(Type controlType, PropertyInfo prop, object propertyOwner, IDataChangeHandler dataChangeHandler)
        {
            PropGridItem control = Activator.CreateInstance(controlType) as PropGridItem;

            control.SetProperty(prop, propertyOwner);
            control.Dock = DockStyle.Fill;
            control.Visible = true;

            if (dataChangeHandler != null)
            {
                control.DataChangeHandler = dataChangeHandler;
                //Engine.PrintLine(stateChangeMethod.Method.Name);
            }
            else
                throw new Exception();

            control.Show();
            return control;
        }
        /// <summary>
        /// Instantiates the given PropGridItem-derived control types for the given object in a list.
        /// </summary>
        /// <param name="controlTypes"></param>
        /// <param name="list"></param>
        /// <param name="listIndex"></param>
        /// <param name="stateChangeMethod"></param>
        /// <returns></returns>
        public static List<PropGridItem> InstantiatePropertyEditors(Deque<Type> controlTypes, IList list, int listIndex, IDataChangeHandler dataChangeHandler)
        {
            Type elementType = list.DetermineElementType();
            return controlTypes.Select(x => InstantiatePropertyEditors(x, list, listIndex, elementType, dataChangeHandler)).ToList();
        }
        /// <summary>
        /// Instantiates the given PropGridItem-derived control type for the given object in a list.
        /// </summary>
        /// <param name="controlType"></param>
        /// <param name="list"></param>
        /// <param name="listIndex"></param>
        /// <param name="stateChangeMethod"></param>
        /// <returns></returns>
        public static PropGridItem InstantiatePropertyEditors(Type controlType, IList list, int listIndex, Type listElementType, IDataChangeHandler dataChangeHandler)
        {
            PropGridItem control = Activator.CreateInstance(controlType) as PropGridItem;

            control.SetIListOwner(list, listElementType, listIndex);
            control.Dock = DockStyle.Fill;
            control.Visible = true;

            if (dataChangeHandler != null)
            {
                control.DataChangeHandler = dataChangeHandler;
                //Engine.PrintLine(stateChangeMethod.Method.Name);
            }
            else
                throw new Exception();

            control.Show();
            return control;
        }

        public static PropGridMethod CreateMethodControl(
            MethodInfo m,
            string displayName,
            object[] attribs,
            Panel panel,
            Dictionary<string, PropGridCategory> categories, 
            object obj)
        {
            PropGridMethod control = new PropGridMethod()
            {
                Method = m,
                PropertyOwner = obj,
            };

            //var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
            string catName = MethodName;//category == null ? MethodName : category.Category;
            if (categories.ContainsKey(catName))
                categories[catName].AddMethod(control, attribs, displayName);
            else
            {
                PropGridCategory methods = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                methods.AddMethod(control, attribs, displayName);
                categories.Add(catName, methods);
                panel.Controls.Add(methods);
            }
            
            return control;
        }
        public static void CreateControls(
            Deque<Type> controlTypes,
            PropertyInfo prop,
            Panel panel,
            Dictionary<string, PropGridCategory> categories,
            object obj,
            object[] attribs,
            bool readOnly,
            IDataChangeHandler dataChangeHandler)
        {
            var controls = InstantiatePropertyEditors(controlTypes, prop, obj, dataChangeHandler);
            
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
            _y = e.Y;
        }
        private void MouseMoveLogicComps(object sender, MouseEventArgs e)
        {
            int diff = e.Y - _y;
            lstLogicComps.Height += diff;
            _y = e.Y;
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

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TargetObject.FilePath))
            {
                Editor.Instance.ContentTree.WatchProjectDirectory = false;
                TargetObject.Export();
                Editor.Instance.ContentTree.WatchProjectDirectory = true;
            }
            else if (TargetObject.References.Count == 1)
            {

            }
            else if (TargetObject.References.Count > 1)
            {
                foreach (IFileRef r in TargetObject.References)
                {

                }
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = TargetObject.GetFilter()
                })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                        TargetObject.Export(sfd.FileName);
                }
            }
            btnSave.Visible = false;
            TargetObject.EditorState.IsDirty = false;
        }
        
        public void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            if (TargetObject == null)
                return;
            btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(TargetObject.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }
        public void ListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex)
        {
            if (TargetObject == null)
                return;
            btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(TargetObject.EditorState, oldValue, newValue, listOwner, listIndex);
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
    public interface IDataChangeHandler
    {
        void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo);
        void ListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex);
    }
}
