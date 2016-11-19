using CustomEngine.Input.Devices.OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Input.Devices
{
    public abstract class CMouse : InputDevice
    {
        public CMouse(int index) : base(index) { }

        protected CursorManager _cursor = new CursorManager();

        public abstract void SetCursorPosition(float x, float y);

        protected override int GetAxisCount() { return 0; }
        protected override int GetButtonCount() { return 3; }

        public void RegisterButtonPressed(EMouseButton button, Action<bool> func)
        {

        }
        public void RegisterButtonEvent(EMouseButton button, ButtonInputType type, Action func)
        {

        }
        public void RegisterScroll(Action<bool> func)
        {

        }
        public void RegisterMouseMove(DelCursorUpdate func, bool continuousUpdate, bool relative)
        {
            _cursor.Register(func, continuousUpdate, relative);
        }
    }
    public delegate void DelCursorUpdate(float x, float y);
    public class CursorManager
    {
        private float _x, _y;

        List<DelCursorUpdate>
            _onUpdate = new List<DelCursorUpdate>(),
            _continuous = new List<DelCursorUpdate>(),
            _onUpdateRelative = new List<DelCursorUpdate>(),
            _continuousRelative = new List<DelCursorUpdate>();

        internal void Tick(float x, float y, float delta)
        {
            float xDiff = x - _x;
            float yDiff = y - _y;
            _x = x;
            _y = y;
            OnContinuousAbsolute(x, y);
            OnContinuousRelative(xDiff, yDiff);
            if (!xDiff.IsZero() && !yDiff.IsZero())
            {
                OnUpdateRelative(xDiff, yDiff);
                OnUpdateAbsolute(x, y);
            }
        }
        public void Register(DelCursorUpdate func, bool continuousUpdate, bool relative)
        {
            if (relative)
            {
                if (continuousUpdate)
                    _continuousRelative.Add(func);
                else
                    _onUpdateRelative.Add(func);
            }
            else
            {
                if (continuousUpdate)
                    _continuous.Add(func);
                else
                    _onUpdate.Add(func);
            }
        }
        private void OnUpdateAbsolute(float x, float y)
        {
            _onUpdate.ForEach(del => del(x, y));
        }
        private void OnUpdateRelative(float x, float y)
        {
            _onUpdateRelative.ForEach(del => del(x, y));
        }
        private void OnContinuousAbsolute(float x, float y)
        {
            _continuous.ForEach(del => del(x, y));
        }
        private void OnContinuousRelative(float x, float y)
        {
            _continuousRelative.ForEach(del => del(x, y));
        }
    }
    public enum EMouseButton
    {
        LeftClick,
        RightClick,
        MiddleClick,
    }
}
