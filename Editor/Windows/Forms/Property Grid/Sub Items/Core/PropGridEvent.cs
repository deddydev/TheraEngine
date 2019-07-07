using Extensions;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridEvent : PropGridItem
    {
        private EventInfo _event;
        private int _subscribedCount = 0;
        public EventInfo Event
        {
            get => _event;
            private set
            {
                _event = value;
                lblEvent.Text = _event.EventHandlerType.GetFriendlyName().ToString();
                lblEvent.Text += $" ({_event.GetSubscribedMethods(MemberInfo.Owner.Value).Length})";
            }
        }
        
        public PropGridEvent() => InitializeComponent();

        protected internal override void SetReferenceHolder(PropGridMemberInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            if (parentInfo is PropGridMemberInfoEvent eventInfo)
            {
                Event = eventInfo.Event;
            }
        }

        public void Raise()
        {
            MethodInfo raiseMethod = Event.GetRaiseMethod();
            if (raiseMethod != null)
                raiseMethod.Invoke(MemberInfo.Owner.Value, new object[] { });
        }
        public void AddMethod(MethodInfo method)
        {
            Type tDelegate = Event.EventHandlerType;
            Delegate del = Delegate.CreateDelegate(tDelegate, this, method);
            MethodInfo addHandler = Event.GetAddMethod();
            object[] addHandlerArgs = { del };
            addHandler.Invoke(MemberInfo.MemberValue, addHandlerArgs);
        }
        public void RemoveMethod(MethodInfo method)
        {
            Type tDelegate = Event.EventHandlerType;
            Delegate del = Delegate.CreateDelegate(tDelegate, this, method);
            MethodInfo removeHandler = Event.GetRemoveMethod();
            object[] removeHandlerArgs = { del };
            removeHandler.Invoke(MemberInfo.MemberValue, removeHandlerArgs);
        }

        protected override bool UpdateDisplayInternal(object value)
        {
            if (pnlSubscribed.Visible)
            {
                if (pnlSubscribed.Controls.Count != Event.GetSubscribedMethods(MemberInfo.Owner.Value).Length)
                {
                    LoadSubscribedMethods();
                }
            }
            return false;
        }

        private void lblMethod_MouseEnter(object sender, EventArgs e)
        {
            if (_subscribedCount > 0)
                lblEvent.BackColor = Color.FromArgb(14, 34, 18);
        }
        private void lblMethod_MouseLeave(object sender, EventArgs e)
            => lblEvent.BackColor = Color.DarkGreen;
        
        private void lblMethod_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
        private void LoadSubscribedMethods()
        {
            pnlSubscribed.Controls.Clear();
            Delegate[] m = Event.GetSubscribedMethods(MemberInfo.Owner.Value);
            foreach (Delegate d in m)
            {
                Label l = new Label
                {
                    Margin = new Padding(0),
                    Padding = new Padding(0),
                    AutoSize = true,
                    Text = $"{d.Method.GetFriendlyName()} [{d.Method.DeclaringType.GetFriendlyName()}]",
                    TextAlign = ContentAlignment.MiddleLeft,
                    Dock = DockStyle.Top
                };
                pnlSubscribed.Controls.Add(l);
            }
            lblEvent.Text = _event.EventHandlerType.GetFriendlyName().ToString();
            lblEvent.Text += $" ({m.Length})";
            _subscribedCount = m.Length;
        }
        private void lblEvent_Click(object sender, EventArgs e)
        {
            pnlSubscribed.Visible = !pnlSubscribed.Visible;
            if (pnlSubscribed.Visible)
            {
                LoadSubscribedMethods();
            }
            else
            {
                pnlSubscribed.Controls.Clear();
            }
        }
    }
}
