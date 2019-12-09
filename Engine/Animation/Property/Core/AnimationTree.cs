using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Animation
{
    public enum EAnimTreeTraversalMethod
    {
        /// <summary>
        /// All members are animated at the same time.
        /// </summary>
        Parallel,
        /// <summary>
        /// Members are animated sequentially in order of appearance, parent-down.
        /// Root-Children-Grandchildren-Etc
        /// </summary>
        BreadthFirst,
        /// <summary>
        /// Left-Root-Right
        /// </summary>
        DepthFirstInOrder,
        /// <summary>
        /// Left-Right-Root
        /// </summary>
        DepthFirstPreOrder,
        /// <summary>
        /// Root-Left-Right
        /// </summary>
        DepthFirstPostOrder
    }

    [TFileExt("pat")]
    [TFileDef("Property Animation Tree")]
    public class AnimationTree : BaseAnimation
    {
        [Browsable(true)]
        [Category("Object")]
        [TSerialize]
        public override string Name
        {
            get => base.Name;
            set => base.Name = value;
        }

        [Browsable(true)]
        [Category(AnimCategory)]
        [TSerialize]
        public EAnimTreeTraversalMethod TraversalMethod { get; set; } = EAnimTreeTraversalMethod.Parallel;

        public AnimationTree()
            : base(0.0f, false) { }
        public AnimationTree(AnimationMember rootFolder)
            : this() => RootMember = rootFolder;

        public AnimationTree(string animationName, string memberPath, BasePropAnim anim) : this()
        {
            Name = animationName;

            string[] memberPathParts = memberPath.Split('.');
            AnimationMember last = null;

            foreach (string childMemberName in memberPathParts)
            {
                AnimationMember member = new AnimationMember(childMemberName);

                if (last is null)
                    RootMember = member;
                else
                    last.Children.Add(member);

                last = member;
            }

            LengthInSeconds = anim.LengthInSeconds;
            Looped = anim.Looped;
            if (last != null)
                last.Animation.File = anim;
        }

        private int _totalAnimCount = 0;
        private AnimationMember _root;

        internal List<IObject> Owners { get; } = new List<IObject>();

        [TSerialize("EndedAnimations", Config = false, State = true)]
        private int _endedAnimations = 0;

        [TSerialize(nameof(RemoveOnEnd))]
        private bool _removeOnEnd;
        [TSerialize(nameof(BeginOnSpawn))]
        private bool _beginOnSpawn;

        [Category(AnimCategory)]
        public bool RemoveOnEnd
        {
            get => _removeOnEnd;
            set => SetBackingField(ref _removeOnEnd, value);
        }
        [Category(AnimCategory)]
        public bool BeginOnSpawn 
        {
            get => _beginOnSpawn;
            set => SetBackingField(ref _beginOnSpawn, value);
        }

        [TSerialize]
        public AnimationMember RootMember
        {
            get => _root;
            set
            {
                _root?.Unregister(this);
                _root = value;
                _totalAnimCount = _root?.Register(this) ?? 0;
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
            foreach (IObject obj in Owners)
                _root?._tick(obj, delta);
        }
    }
}
