using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using PostSharp.Aspects;
using PostSharp.Serialization;

namespace System
{
    [PSerializable]
    public class NotifyPropertyChangedAttribute : LocationInterceptionAspect
    {
        public override void OnSetValue(LocationInterceptionArgs args)
        {
            if (args.Value != args.GetCurrentValue())
            {
                PropertyInfo info = args.Location.PropertyInfo;
                if (info == null)
                    return;

                object currentValue = args.GetCurrentValue();
                args.Value = args.Value;
                args.ProceedSetValue();
                ((IObject)args.Instance).OnPropertyChanged(args.Location.PropertyInfo, currentValue);
            }
        }
    }

    public interface IObjectChild : IObject
    {
        //IObjectParent Parent { get; set; }
    }
    public interface IObjectParent : IObject
    {
        //List<IObjectChild> Children { get; set; }
    }
    public interface IVoidParent : IObjectParent { }
    public interface IVoidChild : IObjectChild { }
    public interface IObject
    {
        bool HasChanged { get; set; }
        bool IsDirty { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(PropertyInfo info, object previousValue);
    }
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public class ObjectBase : IObject, INotifyPropertyChanged
    {
        public static List<IObject> _changedObjects = new List<IObject>();

        protected bool _changed;
        
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyInfo info, object previousValue)
        {
            string output = "Changed property " + info.Name + " in " + GetType().ToString();
            if (this is INameable)
            {
                string name = ((INameable)this).Name;
                output += " \"" + name + "\"";
            }
            Console.WriteLine(output);

            _changed = true;
            _changedObjects.Add(this);

            ChangedEvent e = info.GetCustomAttribute<ChangedEvent>();
            if (e != null)
            {
                MethodInfo method = GetType().GetMethod(e._methodName).MakeGenericMethod(info.PropertyType);
                if (method != null)
                    method.Invoke(this, new object[] { previousValue });
            }

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(info.Name));
        }

        public AbstractRenderer Renderer { get { return Engine.Renderer; } }

        [EditorOnly]
        public bool HasChanged
        {
            get { return _changed; }
            set { _changed = value; }
        }
        [EditorOnly]
        public virtual bool IsDirty
        {
            get { return _changed; }
            set { _changed = value; }
        }

        public enum TickOrder
        {
            PrePhysics,
            DuringPhysics,
            PostPhysics,
        }
        /// <summary>
        /// Specifies that this object wants tick calls.
        /// </summary>
        /// <param name="order"></param>
        public void RegisterTick(TickOrder order)
        {
            Engine.RegisterTick(this, order);
        }
        /// <summary>
        /// Specifies that this object will not have any tick calls.
        /// </summary>
        public void UnregisterTick()
        {
            Engine.UnregisterTick(this);
        }
        
        public virtual void Tick(float delta) { }
    }
    [NotifyPropertyChanged]
    public class ObjectBase<ParentObject, ChildObject> : ObjectBase where ParentObject : IObjectParent where ChildObject : IObjectChild
    {
        private ParentObject _parent;
        private List<ChildObject> _children;

        public ParentObject Parent { get { return _parent; } set { _parent = value; } }
        public List<ChildObject> Children { get { return _children; } set { _children = value; } }

        [EditorOnly]
        public override bool IsDirty
        {
            get
            {
                if (HasChanged)
                    return true;
                if (_children != null)
                    foreach (ChildObject n in _children)
                        if (n.HasChanged || n.IsDirty)
                            return true;
                return false;
            }
            set
            {
                _changed = value;
                if (_children != null)
                    foreach (ChildObject r in _children)
                        r.IsDirty = value;
            }
        }
    }
}
