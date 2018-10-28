﻿using EnumsNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Animation
{
    [FileExt("tkc", ManualXmlConfigSerialize = true)]
    [FileDef("Transform Key Collection")]
    public class TransformKeyCollection : TFileObject, IEnumerable<PropAnimFloat>
    {
        public TransformKeyCollection() { }

        public float LengthInSeconds { get; private set; }
        public TransformOrder TransformOrder { get; set; } = TransformOrder.TRS;
        public RotationOrder EulerOrder { get; set; } = RotationOrder.RYP;

        public PropAnimFloat TranslationX => _tracks[0];
        public PropAnimFloat TranslationY => _tracks[1];
        public PropAnimFloat TranslationZ => _tracks[2];
        
        public PropAnimFloat RotationX => _tracks[3];
        public PropAnimFloat RotationY => _tracks[4];
        public PropAnimFloat RotationZ => _tracks[5];
        
        public PropAnimFloat ScaleX => _tracks[6];
        public PropAnimFloat ScaleY => _tracks[7];
        public PropAnimFloat ScaleZ => _tracks[8];

        public PropAnimFloat this[int index]
        {
            get => _tracks.IndexInRange(index) ? _tracks[index] : null;
            set
            {
                if (_tracks.IndexInRange(index))
                    _tracks[index] = value;
            }
        }

        private PropAnimFloat[] _tracks = new PropAnimFloat[]
        {
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //tx
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //ty
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //tz
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //rx
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //ry
            new PropAnimFloat() { DefaultValue = 0.0f, TickSelf = false },  //rz
            new PropAnimFloat() { DefaultValue = 1.0f, TickSelf = false },  //sx
            new PropAnimFloat() { DefaultValue = 1.0f, TickSelf = false },  //sy
            new PropAnimFloat() { DefaultValue = 1.0f, TickSelf = false },  //sz
        };

        public void SetLength(float seconds, bool stretchAnimation)
        {
            LengthInSeconds = seconds;
            foreach (var track in _tracks)
                track.SetLength(seconds, stretchAnimation);
        }

        public void Progress(float delta) => _tracks.ForEach(x => x.Progress(delta));

        /// <summary>
        /// Retrieves the parts of the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe void GetTransform(Transform bindState,
            out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            Vec3
                bt = bindState.Translation.Raw,
                br = bindState.Rotation.PitchYawRoll,
                bs = bindState.Scale.Raw;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;
            for (int i = 0; i < 3; ++i, ++pbt)
            {
                var track = _tracks[i];
                *pt++ = track.Keyframes.Count == 0 ? *pbt : track.CurrentPosition;
            }
            for (int i = 3; i < 6; ++i, ++pbr)
            {
                var track = _tracks[i];
                *pr++ = track.Keyframes.Count == 0 ? *pbr : track.CurrentPosition;
            }
            for (int i = 6; i < 9; ++i, ++pbs)
            {
                var track = _tracks[i];
                *ps++ = track.Keyframes.Count == 0 ? *pbs : track.CurrentPosition;
            }

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
        }
        /// <summary>
        /// Retrieves the parts of the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe void GetTransform(Transform bindState, float second,
            out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            Vec3
                bt = bindState.Translation.Raw,
                br = bindState.Rotation.PitchYawRoll,
                bs = bindState.Scale.Raw;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;
            for (int i = 0; i < 3; ++i, ++pbt)
            {
                var track = _tracks[i];
                *pt++ = track.Keyframes.Count == 0 ? *pbt : track.GetValue(second);
            }
            for (int i = 3; i < 6; ++i, ++pbr)
            {
                var track = _tracks[i];
                *pr++ = track.Keyframes.Count == 0 ? *pbr : track.GetValue(second);
            }
            for (int i = 6; i < 9; ++i, ++pbs)
            {
                var track = _tracks[i];
                *ps++ = track.Keyframes.Count == 0 ? *pbs : track.GetValue(second);
            }

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
        }
        /// <summary>
        /// Retrieves the parts of the transform at the current frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe void GetTransform(out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;

            for (int i = 0; i < 3; ++i)
                *pt++ = _tracks[i].CurrentPosition;
            for (int i = 3; i < 6; ++i)
                *pr++ = _tracks[i].CurrentPosition;
            for (int i = 6; i < 9; ++i)
                *ps++ = _tracks[i].CurrentPosition;

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
        }
        /// <summary>
        /// Retrieves the parts of the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe void GetTransform(float second, out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;

            for (int i = 0; i < 3; ++i)
                *pt++ = _tracks[i].GetValue(second);
            for (int i = 3; i < 6; ++i)
                *pr++ = _tracks[i].GetValue(second);
            for (int i = 6; i < 9; ++i)
                *ps++ = _tracks[i].GetValue(second);

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
        }
        /// <summary>
        /// Retrieves the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe Transform GetTransform(Transform defaultTransform)
        {
            Vec3 t, r, s;
            Vec3
                bt = defaultTransform.Translation.Raw,
                br = defaultTransform.Rotation.PitchYawRoll,
                bs = defaultTransform.Scale.Raw;

            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;

            for (int i = 0; i < 3; ++i, ++pbt)
            {
                var track = _tracks[i];
                *pt++ = track.Keyframes.Count == 0 ? *pbt : track.CurrentPosition;
            }
            for (int i = 3; i < 6; ++i, ++pbr)
            {
                var track = _tracks[i];
                *pr++ = track.Keyframes.Count == 0 ? *pbr : track.CurrentPosition;
            }
            for (int i = 6; i < 9; ++i, ++pbs)
            {
                var track = _tracks[i];
                *ps++ = track.Keyframes.Count == 0 ? *pbs : track.CurrentPosition;
            }

            return new Transform(t, new Rotator(r, EulerOrder), s, TransformOrder);
        }
        /// <summary>
        /// Retrieves the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe Transform GetTransform(Transform defaultTransform, float second)
        {
            Vec3 t, r, s;
            Vec3
                bt = defaultTransform.Translation.Raw,
                br = defaultTransform.Rotation.PitchYawRoll,
                bs = defaultTransform.Scale.Raw;

            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;

            for (int i = 0; i < 3; ++i, ++pbt)
            {
                var track = _tracks[i];
                *pt++ = track.Keyframes.Count == 0 ? *pbt : track.GetValue(second);
            }
            for (int i = 3; i < 6; ++i, ++pbr)
            {
                var track = _tracks[i];
                *pr++ = track.Keyframes.Count == 0 ? *pbr : track.GetValue(second);
            }
            for (int i = 6; i < 9; ++i, ++pbs)
            {
                var track = _tracks[i];
                *ps++ = track.Keyframes.Count == 0 ? *pbs : track.GetValue(second);
            }

            return new Transform(t, new Rotator(r, EulerOrder), s, TransformOrder);
        }
        /// <summary>
        /// Retrieves the transform at the current frame second.
        /// </summary>
        public unsafe Transform GetTransform()
        {
            Vec3 t, r, s;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;

            for (int i = 0; i < 3; ++i)
                *pt++ = _tracks[i].CurrentPosition;
            for (int i = 3; i < 6; ++i)
                *pr++ = _tracks[i].CurrentPosition;
            for (int i = 6; i < 9; ++i)
                *ps++ = _tracks[i].CurrentPosition;

            return new Transform(t, new Rotator(r, EulerOrder), s, TransformOrder);
        }
        /// <summary>
        /// Retrieves the transform at the requested frame second.
        /// </summary>
        public unsafe Transform GetTransform(float second)
        {
            Vec3 t, r, s;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;

            for (int i = 0; i < 3; ++i)
                *pt++ = _tracks[i].GetValue(second);
            for (int i = 3; i < 6; ++i)
                *pr++ = _tracks[i].GetValue(second);
            for (int i = 6; i < 9; ++i)
                *ps++ = _tracks[i].GetValue(second);

            return new Transform(t, new Rotator(r, EulerOrder), s, TransformOrder);
        }
        public float?[] GetValues()
        {
            float?[] values = new float?[9];
            for (int i = 0; i < 3; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.CurrentPosition;
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.CurrentPosition;
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.CurrentPosition;
            }

            return values;
        }
        public float?[] GetValues(float second)
        {
            float?[] values = new float?[9];
            for (int i = 0; i < 3; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.GetValue(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.GetValue(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                values[i] = track.Keyframes.Count == 0 ? null : (float?)track.GetValue(second);
            }

            return values;
        }

        /// <summary>
        /// Clears all keyframes and sets tracks to the proper length.
        /// </summary>
        public void ResetKeys()
        {
            foreach (var track in _tracks)
            {
                track.Keyframes.Clear();
                track.SetLength(LengthInSeconds, false);
            }
        }
        private static string[] TrackNames { get; } = new string[]
        {
            "TranslationX",
            "TranslationY",
            "TranslationZ",
            "RotationX",
            "RotationY",
            "RotationZ",
            "ScaleX",
            "ScaleY",
            "ScaleZ",
        };
        public override void ManualRead(MemberTreeNode node)
        {
            if (string.Equals(node.MemberInfo.Name, nameof(TransformKeyCollection), StringComparison.InvariantCulture))
            {
                if (node.GetAttributeValue(nameof(LengthInSeconds), out float length))
                    LengthInSeconds = length;
                else
                    LengthInSeconds = 0.0f;

                ResetKeys();

                foreach (MemberTreeNode targetTrackElement in node.ChildElements)
                {
                    int trackIndex = TrackNames.IndexOf(targetTrackElement.Name.ToString());
                    if (!_tracks.IndexInRange(trackIndex))
                        continue;
                        
                    PropAnimFloat track = _tracks[trackIndex];
                    if (node.GetAttributeValue("Count", out int keyCount))
                    {
                        float[] seconds = null, inValues = null, outValues = null, inTans = null, outTans = null;
                        EPlanarInterpType[] interpolation = null;
                        string[] values;

                        foreach (MemberTreeNode keyframePartElement in targetTrackElement.ChildElements)
                        {
                            object o = keyframePartElement.ElementObject;
                            if (o is string str)
                            {
                                values = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                if (keyframePartElement.MemberInfo.Name == "Interpolation")
                                {
                                    interpolation = values.Select(x => Enums.Parse<EPlanarInterpType>(x)).ToArray();
                                }
                                else
                                {
                                    float[] floatValues = values.Select(x => float.Parse(x)).ToArray();
                                    switch (keyframePartElement.MemberInfo.Name)
                                    {
                                        case "Second": seconds = floatValues; break;
                                        case "InValues": inValues = floatValues; break;
                                        case "OutValues": outValues = floatValues; break;
                                        case "InTangents": inTans = floatValues; break;
                                        case "OutTangents": outTans = floatValues; break;
                                    }
                                }
                            }
                        }

                        for (int i = 0; i < keyCount; ++i)
                        {
                            FloatKeyframe kf = new FloatKeyframe(
                                float.Parse(seconds[i]),
                                float.Parse(inValues[i]),
                                float.Parse(outValues[i]),
                                float.Parse(inTans[i]),
                                float.Parse(outTans[i]),
                                Enums.Parse<EPlanarInterpType>(interpolation[i]));
                            track.Keyframes.Add(kf);
                        }
                    }
                    _tracks[trackIndex] = track;
                }
                
            }
            else
            {
                LengthInSeconds = 0;
                ResetKeys();
            }
        }
        public override async void ManualWrite(MemberTreeNode node)
        {
            node.Name = nameof(TransformKeyCollection);
            
            if (node is XMLMemberTreeNode xmlNode)
            {
                xmlNode.AddAttribute(nameof(LengthInSeconds), LengthInSeconds.ToString());
                for (int i = 0; i < 9; ++i)
                {
                    var track = _tracks[i];
                    if (track.Keyframes.Count > 0)
                    {
                        XMLMemberTreeNode trackElement = new XMLMemberTreeNode(TrackNames[i], new SerializeAttribute[] { new SerializeAttribute("Count", track.Keyframes.Count.ToString()) });
                        trackElement.AddChildElementString("Second", string.Join(",", track.Select(x => x.Second)));
                        trackElement.AddChildElementString("InValues", string.Join(",", track.Select(x => x.InValue)));
                        trackElement.AddChildElementString("OutValues", string.Join(",", track.Select(x => x.OutValue)));
                        trackElement.AddChildElementString("InTangents", string.Join(",", track.Select(x => x.InTangent)));
                        trackElement.AddChildElementString("OutTangents", string.Join(",", track.Select(x => x.OutTangent)));
                        trackElement.AddChildElementString("Interpolation", string.Join(",", track.Select(x => x.InterpolationType)));
                        await xmlNode.AddChildElementAsync(trackElement);
                    }
                }
            }
        }

        public IEnumerator<PropAnimFloat> GetEnumerator()
            => ((IEnumerable<PropAnimFloat>)_tracks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<PropAnimFloat>)_tracks).GetEnumerator();
    }
}
