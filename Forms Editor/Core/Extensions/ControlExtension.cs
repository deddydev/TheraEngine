using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Maths;
using TheraEngine.Timers;

namespace TheraEditor.Core.Extensions
{
    public static class ControlExtension
    {
        public static void FadeBackColor(this Control control, Color color, float seconds, ref EventHandler<FrameEventArgs> fadeMethod, Func<float, float> timeModifier = null)
        {
            Color startColor = control.BackColor;
            float totalTime = 0.0f;
            if (fadeMethod != null)
            {
                Engine.UnregisterTick(null, fadeMethod, null);
                fadeMethod = null;
            }
            if (timeModifier != null)
            {
                void method(object sender, FrameEventArgs args)
                {
                    totalTime += args.Time;
                    control.BackColor = Interp.Lerp(startColor, color, timeModifier(totalTime / seconds));
                    if (totalTime >= seconds)
                        Engine.UnregisterTick(null, method, null);
                }
                fadeMethod = method;
                Engine.RegisterTick(null, method, null);
            }
            else
            {
                void method(object sender, FrameEventArgs args)
                {
                    totalTime += args.Time;
                    control.BackColor = Interp.Lerp(startColor, color, totalTime / seconds);
                    if (totalTime >= seconds)
                        Engine.UnregisterTick(null, method, null);
                }
                fadeMethod = method;
                Engine.RegisterTick(null, method, null);
            }
        }
        public static void FadeForeColor(this Control control, Color color, float seconds, ref EventHandler<FrameEventArgs> fadeMethod, Func<float, float> timeModifier = null)
        {
            Color startColor = control.BackColor;
            float totalTime = 0.0f;
            if (fadeMethod != null)
            {
                Engine.UnregisterTick(null, fadeMethod, null);
                fadeMethod = null;
            }
            if (timeModifier != null)
            {
                void method(object sender, FrameEventArgs args)
                {
                    totalTime += args.Time;
                    control.ForeColor = Interp.Lerp(startColor, color, timeModifier(totalTime / seconds));
                    if (totalTime >= seconds)
                        Engine.UnregisterTick(null, method, null);
                }
                fadeMethod = method;
                Engine.RegisterTick(null, method, null);
            }
            else
            {
                void method(object sender, FrameEventArgs args)
                {
                    totalTime += args.Time;
                    control.ForeColor = Interp.Lerp(startColor, color, totalTime / seconds);
                    if (totalTime >= seconds)
                        Engine.UnregisterTick(null, method, null);
                }
                fadeMethod = method;
                Engine.RegisterTick(null, method, null);
            }
        }
        public static Control FindControlAtPoint(this Control container, Point pos)
        {
            Control child;
            foreach (Control c in container.Controls)
            {
                if (c.Visible && c.Bounds.Contains(pos))
                {
                    child = c.FindControlAtPoint(new Point(pos.X - c.Left, pos.Y - c.Top));
                    if (child == null) return c;
                    else return child;
                }
            }
            return null;
        }
        public static Control FindControlAtCursor(this Form form)
        {
            Point pos = Cursor.Position;
            if (form.Bounds.Contains(pos))
                return form.FindControlAtPoint(form.PointToClient(pos));
            return null;
        }
        public static void InvokeIfNecessary(this Control control, Delegate del, params object[] args)
        {
            if (control.InvokeRequired)
                control.Invoke(del, args);
            else
                del.DynamicInvoke(args);
        }
    }
}
