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
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Files;

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
        private class EventData
        {
            public EventInfo Event { get; set; }
            public object[] Attribs { get; set; }
            public string DisplayName { get; set; }
        }

        private const string MiscName = "Miscellaneous";
        private const string MethodName = "Methods";
        private const string EventName = "Events";

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
                LoadProperties();
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
                bool notNull = _targetFileObject != null;
                pnlHeader.Visible = pnlProps2.Visible = notNull;
                if (notNull)
                {
                    IActor actor = _targetFileObject as IActor;
                    tableLayoutPanel1.Visible = actor != null;
                    PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor?.RootComponent);
                    PopulateLogicComponentList(actor?.LogicComponents);
                    lblProperties.Visible = true;
                }
                else
                {
                    tableLayoutPanel1.Visible = false;
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
        protected override void OnHandleCreated(EventArgs e)
        {
            if (!Engine.DesignMode)
                PropGridItem.BeginUpdatingVisibleItems(Editor.GetSettings().PropertyGridRef.File.UpdateRateInSeconds);
            base.OnHandleCreated(e);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!Engine.DesignMode)
            {
                PropGridItem.StopUpdatingVisibleItems();
            }
            base.OnHandleDestroyed(e);
        }
        public event Action<object> PropertiesLoaded;
        private async void LoadProperties()
        {
            if (Disposing || IsDisposed)
                return;

            //pnlProps.SuspendLayout();

            pnlProps.Controls.Clear();
            foreach (var category in _categories.Values)
                category.DestroyProperties();
            _categories.Clear();

            if (_subObject == null)
            {
                //pnlProps.ResumeLayout(true);
                return;
            }
            
            PropertyInfo[] props = null;
            MethodInfo[] methods = null;
            EventInfo[] events = null;
            ConcurrentDictionary<int, PropertyData> propInfo = new ConcurrentDictionary<int, PropertyData>();
            ConcurrentDictionary<int, MethodData> methodInfo = new ConcurrentDictionary<int, MethodData>();
            ConcurrentDictionary<int, EventData> eventInfo = new ConcurrentDictionary<int, EventData>();

            await Task.Run(() =>
            {
                Type targetObjectType = _subObject.GetType();
                props = targetObjectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                methods = targetObjectType.GetMethods(BindingFlags.Public | BindingFlags.Instance);
                events = targetObjectType.GetEvents(BindingFlags.Public | BindingFlags.Instance);
                Parallel.For(0, props.Length, i =>
                {
                    PropertyInfo prop = props[i];
                    var indexParams = prop.GetIndexParameters();
                    if (indexParams != null && indexParams.Length > 0)
                        return;

                    object propObj = prop.GetValue(_subObject);
                    Type subType = propObj?.GetType() ?? prop.PropertyType;
                    var attribs = prop.GetCustomAttributes(true);
                    bool readOnly = false;

                    foreach (var attrib in attribs)
                    {
                        if (attrib is BrowsableAttribute browsable && !browsable.Browsable)
                            return;
                        if (attrib is BrowsableIf browsableIf && !browsableIf.Evaluate(_subObject))
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
                    if (!method.IsSpecialName && (method.GetCustomAttribute<GridCallable>(true)?.Evaluate(_subObject) ?? false))
                    {
                        object[] attribs = method.GetCustomAttributes(true);
                        MethodData m = new MethodData()
                        {
                            Method = method,
                            Attribs = attribs,
                            DisplayName = (attribs.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute)?.DisplayName ?? method.Name,
                        };

                        methodInfo.TryAdd(i, m);
                    }
                });
                Parallel.For(0, events.Length, i =>
                {
                    EventInfo @event = events[i];
                    if (!@event.IsSpecialName/* && (@event.GetCustomAttribute<GridCallable>(true)?.Evaluate(obj) ?? false)*/)
                    {
                        object[] attribs = @event.GetCustomAttributes(true);
                        EventData m = new EventData()
                        {
                            Event = @event,
                            Attribs = attribs,
                            DisplayName = (attribs.FirstOrDefault(x => x is DisplayNameAttribute) as DisplayNameAttribute)?.DisplayName ?? @event.Name,
                        };

                        eventInfo.TryAdd(i, m);
                    }
                });
            });

            if (!Disposing && !IsDisposed && IsHandleCreated)
            {
                pnlProps.SuspendLayout();

                //DateTime startTime = DateTime.Now;
                for (int i = 0; i < props.Length; ++i)
                {
                    if (!propInfo.ContainsKey(i))
                        continue;
                    PropertyData p = propInfo[i];
                    CreateControls(p.ControlTypes, new PropGridItemRefPropertyInfo(() => _subObject, p.Property), pnlProps, _categories, p.Attribs, p.ReadOnly, this);
                }
                //TimeSpan elapsed = DateTime.Now - startTime;
                //Engine.PrintLine("Initializing controls took {0} seconds.", elapsed.TotalSeconds.ToString());

                for (int i = 0; i < methods.Length; ++i)
                {
                    if (!methodInfo.ContainsKey(i))
                        continue;
                    MethodData m = methodInfo[i];
                    var deque = new Deque<Type>();
                    deque.PushBack(typeof(PropGridMethod));
                    CreateControls(deque, new PropGridItemRefMethodInfo(() => _subObject, m.Method), pnlProps, _categories, m.Attribs, false, this);
                }

                for (int i = 0; i < events.Length; ++i)
                {
                    if (!eventInfo.ContainsKey(i))
                        continue;
                    EventData e = eventInfo[i];
                    var deque = new Deque<Type>();
                    deque.PushBack(typeof(PropGridEvent));
                    CreateControls(deque, new PropGridItemRefEventInfo(() => _subObject, e.Event), pnlProps, _categories, e.Attribs, false, this);
                }

                bool ignoreLoneSubCats = Editor.Instance.Project?.EditorSettings?.PropertyGrid?.IgnoreLoneSubCategories ?? true;
                if (ignoreLoneSubCats && _categories.Count == 1)
                    _categories.Values.ToArray()[0].CategoryName = null;
                        
                //Engine.PrintLine("Loaded properties for " + _subObject.GetType().GetFriendlyName());
                pnlProps.ResumeLayout(true);

                PropertiesLoaded?.Invoke(_subObject);
            }
        }

        public void ExpandAll()
        {
            RecursiveExpand(pnlProps.Controls, false);
        }
        public void CollapseAll()
        {
            RecursiveExpand(pnlProps.Controls, true);
        }
        private void RecursiveExpand(ControlCollection collection, bool collapse)
        {
            foreach (Control c in collection)
            {
                if (c is ICollapsible coll)
                {
                    if (collapse)
                        coll.Collapse();
                    else
                        coll.Expand();
                    RecursiveExpand(coll.ChildControls, collapse);
                }
            }
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
                if (mainControlType == null)
                {
                    Type subType2 = subType;
                    if (subType.IsGenericType && !InPlaceEditorTypes.ContainsKey(subType))
                        subType2 = subType.GetGenericTypeDefinition();
                    if (InPlaceEditorTypes.ContainsKey(subType2))
                    {
                        mainControlType = InPlaceEditorTypes[subType2];
                        if (!controlTypes.Contains(mainControlType))
                            controlTypes.PushFront(mainControlType);
                    }
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
        public static PropGridItem InstantiatePropertyEditor(Type controlType, PropGridItemRefInfo info, IDataChangeHandler dataChangeHandler)
        {
            PropGridItem control = Activator.CreateInstance(controlType) as PropGridItem;

            control.SetReferenceHolder(info);
            control.Dock = DockStyle.Fill;
            control.Visible = true;

            if (dataChangeHandler != null)
                control.DataChangeHandler = dataChangeHandler;

            control.Show();
            return control;
        }

        /// <summary>
        /// Instantiates the given PropGridItem-derived control types for the given object in a list.
        /// </summary>
        public static List<PropGridItem> InstantiatePropertyEditors(
            Deque<Type> controlTypes, PropGridItemRefInfo info, IDataChangeHandler dataChangeHandler)
            => controlTypes.Select(x => InstantiatePropertyEditor(x, info, dataChangeHandler)).ToList();

        //public static PropGridMethod CreateMethodControl(
        //    MethodInfo m,
        //    string displayName,
        //    object[] attribs,
        //    Panel panel,
        //    Dictionary<string, PropGridCategory> categories, 
        //    object obj)
        //{
        //    PropGridMethod control = new PropGridMethod()
        //    {
        //        Method = m,
        //        ParentInfo = new PropGridItemRefPropertyInfo(obj, null),
        //    };

        //    //var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
        //    string catName = MethodName;//category == null ? MethodName : category.Category;
        //    if (categories.ContainsKey(catName))
        //        categories[catName].AddMethod(control, attribs, displayName);
        //    else
        //    {
        //        PropGridCategory methods = new PropGridCategory()
        //        {
        //            CategoryName = catName,
        //            Dock = DockStyle.Top,
        //        };
        //        methods.AddMethod(control, attribs, displayName);
        //        categories.Add(catName, methods);
        //        panel.Controls.Add(methods);
        //    }
            
        //    return control;
        //}
        
        //public static PropGridEvent CreateEventControl(
        //    EventInfo m,
        //    string displayName,
        //    object[] attribs,
        //    Panel panel,
        //    Dictionary<string, PropGridCategory> categories,
        //    object obj)
        //{
        //    PropGridEvent control = new PropGridEvent()
        //    {
        //        Event = m,
        //        ParentInfo = new PropGridItemRefPropertyInfo(obj, null),
        //    };

        //    //var category = attribs.FirstOrDefault(x => x is CategoryAttribute) as CategoryAttribute;
        //    string catName = EventName;//category == null ? MethodName : category.Category;
        //    if (categories.ContainsKey(catName))
        //        categories[catName].AddEvent(control, attribs, displayName);
        //    else
        //    {
        //        PropGridCategory methods = new PropGridCategory()
        //        {
        //            CategoryName = catName,
        //            Dock = DockStyle.Top,
        //        };
        //        methods.AddEvent(control, attribs, displayName);
        //        categories.Add(catName, methods);
        //        panel.Controls.Add(methods);
        //    }

        //    return control;
        //}

        /// <summary>
        /// Constructs all given control types and 
        /// </summary>
        /// <param name="controlTypes"></param>
        /// <param name="prop"></param>
        /// <param name="panel"></param>
        /// <param name="categories"></param>
        /// <param name="obj"></param>
        /// <param name="attribs"></param>
        /// <param name="readOnly"></param>
        /// <param name="dataChangeHandler"></param>
        public static void CreateControls(
            Deque<Type> controlTypes,
            PropGridItemRefInfo info,
            Panel panel,
            Dictionary<string, PropGridCategory> categories,
            object[] attribs,
            bool readOnly,
            IDataChangeHandler dataChangeHandler)
        {
            var controls = InstantiatePropertyEditors(controlTypes, info, dataChangeHandler);
            string GetDefaultCatName()
            {
                if (info is PropGridItemRefEventInfo)
                    return "Events";
                if (info is PropGridItemRefMethodInfo)
                    return "Methods";
                return MiscName;
            }
            string catName = !(attribs.FirstOrDefault(x => x is CategoryAttribute) is CategoryAttribute category) ? GetDefaultCatName() : category.Category;
            if (categories.ContainsKey(catName))
                categories[catName].AddMember(controls, attribs, readOnly);
            else
            {
                PropGridCategory misc = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                };
                misc.AddMember(controls, attribs, readOnly);
                categories.Add(catName, misc);
                panel.Controls.Add(misc);
            }
        }
#endregion

        private void PopulateSceneComponentTree(TreeNodeCollection nodes, SceneComponent currentSceneComp)
        {
            if (currentSceneComp == null)
            {
                nodes.Clear();
                return;
            }
            TreeNode s = new TreeNode(currentSceneComp.Name) { Tag = currentSceneComp };
            foreach (SceneComponent childSceneComp in currentSceneComp.ChildComponents)
                PopulateSceneComponentTree(s.Nodes, childSceneComp);
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
            IFileObject file = TargetFileObject.Root ?? TargetFileObject;

            if (file == null)
                return;

            if (!string.IsNullOrWhiteSpace(file.FilePath))
            {
                Editor.Instance.ContentTree.WatchProjectDirectory = false;
                file.Export();
                Editor.Instance.ContentTree.WatchProjectDirectory = true;
            }
            else
            {
                using (SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = file.GetFilter()
                })
                {
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                        file.Export(sfd.FileName);
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
            file.EditorState.IsDirty = false;
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
        public void IDictionaryObjectChanged(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey)
        {
            if (TargetFileObject == null)
                return;
            btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(TargetFileObject.EditorState, oldValue, newValue, dicOwner, key, isKey);
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
            if (!(TargetFileObject is IActor a) || a.LogicComponents.Count <= 1)
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

            var propControls = Engine.FindTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(PropGridItem)), false, Assembly.GetExecutingAssembly());
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
            var fullEditors = Engine.FindTypes(x => !x.IsAbstract && x.IsSubclassOf(typeof(Form)) && x.GetCustomAttribute<EditorForAttribute>() != null, false, Assembly.GetExecutingAssembly());
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
            if (_selectedSceneComp.Parent == null)
                treeViewSceneComps.Nodes.Add(t);
            else
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

        private void toolStripSeparator1_Click(object sender, EventArgs e)
        {

        }
    }
    public interface IDataChangeHandler
    {
        void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo);
        void IListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex);
        void IDictionaryObjectChanged(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey);
    }
}
