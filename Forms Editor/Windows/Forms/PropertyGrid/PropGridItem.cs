using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using TheraEngine;
using System.Collections;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridItem : UserControl
    {
        private bool _readOnly = false;
        private bool _isEditing = false;
        private object _oldValue, _newValue;
        protected bool _updating = false;

        public PropGridItem() => InitializeComponent();

        public event Action DoneEditing;
        public event Action ValueChanged;

        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Browsable(false)]
        //public TheraPropertyGrid PropertyGrid { get; internal set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type DataType { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropertyInfo Property { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public object PropertyOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Label Label { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int IListIndex { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList IListOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                SetControlsEnabled(!_readOnly && (Property != null ? Property.CanWrite : (IListOwner != null ? !IListOwner.IsReadOnly : true)));
            }
        }
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
            if (DataChangeHandler == null)
                return;

            if (IListOwner != null)
            {
                DataChangeHandler.ListObjectChanged(oldValue, newValue, IListOwner, IListIndex);
                //PropertyGrid.btnSave.Visible = true;
                //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
                //    oldValue, newValue, IListOwner, IListIndex);
            }
            else if (Property != null && Property.CanWrite)
            {
                DataChangeHandler.PropertyObjectChanged(oldValue, newValue, PropertyOwner, Property);
                //PropertyGrid.btnSave.Visible = true;
                //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
                //    oldValue, newValue, PropertyOwner, Property);
            }
            DoneEditing?.Invoke();
        }

        public object GetValue()
        {
            try
            {
                if (IListOwner != null)
                    return IListOwner[IListIndex];

                if (Property == null)
                    throw new InvalidOperationException();

                if (!Property.CanRead)
                    return null;

                return Property.GetValue(PropertyOwner);
            }
            catch (Exception ex)
            {
                Engine.PrintLine(ex.ToString());
                return ex;
            }
        }
        public void OnValueChanged() => ValueChanged?.Invoke();
        public void UpdateValue(object newValue, bool submitStateChange)
        {
            if (_updating)
                return;

            if (IsReferenceType && ReferenceEquals(newValue, GetValue()))
            {
                Engine.LogWarning("Tried setting class object to the same reference. Are you sure you didn't mean to update a property within?");
                return;
            }

            if (IListOwner != null)
            {
                if (submitStateChange)
                {
                    object oldValue = IListOwner[IListIndex];
                    IListOwner[IListIndex] = newValue;
                    SubmitStateChange(oldValue, newValue);
                }
                else
                    IListOwner[IListIndex] = newValue;

                if (_isEditing)
                    _newValue = newValue;
            }
            else if (Property != null && Property.CanWrite)
            {
                if (submitStateChange)
                {
                    object oldValue = Property.GetValue(PropertyOwner);
                    Property.SetValue(PropertyOwner, newValue);
                    newValue = Property.GetValue(PropertyOwner);
                    SubmitStateChange(oldValue, newValue);
                    if (_isEditing)
                        _newValue = newValue;
                }
                else
                {
                    Property.SetValue(PropertyOwner, newValue);
                    if (_isEditing)
                        _newValue = Property.GetValue(PropertyOwner);
                }
                //Update the display in case the property's set method modified the submitted data
                UpdateDisplay();
            }
            //else
            //    throw new InvalidOperationException();
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
                DataChangeHandler.PropertyObjectChanged(_oldValue, _newValue, classObject, info);
                DoneEditing?.Invoke();
                //PropertyGrid.btnSave.Visible = true;
                //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
                //    _oldValue, _newValue, classObject, info);
            }
        }
        internal protected virtual void SetIListOwner(IList list, Type elementType, int index)
        {
            IListOwner = list;
            IListIndex = index;
            DataType = elementType;
            SetControlsEnabled(!list.IsReadOnly && !_readOnly);
            UpdateDisplay();
        }
        internal protected virtual void SetProperty(PropertyInfo propertyInfo, object propertyOwner)
        {
            Property = propertyInfo;
            PropertyOwner = propertyOwner;
            DataType = Property.PropertyType;
            SetControlsEnabled(Property.CanWrite && !_readOnly);
            UpdateDisplay();
        }
        internal void SetLabel(Label label)
        {
            Label = label;
            OnLabelSet();
        }
        public void UpdateDisplay()
        {
            if (IsEditing)
                return;

            _updating = true;
            UpdateDisplayInternal();
            _updating = false;
        }
        protected virtual void SetControlsEnabled(bool enabled) { Enabled = enabled; }
        protected virtual void UpdateDisplayInternal() { }
        protected virtual void OnLabelSet() { }

        /// <summary>
        /// List of all visible PropGridItems that need to be updated.
        /// </summary>
        private static List<PropGridItem> VisibleItems { get; } = new List<PropGridItem>();
        internal static void UpdateVisibleItems()
        {
            if (VisibleItems.Count > 0)
            {
                //Parallel.For(0, VisibleItems.Count, i =>
                //{
                //    if (VisibleItems.IndexInRange(i))
                //        VisibleItems[i].Invoke((Action)VisibleItems[i].UpdateDisplay);
                //});
                //Application.DoEvents();
            }
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            VisibleItems.Add(this);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            VisibleItems.Remove(this);
            base.OnHandleDestroyed(e);
        }
        public override string ToString()
            => DataType?.ToString() + " - " + Property.Name;
    }
}
