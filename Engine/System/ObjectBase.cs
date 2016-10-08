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
    public delegate void ResourceEventHandler(ObjectBase node);
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
        public event RenamedEventHandler Renamed;
        public event ResourceEventHandler Disposing, UpdateProperties, UpdateEditor;

        private string _name;
        protected bool _changed;
        private TickGroup? _tickGroup = null;
        private TickOrder? _tickOrder = null;

        [Category("Info"), Default, Browsable(false)]
        public virtual ResourceType ResourceType { get { return ResourceType.Undocumented; } }
        
        [Category("State"), Default, PostChanged("OnRenamed")]
        public string Name
        {
            get { return _name; }
#if EDITOR
            set { _name = value; }
#endif
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
        /// <summary>
        /// If this class needs to be rebuilt.
        /// </summary>
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
        /// <summary>
        /// Updates logic for this class
        /// </summary>
        /// <param name="delta">The amount of time that has passed since the last tick update</param>
        public virtual void Tick(float delta) { }

        public void OnPropertyChanged(PropertyInfo info, object previousValue)
        {
            string output = "Changed property " + info.Name + " in " + GetType().ToString() + " \"{Name}\"";
            Console.WriteLine(output);

            _changed = true;
            _changedObjects.Add(this);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));
        }
        protected virtual void OnUpdateProperties() { UpdateProperties?.Invoke(this); }
        protected virtual void OnUpdateEditor() { UpdateEditor?.Invoke(this); }
        protected virtual void OnDisposing() { Disposing?.Invoke(this); }
        protected virtual void OnRenamed(string oldName) { Renamed?.Invoke(this, oldName); }
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

                Default def = info.GetCustomAttribute<Default>();
                if (def != null)
                {
#if EDITOR
                    return;
#endif
                }

                PreChanged pre = info.GetCustomAttribute<PreChanged>();
                if (pre != null)
                {
                    MethodInfo method = GetType().GetMethod(pre._methodName).MakeGenericMethod(info.PropertyType);
                    if (method != null)
                        method.Invoke(this, new object[] { currentValue });
                }

                args.Value = args.Value;
                args.ProceedSetValue();

                PostChanged post = info.GetCustomAttribute<PostChanged>();
                if (post != null)
                {
                    MethodInfo method = GetType().GetMethod(post._methodName).MakeGenericMethod(info.PropertyType);
                    if (method != null)
                        method.Invoke(this, new object[] { args.Value });
                }

                ((ObjectBase)args.Instance).OnPropertyChanged(args.Location.PropertyInfo, currentValue);
            }
        }
    }
}
