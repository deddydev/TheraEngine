using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    [TFileExt("pat")]
    [TFileDef("Property Animation Tree")]
    public class AnimationTree : BaseAnimation
    {
        public AnimationTree()
            : base(0.0f, false) { }
        public AnimationTree(AnimationMember rootFolder)
            : this() => RootMember = rootFolder;
        
        public AnimationTree(string animationName, string memberName, EAnimationMemberType memberType, BasePropAnim anim) : this()
        {
            Name = animationName;

            string[] memberPath = memberName.Split('.');
            AnimationMember last = null;

            foreach (string childMemberName in memberPath)
            {
                AnimationMember member = new AnimationMember(childMemberName);

                if (last == null)
                    RootMember = member;
                else
                    last.Children.Add(member);

                last = member;
            }
            
            last?.SetAnimation(anim, memberType);
        }

        private int _totalAnimCount = 0;
        private AnimationMember _root;

        internal List<TObject> Owners { get; } = new List<TObject>();

        [TSerialize("EndedAnimations", Config = false, State = true)]
        private int _endedAnimations = 0;

        [TSerialize]
        public bool BeginOnSpawn { get; set; }

        [TSerialize]
        public AnimationMember RootMember
        {
            get => _root;
            set
            {
                _root = value;
                _totalAnimCount = _root != null ? _root.Register(this) : 0;
            }
        }

        private void OwnersModified()
        {
            if (Owners.Count == 0 && IsTicking)
                UnregisterTick(_group, _order, OnProgressed, _pausedBehavior);
            else if (_state == EAnimationState.Playing && Owners.Count != 0 && !IsTicking)
                RegisterTick(_group, _order, OnProgressed, _pausedBehavior);
        }
        internal void AnimationHasEnded(BaseAnimation obj)
        {
            if (++_endedAnimations >= _totalAnimCount)
                Stop();
        }
        public Dictionary<string, BasePropAnim> GetAllAnimations()
        {
            Dictionary<string, BasePropAnim> anims = new Dictionary<string, BasePropAnim>();
            _root.CollectAnimations(null, anims);
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
            foreach (TObject obj in Owners)
                _root?._tick(obj, delta);
        }
    }
}
