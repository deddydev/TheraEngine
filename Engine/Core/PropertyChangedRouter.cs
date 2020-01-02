using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Core
{
    public class PropertyChangedRouter<T> : TObjectSlim where T : INotifyPropertyChanged
    {
        public PropertyChangedRouter() { }
        public PropertyChangedRouter(T obj, IDictionary<string, Action<T>> methods)
        {
            PropertyChangedMethods = methods;
            Object = obj;
        }
        public PropertyChangedRouter(T obj, params (string name, Action<T> action)[] methods)
        {
            PropertyChangedMethods = new Dictionary<string, Action<T>>();
            foreach (var (name, action) in methods)
                if (!PropertyChangedMethods.ContainsKey(name))
                    PropertyChangedMethods.Add(name, action);
            Object = obj;
        }

        public bool CallMethodsOnObjectSet { get; set; } = true;
        public IDictionary<string, Action<T>> PropertyChangedMethods { get; set; }

        private T _monitoredObject;
        public T Object
        {
            get => _monitoredObject;
            set
            {
                if (_monitoredObject != null)
                    _monitoredObject.PropertyChanged -= MonitoredObject_PropertyChanged;
                _monitoredObject = value;
                if (_monitoredObject != null)
                {
                    _monitoredObject.PropertyChanged += MonitoredObject_PropertyChanged;
                    if (CallMethodsOnObjectSet)
                        foreach (var prop in PropertyChangedMethods)
                            prop.Value?.Invoke(_monitoredObject);
                }
            }
        }

        private void MonitoredObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is T obj))
                return;

            string name = e.PropertyName;
            if (PropertyChangedMethods?.ContainsKey(name) ?? false)
                PropertyChangedMethods[name]?.Invoke(obj);
        }
    }
}
