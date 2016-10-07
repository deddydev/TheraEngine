using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using PostSharp.Aspects;
using PostSharp.Serialization;
using System.Collections;
using System.Collections.ObjectModel;
using CustomEngine.Files;
using System.Threading.Tasks;
using System.IO;

namespace System
{
    public delegate void SelectEventHandler(ObjectBase node, int index);
    public delegate void ResourceEventHandler(ObjectBase node);
    public delegate void ResourceChildEventHandler(ObjectBase node, ObjectBase child);
    public delegate void ResourceChildInsertEventHandler(int index, ObjectBase node, ObjectBase child);
    public delegate void RenamedEventHandler(ObjectBase node, string oldName);
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
        public event RenamedEventHandler Renamed;
        public event ResourceEventHandler Disposing, Replaced, Restored, UpdateProperties, UpdateEditor;
        public event ResourceChildEventHandler ChildAdded, ChildRemoved, ParentChanged;
        public event ResourceChildInsertEventHandler ChildInserted;

        private string _name;
        protected bool _initialized, _changed, _replaced, _isPopulating, _isInitializing;
        private TickGroup? _tickGroup = null;
        private TickOrder? _tickOrder = null;

        [Category("Info"), Default, Browsable(false)]
        public virtual ResourceType ResourceType { get { return ResourceType.Undocumented; } }
        
        [Category("State"), State, PostChanged("OnRenamed")]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public AbstractRenderer Renderer { get { return Engine.Renderer; } }
        [Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        public TickGroup? TickGroup
        {
            get { return _tickGroup; }
            set { _tickGroup = value; }
        }
        [Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        public TickOrder? TickOrder
        {
            get { return _tickOrder; }
            set { _tickOrder = value; }
        }
        [Category("State"), State, EditorOnly]
        public bool HasChanged
        {
            get { return _changed; }
            set { _changed = value; }
        }
        [Category("State"), State, EditorOnly]
        public virtual bool IsDirty
        {
            get { return HasChanged; }
            set { HasChanged = value; }
        }
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
            output += " \"{Name}\"";
            Console.WriteLine(output);

            _changed = true;
            _changedObjects.Add(this);

            PostChanged e = info.GetCustomAttribute<PostChanged>();
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
        protected virtual void OnParentChanged(ObjectBase previousParent) { ParentChanged?.Invoke(this, previousParent); }
        protected virtual void OnChildAdded(ObjectBase addedChild) { ChildAdded?.Invoke(this, addedChild); }
        protected virtual void OnChildRemoved(ObjectBase removedChild) { ChildRemoved?.Invoke(this, removedChild); }
        protected virtual void OnChildInserted(int index, ObjectBase insertedChild) { ChildInserted?.Invoke(index, this, insertedChild); }
        protected virtual void OnSelectChild(int index) { SelectChild?.Invoke(this, index); }
        protected virtual void OnUpdateProperties() { UpdateProperties?.Invoke(this); }
        protected virtual void OnUpdateEditor() { UpdateEditor?.Invoke(this); }
        protected virtual void OnDisposing() { Disposing?.Invoke(this); }
        protected virtual void OnRenamed(string oldName) { Renamed?.Invoke(this, oldName); }
        protected virtual void OnReplaced() { Replaced?.Invoke(this); }
        protected virtual void OnRestored() { Restored?.Invoke(this); }
    }
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
}
