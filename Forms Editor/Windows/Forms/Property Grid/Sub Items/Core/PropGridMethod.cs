using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridMethod : PropGridItem
    {
        private MethodInfo _method;
        public MethodInfo Method
        {
            get => _method;
            private set
            {
                _method = value;
                string fName = _method.GetFriendlyName();
                int paren = fName.IndexOf('(');
                lblMethod.Text = fName.Substring(paren + 1, fName.Length - paren - 2);
                //if (Label != null)
                //    Label.Text = _method.Name;//fName.Substring(0, paren);
            }
        }
        
        public PropGridMethod() => InitializeComponent();

        protected internal override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            if (parentInfo is PropGridItemRefMethodInfo methodInfo)
            {
                Method = methodInfo.Method;
            }
        }

        protected override void UpdateDisplayInternal(object value)
        {

        }
        
        private void lblMethod_MouseEnter(object sender, EventArgs e)
            => lblMethod.BackColor = Color.FromArgb(14, 34, 18);
        private void lblMethod_MouseLeave(object sender, EventArgs e)
            => lblMethod.BackColor = Color.DarkGreen;
        
        private void lblMethod_MouseDown(object sender, MouseEventArgs e)
        {
            if (Method.GetParameters().Length == 0)
                Method.Invoke(GetParentInfo<PropGridItemRefPropertyInfo>().GetOwner(), new object[0]);
            else
            {
                //TODO: retrieve parameters from the user
            }
        }
    }
}
