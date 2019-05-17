using AppDomainToolkit;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Core.Extensions;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Components;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Editor;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class TheraPropertyGrid : UserControl, IPropGridMemberOwner
    {
        object IPropGridMemberOwner.Value => _targetObject;
        bool IPropGridMemberOwner.ReadOnly => false;
        PropGridMemberInfo IPropGridMemberOwner.MemberInfo { get; }

        private readonly ValueChangeHandler _changeHandler = new ValueChangeHandler();
        private class ValueChangeHandler : MarshalByRefObject, PropertyGrid.ValueChangeHandler
        {
            public TheraPropertyGrid Grid { get; set; }
            public void HandleChange(params LocalValueChange[] changes)
            {
                if (!(Grid.TargetObject is IObject obj))
                    return;
                Grid.btnSave.Visible = Grid.btnSaveAs.Visible = true;
                Editor.Instance.UndoManager.AddChange(obj.EditorState, changes);
            }
        }

        public TheraPropertyGrid()
        {
            InitializeComponent();

            ctxSceneComps.RenderMode = ToolStripRenderMode.Professional;
            ctxSceneComps.Renderer = new TheraForm.TheraToolstripRenderer();

            _lblObjectName_StartColor = lblObjectName.BackColor;
            _lblObjectName_EndColor = Color.FromArgb(_lblObjectName_StartColor.R + 10, _lblObjectName_StartColor.G + 10, _lblObjectName_StartColor.B + 10);

            _changeHandler = new ValueChangeHandler() { Grid = this };
        }
        private class PropGridData
        {
            public string Category { get; set; }
        }
        private class PropertyData : PropGridData
        {
            public PropertyData(PropertyInfoProxy property, object obj, string category)
            {
                Deque<TypeProxy> types;
                try
                {
                    object propObj = property.GetValue(obj);
                    TypeProxy subType = propObj?.GetTypeProxy() ?? property.PropertyType;
                    types = GetControlTypes(subType);
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                    types = GetControlTypes(null);
                }
                ControlTypes = types;
                Property = property;
                Category = category;
            }
            public Deque<TypeProxy> ControlTypes { get; set; }
            public PropertyInfoProxy Property { get; set; }
        }
        private class MethodData : PropGridData
        {
            public MethodInfoProxy Method { get; set; }
        }
        private class EventData : PropGridData
        {
            public EventInfoProxy Event { get; set; }
        }

        private const string MiscName = "Miscellaneous";
        private const string MethodName = "Methods";
        private const string EventName = "Events";

        private List<object> _prevObjectChain = new List<object>();
        private Dictionary<string, PropGridCategory> _categories = new Dictionary<string, PropGridCategory>();
        private bool _updating;
        //private IFileObject _targetFileObject;
        private object _targetObject;

        public Stack<(object, string)> TargetObjects { get; } = new Stack<(object, string)>();
        private bool AllowExplorer { get; set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object TargetObject
        {
            get => _targetObject;
            set => SetObject(value, null);
        }

        private void lblObjectName_Click(object sender, EventArgs e)
        {
            if (TargetObjects.Count <= 1)
                return;
            
            TargetObjects.Pop();
            var current = TargetObjects.Peek();
            SetObject(current.Item1, current.Item2);
            UpdateLabel();
        }
        internal void SetObject(object value, string memberAccessor)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<object, string>)SetObject, value, memberAccessor);
                return;
            }

            //Do nothing if target object is the same
            if (_targetObject == value)
                return;

            if (_targetObject != null)
            {
                bool visible = pnlProps.Visible;
                pnlProps.Visible = false;
                pnlProps.Controls.Clear();
                foreach (var category in _categories.Values)
                    category.DestroyProperties();
                _categories.Clear();
                pnlProps.Visible = visible;
                lblFilePath.Text = null;
            }

            _targetObject = value;
            bool notNull = _targetObject != null;

            treeViewSceneComps.Nodes.Clear();
            lstLogicComps.DataSource = null;

            pnlHeader.Visible = pnlProps2.Visible = notNull;
            if (Enabled = notNull)
            {
                IActor actor = _targetObject as IActor;
                tblActor.Visible = actor != null;
                PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor?.RootComponent);
                PopulateLogicComponentList(actor?.LogicComponents);

                lblProperties.Visible = actor != null && ShowPropertiesHeader;
                CalcSceneCompTreeHeight();
                // lblProperties.Text = string.Format("Properties: {0} [{1}]",
                //_targetObject.ToString(),
                //_targetObject.GetType().GetFriendlyName());

                if (_targetObject is IObject obj)
                {
                    btnSave.Visible = btnSaveAs.Visible = obj.EditorState.IsDirty;
                    obj.EditorState.Selected = true;
                    if (_targetObject is IFileObject fobj)
                    {
                        lblFilePath.Text = fobj.FilePath;
                        AllowExplorer = fobj.FilePath.IsValidExistingPath();
                    }
                    else
                    {
                        AllowExplorer = false;
                    }
                }
                else
                {
                    btnSave.Visible = btnSaveAs.Visible = AllowExplorer = false;
                }

            }
            else
            {
                lblProperties.Visible = false;
                tblActor.Visible = false;
                btnSave.Visible = btnSaveAs.Visible = false;
            }

            if (_targetObject is ISceneComponent sc)
            {
                var players = sc.OwningWorld?.CurrentGameMode?.LocalPlayers;
                if (players != null && players.Count > 0)
                {
                    EditorUI3D hud = players[0]?.ControlledPawn?.HUD?.File as EditorUI3D;
                    hud?.SetSelectedComponent(false, sc);
                }
            }

            //Load the properties of the object
            LoadProperties();

            if (memberAccessor != null)
            {
                if (TargetObjects.Count == 0 || TargetObjects.Peek().Item1 != _targetObject)
                    TargetObjects.Push((_targetObject, memberAccessor));
            }
            else
            {
                TargetObjects.Clear();
                TargetObjects.Push((_targetObject, memberAccessor));
            }

            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (TargetObjects.Count > 0)
            {
                string s = "";
                string add;
                foreach (var obj in TargetObjects)
                {
                    add = obj.Item2?.ToString() ?? obj.Item1?.ToString() ?? "<null>";
                    s = add + s;
                }
                lblObjectName.Text = $"[{TargetObject?.GetTypeProxy()?.GetFriendlyName() ?? "<null>"}] - " + s;
            }
            else
            {
                lblObjectName.Text = "<null>";
            }
        }

        private bool _showObjectNameAndType = true;
        private bool _showPropertiesHeader = true;

        public bool ShowObjectNameAndType
        {
            get => _showObjectNameAndType;
            set
            {
                _showObjectNameAndType = value;
                lblObjectName.Visible = value;
            }
        }
        public bool ShowPropertiesHeader
        {
            get => _showPropertiesHeader;
            set
            {
                _showPropertiesHeader = value;
                lblProperties.Visible = value;
            }
        }
        //[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        //public IFileObject TargetFileObject
        //{
        //    get => _targetFileObject;
        //    set
        //    {
        //        if (InvokeRequired)
        //        {
        //            BeginInvoke((Action)(() => TargetFileObject = value));
        //            return;
        //        }

        //        if (_targetFileObject == value)
        //            return;

        //        _targetFileObject = value;

        //        //_updating = true;
        //        //treeViewSceneComps.Nodes.Clear();
        //        //lstLogicComps.DataSource = null;
        //        //btnSave.Visible = _targetFileObject != null && _targetFileObject.EditorState.IsDirty;
        //        //bool notNull = _targetFileObject != null;
        //        //pnlHeader.Visible = pnlProps2.Visible = notNull;
        //        //if (notNull)
        //        //{
        //        //    IActor actor = _targetFileObject as IActor;
        //        //    tableLayoutPanel1.Visible = actor != null;
        //        //    PopulateSceneComponentTree(treeViewSceneComps.Nodes, actor?.RootComponent);
        //        //    PopulateLogicComponentList(actor?.LogicComponents);
        //        //    lblProperties.Visible = ShowPropertiesHeader;
        //        //    CalcSceneCompTreeHeight();
        //        //}
        //        //else
        //        //{
        //        //    tableLayoutPanel1.Visible = false;
        //        //}
        //        //_updating = false;

        //        TargetObject = value;
        //    }
        //}

        private void PopulateLogicComponentList(IEventList<ILogicComponent> logicComponents)
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
        protected override async void OnHandleCreated(EventArgs e)
        {
            if (!Engine.DesignMode)
            {
                var sref = Editor.GetSettingsRef();
                var inst = await sref.GetInstanceAsync();
                BeginUpdatingVisibleItems(inst?.PropertyGridRef.File.UpdateRateInSeconds ?? 0.2f);
            }

            base.OnHandleCreated(e);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!Engine.DesignMode)
                StopUpdatingVisibleItems();
            
            base.OnHandleDestroyed(e);
        }
        internal void AddVisibleItem(PropGridItem item)
        {
            VisibleItemsAdditionQueue.Enqueue(item);
            item.UpdateDisplay();
        }
        internal void RemoveVisibleItem(PropGridItem item)
        {
            VisibleItemsRemovalQueue.Enqueue(item);
        }
        internal void StopUpdatingVisibleItems()
        {
            _updatingVisibleItems = false;
        }
        /// <summary>
        /// List of all visible PropGridItems that need to be updated.
        /// </summary>
        private List<PropGridItem> VisibleItems { get; } = new List<PropGridItem>();
        private Queue<PropGridItem> VisibleItemsRemovalQueue { get; } = new Queue<PropGridItem>();
        private Queue<PropGridItem> VisibleItemsAdditionQueue { get; } = new Queue<PropGridItem>();

        private bool _updatingVisibleItems = false;
        internal void BeginUpdatingVisibleItems(float updateRateInSeconds)
        {
            if (_updatingVisibleItems)
                return;

            _updatingVisibleItems = true;

            int sleepTime = (int)(updateRateInSeconds * 1000.0f);

            Task.Run(() =>
            {
                while (_updatingVisibleItems)
                {
                    Parallel.For(0, VisibleItems.Count, UpdateItem);
                    while (VisibleItemsRemovalQueue.Count > 0)
                        VisibleItems.Remove(VisibleItemsRemovalQueue.Dequeue());
                    while (VisibleItemsAdditionQueue.Count > 0)
                        VisibleItems.Add(VisibleItemsAdditionQueue.Dequeue());
                    Thread.Sleep(sleepTime);
                }
            });
        }
        private void UpdateItem(int i)
        {
            try
            {
                PropGridItem item = VisibleItems[i];
                if (item.IsDisposed || item.Disposing)
                    RemoveVisibleItem(item);
                else// if (item.UpdateTimeSpan == null || DateTime.Now - item.LastUpdateTime >= item.UpdateTimeSpan.Value)
                    BaseRenderPanel.ThreadSafeBlockingInvoke(
                        (Action)item.UpdateDisplay,
                        BaseRenderPanel.EPanelType.Rendering);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
        }

        public event Action<object> PropertiesLoaded;

        private object GetObject() => _targetObject;
        private async void LoadProperties(bool showProperties = true, bool showEvents = false, bool showMethods = false)
        {
            if (Disposing || IsDisposed)
                return;

            var propGridSettings = Editor.GetSettings().PropertyGrid;
            try
            {
                await LoadPropertiesToPanel(
                    this,
                    pnlProps, _categories,
                    _targetObject,
                    this, _changeHandler, true,
                    propGridSettings.DisplayMethods, propGridSettings.DisplayEvents);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }

            PropertiesLoaded?.Invoke(_targetObject);
        }
        public static async Task LoadPropertiesToPanel(
            TheraPropertyGrid grid,
            BetterTableLayoutPanel pnlProps,
            Dictionary<string, PropGridCategory> categories,
            object obj,
            IPropGridMemberOwner memberOwner,
            PropertyGrid.ValueChangeHandler changeHandler,
            bool showProperties = true,
            bool showMethods = true,
            bool showEvents = true)
        {
            foreach (Control control in pnlProps.Controls)
                control.Dispose();
            pnlProps.Controls.Clear();
            foreach (PropGridCategory category in categories.Values)
                category.DestroyProperties();
            categories.Clear();

            if (obj == null)
                return;

            pnlProps.RowStyles.Clear();
            pnlProps.ColumnStyles.Clear();
            pnlProps.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlProps.ColumnCount = 1;

            PropertyInfoProxy[] props = null;
            MethodInfoProxy[] methods = null;
            EventInfoProxy[] events = null;

            ConcurrentDictionary<int, PropertyData> propInfo = new ConcurrentDictionary<int, PropertyData>();
            ConcurrentDictionary<int, MethodData> methodInfo = new ConcurrentDictionary<int, MethodData>();
            ConcurrentDictionary<int, EventData> eventInfo = new ConcurrentDictionary<int, EventData>();

            //DateTime startTime = DateTime.Now;

            Engine.PrintLine("Loading properties on AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            await Task.Run(() =>
            {
                TypeProxy targetObjectType = obj.GetTypeProxy();

                const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
                props = showProperties ? targetObjectType.GetProperties(flags) : new PropertyInfoProxy[0];
                methods = showMethods ? targetObjectType.GetMethods(flags) : new MethodInfoProxy[0];
                events = showEvents ? targetObjectType.GetEvents(flags) : new EventInfoProxy[0];

                Parallel.For(0, props.Length, i =>
                {
                    PropertyInfoProxy prop = props[i];
                    ParameterInfoProxy[] indexParams = prop.GetIndexParameters();
                    if (indexParams.Length > 0)
                        return;

                    //BrowsableAttribute browsable = prop.GetCustomAttribute<BrowsableAttribute>(true);
                    //if (!(browsable?.Browsable ?? true))
                    //    return;

                    string category = null;//prop.GetCustomAttribute<CategoryAttribute>(true)?.Category;

                    PropertyData propData = new PropertyData(prop, obj, category);
                    propInfo.TryAdd(i, propData);
                });

                Parallel.For(0, methods.Length, i =>
                {
                    MethodInfoProxy method = methods[i];
                    if (method.IsSpecialName || (!(method.GetCustomAttribute<GridCallable>(true)?.Evaluate(obj) ?? false)))
                        return;

                    string category = method.GetCustomAttribute<CategoryAttribute>(true)?.Category;

                    MethodData methodData = new MethodData()
                    {
                        Method = method,
                        Category = category,
                    };

                    methodInfo.TryAdd(i, methodData);
                });

                Parallel.For(0, events.Length, i =>
                {
                    EventInfoProxy e = events[i];
                    if (e.IsSpecialName)
                        return;

                    string category = e.GetCustomAttribute<CategoryAttribute>(true)?.Category;

                    EventData eventData = new EventData()
                    {
                        Event = e,
                        Category = category,
                    };

                    eventInfo.TryAdd(i, eventData);
                });
            });

            if (!pnlProps.Disposing && !pnlProps.IsDisposed && pnlProps.IsHandleCreated)
            {
                pnlProps.SuspendLayout();

                for (int i = 0; i < props.Length; ++i)
                {
                    if (!propInfo.ContainsKey(i))
                        continue;

                    PropertyData p = propInfo[i];
                    CreateControls(grid, p.ControlTypes, new PropGridMemberInfoProperty(memberOwner, p.Property), pnlProps, categories, p.Category, changeHandler);
                }

                for (int i = 0; i < methods.Length; ++i)
                {
                    if (!methodInfo.ContainsKey(i))
                        continue;

                    MethodData m = methodInfo[i];
                    Deque<TypeProxy> deque = new Deque<TypeProxy>();
                    deque.PushBack(typeof(PropGridMethod));
                    CreateControls(grid, deque, new PropGridMemberInfoMethod(memberOwner, m.Method), pnlProps, categories, m.Category, changeHandler);
                }

                for (int i = 0; i < events.Length; ++i)
                {
                    if (!eventInfo.ContainsKey(i))
                        continue;

                    EventData e = eventInfo[i];
                    Deque<TypeProxy> deque = new Deque<TypeProxy>();
                    deque.PushBack(typeof(PropGridEvent));
                    CreateControls(grid, deque, new PropGridMemberInfoEvent(memberOwner, e.Event), pnlProps, categories, e.Category, changeHandler);
                }

                bool ignoreLoneSubCats = Editor.Instance.Project?.EditorSettings?.PropertyGrid?.IgnoreLoneSubCategories ?? true;
                if (ignoreLoneSubCats && categories.Count == 1)
                    categories.Values.ToArray()[0].CategoryName = null;

                pnlProps.ResumeLayout(true);
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
        private static void RecursiveExpand(ControlCollection collection, bool collapse)
        {
            foreach (Control c in collection)
            {
                if (!(c is ICollapsible coll))
                    continue;

                if (collapse)
                    coll.Collapse();
                else
                    coll.Expand();
                RecursiveExpand(coll.ChildControls, collapse);
            }
        }

        #region Control Generation
        /// <summary>
        /// Returns a deque of all control types that can edit the given class type.
        /// </summary>
        public static Deque<TypeProxy> GetControlTypes(TypeProxy propertyType)
        {
            TypeProxy mainControlType = null;
            TypeProxy subType = propertyType;
            Deque<TypeProxy> controlTypes = new Deque<TypeProxy>();
            var inPlace = Editor.Instance.DomainProxy.InPlaceEditorTypes;
            while (subType != null)
            {
                if (mainControlType == null)
                {
                    TypeProxy subType2 = subType;
                    if (subType.IsGenericType && !inPlace.ContainsKey(subType))
                        subType2 = subType.GetGenericTypeDefinition();
                    if (inPlace.ContainsKey(subType2))
                    {
                        mainControlType = inPlace[subType2];
                        if (!controlTypes.Contains(mainControlType))
                            controlTypes.PushFront(mainControlType);
                    }
                }
                TypeProxy[] interfaces = subType.GetInterfaces();
                foreach (TypeProxy i in interfaces)
                    if (inPlace.ContainsKey(i))
                    {
                        TypeProxy controlType = inPlace[i];
                        if (!controlTypes.Contains(controlType))
                            controlTypes.PushBack(controlType);
                    }

                subType = subType.BaseType;
            }
            if (controlTypes.Count == 0)
            {
                //Engine.LogWarning("Unable to find control for " + (propertyType == null ? "null" : propertyType.GetFriendlyName()));
                controlTypes.PushBack(typeof(PropGridObject));
            }
            else if (controlTypes.Count > 1)
                if (mainControlType == typeof(PropGridObject))
                    controlTypes.PopFront();

            return controlTypes;
        }
        public static PropGridItem InstantiatePropertyEditor(TypeProxy controlType, PropGridMemberInfo info, PropGridCategory category, PropertyGrid.ValueChangeHandler dataChangeHandler)
        {
            PropGridItem control = (PropGridItem)Editor.Instance.Invoke((Func<PropGridItem>)(() => 
            {
                PropGridItem item = controlType.CreateInstance() as PropGridItem;
                if (item != null)
                {
                    item.Dock = DockStyle.Fill;
                    item.ParentCategory = category;
                    item.DataChangeHandler = dataChangeHandler;
                    item.SetReferenceHolder(info);
                }
                return item;

            }));
            return control;
        }

        /// <summary>
        /// Instantiates the given PropGridItem-derived control types for the given object in a list.
        /// </summary>
        public static List<PropGridItem> InstantiatePropertyEditors(
            Deque<TypeProxy> controlTypes, PropGridMemberInfo info, PropGridCategory category, PropertyGrid.ValueChangeHandler dataChangeHandler)
            => controlTypes.Select(x => InstantiatePropertyEditor(x, info, category, dataChangeHandler)).ToList();

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
        private static string GetDefaultCatName(PropGridMemberInfo info)
        {
            switch (info)
            {
                case PropGridMemberInfoEvent _:
                    return "Events";
                case PropGridMemberInfoMethod _:
                    return "Methods";
                default:
                    return MiscName;
            }
        }
        /// <summary>
        /// Constructs all given control types
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="controlTypes"></param>
        /// <param name="info"></param>
        /// <param name="panel"></param>
        /// <param name="categories"></param>
        /// <param name="attribs"></param>
        /// <param name="readOnly"></param>
        /// <param name="dataChangeHandler"></param>
        public static void CreateControls(
            TheraPropertyGrid grid,
            Deque<TypeProxy> controlTypes,
            PropGridMemberInfo info,
            BetterTableLayoutPanel panel,
            Dictionary<string, PropGridCategory> categories,
            string category,
            PropertyGrid.ValueChangeHandler dataChangeHandler)
        {
            string catName = category ?? GetDefaultCatName(info);
            PropGridCategory targetCategory;

            if (categories.ContainsKey(catName))
                targetCategory = categories[catName];
            else
            {
                targetCategory = new PropGridCategory()
                {
                    CategoryName = catName,
                    Dock = DockStyle.Top,
                    PropertyGrid = grid,
                };
                categories.Add(catName, targetCategory);

                panel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                panel.RowCount = panel.RowStyles.Count;
                panel.Controls.Add(targetCategory, 0, panel.RowCount - 1);
            }

            List<PropGridItem> controls = InstantiatePropertyEditors(controlTypes, info, targetCategory, dataChangeHandler);
            targetCategory.AddMember(controls);
        }
        #endregion

        private void PopulateSceneComponentTree(TreeNodeCollection nodes, ISceneComponent currentSceneComp)
        {
            if (currentSceneComp == null)
            {
                nodes.Clear();
                return;
            }
            TreeNode s = new TreeNode(currentSceneComp.Name) { Tag = currentSceneComp };
            foreach (ISceneComponent childSceneComp in currentSceneComp.ChildComponents)
                PopulateSceneComponentTree(s.Nodes, childSceneComp);
            nodes.Add(s);
        }

        //int _y = 0;
        private void pnlLogicComps_MouseDown(object sender, MouseEventArgs e)
        {
            //_y = e.Y;
            //pnlLogicComps.MouseMove += MouseMoveSceneComps;
        }

        private void lblProperties_MouseDown(object sender, MouseEventArgs e)
        {
            //_y = e.Y;
            //if (lstLogicComps.Visible)
            //    lblProperties.MouseMove += MouseMoveLogicComps;
            //else
            //    lblProperties.MouseMove += MouseMoveSceneComps;
        }
        private void pnlLogicComps_MouseUp(object sender, MouseEventArgs e)
        {
            //pnlLogicComps.MouseMove -= MouseMoveSceneComps;
        }
        private void lblProperties_MouseUp(object sender, MouseEventArgs e)
        {
            //if (lstLogicComps.Visible)
            //    lblProperties.MouseMove -= MouseMoveLogicComps;
            //else
            //    lblProperties.MouseMove -= MouseMoveSceneComps;
        }
        private void MouseMoveSceneComps(object sender, MouseEventArgs e)
        {
            //int diff = e.Y - _y;
            //treeViewSceneComps.Height += diff;
            //_y = e.Y;
        }
        private void MouseMoveLogicComps(object sender, MouseEventArgs e)
        {
            //int diff = e.Y - _y;
            //lstLogicComps.Height += diff;
            //_y = e.Y;
        }

        private void lstLogicComps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            btnMoveUpLogicComp.Visible = lstLogicComps.SelectedIndex >= 0 && lstLogicComps.SelectedIndex < lstLogicComps.Items.Count - 1;
            btnMoveDownLogicComp.Visible = lstLogicComps.SelectedIndex < lstLogicComps.Items.Count && lstLogicComps.SelectedIndex > 0;
            btnRemoveLogicComp.Visible = lstLogicComps.SelectedIndex >= 0 && lstLogicComps.SelectedIndex < lstLogicComps.Items.Count;
        }

        private readonly Color _lblObjectName_StartColor;
        private readonly Color _lblObjectName_EndColor;
        private EventHandler<FrameEventArgs> lblObjectName_FadeMethod;
        private void lblObjectName_MouseEnter(object sender, EventArgs e)
            => lblObjectName.FadeBackColor(_lblObjectName_EndColor, 0.5f, ref lblObjectName_FadeMethod);
        private void lblObjectName_MouseLeave(object sender, EventArgs e)
            => lblObjectName.FadeBackColor(_lblObjectName_StartColor, 0.5f, ref lblObjectName_FadeMethod);

        private TreeNode _selectedSceneComp = null;
        private void treeViewSceneComps_MouseDown(object sender, MouseEventArgs e)
        {
            if (_updating)
                return;

            TreeNode node = treeViewSceneComps.GetNodeAt(e.Location);
            if (node == null)
            {
                treeViewSceneComps.SelectedNode = _selectedSceneComp = null;
                return;
            }

            Rectangle r = node.Bounds;
            //Adjust for icon
            r.X -= 25;
            r.Width += 25;

            if (!r.Contains(e.Location))
                return;
            
            treeViewSceneComps.SelectedNode = _selectedSceneComp = node;

            if (e.Button == MouseButtons.Right)
            {
                UpdateCtxSceneComp();
                ctxSceneComps.Show(treeViewSceneComps, e.Location);
            }
        }
        private void lstLogicComps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ILogicComponent comp = lstLogicComps.SelectedItem as ILogicComponent;
            SetObject(comp, $".LogicComponents[{(lstLogicComps.SelectedIndex).ToString()}]");
        }
        private void treeViewSceneComps_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ISceneComponent comp = _selectedSceneComp.Tag as ISceneComponent;
            SetObject(comp, $".SceneComponentCache[{(comp?.ActorSceneComponentCacheIndex ?? -1).ToString()}]");
        }

        private void treeViewSceneComps_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (_updating ||
                e.Action == TreeViewAction.ByMouse ||
                e.Action == TreeViewAction.Unknown)
                return;

            _selectedSceneComp = e.Node;
        }
        private void btnMoveDownLogicComp_Click(object sender, EventArgs e)
        {
            if (!(TargetObject is IActor a) || a.LogicComponents.Count <= 1)
                return;
            int i = lstLogicComps.SelectedIndex;
            if (i == 0 || !a.LogicComponents.IndexInRange(i))
                return;
            ILogicComponent c = a.LogicComponents[i];
            a.LogicComponents.RemoveAt(i);
            a.LogicComponents.Insert(--i, c);
            lstLogicComps.DataSource = null;
            lstLogicComps.DataSource = a.LogicComponents;
            lstLogicComps.SelectedIndex = i;
        }

        private void btnMoveUpLogicComp_Click(object sender, EventArgs e)
        {
            if (!(TargetObject is IActor a) || a.LogicComponents.Count <= 1)
                return;
            int i = lstLogicComps.SelectedIndex;
            if (i == a.LogicComponents.Count - 1 || !a.LogicComponents.IndexInRange(i))
                return;
            ILogicComponent c = a.LogicComponents[i];
            a.LogicComponents.RemoveAt(i);
            a.LogicComponents.Insert(++i, c);
            lstLogicComps.DataSource = null;
            lstLogicComps.DataSource = a.LogicComponents;
            lstLogicComps.SelectedIndex = i;
        }

        private void btnAddLogicComp_Click(object sender, EventArgs e)
        {
            if (!(TargetObject is IActor a))
                return;
            ILogicComponent comp = Editor.UserCreateInstanceOf<ILogicComponent>();
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
            int i = lstLogicComps.SelectedIndex;
            if (!(TargetObject is IActor a) || a.LogicComponents.Count == 0 || !a.LogicComponents.IndexInRange(i))
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
                btnAddAsSibToParentSceneComp.Enabled =
                btnRemoveSceneComp.Enabled =
                false;
            }
            else
            {
                var sibComps = _selectedSceneComp.Parent?.Nodes;
                int index = sibComps?.IndexOf(_selectedSceneComp) ?? -1;
                int count = sibComps?.Count ?? -1;

                btnAddChildSceneComp.Enabled = true;

                btnMoveDownSceneComp.Enabled = index >= 0 && index < count - 1;
                btnMoveUpSceneComp.Enabled = index > 0 && index <= count - 1;

                btnAddToSibAboveSceneComp.Enabled = index > 0;
                btnAddToSibBelowSceneComp.Enabled = index < count - 1;

                ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;
                btnAddSiblingSceneComp.Enabled = sceneCompSel.ParentSocket is ISceneComponent && _selectedSceneComp.Parent != null;
                btnAddAsSibToParentSceneComp.Enabled = sceneCompSel.ParentSocket?.ParentSocket is ISceneComponent && _selectedSceneComp.Parent?.Parent != null;

                btnRemoveSceneComp.Enabled = _selectedSceneComp.Parent != null;
            }
        }
        private void btnAddSiblingSceneComp_Click(object sender, EventArgs e)
        {
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;
            var sibComps = sceneCompSel.ParentSocket.ChildComponents;
            ISceneComponent comp = Editor.UserCreateInstanceOf<ISceneComponent>();
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
            ISceneComponent comp = Editor.UserCreateInstanceOf<ISceneComponent>();
            if (comp == null)
                return;

            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

            sceneCompSel.ChildComponents.Add(comp);

            TreeNode t = new TreeNode(comp.Name) { Tag = comp };
            _selectedSceneComp.Nodes.Add(t);
            
            t.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = t;
        }

        private void btnMoveUpSceneComp_Click(object sender, EventArgs e)
        {
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

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
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

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
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

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
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

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
        private void btnRemoveSceneComp_Click(object sender, EventArgs e)
        {
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

            TreeNode parentNode = _selectedSceneComp.Parent;
            sceneCompSel.DetachFromParent();
            _selectedSceneComp.Remove();
            _selectedSceneComp = parentNode;

            _selectedSceneComp?.EnsureVisible();
            CalcSceneCompTreeHeight();
            treeViewSceneComps.SelectedNode = _selectedSceneComp;
        }
        private void btnAddAsSibToParentSceneComp_Click(object sender, EventArgs e)
        {
            ISceneComponent sceneCompSel = _selectedSceneComp.Tag as ISceneComponent;

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
            => CalcSceneCompTreeHeight();
        private void treeViewSceneComps_AfterCollapse(object sender, TreeViewEventArgs e)
            => CalcSceneCompTreeHeight();
        
        private void toolStripSeparator1_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveAs_Click(object sender, EventArgs e) => Save(true);
        private void btnSave_Click(object sender, EventArgs e) => Save(false);
        
        private void Save(bool saveAs)
        {
            (object, string) o = TargetObjects.Count == 0 ? (null, null) : TargetObjects.Peek();
            if (!(o.Item1 is IFileObject fobj))
                return;

            IFileObject file = fobj?.RootFile ?? fobj;

            string path;
            if (saveAs)
                path = SelectPath(file);
            else
                path = file?.FilePath ?? SelectPath(file);

            Save(file, path);
        }
        private string SelectPath(IFileObject file)
        {
            string path = null;
            using (SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = file.GetFilter()
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    path = sfd.FileName;
                else
                    Engine.PrintLine("Save canceled.");
            }
            return path;
        }
        private async void Save(IFileObject file, string path)
        {
            Editor editor = Editor.Instance;
            int op = editor.BeginOperation($"Property Grid: saving {path}", $"Property Grid: done saving {path}", out Progress<float> progress, out CancellationTokenSource cancel);
            await file.ExportAsync(path, ESerializeFlags.Default, progress, cancel.Token);
            editor.EndOperation(op);

            //if (TargetFileObject.References.Count == 1)
            //{

            //}
            //else if (TargetFileObject.References.Count > 1)
            //{
            //    foreach (IFileRef r in TargetFileObject.References)
            //    {

            //    }
            //}

            btnSave.Visible = btnSaveAs.Visible = false;
            file.EditorState.IsDirty = false;
        }

        private void btnExplorer_Click(object sender, EventArgs e)
        {
            if (AllowExplorer && TargetObject is IFileObject fobj && fobj.FilePath.IsValidExistingPath())
                Process.Start("explorer.exe", Path.GetDirectoryName(fobj.FilePath));
        }
        private void lblFilePath_MouseEnter(object sender, EventArgs e)
        {
            if (AllowExplorer)
                lblFilePath.BackColor = Color.FromArgb(70, 112, 110);
        }
        private void lblFilePath_MouseLeave(object sender, EventArgs e)
        {
            if (AllowExplorer)
                lblFilePath.BackColor = Color.FromArgb(60, 102, 100);
        }
    }
    public interface ValueChangeHandler
    {
        void HandleChange(params LocalValueChange[] changes);
    }
}
