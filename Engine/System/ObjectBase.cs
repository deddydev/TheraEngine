using CustomEngine;
using CustomEngine.Rendering;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using PostSharp.Aspects;
using PostSharp.Serialization;
using CustomEngine.Rendering.Animation;

namespace System
{
    public delegate void ResourceEventHandler(ObjectBase node);
    public delegate void RenamedEventHandler(ObjectBase node, string oldName);
    public delegate void ObjectPropertyChangedEventHandler(object sender, PropertyChangedEventArgs e);
    public enum TickGroup
    {
        PrePhysics = 0,
        PostPhysics = 1,
        DuringPhysics = 2,
    }
    public enum TickOrder
    {
        Timers = 0, //Call timing events
        Input = 1, //Call input events
        Logic = 2, //Call update tick
        Scene = 3, //Render scene
    }
    [NotifyPropertyChanged]
    public class ObjectBase : INotifyPropertyChanged
    {
        //public static List<ObjectBase> _changedObjects = new List<ObjectBase>();

        public event PropertyChangedEventHandler PropertyChanged;
        public event RenamedEventHandler Renamed;
        public event ResourceEventHandler Disposing, UpdateProperties, UpdateEditor;

        protected string _name;
        protected bool _changed;
        protected TickGroup? _tickGroup = null;
        protected TickOrder? _tickOrder = null;

        [Browsable(false)]
        public virtual ResourceType ResourceType { get { return ResourceType.Object; } }

        public AbstractRenderer Renderer { get { return Engine.Renderer; } }

        [Default]
#if EDITOR
        [Category("State"), PostChanged("OnRenamed")]
#endif
        public string Name
        {
            get { return _name; }
#if EDITOR
            set { _name = value; }
#endif
        }

#if EDITOR
        [Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
#endif
        public TickGroup? TickGroup
        {
            get { return _tickGroup; }
#if EDITOR
            set { _tickGroup = value; }
#endif
        }
        [Category("Tick"), PreChanged("UnregisterTick"), PostChanged("RegisterTick")]
        public TickOrder? TickOrder
        {
            get { return _tickOrder; }
            set { _tickOrder = value; }
        }

#if EDITOR
        /// <summary>
        /// 
        /// </summary>
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
#endif

        /// <summary>
        /// Specifies that this object wants tick calls.
        /// </summary>
        public void RegisterTick() { Engine.RegisterTick(this); }
        /// <summary>
        /// Specifies that this object will not have any tick calls.
        /// </summary>
        public void UnregisterTick() { Engine.UnregisterTick(this); }
        /// <summary>
        /// Updates logic for this class
        /// </summary>
        /// <param name="delta">The amount of time that has passed since the last tick update</param>
        internal virtual void Tick(float delta)
        {
            foreach (AnimationContainer anim in _animations)
                anim.Tick(delta, this);
        }

        public void OnPropertyChanged(PropertyInfo info, object previousValue)
        {
            if (info.Name == "_changed")
                return;

            string output = "Changed property " + info.Name + " in " + GetType().ToString() + " \"" + Name + "\"";
            Console.WriteLine(output);

            _changed = true;
            //_changedObjects.Add(this);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info.Name));
        }
        protected virtual void OnUpdateProperties() { UpdateProperties?.Invoke(this); }
        protected virtual void OnUpdateEditor() { UpdateEditor?.Invoke(this); }
        protected virtual void OnDisposing() { Disposing?.Invoke(this); }
        protected virtual void OnRenamed(string oldName) { Renamed?.Invoke(this, oldName); }

        private List<AnimationContainer> _animations = new List<AnimationContainer>();
        public void AddAnimation(AnimationContainer anim)
        {
            anim.AnimationEnded += Anim_AnimationEnded;
            _animations.Add(anim);
        }

        private void Anim_AnimationEnded(object sender, EventArgs e)
        {
            RemoveAnimation(sender as AnimationContainer);
        }

        public void RemoveAnimation(AnimationContainer anim)
        {
            _animations.Remove(anim);
        }
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
