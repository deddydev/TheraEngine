using System;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Collections;
using CustomEngine.Components;

namespace CustomEngine.Rendering.Animation
{
    public abstract class AnimationContainer
    {
        event EventHandler AnimationStarted;
        event EventHandler AnimationEnded;

        public class SubPropertyNode
        {
            public string _propertyName;
            public List<SubPropertyNode> _children;
            public IPropertyAnimation _animation;

            public void CollectAnimations(string path, Dictionary<string, IPropertyAnimation> animations)
            {
                if (String.IsNullOrEmpty(path))
                    path = _propertyName;
                else
                    path += ".{_propertyName}";
                if (_animation != null)
                    animations.Add(path, _animation);
                foreach (SubPropertyNode p in _children)
                    p.CollectAnimations(path, animations);
            }
        }

        SubPropertyNode _root;
        int _frameCount;
        public double _currentFrame;
        bool _isPlaying = false;
        bool _looping = true;
        bool _independentFrameCounts;
        public List<IPropertyAnimation> _properties;

        public int FrameCount
        {
            get { return _frameCount; }
            set
            {
                _frameCount = value;
            }
        }

        public Dictionary<string, IPropertyAnimation> GetAllAnimations()
        {
            Dictionary<string, IPropertyAnimation> anims = new Dictionary<string, IPropertyAnimation>();
            _root.CollectAnimations(null, anims);
            return anims;
        }

        public void Play()
        {
            AnimationStarted?.Invoke(this, EventArgs.Empty);
        }
        public void Stop()
        {
            AnimationEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
