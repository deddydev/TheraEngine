using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace System
{
    public delegate void CheckedAction(bool checkedState);
    public class CustomMenuItem : MenuItem
    {
        List<Key> _keys;
        List<Key> _currentKeys;
        Action _method;
        CheckedAction _checkedMethod;

        public CustomMenuItem(
            string text,
            Action method,
            params Key[] keys)
        {
            _method = method;
            _keys = keys.ToList();
            Header = text;
            IsCheckable = false;
        }
        public CustomMenuItem(
            string text,
            CheckedAction method,
            params Key[] keys)
        {
            _checkedMethod = method;
            _keys = keys.ToList();
            Header = text;
            IsCheckable = true;
        }

        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            _checkedMethod(true);
        }

        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            _checkedMethod(false);
        }

        protected override void OnClick()
        {
            base.OnClick();
            if (!IsCheckable)
                _method();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            _currentKeys.Add(e.Key);
            if (_currentKeys.Equals(_keys))
                _method();
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            _currentKeys.Remove(e.Key);
        }
    }
}
