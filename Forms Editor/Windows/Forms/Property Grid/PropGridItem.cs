using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;

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

        public T GetParentInfo<T>() where T : PropGridItemParentInfo
            => ParentInfo as T;
        
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public PropGridItemParentInfo ParentInfo { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Type DataType { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public Label Label { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public bool ReadOnly
        {
            get => _readOnly;
            set
            {
                _readOnly = value;
                SetControlsEnabled(!_readOnly && (ParentInfo == null || !ParentInfo.IsReadOnly()));
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
            ParentInfo?.SubmitStateChange(oldValue, newValue, DataChangeHandler);
            DoneEditing?.Invoke();
        }

        public object GetValue()
        {
            try
            {
                return ParentInfo?.Value;
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

            if (submitStateChange)
            {
                object oldValue = ParentInfo.Value;

                ParentInfo.Value = newValue;
                newValue = ParentInfo.Value;

                SubmitStateChange(oldValue, newValue);
            }
            else
            {
                ParentInfo.Value = newValue;
                newValue = ParentInfo.Value;
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
                DataChangeHandler.PropertyObjectChanged(_oldValue, _newValue, classObject, info);
                DoneEditing?.Invoke();
                //PropertyGrid.btnSave.Visible = true;
                //Editor.Instance.UndoManager.AddChange(PropertyGrid.TargetObject.EditorState,
                //    _oldValue, _newValue, classObject, info);
            }
        }
        internal protected virtual void SetIDictionaryOwner(IDictionary dic, Type dataType, object key, bool isKey)
        {
            ParentInfo = new PropGridItemParentIDictionaryInfo(dic, key, isKey);
            DataType = dataType;
            SetControlsEnabled(!dic.IsReadOnly && !_readOnly);
            UpdateDisplay();
        }
        internal protected virtual void SetIListOwner(IList list, Type elementType, int index)
        {
            ParentInfo = new PropGridItemParentIListInfo(list, index);
            DataType = elementType;
            SetControlsEnabled(!list.IsReadOnly && !_readOnly);
            UpdateDisplay();
        }
        protected internal void SetPropertyByName(string propertyName, object propertyOwner)
        {
            PropertyInfo propertyInfo = propertyOwner.GetType().GetProperty(propertyName);
            SetProperty(propertyInfo, propertyOwner);
        }
        internal protected virtual void SetProperty(PropertyInfo propertyInfo, object propertyOwner)
        {
            ParentInfo = new PropGridItemParentPropertyInfo(propertyOwner, propertyInfo);
            DataType = propertyInfo.PropertyType;
            SetControlsEnabled(propertyInfo.CanWrite && !_readOnly);
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
                                if (!item.Disposing && !item.IsDisposed)
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
            => DataType?.ToString() + " - " + ParentInfo?.ToString();
    }
}
