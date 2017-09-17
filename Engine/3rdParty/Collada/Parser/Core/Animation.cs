using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            internal List<AnimationClipEntry> _animationClips = new List<AnimationClipEntry>();
            internal List<AnimationEntry> _animations = new List<AnimationEntry>();

            private void ParseLibAnimationClips()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("animation_clip", true))
                        _animationClips.Add(ParseAnimationClip());
                    _reader.EndElement();
                }
            }

            private AnimationClipEntry ParseAnimationClip()
            {
                AnimationClipEntry clip = new AnimationClipEntry();

                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("id", true))
                    {
                        clip._id = _reader.Value;
                        AddIdEntry(clip);
                    }
                    else if (_reader.Name.Equals("name", true))
                        clip._name = _reader.Value;
                    else if (_reader.Name.Equals("start", true))
                        clip._start = float.Parse(_reader.Value);
                    else if (_reader.Name.Equals("end", true))
                        clip._end = float.Parse(_reader.Value);
                }

                return clip;
            }

            private void ParseLibAnimations()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("animation", true))
                        _animations.Add(ParseAnimation());
                    _reader.EndElement();
                }
            }

            private AnimationEntry ParseAnimation()
            {
                AnimationEntry anim = new AnimationEntry();
                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("id", true))
                    {
                        anim._id = _reader.Value;
                        AddIdEntry(anim);
                    }
                    else if (_reader.Name.Equals("name", true))
                        anim._name = _reader.Value;
                }
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("animation", true))
                        anim._animations.Add(ParseAnimation());
                    else if (_reader.Name.Equals("source", true))
                        anim._sources.Add(ParseSource());
                    else if (_reader.Name.Equals("sampler", true))
                        anim._samplers.Add(ParseSampler());
                    else if (_reader.Name.Equals("channel", true))
                        anim._channels.Add(ParseChannel());
                    _reader.EndElement();
                }
                return anim;
            }
            private ChannelEntry ParseChannel()
            {
                ChannelEntry entry = new ChannelEntry();
                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("source", true))
                        entry._source = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;
                    else if (_reader.Name.Equals("target", true))
                        entry._target = _reader.Value;
                }
                return entry;
            }
            private SamplerEntry ParseSampler()
            {
                SamplerEntry entry = new SamplerEntry();
                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("id", true))
                    {
                        entry._id = _reader.Value;
                        AddIdEntry(entry);
                    }
                    else if (_reader.Name.Equals("pre_behavior", true))
                        entry._preBehavior = (SamplerBehavior)Enum.Parse(typeof(SamplerBehavior), _reader.Value);
                    else if (_reader.Name.Equals("post_behavior", true))
                        entry._postBehavior = (SamplerBehavior)Enum.Parse(typeof(SamplerBehavior), _reader.Value);
                }
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("input", true))
                        entry._inputs.Add(ParseInput());
                    _reader.EndElement();
                }
                return entry;
            }
        }
        private class AnimationClipEntry : BaseColladaElement
        {
            public float _start, _end;
            public List<InstanceEntry> _animationInstances = new List<InstanceEntry>();
        }
        private class AnimationEntry : BaseColladaElement
        {
            public List<SourceEntry> _sources = new List<SourceEntry>();
            public List<SamplerEntry> _samplers = new List<SamplerEntry>();
            public List<ChannelEntry> _channels = new List<ChannelEntry>();
            public List<AnimationEntry> _animations = new List<AnimationEntry>();
        }
        private enum SamplerBehavior
        {
            UNDEFINED, //Default value. The before and after behaviors are not defined.  
            CONSTANT, //The value for the first (behavior_before) or last (behavior_after) is returned  
            GRADIENT, //The value follows the line given by the last two keys in the sample. (Same as LINEAR in Maya®.)
            CYCLE, //The key is mapped in the [first_key, last_key] interval so that the animation cycles.
            OSCILLATE, //The key is mapped in the [first_key, last_key] interval so that the animation oscillates.
            CYCLE_RELATIVE, //The animation continues indefinitely.
        }
        private class SamplerEntry : BaseColladaElement
        {
            public SamplerBehavior _preBehavior, _postBehavior;
            public List<InputEntry> _inputs = new List<InputEntry>();
        }
        private class ChannelEntry : BaseColladaElement
        {
            public string _source, _target;
        }
    }
}