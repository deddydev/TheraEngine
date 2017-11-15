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
        public bool IsEditing { get; protected set; }

        protected bool _updating = false;

        public PropGridItem() => InitializeComponent();
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

        public void UpdateValue(object newValue)
        {
            if (_updating)
                return;
            if (IListOwner != null)
            {
                IListOwner[IListIndex] = newValue;
            }
            else if (Property != null)
            {
                if (Property.CanWrite)
                {
                    Property.SetValue(PropertyOwner, newValue);

                    //Update the display in case the property's set method modifies the submitted data
                    UpdateDisplay();
                }
            }
            else
                throw new InvalidOperationException();
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

        internal static GameTimer UpdateTimer = new GameTimer();

        private static List<PropGridItem> VisibleItems = new List<PropGridItem>();
        internal static void UpdateVisibleItems()
        {
            Parallel.For(0, VisibleItems.Count, i => VisibleItems[i].Invoke((Action)VisibleItems[i].UpdateDisplay));
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
        {
            return DataType?.ToString() + " - " + Property.Name;
        }
    }
}
