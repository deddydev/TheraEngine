using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Editor;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridItem : UserControl, IPropGridMemberOwner
    {
        private bool _isEditing = false;
        private object _oldValue, _newValue;
        protected bool _updating = false;

        public PropGridItem()
        {
            InitializeComponent();
        }

        public event Action DoneEditing;
        public event Action ValueChanged;

        public T GetParentInfo<T>() where T : PropGridMemberInfo
            => MemberInfo as T;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropGridCategory ParentCategory { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropGridMemberInfo MemberInfo { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type DataType => MemberInfo?.DataType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Label Label { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual bool ReadOnly { get; set; } = false;

        protected bool IsEditable() => !ReadOnly && (MemberInfo == null || !MemberInfo.IsReadOnly()) && (!(ParentCategory?.ReadOnly ?? false));

        /// <summary>
        /// When true, disallows UpdateDisplay() from doing anything until set to false.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool IsEditing
        {
            get => _isEditing;
            protected set
            {
                if (_isEditing == value)
                    return;
                _isEditing = value;

                if (IsReferenceType)
                    return;

                if (_isEditing)
                    _oldValue = _newValue = GetValue();
                else if (_oldValue != _newValue)
                    SubmitStateChange(_oldValue, _newValue);
            }
        }
        public bool IsReferenceType => DataType.IsClass && DataType != typeof(string);
        
        protected virtual object RefObject => null;
        protected void InputLostFocus(object sender, EventArgs e)
        {
            if (RefObject != null && sender is Control ctrl && ctrl.Tag is string propName)
                SubmitPostManualStateChange(RefObject, propName);
            IsEditing = false;
        }
        protected void InputGotFocus(object sender, EventArgs e)
        {
            IsEditing = true;
            if (RefObject != null && sender is Control ctrl && ctrl.Tag is string propName)
                SubmitPreManualStateChange(RefObject, propName);
        }

        /// <summary>
        /// The object that should handle changes to this property.
        /// </summary>
        public IDataChangeHandler DataChangeHandler { get; set; }

        /// <summary>
        /// Records that a value has changed to the undo buffer and enables saving the owning file.
        /// </summary>
        protected void SubmitStateChange(object oldValue, object newValue)
        {
            MemberInfo?.SubmitStateChange(oldValue, newValue, DataChangeHandler);
            DoneEditing?.Invoke();
        }

        public object GetValue()
        {
            //try
            //{
                return MemberInfo?.MemberValue;
            //}
            //catch (Exception ex)
            //{
            //    Engine.PrintLine(ex.ToString());
            //    return ex;
            //}
        }
        public void OnValueChanged() => ValueChanged?.Invoke();
        public void UpdateValue(object newValue, bool submitStateChange)
        {
            if (_updating || MemberInfo.IsReadOnly())
                return;

            //if (IsReferenceType && ReferenceEquals(newValue, GetValue()))
            //{
            //    Engine.LogWarning("Tried setting class object to the same reference. Are you sure you didn't mean to update a property within?");
            //    return;
            //}

            if (submitStateChange)
            {
                object oldValue = MemberInfo.MemberValue;

                MemberInfo.MemberValue = newValue;
                newValue = MemberInfo.MemberValue;

                SubmitStateChange(oldValue, newValue);
            }
            else
            {
                MemberInfo.MemberValue = newValue;
                newValue = MemberInfo.MemberValue;
            }
            if (_isEditing)
                _newValue = newValue;
            
            UpdateDisplay();
            ValueChanged?.Invoke();
        }

        /// <summary>
        /// Use if DataType is a class and not a string.
        /// </summary>
        /// <param name="classObject"></param>
        /// <param name="propertyName"></param>
        protected void SubmitPreManualStateChange(object classObject, string propertyName)
        {
            _oldValue = DataType.GetProperty(propertyName).GetValue(classObject);
        }
        /// <summary>
        /// Use if DataType is a class and not a string.
        /// </summary>
        /// <param name="classObject"></param>
        /// <param name="propertyName"></param>
        protected void SubmitPostManualStateChange(object classObject, string propertyName)
        {
            PropertyInfo info = DataType.GetProperty(propertyName);
            _newValue = info.GetValue(classObject);
            if (_newValue != _oldValue)
            {
                DataChangeHandler?.HandleChange(new LocalValueChangeProperty(_oldValue, _newValue, classObject, info));
                DoneEditing?.Invoke();
                //PropertyGrid.btnSave.Visible = true;
                //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
                //    _oldValue, _newValue, classObject, info);
            }
        }
        //internal protected virtual void SetIDictionaryOwner(IDictionary dic, Type dataType, object key, bool isKey)
        //{
        //    ParentInfo = new PropGridItemParentIDictionaryInfo(dic, key, isKey);
        //    DataType = dataType;
        //    SetControlsEnabled(!dic.IsReadOnly && !_readOnly);
        //    UpdateDisplay();
        //}
        //internal protected virtual void SetIListOwner(IList list, Type elementType, int index)
        //{
        //    ParentInfo = new PropGridItemParentIListInfo(list, index);
        //    DataType = elementType;
        //    SetControlsEnabled(!list.IsReadOnly && !_readOnly);
        //    UpdateDisplay();
        //}
        //protected internal void SetPropertyByName(string propertyName, object propertyOwner)
        //{
        //    PropertyInfo propertyInfo = propertyOwner.GetType().GetProperty(propertyName);
        //    SetReferenceHolder(propertyInfo, propertyOwner);
        //}
        /// <summary>
        /// Designates where this item gets and sets its value.
        /// </summary>
        internal protected virtual void SetReferenceHolder(PropGridMemberInfo memberInfo)
        {
            MemberInfo = memberInfo;
            if (DataType != null)
            {
                //Double check that this control is valid for the given type
                PropGridControlForAttribute attr = GetType().GetCustomAttributeExt<PropGridControlForAttribute>();
                if (attr != null)
                {
                    Type[] types = attr.Types;
                    string str = types.ToStringList(", ", ", or ", t => t.GetFriendlyName());

                    bool condition = types.Any(x => x.IsAssignableFrom(DataType) || (DataType.IsGenericType && x == DataType.GetGenericTypeDefinition()));
                    string errorMsg = $"{DataType.GetFriendlyName()} is not a {str} type.";

                    if ((condition || !types.Any(x => x == typeof(string))) && !Engine.Assert(condition, errorMsg, false))
                        return;
                }
            }
            UpdateDisplay();
        }
        internal void SetLabel(Label label)
        {
            Label = label;
            OnLabelSet();
        }
        public void UpdateDisplay()
        {
            if (IsEditing || MemberInfo == null)
                return;

            _updating = true;
            object value = GetValue();
            //if (value is Exception ex)
            //    Engine.LogWarning(ex.ToString());
            //else
                UpdateDisplayInternal(value);
            _updating = false;
        }
        protected virtual void UpdateDisplayInternal(object value) { }

        /// <summary>
        /// List of all visible PropGridItems that need to be updated.
        /// </summary>
        private static List<PropGridItem> VisibleItems { get; } = new List<PropGridItem>();
        public virtual bool CanAnimate => false;

        object IPropGridMemberOwner.Value => GetValue();

        private static bool UpdatingVisibleItems = false;
        internal static void BeginUpdatingVisibleItems(float updateRateInSeconds)
        {
            if (UpdatingVisibleItems)
                return;
            int sleepTime = (int)(updateRateInSeconds * 1000.0f);
            UpdatingVisibleItems = true;
            Task.Run(() =>
            {
                while (UpdatingVisibleItems)
                {
                    if (VisibleItems.Count > 0)
                        Parallel.For(0, VisibleItems.Count, i =>
                        {
                            try
                            {
                                if (!VisibleItems.IndexInRange(i))
                                    return;
                                var item = VisibleItems[i];
                                if (!item.Disposing && !item.IsDisposed && Engine.CurrentFramesPerSecond > 30.0f)
                                    BaseRenderPanel.ThreadSafeBlockingInvoke((Action)item.UpdateDisplay, BaseRenderPanel.PanelType.Rendering);
                            }
                            catch { }
                        });
                    Thread.Sleep(sleepTime);
                }
            });
        }
        internal static void StopUpdatingVisibleItems()
        {
            UpdatingVisibleItems = false;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!Engine.DesignMode)
                VisibleItems.Add(this);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!Engine.DesignMode)
                VisibleItems.Remove(this);
            base.OnHandleDestroyed(e);
        }
        public override string ToString()
            => DataType?.ToString() + " - " + MemberInfo?.ToString();
        protected virtual void OnLabelSet()
        {
            if (CanAnimate)
            {
                PropGridMemberInfoProperty propInfo = GetParentInfo<PropGridMemberInfoProperty>();
                if (propInfo.Owner.Value is TObject obj)
                {
                    var anims = obj.Animations?.
                        Where(x => x.RootMember?.MemberName == propInfo.Property.Name && x.RootMember?.Animation.File != null).
                        Select(x => new ToolStripMenuItem(x.Name, null, EditAnimation) { Tag = x }).
                        ToArray();

                    ContextMenuStrip strip = new ContextMenuStrip();
                    if (anims != null && anims.Length > 0)
                    {
                        ToolStripMenuItem animsBtn = new ToolStripMenuItem("Animations...");
                        animsBtn.DropDownItems.AddRange(anims);
                        ToolStripMenuItem[] m = new ToolStripMenuItem[]
                        {
                            new ToolStripMenuItem("New Animation", null, CreateAnimation),
                            animsBtn,
                        };
                        strip.Items.AddRange(m);
                    }
                    else
                    {
                        strip.Items.Add(new ToolStripMenuItem("New Animation", null, CreateAnimation));
                    }

                    strip.RenderMode = ToolStripRenderMode.Professional;
                    strip.Renderer = new TheraToolstripRenderer();
                    Label.ContextMenuStrip = strip;
                }
            }
        }
        private void EditAnimation(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem item && item.Tag is AnimationTree anim)
            {

            }
        }
        protected virtual BasePropAnim CreateAnimation() => throw new NotImplementedException($"{nameof(CreateAnimation)} must be overloaded when {nameof(CanAnimate)} is true.");
        protected virtual string GetAnimationMemberPath()
        {
            PropGridMemberInfoProperty propInfo = GetParentInfo<PropGridMemberInfoProperty>();
            return propInfo.Property.Name;
        }
        private void CreateAnimation(object sender, EventArgs e)
        {
            PropGridMemberInfoProperty propInfo = GetParentInfo<PropGridMemberInfoProperty>();
            if (propInfo.Owner.Value is TObject obj)
            {
                var anim = new AnimationTree(propInfo.Property.Name, GetAnimationMemberPath(), CreateAnimation());

                if (obj.Animations == null)
                    obj.Animations = new EventList<AnimationTree>();
                obj.Animations.Add(anim);

                var menu = Label.ContextMenuStrip.Items;
                var menuItem = new ToolStripMenuItem(anim.Name, null, EditAnimation) { Tag = anim };
                if (menu.Count == 1)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem("Animations...");
                    item.DropDownItems.Add(menuItem);
                    menu.Add(item);
                }
                else
                    ((ToolStripMenuItem)menu[1]).DropDownItems.Add(menuItem);
            }
        }
    }
}
