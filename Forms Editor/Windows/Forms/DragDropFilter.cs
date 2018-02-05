using Core.Win32.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms
{
    /// <summary>
    /// Pre-filter mouse messages to impersonate drag and drop mechanism.
    /// </summary>
    public class DragDropFilter : IMessageFilter
    {
        public event EventHandler Done;
        private Type ControlType = typeof(Control);
        private Control _hoveredControl = null;
        private MethodInfo
            _dragDrop, //DragEventArgs drgevent
            _dragEnter, //DragEventArgs drgevent
            _dragLeave, //EventArgs e
            _dragOver, //DragEventArgs drgevent
            _giveFeedback, //GiveFeedbackEventArgs gfbevent
            _queryContinue; //QueryContinueDragEventArgs qcdevent
        private DataObject _data;
        private Point _mousePoint;
        private DragDropEffects _allowed, _current;
        public DragDropFilter(object data, DragDropEffects allowedEffects)
        {
            _mousePoint = Cursor.Position;
            _data = new DataObject(DataFormats.FileDrop, data);
            _allowed = allowedEffects;
            _current = DragDropEffects.Move & allowedEffects;
            _dragDrop = ControlType.GetMethod("OnDragDrop", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragEnter = ControlType.GetMethod("OnDragEnter", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragLeave = ControlType.GetMethod("OnDragLeave", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragOver = ControlType.GetMethod("OnDragOver", BindingFlags.NonPublic | BindingFlags.Instance);
            _giveFeedback = ControlType.GetMethod("OnGiveFeedback", BindingFlags.NonPublic | BindingFlags.Instance);
            _queryContinue = ControlType.GetMethod("OnQueryContinueDrag", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public bool PreFilterMessage(ref Message m)
        {
            WindowsMessage msg = (WindowsMessage)m.Msg;
            //msg.ToString().PrintLine();
            switch (msg)
            {
                case WindowsMessage.WM_MOUSEMOVE:
                    int keyState = (int)m.WParam;
                    Control c = Control.FromHandle(m.HWnd);
                    //short x = (short)((int)m.LParam & 0xFFFF);
                    //short y = (short)(((int)m.LParam >> 16) & 0xFFFF);
                    _mousePoint = Cursor.Position;//new Point(x, y);
                    while (c != null && !c.AllowDrop)
                        c = c.Parent;
                    bool canDrop = c != null && c.AllowDrop;
                    if (canDrop)
                    {
                        //Engine.PrintLine(c.GetType().GetFriendlyName());
                        GiveFeedbackEventArgs feedback = new GiveFeedbackEventArgs(_current, true);
                        _giveFeedback.Invoke(c, new object[] { feedback });
                        _current = feedback.Effect;
                    }
                    if (c != _hoveredControl)
                    {
                        if (_hoveredControl != null && _hoveredControl.AllowDrop)
                        {
                            EventArgs args = EventArgs.Empty;
                            _dragLeave.Invoke(_hoveredControl, new object[] { args });
                        }
                        if (canDrop)
                        {
                            DragEventArgs dragArgs = new DragEventArgs(_data, keyState, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                            _dragEnter.Invoke(c, new object[] { dragArgs });
                            _current = dragArgs.Effect;
                        }
                        _hoveredControl = c;
                        //Engine.PrintLine(c.GetType().GetFriendlyName());
                    }
                    else if (canDrop)
                    {
                        DragEventArgs dragArgs = new DragEventArgs(_data, keyState, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                        _dragOver.Invoke(c, new object[] { dragArgs });
                        _current = dragArgs.Effect;
                    }
                    else
                    {
                        bool found = false;
                        foreach (Form f in Application.OpenForms)
                        {
                            if (f.DesktopBounds.Contains(_mousePoint))
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            Application.RemoveMessageFilter(this);
                            Editor.Instance.DoDragDrop(_data.GetData(DataFormats.FileDrop), _allowed);
                            Done?.Invoke(null, EventArgs.Empty);
                        }
                    }
                    break;
                case WindowsMessage.WM_LBUTTONDOWN:
                    return true;
                case WindowsMessage.WM_LBUTTONUP:
                    Application.RemoveMessageFilter(this);
                    if (_hoveredControl != null)
                    {
                        DragEventArgs dragArgs = new DragEventArgs(_data, 0, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                        _dragDrop.Invoke(_hoveredControl, new object[] { dragArgs });
                    }
                    Done?.Invoke(null, EventArgs.Empty);
                    break;
                case WindowsMessage.WM_KEYDOWN:
                    if (_hoveredControl != null)
                    {
                        QueryContinueDragEventArgs queryArgs = new QueryContinueDragEventArgs(0, false, DragAction.Continue);
                        _queryContinue.Invoke(_hoveredControl, new object[] { queryArgs });
                    }
                    break;
            }
            return false;
        }
    }
}
