using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Collections;
using System.Collections.ObjectModel;

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
                ((ObjectBase)args.Instance).OnPropertyChanged(args.Location.PropertyInfo, currentValue);
            }
        }
    }
    public delegate void SelectEventHandler(int index);
    public delegate void MoveEventHandler(ObjectBase node, bool select);
    public delegate void ResourceEventHandler(ObjectBase node);
    public delegate void ResourceChildEventHandler(ObjectBase node, ObjectBase child);
    public delegate void ResourceChildInsertEventHandler(int index, ObjectBase node, ObjectBase child);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public enum TickOrder
    {
        PrePhysics,
        //DuringPhysics,
        PostPhysics,
    }
    [NotifyPropertyChanged]
    public class ObjectBase : INotifyPropertyChanged
    {
        public static List<ObjectBase> _changedObjects = new List<ObjectBase>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event SelectEventHandler SelectChild;
        public event EventHandler UpdateProps, UpdateControl;
        public event MoveEventHandler MovedUp, MovedDown;
        public event ResourceEventHandler Disposing, Renamed, Replaced, Restored;
        public event ResourceChildEventHandler ChildAdded, ChildRemoved;
        public event ResourceChildInsertEventHandler ChildInserted;

        private string _name;
        protected bool _initialized;
        protected bool _changed;
        private ObjectBase _parent;
        private List<ObjectBase> _children;
        private TickGroup? _tickGroup = null;
        private TickOrder? _tickOrder = null;
        public bool _isPopulating;

        [Category("Info"), Default, Browsable(false)]
        public virtual ResourceType ResourceType { get { return ResourceType.Undocumented; } }

        [Category("State"), State]
        public bool HasChildren { get { return _children != null; } }
        [Category("State"), State]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public AbstractRenderer Renderer { get { return Engine.Renderer; } }
        public TickGroup? TickGroup
        {
            get { return _tickGroup; }
            set
            {
                if (_tickGroup == value)
                    return;
                if (_tickGroup != null)
                    UnregisterTick();
                if ((_tickGroup = value) != null)
                    RegisterTick();
            }
        }
        public TickOrder? TickOrder
        {
            get { return _tickOrder; }
            set
            {
                if (_tickOrder == value)
                    return;
                if (_tickOrder != null)
                    UnregisterTick();
                if ((_tickOrder = value) != null)
                    RegisterTick();
            }
        }
        [Category("State"), State]
        public bool HasInitialized { get { return _initialized; } }
        [Category("State"), State]
        public bool HasPopulated { get { return _children != null; } }
        [Category("State"), State, EditorOnly]
        public bool HasChanged
        {
            get { return _changed; }
            set { _changed = value; }
        }
        [Category("State"), State, EditorOnly]
        public bool IsDirty
        {
            get
            {
                if (HasChanged)
                    return true;
                if (_children != null)
                    foreach (ObjectBase n in _children)
                        if (n.HasChanged || n.IsDirty)
                            return true;
                return false;
            }
            set
            {
                _changed = value;
                if (_children != null)
                    foreach (ObjectBase r in _children)
                        r.IsDirty = value;
            }
        }
        [Category("State"), State]
        public ObjectBase Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
            }
        }
        [Category("State"), State]
        public virtual ReadOnlyCollection<ObjectBase> Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new List<ObjectBase>();
                    _isPopulating = true;
                    OnPopulate();
                    _isPopulating = false;
                }
                return _children.AsReadOnly();
            }
        }
        public virtual Type[] ChildTypes { get { return new Type[] { typeof(ObjectBase) }; } }

        /// <summary>
        /// Specifies that this object wants tick calls.
        /// </summary>
        /// <param name="order"></param>
        public void RegisterTick() { Engine.RegisterTick(this); }
        /// <summary>
        /// Specifies that this object will not have any tick calls.
        /// </summary>
        public void UnregisterTick() { Engine.UnregisterTick(this); }

        public virtual void Tick(float delta) { }

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

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));
        }

        public void Initialize(DataSource source)
        {
            if (OnInitialize())
                OnPopulate();
            _initialized = true;
        }
        protected virtual bool OnInitialize()
        {
            return false;
        }
        protected virtual void OnPopulate()
        {

        }
        public void AddChild(ObjectBase obj)
        {
            Children.Add(obj);
            obj.Parent = this;
            OnChildAdded(obj);
        }
        protected virtual void OnChildAdded(ObjectBase obj)
        {

        }
    }
}
