using EnumsNET;
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
    [TFileExt("pkc", ManualXmlConfigSerialize = true)]
    [TFileDef("Perspective Camera Key Collection")]
    public class PerspCamKeyCollection : TFileObject
    {
        public PerspCamKeyCollection() { }
        
        public float LengthInSeconds { get; private set; }
        
        public bool VerticalFOV { get; set; }
        public bool OverrideAspect { get; set; }

        public PropAnimFloat TranslationX => _tracks[0];
        public PropAnimFloat TranslationY => _tracks[1];
        public PropAnimFloat TranslationZ => _tracks[2];

        public PropAnimFloat RotationX => _tracks[3];
        public PropAnimFloat RotationY => _tracks[4];
        public PropAnimFloat RotationZ => _tracks[5];
        
        public PropAnimFloat FOV => _tracks[6];
        public PropAnimFloat Aspect => _tracks[7];
        public PropAnimFloat NearZ => _tracks[8];
        public PropAnimFloat FarZ => _tracks[9];

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

            new PropAnimFloat() { DefaultValue = 90.0f, TickSelf = false },  //fov
            new PropAnimFloat() { DefaultValue = 9.0f / 16.0f, TickSelf = false },  //aspect
            new PropAnimFloat() { DefaultValue = 0.1f, TickSelf = false },  //nearZ
            new PropAnimFloat() { DefaultValue = 1000.0f, TickSelf = false },  //farZ
        };

        public void SetLength(float seconds, bool stretchAnimation, bool notifyChanged = true)
        {
            LengthInSeconds = seconds;
            foreach (var track in _tracks)
                track.SetLength(seconds, stretchAnimation, notifyChanged);
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

            "FOV",
            "Aspect",
            "NearZ",
            "FarZ",
        };
        public override void ManualRead(SerializeElement node)
        {
            if (string.Equals(node.MemberInfo.Name, nameof(TransformKeyCollection), StringComparison.InvariantCulture))
            {
                if (node.GetAttributeValue(nameof(LengthInSeconds), out float length))
                    LengthInSeconds = length;
                else
                    LengthInSeconds = 0.0f;

                ResetKeys();

                foreach (SerializeElement targetTrackElement in node.Children)
                {
                    int trackIndex = TrackNames.IndexOf(targetTrackElement.Name.ToString());
                    if (!_tracks.IndexInRange(trackIndex))
                        continue;
                        
                    PropAnimFloat track = _tracks[trackIndex];
                    if (targetTrackElement.GetAttributeValue("Count", out int keyCount))
                    {
                        float[] seconds = null, inValues = null, outValues = null, inTans = null, outTans = null;
                        EVectorInterpType[] interpolation = null;

                        foreach (SerializeElement keyframePartElement in targetTrackElement.Children)
                        {
                            if (keyframePartElement.MemberInfo.Name == "Interpolation" && 
                                keyframePartElement.Content.GetObjectAs(out EVectorInterpType[] array1))
                            {
                                interpolation = array1;
                            }
                            else if (keyframePartElement.Content.GetObjectAs(out float[] array2))
                            {
                                switch (keyframePartElement.Name)
                                {
                                    case "Second": seconds = array2; break;
                                    case "InValues": inValues = array2; break;
                                    case "OutValues": outValues = array2; break;
                                    case "InTangents": inTans = array2; break;
                                    case "OutTangents": outTans = array2; break;
                                }
                            }
                        }
                        for (int i = 0; i < keyCount; ++i)
                        {
                            FloatKeyframe kf = new FloatKeyframe(
                                seconds[i],
                                inValues[i],
                                outValues[i],
                                inTans[i],
                                outTans[i],
                                interpolation[i]);
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
        public override void ManualWrite(SerializeElement node)
        {
            node.Name = nameof(TransformKeyCollection);
            node.AddAttribute(nameof(LengthInSeconds), LengthInSeconds);
            for (int i = 0; i < 9; ++i)
            {
                var track = _tracks[i];
                if (track.Keyframes.Count > 0)
                {
                    SerializeElement trackElement = new SerializeElement(null, new TSerializeMemberInfo(null, TrackNames[i]));
                    trackElement.AddAttribute("Count", track.Keyframes.Count);
                    trackElement.AddChildElementObject("Second", track.Select(x => x.Second).ToArray());
                    trackElement.AddChildElementObject("InValues", track.Select(x => x.InValue).ToArray());
                    trackElement.AddChildElementObject("OutValues", track.Select(x => x.OutValue).ToArray());
                    trackElement.AddChildElementObject("InTangents", track.Select(x => x.InTangent).ToArray());
                    trackElement.AddChildElementObject("OutTangents", track.Select(x => x.OutTangent).ToArray());
                    trackElement.AddChildElementObject("Interpolation", track.Select(x => x.InterpolationType).ToArray());
                    node.Children.Add(trackElement);
                }
            }
        }
    }
}
