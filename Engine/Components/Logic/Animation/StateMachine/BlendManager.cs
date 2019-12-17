using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Animation;
using TheraEngine.Core.Maths;
using TheraEngine.Rendering.Models;

namespace TheraEngine.Components.Logic.Animation
{
    public enum EAnimBlendType
    {
        Linear,             //start + (end - start) * time
        CosineEaseInOut,    //start + (end - start) * (1.0f - cos(time)) / 2.0f
        QuadraticEaseStart, //start + (end - start) * time^power
        QuadraticEaseEnd,   //start + (end - start) * (1.0f - (1.0f - time)^power)
        Custom,             //start + (end - start) * customInterp(time)
    }
    public class BlendManager
    {
        private class BlendInfo
        {
            /// <summary>
            /// How long the blend lasts in seconds.
            /// </summary>
            [TSerialize(IsAttribute = true)]
            public float Duration
            {
                get => _invDuration == 0.0f ? 0.0f : 1.0f / _invDuration;
                set => _invDuration = value == 0.0f ? 0.0f : 1.0f / value;
            }
            /// <summary>
            /// How long until the blend is done in seconds.
            /// </summary>
            [TSerialize(State = true)]
            public float ElapsedTime { get; private set; }
            /// <summary>
            /// The blending method to use.
            /// </summary>
            [TSerialize]
            public EAnimBlendType Type
            {
                get => _blendType;
                set
                {
                    _blendType = value;
                    switch (_blendType)
                    {
                        default:
                        case EAnimBlendType.Linear:
                            _blendFunction = (time) => time;
                            break;
                        case EAnimBlendType.CosineEaseInOut:
                            _blendFunction = (time) => Interp.Cosine(0.0f, 1.0f, time);
                            break;
                        case EAnimBlendType.QuadraticEaseStart:
                            _blendFunction = (time) => Interp.QuadraticEaseStart(0.0f, 1.0f, time);
                            break;
                        case EAnimBlendType.QuadraticEaseEnd:
                            _blendFunction = (time) => Interp.QuadraticEaseEnd(0.0f, 1.0f, time);
                            break;
                        case EAnimBlendType.Custom:
                            _blendFunction = (time) => _customBlendFunction.First.Interpolate(time, EVectorInterpValueType.Position);
                            break;
                    }
                }
            }

            private AnimStateTransition _transition;
            private float _invDuration;
            private EAnimBlendType _blendType;
            private Func<float, float> _blendFunction;
            [TSerialize("CustomFunction", Order = 0)]
            private KeyframeTrack<FloatKeyframe> _customBlendFunction;

            public BlendInfo() { }
            public BlendInfo(AnimStateTransition transition)
            {
                _transition = transition;
                Duration = transition.BlendDuration;
                ElapsedTime = 0.0f;
                Type = transition.BlendType;
                _customBlendFunction = transition.CustomBlendFunction;
            }

            /// <summary>
            /// Increments the blending time.
            /// Returns true if the blend is done.
            /// </summary>
            internal bool Tick(float delta) => (ElapsedTime += delta) > Duration;
            
            //Multiplying is faster than division, store duration as inverse
            /// <summary>
            /// Returns a value from 0.0f - 1.0f indicating a linear time between two animations.
            /// </summary>
            public float GetBlendTime() => _blendFunction(_invDuration == 0.0f ? 1.0f : (ElapsedTime * _invDuration).ClampMax(1.0f));

            internal void OnStarted()
            {
                _transition?.OnStarted();
            }
            internal void OnFinished()
            {
                _transition?.OnFinished();
            }
        }

        private LinkedList<AnimState> _stateQueue;
        private LinkedList<BlendInfo> _blendQueue;

        public BlendManager(AnimState initialState)
        {
            _blendQueue = new LinkedList<BlendInfo>();
            _stateQueue = new LinkedList<AnimState>();
            if (initialState != null)
            {
                _stateQueue.AddFirst(initialState);
                //initialState.Start();
            }
        }

        public void QueueState(
            AnimState destinationState,
            AnimStateTransition transition)
        {
            if (destinationState is null)
                return;
            //destinationState.Start();
            _stateQueue.AddLast(destinationState);
            if (_stateQueue.Count > 1)
            {
                BlendInfo blend = new BlendInfo(transition);
                blend.OnStarted();
                _blendQueue.AddLast(blend);
            }
        }
        internal void Tick(float delta, Skeleton skeleton)
        {
            if (skeleton is null)
                return;

            var stateNode = _stateQueue.First;

            //Tick first animation
            AnimState anim = stateNode?.Value;
            SkeletalAnimationPose baseFrame = null;
            if (anim != null)
            {
                baseFrame = anim.GetFrame();
                anim.Tick(delta);
            }

            if (baseFrame is null)
                return;

            if (_blendQueue.Count > 0)
            {
                //Execute all blends on top of the first animation's frame
                var blendNode = _blendQueue.First;
                while (blendNode != null)
                {
                    //Get the blend info for the current animation and the next
                    BlendInfo blend = blendNode.Value;
                    
                    //Tick the animation to be blended with next
                    stateNode = stateNode.Next;
                    anim = stateNode?.Value;
                    if (anim != null)
                    {
                        //Update blending information
                        if (blend.Tick(delta))
                        {
                            //Frame is now entirely the next animation.
                            baseFrame = anim.GetFrame();

                            //All previous animations are now irrelevant. Remove them
                            while (_stateQueue.First != stateNode)
                            {
                                _stateQueue.First.Value.OnEnded();
                                _stateQueue.RemoveFirst();
                            }
                            //Remove all previous blends
                            while (_blendQueue.First != blendNode)
                            {
                                _blendQueue.First.Value.OnFinished();
                                _blendQueue.RemoveFirst();
                            }
                            //And this one
                            _blendQueue.First.Value.OnFinished();
                            _blendQueue.RemoveFirst();

                            //Get next blend info
                            blendNode = _blendQueue.First;
                        }
                        else
                        {
                            //Update frame by blending it with the next animation using the current blending time
                            baseFrame?.BlendWith(stateNode.Value.GetFrame(), blend.GetBlendTime());

                            //Increment to next blend info
                            blendNode = blendNode.Next;
                        }

                        anim.Tick(delta);
                    }
                }
            }

            //Update the skeleton with the final blended frame
            baseFrame.UpdateSkeleton(skeleton);
        }
    }
}
