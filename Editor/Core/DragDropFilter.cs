using Core.Win32.Native;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEditor.Core.Extensions;

namespace TheraEditor.Core
{
    /// <summary>
    /// Pre-filter mouse messages to impersonate drag and drop mechanism.
    /// </summary>
    public class DragDropFilter : IMessageFilter
    {
        private static DragDropFilter Instance = null;

        /// <summary>
        /// Event called when the user drops the dragged data 
        /// and this filter has removed itself from the application filter loop.
        /// </summary>
        public event EventHandler Done;
        
        /// <summary>
        /// The last control that the mouse was hovering over and accepted dropping data.
        /// </summary>
        private Control _hoveredControl = null;

        /// <summary>
        /// The private methods used to impersonate the drag/drop pipeline.
        /// </summary>
        private MethodInfo
            _dragDrop, //DragEventArgs drgevent
            _dragEnter, //DragEventArgs drgevent
            _dragLeave, //EventArgs e
            _dragOver, //DragEventArgs drgevent
            _giveFeedback, //GiveFeedbackEventArgs gfbevent
            _queryContinue; //QueryContinueDragEventArgs qcdevent
        
        /// <summary>
        /// The data being dragged.
        /// </summary>
        private IDataObject _data;

        /// <summary>
        /// The last recorded position of the mouse.
        /// </summary>
        private Point _mousePoint;

        /// <summary>
        /// Drag/drop effects allowed in the operation.
        /// </summary>
        private DragDropEffects _allowed;

        /// <summary>
        /// Drag/drop effect currently in use.
        /// </summary>
        private DragDropEffects _current;

        private DragDropFilter()
        {

        }

        public static DragDropFilter Initialize(object data, DragDropEffects allowedEffects)
        {
            Instance?.StopFiltering();

            DragDropFilter f = new DragDropFilter()
            {
                _mousePoint = Cursor.Position,
                _data = new DataObject(DataFormats.FileDrop, data),
                _allowed = allowedEffects,
                _current = DragDropEffects.Move & allowedEffects,
            };
            f.Init();

            Instance = f;

            return f;
        }

        public static DragDropFilter Initialize(IDataObject data, DragDropEffects allowedEffects)
        {
            Instance?.StopFiltering();

            DragDropFilter f = new DragDropFilter()
            {
                _mousePoint = Cursor.Position,
                _data = data,
                _allowed = allowedEffects,
                _current = DragDropEffects.Move & allowedEffects,
            };
            f.Init();

            Instance = f;

            return f;
        }

        /// <summary>
        /// Get MethodInfo variables for private drag/drop methods.
        /// This allows us to impersonate the typical drag/dropping pipeline 
        /// by invoking these methods in the same expected manner.
        /// </summary>
        private void Init()
        {
            Type cType = typeof(Control);
            _dragDrop = cType.GetMethod("OnDragDrop", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragEnter = cType.GetMethod("OnDragEnter", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragLeave = cType.GetMethod("OnDragLeave", BindingFlags.NonPublic | BindingFlags.Instance);
            _dragOver = cType.GetMethod("OnDragOver", BindingFlags.NonPublic | BindingFlags.Instance);
            _giveFeedback = cType.GetMethod("OnGiveFeedback", BindingFlags.NonPublic | BindingFlags.Instance);
            _queryContinue = cType.GetMethod("OnQueryContinueDrag", BindingFlags.NonPublic | BindingFlags.Instance);
        }
        public bool PreFilterMessage(ref Message m)
        {
            EWindowsMessage msg = (EWindowsMessage)m.Msg;
            switch (msg)
            {
                case EWindowsMessage.WM_MOUSEMOVE:

                    //Get cursor position
                    //LParam seems to be off compared to Cursor.Position
                    //short x = (short)((int)m.LParam & 0xFFFF);
                    //short y = (short)(((int)m.LParam >> 16) & 0xFFFF);
                    _mousePoint = Cursor.Position;//new Point(x, y);

                    int keyState = (int)m.WParam;
                    Control ctrl = Control.FromHandle(m.HWnd);
                    foreach (Form f in Application.OpenForms)
                    {
                        Control c = f.FindControlAtCursor();
                        if (c != null)
                            ctrl = c;
                    }

                    //Find the owning control that allows dropping data.
                    //Repeats until the control reference is null or the control accepts dropping.
                    while (ctrl != null && !ctrl.AllowDrop)
                        ctrl = ctrl.Parent;

                    //Determine if a control that accepts dropped data was found
                    bool canDrop = ctrl != null;

                    if (canDrop)
                    {
                        //Get feedback from the control
                        GiveFeedbackEventArgs feedback = new GiveFeedbackEventArgs(_current, true);
                        _giveFeedback.Invoke(ctrl, new object[] { feedback });
                        _current = feedback.Effect;
                    }

                    //If the found control isn't the hovered control, a change in target has occurred
                    if (ctrl != _hoveredControl)
                    {
                        //If the previous target isn't null and accepts drop, invoke DragLeave
                        if (_hoveredControl != null)
                        {
                            EventArgs args = EventArgs.Empty;
                            _dragLeave.Invoke(_hoveredControl, new object[] { args });
                        }

                        //If the new target isn't null and accepts drop, invoke DragEnter 
                        if (canDrop)
                        {
                            DragEventArgs dragArgs = new DragEventArgs(_data, keyState, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                            _dragEnter.Invoke(ctrl, new object[] { dragArgs });
                            _current = dragArgs.Effect;

                            //Engine.PrintLine("[Drag/Drop] Hovering over " + ctrl.GetType().GetFriendlyName());
                        }
                        else
                            CheckNullTarget();

                        _hoveredControl = ctrl;
                    }
                    else if (canDrop)
                    {
                        //This is the same drag target from the last filter loop (and isn't null), so invoke DragOver
                        DragEventArgs dragArgs = new DragEventArgs(_data, keyState, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                        _dragOver.Invoke(ctrl, new object[] { dragArgs });
                        _current = dragArgs.Effect;
                    }
                    else
                        CheckNullTarget();

                    break;
                
                case EWindowsMessage.WM_LBUTTONDOWN:
                    return true;
                case EWindowsMessage.WM_LBUTTONUP:
                    if (_hoveredControl != null)
                    {
                        DragEventArgs dragArgs = new DragEventArgs(_data, 0, _mousePoint.X, _mousePoint.Y, _allowed, _current);
                        _dragDrop.Invoke(_hoveredControl, new object[] { dragArgs });
                    }
                    StopFiltering();
                    break;
                
                case EWindowsMessage.WM_KEYDOWN:
                    if (_hoveredControl != null)
                    {
                        QueryContinueDragEventArgs queryArgs = new QueryContinueDragEventArgs(0, false, DragAction.Continue);
                        _queryContinue.Invoke(_hoveredControl, new object[] { queryArgs });
                    }
                    break;
                
            }
            return false;
        }

        public void StopFiltering()
        {
            if (Instance != this)
                return;

            Application.RemoveMessageFilter(this);
            //Engine.PrintLine("[Drag/Drop] Stopped filtering drag/drop.");
            Instance = null;
            Done?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called when the current target is null and/or don't accept dropping.
        /// Makes sure the cursor position is still on a form for this application.
        /// If not, removes this messaging filter and invokes the regular blocking drag/drop method.
        /// </summary>
        private void CheckNullTarget()
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
                StopFiltering();
                Editor.Instance.DoDragDrop(_data.GetData(DataFormats.FileDrop), _allowed);
            }
        }

        /// <summary>
        /// Starts the non-blocking "async" drag/drop operation.
        /// Code after this method will run immediately.
        /// </summary>
        public void BeginFiltering()
        {
            Application.AddMessageFilter(this);
            //Engine.PrintLine("[Drag/Drop] Now filtering drag/drop.");
        }
    }
}
