using TheraEngine.Core.Files;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Input.Devices;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Animation.Property;

namespace TheraEngine.Animation
{
    [TFileExt("animtree")]
    [TFileDef("Property Animation Tree")]
    public class AnimationContainer : BaseAnimation
    {
        private int _totalAnimCount = 0;
        private AnimFolder _root;

        internal List<TObject> Owners { get; } = new List<TObject>();
        
        [TSerialize("EndedAnimations")]
        private int _endedAnimations = 0;
        
        //public AnimationContainer(Action<bool> func, PropAnimBool anim) : this()
        //{

        //}
        //public AnimationContainer(Action<string> func, PropAnimString anim) : this()
        //{

        //}
        //public AnimationContainer(Action<float> func, PropAnimFloat anim) : this()
        //{

        //}

        public AnimationContainer() : base(0.0f, false) { }
        public AnimationContainer(AnimFolder rootFolder) : this()
        {
            RootFolder = rootFolder;
        }
        public AnimationContainer(string animationName, string propertyName, bool method, BasePropAnim anim) : this()
        {
            Name = animationName;
            string[] parts = propertyName.Split('.');
            bool first = true;
            AnimFolder last = null;
            foreach (string i in parts)
            {
                if (first)
                {
                    last = RootFolder = new AnimFolder(i);
                    first = false;
                }
                else
                {
                    AnimFolder folder = new AnimFolder(i);
                    last.Children.Add(folder);
                    last = folder;
                }
            }
            if (last != null)
                last.SetAnimation(anim, method);
        }
        private void OwnersModified()
        {
            if (Owners.Count == 0 && IsTicking)
                UnregisterTick(_group, _order, OnProgressed, _pausedBehavior);
            else if (_state == EAnimationState.Playing && Owners.Count != 0 && !IsTicking)
                RegisterTick(_group, _order, OnProgressed, _pausedBehavior);
        }
        [TSerialize]
        public AnimFolder RootFolder
        {
            get => _root;
            set
            {
                _root = value;
                _totalAnimCount = _root != null ? _root.Register(this) : 0;
            }
        }
        internal void AnimationHasEnded(BaseAnimation obj)
        {
            if (++_endedAnimations >= _totalAnimCount)
                Stop();
        }
        public Dictionary<string, BasePropAnim> GetAllAnimations()
        {
            Dictionary<string, BasePropAnim> anims = new Dictionary<string, BasePropAnim>();
            _root.CollectAnimations("", anims);
            return anims;
        }
        protected override void PostStarted()
        {
            _root?.StartAnimations();
        }
        protected override void PostStopped()
        {
            if (_endedAnimations < _totalAnimCount)
                _root?.StopAnimations();
        }
        protected override void OnProgressed(float delta)
        {
            foreach (TObject b in Owners)
                _root?._tick(b, delta);
        }
    }
}
