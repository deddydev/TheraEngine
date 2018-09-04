using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public partial class PropGridEvent : PropGridItem
    {
        private EventInfo _event;
        public EventInfo Event
        {
            get => _event;
            private set
            {
                _event = value;
                lblEvent.Text = _event.EventHandlerType.GetFriendlyName().ToString();
                //if (Label != null)
                //    Label.Text = _method.Name;//fName.Substring(0, paren);
            }
        }
        
        public PropGridEvent() => InitializeComponent();

        protected internal override void SetReferenceHolder(PropGridItemRefInfo parentInfo)
        {
            base.SetReferenceHolder(parentInfo);
            if (parentInfo is PropGridItemRefEventInfo eventInfo)
            {
                Event = eventInfo.Event;
                
            }
        }

        public void Raise()
        {
            MethodInfo raiseMethod = Event.RaiseMethod;
            if (raiseMethod != null)
                raiseMethod.Invoke(ParentInfo.Owner, new object[] { });
        }
        public void AddMethod(MethodInfo method)
        {
            Type tDelegate = Event.EventHandlerType;
            Delegate del = Delegate.CreateDelegate(tDelegate, this, method);
            MethodInfo addHandler = Event.AddMethod;
            object[] addHandlerArgs = { del };
            addHandler.Invoke(ParentInfo.Target, addHandlerArgs);
        }
        public void RemoveMethod(MethodInfo method)
        {
            Type tDelegate = Event.EventHandlerType;
            Delegate del = Delegate.CreateDelegate(tDelegate, this, method);
            MethodInfo removeHandler = Event.RemoveMethod;
            object[] removeHandlerArgs = { del };
            removeHandler.Invoke(ParentInfo.Target, removeHandlerArgs);
        }

        protected override void UpdateDisplayInternal(object value)
        {

        }
        
        private void lblMethod_MouseEnter(object sender, EventArgs e)
            => lblEvent.BackColor = Color.FromArgb(14, 34, 18);
        private void lblMethod_MouseLeave(object sender, EventArgs e)
            => lblEvent.BackColor = Color.DarkGreen;
        
        private void lblMethod_MouseDown(object sender, MouseEventArgs e)
        {
            
        }

        private void lblEvent_Click(object sender, EventArgs e)
        {
            Raise();
        }
    }
}
