﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
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
            if (IListOwner != null)
                IListOwner[IListIndex] = newValue;

            if (Property == null)
                throw new InvalidOperationException();

            if (Property.CanWrite && !_updating)
            {
                Property.SetValue(PropertyOwner, newValue);

                //Update the display in case the property's set method modifies the submitted data
                UpdateDisplay();
            }
        }
        internal protected virtual void SetIListOwner(IList list, Type elementType, int index)
        {
            IListOwner = list;
            IListIndex = index;
            DataType = elementType;
            SetControlsEnabled(!list.IsReadOnly);
            UpdateDisplay();
        }
        internal protected virtual void SetProperty(PropertyInfo propertyInfo, object propertyOwner)
        {
            Property = propertyInfo;
            PropertyOwner = propertyOwner;
            DataType = Property.PropertyType;
            SetControlsEnabled(Property.CanWrite);
            UpdateDisplay();
        }
        internal void SetLabel(Label label)
        {
            Label = label;
            OnLabelSet();
        }
        public void UpdateDisplay()
        {
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
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            //if (Visible)
            //    VisibleItems.Add(this);
            //else
            //    VisibleItems.Remove(this);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            VisibleItems.Add(this);
        }
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            VisibleItems.Remove(this);
        }
        public override string ToString()
        {
            return DataType?.ToString() + " - " + Property.Name;
        }
    }
}
