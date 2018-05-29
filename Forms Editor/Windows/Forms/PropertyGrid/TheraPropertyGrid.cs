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
        public TheraPropertyGrid()
        {
            InitializeComponent();
            ctxSceneComps.RenderMode = ToolStripRenderMode.Professional;
            ctxSceneComps.Renderer = new TheraForm.TheraToolstripRenderer();
        }
        
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
        private bool _updating;
        private IFileObject _targetFileObject;
        private object _subObject;
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object TargetObject
        {
            get => _subObject;
            set
            {
                if (InvokeRequired)
                {
                    Invoke((Action)(() => TargetObject = value));
                    return;
                }

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

                if (Enabled = _subObject != null)
                {
                    lblObjectName.Text = string.Format("{0} [{1}]",
                      _subObject.ToString(),
                      _subObject.GetType().GetFriendlyName());
                }
                else
                {
                    lblObjectName.Text = "<null>";
                }

                if (_subObject is TObject obj)
                    obj.EditorState.Selected = true;
                
                //Load the properties of the object
                LoadProperties(_subObject);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IFileObject TargetFileObject
        {
            get => _targetFileObject;
            set
            {
                if (InvokeRequired)
                {
                    Invoke((Action)(() => TargetFileObject = value));
                    return;
                }

                if (_targetFileObject == value)
                    return;
                
                _targetFileObject = value;

                _updating = true;
                treeViewSceneComps.Nodes.Clear();
                lstLogicComps.DataSource = null;
                btnSave.Visible = _targetFileObject != null && _targetFileObject.EditorState.IsDirty;
                pnlHeader.Visible = _targetFileObject != null;
                if (pnlHeader.Visible)
                {
                    if (_targetFileObject is IActor actor)
                    {
                        PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor.RootComponent);
                        PopulateLogicComponentList(actor.LogicComponents);
                        pnlLogicComps.Visible =
                        lblSceneComps.Visible =
                        treeViewSceneComps.Visible = true;
                    }
                    else
                    {
                        pnlLogicComps.Visible =
                        lstLogicComps.Visible =
                        lblSceneComps.Visible =
                        treeViewSceneComps.Visible = false;
                    }
                    lblProperties.Visible = true;
                }
                else
                {
                    pnlLogicComps.Visible =
                    lstLogicComps.Visible =
                    lblSceneComps.Visible =
                    treeViewSceneComps.Visible =
                    lblProperties.Visible = false;
                }
                _updating = false;

                CalcSceneCompTreeHeight();

                TargetObject = value;
            }
        }

        private void PopulateLogicComponentList(EventList<LogicComponent> logicComponents)
        {
            if (lstLogicComps.Visible = logicComponents != null && logicComponents.Count > 0)
            {
                lstLogicComps.DataSource = null;
                lstLogicComps.DataSource = logicComponents;
                lstLogicComps.Height = logicComponents.Count * lstLogicComps.ItemHeight;
                lstLogicComps.SelectedIndex = logicComponents.Count > 0 ? 0 : -1;
            }
            else
            {
                btnMoveUpLogicComp.Visible = 
                btnMoveDownLogicComp.Visible = 
                btnRemoveLogicComp.Visible = false;
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

        #region Control Generation
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
        public static List<PropGridItem> InstantiatePropertyEditors(
            Deque<Type> controlTypes, PropertyInfo prop, object propertyOwner, IDataChangeHandler dataChangeHandler)
            => controlTypes.Select(x => InstantiatePropertyEditor(x, prop, propertyOwner, dataChangeHandler)).ToList();
        /// <summary>
        /// Instantiates the given PropGridItem-derived control type for the given property.
        /// </summary>
        /// <param name="controlType">The control to create. Must derive from <see cref="PropGridItem"/>.</param>
        /// <param name="prop">The info of the property that will be modified.</param>
        /// <param name="propertyOwner">The object that owns the property.</param>
        /// <param name="stateChangeMethod">The method that will be called upon a change in value.</param>
        /// <returns></returns>
        public static PropGridItem InstantiatePropertyEditor(
            Type controlType, PropertyInfo prop, object propertyOwner, IDataChangeHandler dataChangeHandler)
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
        public static List<PropGridItem> InstantiatePropertyEditors(
            Deque<Type> controlTypes, IList list, int listIndex, IDataChangeHandler dataChangeHandler)
        {
            Type elementType = list.DetermineElementType();
            return controlTypes.Select(x => InstantiatePropertyEditors(x, list, listIndex, elementType, dataChangeHandler)).ToList();
        }
        public static List<PropGridItem> InstantiateValuePropertyEditors(
            Deque<Type> controlTypes, IDictionary dic, int listIndex, IDataChangeHandler dataChangeHandler)
        {
            Type elementType = dic.DetermineValueType();
            return controlTypes.Select(x => InstantiatePropertyEditors(x, dic, listIndex, elementType, dataChangeHandler)).ToList();
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
        public static PropGridItem InstantiatePropertyEditors(Type controlType, IDictionary dic, object key, Type valueType, IDataChangeHandler dataChangeHandler)
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
#endregion

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
        private void pnlLogicComps_MouseDown(object sender, MouseEventArgs e)
        {
            _y = e.Y;
            pnlLogicComps.MouseMove += MouseMoveSceneComps;
        }

        private void lblProperties_MouseDown(object sender, MouseEventArgs e)
        {
            _y = e.Y;
            if (lstLogicComps.Visible)
                lblProperties.MouseMove += MouseMoveLogicComps;
            else
                lblProperties.MouseMove += MouseMoveSceneComps;
        }
        private void pnlLogicComps_MouseUp(object sender, MouseEventArgs e)
        {
            pnlLogicComps.MouseMove -= MouseMoveSceneComps;
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
            btnMoveUpLogicComp.Visible = lstLogicComps.SelectedIndex >= 0 && lstLogicComps.SelectedIndex < lstLogicComps.Items.Count - 1;
            btnMoveDownLogicComp.Visible = lstLogicComps.SelectedIndex < lstLogicComps.Items.Count && lstLogicComps.SelectedIndex > 0;
            btnRemoveLogicComp.Visible = lstLogicComps.SelectedIndex >= 0 && lstLogicComps.SelectedIndex < lstLogicComps.Items.Count;
        }

        private void lblObjectName_MouseEnter(object sender, EventArgs e)
        {
            lblObjectName.BackColor = Color.FromArgb(14, 18, 34);
        }

        private void lblObjectName_MouseLeave(object sender, EventArgs e)
        {
            lblObjectName.BackColor = Color.FromArgb(55, 55, 60);
        }
        
        private TreeNode _selectedSceneComp = null;
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
                {
                    _selectedSceneComp = node;
                    treeViewSceneComps.SelectedNode = node;
                    if (e.Button == MouseButtons.Right)
                    {
                        UpdateCtxSceneComp();
                        ctxSceneComps.Show(treeViewSceneComps, e.Location);
                    }
                }
            }
        }
        private void lblObjectName_Click(object sender, EventArgs e)
        {
            TargetObject = _targetFileObject;
        }
        private void lstLogicComps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TargetObject = lstLogicComps.SelectedItem as LogicComponent;
        }
        private void treeViewSceneComps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            TargetObject = _selectedSceneComp.Tag as SceneComponent;
        }

        private void treeViewSceneComps_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_updating || e.Action == TreeViewAction.ByMouse || e.Action == TreeViewAction.Unknown)
                return;

            _selectedSceneComp = e.Node;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TargetFileObject.FilePath))
            {
                Editor.Instance.ContentTree.WatchProjectDirectory = false;
                TargetFileObject.Export();
                Editor.Instance.ContentTree.WatchProjectDirectory = true;
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = TargetFileObject.GetFilter()
                })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                        TargetFileObject.Export(sfd.FileName);
                }
            }

            //if (TargetFileObject.References.Count == 1)
            //{

            //}
            //else if (TargetFileObject.References.Count > 1)
            //{
            //    foreach (IFileRef r in TargetFileObject.References)
            //    {

            //    }
            //}

            btnSave.Visible = false;
            TargetFileObject.EditorState.IsDirty = false;
        }
        
        public void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            if (TargetFileObject == null)
                return;
            btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(TargetFileObject.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }
        public void IListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex)
        {
            if (TargetFileObject == null)
                return;
            btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(TargetFileObject.EditorState, oldValue, newValue, listOwner, listIndex);
        }

        private void btnMoveDownLogicComp_Click(object sender, EventArgs e)
        {
            IActor a = TargetFileObject as IActor;
            if (a == null || a.LogicComponents.Count <= 1)
                return;
            int i = lstLogicComps.SelectedIndex;
            if (i == 0 || !a.LogicComponents.IndexInRange(i))
                return;
            LogicComponent c = a.LogicComponents[i];
            a.LogicComponents.RemoveAt(i);
            a.LogicComponents.Insert(--i, c);
            lstLogicComps.DataSource = null;
            lstLogicComps.DataSource = a.LogicComponents;
            lstLogicComps.SelectedIndex = i;
        }

        private void btnMoveUpLogicComp_Click(object sender, EventArgs e)
        {
            IActor a = TargetFileObject as IActor;
            if (a == null || a.LogicComponents.Count <= 1)
                return;
            int i = lstLogicComps.SelectedIndex;
            if (i == a.LogicComponents.Count - 1 || !a.LogicComponents.IndexInRange(i))
                return;
            LogicComponent c = a.LogicComponents[i];
            a.LogicComponents.RemoveAt(i);
            a.LogicComponents.Insert(++i, c);
            lstLogicComps.DataSource = null;
            lstLogicComps.DataSource = a.LogicComponents;
            lstLogicComps.SelectedIndex = i;
        }

        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {
            IActor a = TargetFileObject as IActor;
            if (a == null)
                return;
            LogicComponent comp = Editor.UserCreateInstanceOf<LogicComponent>(true);
            if (comp == null)
                return;
            int i = (lstLogicComps.SelectedIndex + 1).Clamp(0, a.LogicComponents.Count);
            if (i == a.LogicComponents.Count)
                a.LogicComponents.Add(comp);
            else
                a.LogicComponents.Insert(i, comp);
            lstLogicComps.DataSource = null;
            lstLogicComps.DataSource = a.LogicComponents;
            if (!lstLogicComps.Visible)
            {
                lstLogicComps.Visible = true;
                lstLogicComps.SelectedIndex = 0;
            }
            lstLogicComps.Height = lstLogicComps.Items.Count * lstLogicComps.ItemHeight;
        }

        private void btnRemoveLogicComp_Click(object sender, EventArgs e)
        {
            IActor a = TargetFileObject as IActor;
            int i = lstLogicComps.SelectedIndex;
            if (a == null || a.LogicComponents.Count == 0 || !a.LogicComponents.IndexInRange(i))
                return;
            a.LogicComponents.RemoveAt(i);
            if (a.LogicComponents.Count == 0)
            {
                if (lstLogicComps.Visible)
                    lstLogicComps.Visible = false;

                btnRemoveLogicComp.Visible = 
                btnMoveDownLogicComp.Visible = 
                btnMoveUpLogicComp.Visible = false;
            }
            else
            {
                lstLogicComps.DataSource = null;
                lstLogicComps.DataSource = a.LogicComponents;
                lstLogicComps.Height = lstLogicComps.Items.Count * lstLogicComps.ItemHeight;
                lstLogicComps.SelectedIndex = i.Clamp(0, lstLogicComps.Items.Count - 1);
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

        #region Static
        public static Dictionary<Type, Type> InPlaceEditorTypes = new Dictionary<Type, Type>();
        public static Dictionary<Type, Type> FullEditorTypes = new Dictionary<Type, Type>();
        static TheraPropertyGrid()
        {
            if (Engine.DesignMode)
                return;

            var propControls = Engine.FindAllTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(PropGridItem)));
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
            var fullEditors = Engine.FindAllTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(Form)) && x.GetCustomAttribute<EditorForAttribute>() != null);
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
        #endregion

        private void UpdateCtxSceneComp()
        {
            if (_selectedSceneComp == null)
            {
                btnAddChildSceneComp.Enabled =
                btnMoveDownSceneComp.Enabled =
                btnMoveUpSceneComp.Enabled =
                btnAddSiblingSceneComp.Enabled =
                btnAddToSibAboveSceneComp.Enabled =
                btnAddToSibBelowSceneComp.Enabled =
                btnAddSibToParentSceneComp.Enabled =
                false;
            }
            else
            {
                SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
                var sibComps = sceneCompSel.ParentSocket?.ChildComponents;
                int index = sibComps?.IndexOf(sceneCompSel) ?? -1;
                int count = sibComps?.Count ?? -1;

                btnAddChildSceneComp.Enabled = true;

                btnMoveDownSceneComp.Enabled = index >= 0 && index < count - 1;
                btnMoveUpSceneComp.Enabled = index > 0 && index <= count - 1;

                btnAddToSibAboveSceneComp.Enabled = index > 0;
                btnAddToSibBelowSceneComp.Enabled = index < count - 1;

                btnAddSiblingSceneComp.Enabled = sceneCompSel.ParentSocket is SceneComponent;
                btnAddSibToParentSceneComp.Enabled = sceneCompSel.ParentSocket?.ParentSocket is SceneComponent;
            }
        }
        private void btnAddSiblingSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            SceneComponent comp = Editor.UserCreateInstanceOf<SceneComponent>(true);
            if (comp == null)
                return;

            sibComps.Add(comp);

            TreeNode t = new TreeNode(comp.Name) { Tag = comp };
            _selectedSceneComp.Parent.Nodes.Add(t);

            t.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = t;
        }

        private void CalcSceneCompTreeHeight()
        {
            int count = NodeCount(treeViewSceneComps.Nodes);
            treeViewSceneComps.Height = count * treeViewSceneComps.ItemHeight;
        }
        private int NodeCount(TreeNodeCollection nodes)
        {
            int count = 0;
            foreach (TreeNode node in nodes)
            {
                count += 1;
                if (node.IsExpanded)
                    count += NodeCount(node.Nodes);
            }
            return count;
        }

        private void btnAddChildSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            SceneComponent comp = Editor.UserCreateInstanceOf<SceneComponent>(true);
            if (comp == null)
                return;

            sceneCompSel.ChildComponents.Add(comp);

            TreeNode t = new TreeNode(comp.Name) { Tag = comp };
            _selectedSceneComp.Nodes.Add(t);
            
            t.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = t;
        }

        private void btnMoveUpSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            int index = sibComps.IndexOf(sceneCompSel);
            sibComps.RemoveAt(index);
            sibComps.Insert(index - 1, sceneCompSel);

            TreeNode parentNode = _selectedSceneComp.Parent;
            int i = _selectedSceneComp.Index;
            _selectedSceneComp.Remove();
            parentNode.Nodes.Insert(i - 1, _selectedSceneComp);
            
            _selectedSceneComp.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }

        private void btnMoveDownSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            int index = sibComps.IndexOf(sceneCompSel);
            sibComps.RemoveAt(index);
            sibComps.Insert(index + 1, sceneCompSel);

            TreeNode parentNode = _selectedSceneComp.Parent;
            int i = _selectedSceneComp.Index;
            _selectedSceneComp.Remove();
            parentNode.Nodes.Insert(i + 1, _selectedSceneComp);
            
            _selectedSceneComp.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }

        private void btnAddToSibAboveSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            int index = sibComps.IndexOf(sceneCompSel);
            sibComps.RemoveAt(index);
            sibComps[index - 1].ChildComponents.Add(sceneCompSel);

            TreeNode parentNode = _selectedSceneComp.Parent;
            int i = _selectedSceneComp.Index;
            _selectedSceneComp.Remove();
            parentNode.Nodes[i - 1].Nodes.Add(_selectedSceneComp);
            
            _selectedSceneComp.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }

        private void btnAddToSibBelowSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            int index = sibComps.IndexOf(sceneCompSel);
            sibComps.RemoveAt(index);
            sibComps[index].ChildComponents.Add(sceneCompSel);

            TreeNode parentNode = _selectedSceneComp.Parent;
            int i = _selectedSceneComp.Index;
            _selectedSceneComp.Remove();
            parentNode.Nodes[i].Nodes.Add(_selectedSceneComp);
            
            _selectedSceneComp.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }

        private void btnAddSibToParentSceneComp_Click(object sender, EventArgs e)
        {
            SceneComponent sceneCompSel = _selectedSceneComp.Tag as SceneComponent;
            var parent = sceneCompSel.ParentSocket;
            var sibComps = parent.ChildComponents;
            int index = sibComps.IndexOf(sceneCompSel);
            sibComps.RemoveAt(index);
            var parentSibs = parent.ParentSocket.ChildComponents;
            int parentIndex = parentSibs.IndexOf(parent as SceneComponent);
            int newIndex = parentIndex == 0 ? parentIndex + 1 : parentIndex - 1;

            TreeNode parentNode = _selectedSceneComp.Parent;
            _selectedSceneComp.Remove();

            if (newIndex == parentSibs.Count)
            {
                parent.ParentSocket.ChildComponents.Add(sceneCompSel);
                parentNode.Parent.Nodes.Add(_selectedSceneComp);
            }
            else
            {
                parent.ParentSocket.ChildComponents.Insert(newIndex, sceneCompSel);
                parentNode.Parent.Nodes.Insert(newIndex, _selectedSceneComp);
            }
            
            _selectedSceneComp.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }

        private void treeViewSceneComps_AfterExpand(object sender, TreeViewEventArgs e)
        {
            CalcSceneCompTreeHeight();
        }

        private void treeViewSceneComps_AfterCollapse(object sender, TreeViewEventArgs e)
        {
            CalcSceneCompTreeHeight();
        }
    }
    public interface IDataChangeHandler
    {
        void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo);
        void IListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex);
        void IDictionaryObjectChanged(object oldValue, object newValue, IDictionary dicOwner, object key);
    }
}
