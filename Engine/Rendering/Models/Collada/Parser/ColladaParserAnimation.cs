using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            private List<AnimationClipEntry> _animationClips = new List<AnimationClipEntry>();
            private List<AnimationEntry> _animations = new List<AnimationEntry>();

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
                        clip._id = (string)_reader.Value;
                    else if (_reader.Name.Equals("name", true))
                        clip._name = (string)_reader.Value;
                    else if (_reader.Name.Equals("start", true))
                        clip._start = float.Parse((string)_reader.Value);
                    else if (_reader.Name.Equals("end", true))
                        clip._end = float.Parse((string)_reader.Value);
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
                AnimationEntry clip = new AnimationEntry();



                return clip;
            }

            private class AnimationClipEntry : ColladaEntry
            {
                public float _start, _end;
                public List<InstanceEntry> _animationInstances = new List<InstanceEntry>();
            }
            private class AnimationEntry : ColladaEntry
            {
                List<SourceEntry> _sources = new List<SourceEntry>();
            }
            private class SamplerEntry : ColladaEntry
            {

            }
        }
    }
}