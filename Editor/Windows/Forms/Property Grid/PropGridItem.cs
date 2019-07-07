using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Attributes;
using TheraEngine.Core.Tools;
using TheraEngine.Editor;
using TheraEngine.Rendering.UI;
using static TheraEditor.Windows.Forms.TheraForm;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class DesignerBugFixUserPropGridItem : PropGridItem
    {
        protected override bool UpdateDisplayInternal(object value)
        {
            return false;
        }
    }
    public class DesignerBugFixUserControl : UserControl { }
    public abstract partial class PropGridItem :
#if DEBUG
        DesignerBugFixUserControl,
#else
        UserControl,
#endif
        IPropGridMemberOwner
    {
        private bool _isEditing = false;
        private object _oldValue, _newValue;
        protected bool _updating = false;

        protected PropGridItem()
        {
            InitializeComponent();
        }

        public event Action DoneEditing;
        public event Action ValueChanged;

        public T GetMemberInfoAs<T>() where T : PropGridMemberInfo
            => MemberInfo as T;

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual PropGridCategory ParentCategory { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropGridMemberInfo MemberInfo { get; private set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TypeProxy DataType => MemberInfo?.DataType;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual Label Label { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public virtual bool ReadOnly { get; set; } = false;

        protected bool IsEditable() =>
            !ReadOnly &&
            (MemberInfo == null || !MemberInfo.IsReadOnly()) &&
            (!(ParentCategory?.ReadOnly ?? false));

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
        public ValueChangeHandler DataChangeHandler { get; set; }

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
            if (InvokeRequired)
            {
                BeginInvoke((Action<object, bool>)UpdateValue, newValue, submitStateChange);
                return;
            }

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
                //if (MemberInfo is PropGridMemberInfoProperty prop && prop.Property.DeclaringType.IsValueType)
                //{
                    
                //}
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
            _oldValue = DataType?.GetProperty(propertyName)?.GetValue(classObject);
        }
        /// <summary>
        /// Use if DataType is a class and not a string.
        /// </summary>
        /// <param name="classObject"></param>
        /// <param name="propertyName"></param>
        protected void SubmitPostManualStateChange(object classObject, string propertyName)
        {
            PropertyInfo info = DataType?.GetProperty(propertyName);
            _newValue = info?.GetValue(classObject);
            if (_newValue == _oldValue)
                return;

            DataChangeHandler?.HandleChange(new LocalValueChangeProperty(_oldValue, _newValue, classObject, info));
            DoneEditing?.Invoke();

            //PropertyGrid.btnSave.Visible = true;
            //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
            //    _oldValue, _newValue, classObject, info);
        }
        private void ResolveMemberName(string displayNameOverride)
        {
            if (Label == null)
                return;

            string propName = MemberInfo?.DisplayName; //editors[0].GetParentInfo<PropGridItemRefPropertyInfo>()?.Property?.Name;
            string name;

            if (!string.IsNullOrWhiteSpace(displayNameOverride))
                name = displayNameOverride;
            else if (!string.IsNullOrWhiteSpace(propName))
                name = Editor.GetSettings().PropertyGridRef.File.SplitCamelCase ? propName.SplitCamelCase() : propName;
            else
                name = "[No Name]";

            Label.Text = name;
        }
        /// <summary>
        /// Designates where this item gets and sets its value.
        /// </summary>
        protected internal virtual void SetReferenceHolder(PropGridMemberInfo memberInfo)
        {
            MemberInfo = memberInfo;

            string displayNameOverride = null;
            
            if (MemberInfo is PropGridMemberInfoProperty propInfo)
            {
                var attribs = propInfo.Property.GetCustomAttributes(true);
                foreach (object attrib in attribs)
                {
                    switch (attrib)
                    {
                        case BrowsableIf browsableIf:
                            VisibilityCondition = browsableIf.Condition;
                            break;
                        case BrowsableAttribute browsable:
                            Visible = browsable.Browsable;
                            break;
                        case ReadOnlyAttribute readOnlyAttrib:
                            ReadOnly = readOnlyAttrib.IsReadOnly;
                            break;
                        case DisplayNameAttribute displayName:
                            displayNameOverride = displayName.DisplayName;
                            break;
                        case EditInPlace editInPlace:
                            if (this is PropGridObject pobj)
                                pobj.EditInPlace = pobj.AlwaysVisible = true;
                            break;
                        case DescriptionAttribute desc:
                            if (Label?.Tag is MemberLabelInfo info)
                                info.Description = desc.Description;
                            break;
                    }
                }
            }

            ResolveMemberName(displayNameOverride);

            if (DataType != null)
            {
                //Double check that this control is valid for the given type
                PropGridControlForAttribute attr = this.GetType().GetCustomAttribute<PropGridControlForAttribute>();
                if (attr != null)
                {
                    Type[] types = attr.Types;
                    string str = types.ToStringList(", ", ", or ", t => t.GetFriendlyName());

                    bool condition = types.Any(x => DataType.IsAssignableTo(x) || (DataType.IsGenericType && x == DataType.GetGenericTypeDefinition()));
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

            var prop = (MemberInfo as PropGridMemberInfoProperty)?.Property;
            string displayNameOverride = prop?.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName;
            ResolveMemberName(displayNameOverride);

            string desc = prop?.GetCustomAttribute<DescriptionAttribute>()?.Description;
            if (!string.IsNullOrWhiteSpace(desc) && Label?.Tag is MemberLabelInfo info)
                info.Description = desc;

            OnLabelSet();
        }
        public bool EvaluateVisibilityCondition(object owningObject)
            => string.IsNullOrWhiteSpace(VisibilityCondition) ? true : ExpressionParser.Evaluate<bool>(VisibilityCondition, owningObject);
        public void UpdateDisplay()
        {
            if (IsEditing ||
                MemberInfo == null || 
                ParentCategory == null || 
                !ParentCategory.Visible ||
                !ParentCategory.PropertyTable.Visible)
                return;

            _updating = true;
            DateTime now = DateTime.Now;
            bool displayChanged = false;

            var parent = MemberInfo?.Owner?.Value;
            if (parent == null || EvaluateVisibilityCondition(parent))
            {
                if (!Visible)
                    Visible = true;
                if (Label != null && !Label.Visible)
                    Label.Visible = true;
                if (ParentCategory != null && !ParentCategory.Visible)
                    ParentCategory.Visible = true;

                object value = GetValue();
                //if (value is Exception ex)
                //    Engine.LogWarning(ex.ToString());
                //else
                displayChanged = UpdateDisplayInternal(value);
            }
            else
            {
                if (Visible)
                    Visible = false;
                if (Label != null && !Label.Visible)
                    Label.Visible = false;
                if (ParentCategory != null && ParentCategory.Visible)
                {
                    bool anyVisible = false;
                    for (int i = 0; i < ParentCategory.PropertyTable.RowCount; ++i)
                    {
                        if (ParentCategory.PropertyTable.GetControlFromPosition(1, i)?.Visible ?? false)
                        {
                            anyVisible = true;
                            break;
                        }
                    }
                    if (!anyVisible)
                        ParentCategory.Visible = false;
                }
            }

            if (displayChanged)
            {
                //UpdateTimeSpan = now - LastDisplayChangeTime;
                LastDisplayChangeTime = now;
            }

            LastUpdateTime = now;
            _updating = false;
        }
        
        public string VisibilityCondition { get; set; }
        public TimeSpan? UpdateTimeSpan { get; set; }
        public DateTime LastUpdateTime { get; set; }
        public DateTime LastDisplayChangeTime { get; set; }

        protected abstract bool UpdateDisplayInternal(object value);
        
        public virtual bool CanAnimate => false;

        object IPropGridMemberOwner.Value => GetValue();

        protected virtual void OnLabelSet()
        {
            if (!CanAnimate)
                return;

            GetAnimationMemberPath(out PropGridMemberInfo animOwner);
            if (!(animOwner?.Owner?.Value is IObject obj))
                return;

            ToolStripMenuItem[] anims = obj.Animations?.
                Where(x => x.RootMember?.MemberName == animOwner.DisplayName && x.RootMember?.Animation.File != null).
                Select(x => new ToolStripMenuItem(x.Name, null, EditAnimation) { Tag = x }).
                ToArray();

            ContextMenuStrip strip = new ContextMenuStrip();
            if (anims != null && anims.Length > 0)
            {
                ToolStripMenuItem animsBtn = new ToolStripMenuItem("Animations...");
                animsBtn.DropDownItems.AddRange(anims);
                ToolStripMenuItem[] items = 
                {
                    new ToolStripMenuItem("New Animation", null, CreateAnimation),
                    animsBtn,
                };
                strip.Items.AddRange(items);
            }
            else
            {
                strip.Items.Add(new ToolStripMenuItem("New Animation", null, CreateAnimation));
            }

            strip.RenderMode = ToolStripRenderMode.Professional;
            strip.Renderer = new TheraToolstripRenderer();

            Label.ContextMenuStrip = strip;
            Label.Name += "*";
        }
        private static void EditAnimation(object sender, EventArgs e)
        {
            //if (sender is ToolStripMenuItem item && item.Tag is AnimationTree anim)
            //{

            //}
        }
        protected virtual BasePropAnim CreateAnimation() => throw new NotImplementedException($"{nameof(CreateAnimation)} must be overloaded when {nameof(CanAnimate)} is true.");
        protected virtual string GetAnimationMemberPath(out PropGridMemberInfo animOwner)
        {
            var info = MemberInfo;
            string accessor = string.Empty;
            int count = 0;
            while (info != null && !(info.Owner?.Value is IObject))
            {
                accessor = info.MemberAccessor + accessor;
                info = info.Owner.MemberInfo;
                ++count;
            }
            if (!(info?.Owner?.Value is IObject))
            {
                animOwner = null;
                return null;
            }
            animOwner = info;
            if (count > 0 && accessor[0] == '.')
                accessor = accessor.Substring(1);
            return accessor;
        }
        private void CreateAnimation(object sender, EventArgs e)
        {
            string path = GetAnimationMemberPath(out PropGridMemberInfo animOwner);
            if (animOwner == null)
                return;
            TObject obj = animOwner.Owner?.Value as TObject;
            
            AnimationTree anim = new AnimationTree(animOwner.DisplayName, path, CreateAnimation());

            if (obj.Animations == null)
                obj.Animations = new EventList<AnimationTree>();

            obj.Animations.Add(anim);

            ToolStripItemCollection menu = Label.ContextMenuStrip.Items;
            ToolStripMenuItem menuItem = new ToolStripMenuItem(anim.Name, null, EditAnimation) { Tag = anim };
            if (menu.Count == 1)
            {
                ToolStripMenuItem item = new ToolStripMenuItem("Animations...");
                item.DropDownItems.Add(menuItem);
                menu.Add(item);
            }
            else
                ((ToolStripMenuItem)menu[1]).DropDownItems.Add(menuItem);
        }

        public override string ToString() => $"[{DataType.GetFriendlyName()}] - {MemberInfo}";
    }
}
