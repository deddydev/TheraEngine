using Extensions;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(IList))]
    public partial class PropGridIList : PropGridItem, ICollapsible
    {
        public PropGridIList() => InitializeComponent();

        public IList List { get; private set; } = null;

        private Type _elementType;
        private int _displayedCount = 0;

        public override PropGridCategory ParentCategory
        {
            get => base.ParentCategory;
            set
            {
                base.ParentCategory = value;
                propGridListItems.PropertyGrid = ParentCategory?.PropertyGrid;
            }
        }
        protected override bool UpdateDisplayInternal(object value)
        {
            List = value as IList;
            
            if (Editor.GetSettings().PropertyGrid.ShowTypeNames)
            {
                string typeName = (value?.GetType() ?? DataType).GetFriendlyName();
                lblObjectTypeName.Text = "[" + typeName + "] ";
            }
            else
                lblObjectTypeName.Text = string.Empty;

            lblObjectTypeName.Text += List is null ? "null" : List.Count + (List.Count == 1 ? " item" : " items");
            
            chkNull.Visible = !DataType.IsValueType;
            
            if (!(chkNull.Checked = List is null))
            {
                lblObjectTypeName.Enabled = List.Count > 0;
                btnAdd.Visible = !List.IsFixedSize;
                _elementType = List.DetermineElementType();

                if (propGridListItems.Visible && List.Count != _displayedCount)
                    LoadList(List);
            }
            else
            {
                lblObjectTypeName.Enabled = false;
                if (propGridListItems.Visible)
                {
                    propGridListItems.Visible = false;
                    LoadList(null);
                }
                btnAdd.Visible = false;
            }
            return false;
        }

        public void Expand() => propGridListItems.Visible = true;
        public void Collapse() => propGridListItems.Visible = false;
        public void Toggle() => propGridListItems.Visible = !propGridListItems.Visible;
        public ControlCollection ChildControls => propGridListItems.ChildControls;

        private void pnlElements_VisibleChanged(object sender, EventArgs e)
        {
            if (propGridListItems.Visible)
            {
                if (propGridListItems.PropertyTable.Controls.Count == 0)
                    LoadList(List);
            }
            else
            {
                LoadList(null);
            }
        }

        protected override void DestroyHandle()
        {
            LoadList(null);
            base.DestroyHandle();
        }

        private void LoadList(IList list)
        {
            if (propGridListItems.InvokeRequired)
            {
                propGridListItems.Invoke((Action<IList>)LoadList, list);
                return;
            }

            //propGridListItems.PropertyTable.SuspendLayout();
            propGridListItems.DestroyProperties();

            if (list != null)
            {
                _displayedCount = list.Count;
                //await Task.Run(() =>
                //{
                    ConcurrentDictionary<int, List<PropGridItem>> controls = new ConcurrentDictionary<int, List<PropGridItem>>();
                    ConcurrentDictionary<Type, Deque<Type>> editorTypeCaches = new ConcurrentDictionary<Type, Deque<Type>>();

                    Parallel.For(0, list.Count, i =>
                    {
                        //Type elementType = list[i]?.GetType() ?? _elementType;

                        //Deque<Type> controlTypes;
                        //if (editorTypeCaches.ContainsKey(elementType))
                        //    controlTypes = editorTypeCaches[elementType];
                        //else
                        //{
                        //    controlTypes = TheraPropertyGrid.GetControlTypes(elementType);
                        //    editorTypeCaches.TryAdd(elementType, controlTypes);
                        //}

                        var info = new PropGridMemberInfoIList(this, i);
                        var item = new PropGridIListElementWrapper();
                        item.SetReferenceHolder(info);
                        item.Dock = DockStyle.Fill;
                        item.Visible = true;
                        item.DataChangeHandler = DataChangeHandler;
                        controls.TryAdd(i, new List<PropGridItem>() { item });

                        //List<PropGridItem> items = TheraPropertyGrid.InstantiatePropertyEditors(controlTypes, info, DataChangeHandler);
                        //controls.TryAdd(i, items);
                    });

                    //TODO: wrap editors in a control that contains a minus button to remove the item from the list
                    for (int i = 0; i < controls.Count; ++i)
                    {
                        Label label = propGridListItems.AddMember(controls[i]);
                        label.MouseEnter += Label_MouseEnter;
                        label.MouseLeave += Label_MouseLeave;
                        label.MouseDown += Label_MouseDown;
                        label.MouseUp += Label_MouseUp;
                    }
                //});
            }
            else
                _displayedCount = 0;

            //propGridListItems.PropertyTable.ResumeLayout(true);
        }

        private void Label_HandleDestroyed(object sender, EventArgs e)
        {
            if (!(sender is Label label))
                return;

            label.MouseEnter -= Label_MouseEnter;
            label.MouseLeave -= Label_MouseLeave;
            label.MouseDown -= Label_MouseDown;
            label.MouseUp -= Label_MouseUp;
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {

        }
        private void Label_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private Color _prevLabelColor;
        private void Label_MouseLeave(object sender, EventArgs e)
        {
            if (List is null || List.Count == 0)
                return;

            Label label = (Label)sender;
            label.BackColor = _prevLabelColor;
        }

        private void Label_MouseEnter(object sender, EventArgs e)
        {
            if (List is null || List.Count == 0)
                return;

            Label label = (Label)sender;
            _prevLabelColor = label.BackColor;
            label.BackColor = Color.FromArgb(44, 48, 64);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            object value = Editor.DomainProxy.UserCreateInstanceOf(_elementType, true);

            if (value is null)
                return;

            List.Add(value);

            //Don't add to the grid manually right now, because the UpdateDisplayInternal method will do it automatically on the next update.
            //var items = TheraPropertyGrid.InstantiatePropertyEditors(
            //    TheraPropertyGrid.GetControlTypes(value?.GetType()), new PropGridMemberInfoIList(this, i), DataChangeHandler);
            //propGridListItems.AddMember(items, new object[0], false);
            //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(items[items.Count - 1]);
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            if (List != null)
                pnlHeader.BackColor = Color.FromArgb(105, 140, 170);
        }
        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            if (List != null)
                pnlHeader.BackColor = Color.FromArgb(75, 120, 160);
        }
        
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            if (List != null)
            {
                propGridListItems.Visible = !propGridListItems.Visible;
                //Editor.Instance.PropertyGridForm.PropertyGrid.pnlProps.ScrollControlIntoView(this);
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
                UpdateValue(chkNull.Checked ? null : Editor.UserCreateInstanceOf(DataType, true, this), true);
        }
    }
}
