﻿using System;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridMethod : PropGridItem
    {
        private MethodInfoProxy _method;
        public MethodInfoProxy Method
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

        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            if (parentInfo is PropGridMemberInfoMethod methodInfo)
            {
                Method = methodInfo.Method;
            }
        }

        protected override bool UpdateDisplayInternal(object value)
        {
            return false;
        }
        
        private void lblMethod_MouseEnter(object sender, EventArgs e)
            => lblMethod.BackColor = Color.FromArgb(14, 34, 18);
        private void lblMethod_MouseLeave(object sender, EventArgs e)
            => lblMethod.BackColor = Color.DarkGreen;
        
        private void lblMethod_MouseDown(object sender, MouseEventArgs e)
        {
            if (Method.GetParameters().Length == 0)
                Method.Invoke(GetMemberInfoAs<PropGridMemberInfoMethod>().Owner.Value, new object[0]);
            else
            {
                //TODO: retrieve parameters from the user
            }
        }
    }
}
