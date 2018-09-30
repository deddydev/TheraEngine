using EnumsNET;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using TheraEngine.Files;
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

        protected internal override void Read(XMLReader reader)
        {
            string[] names =
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
            if (string.Equals(reader.Name, nameof(TransformKeyCollection), StringComparison.InvariantCulture))
            {
                if (reader.ReadAttribute() && string.Equals(reader.Name, nameof(LengthInSeconds), StringComparison.InvariantCulture))
                    LengthInSeconds = float.TryParse(reader.Value, out float length) ? length : 0.0f;

                ResetKeys();

                while (reader.BeginElement())
                {
                    int trackIndex = names.IndexOf(reader.Name.ToString());
                    if (_tracks.IndexInRange(trackIndex))
                    {
                        PropAnimFloat track = _tracks[trackIndex];
                        bool read = reader.ReadAttribute();
                        if (read &&
                            string.Equals(reader.Name, "Count", StringComparison.InvariantCulture) &&
                            int.TryParse(reader.Value, out int keyCount))
                        {
                            string[] seconds = null, inValues = null, outValues = null, inTans = null, outTans = null, interpolation = null;
                            while (reader.BeginElement())
                            {
                                switch (reader.Name)
                                {
                                    case "Second":
                                        seconds = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                    case "InValues":
                                        inValues = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                    case "OutValues":
                                        outValues = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                    case "InTangents":
                                        inTans = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                    case "OutTangents":
                                        outTans = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                    case "Interpolation":
                                        interpolation = reader.ReadElementString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                                        break;
                                }
                                reader.EndElement();
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
                    reader.EndElement();
                }
            }
            else
            {
                LengthInSeconds = 0;
                ResetKeys();
            }
        }

        protected internal override void Write(XmlWriter writer, ESerializeFlags flags)
        {
            string[] names = 
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
            writer.WriteStartElement(nameof(TransformKeyCollection));
            {
                writer.WriteAttributeString(nameof(LengthInSeconds), LengthInSeconds.ToString());
                for (int i = 0; i < 9; ++i)
                {
                    var track = _tracks[i];
                    if (track.Keyframes.Count > 0)
                    {
                        writer.WriteStartElement(names[i]);
                        {
                            writer.WriteAttributeString("Count", track.Keyframes.Count.ToString());
                            writer.WriteElementString("Second", string.Join(",", track.Select(x => x.Second)));
                            writer.WriteElementString("InValues", string.Join(",", track.Select(x => x.InValue)));
                            writer.WriteElementString("OutValues", string.Join(",", track.Select(x => x.OutValue)));
                            writer.WriteElementString("InTangents", string.Join(",", track.Select(x => x.InTangent)));
                            writer.WriteElementString("OutTangents", string.Join(",", track.Select(x => x.OutTangent)));
                            writer.WriteElementString("Interpolation", string.Join(",", track.Select(x => x.InterpolationType)));
                        }
                        writer.WriteEndElement();
                    }
                }
            }
            writer.WriteEndElement();
        }

        public IEnumerator<PropAnimFloat> GetEnumerator()
            => ((IEnumerable<PropAnimFloat>)_tracks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<PropAnimFloat>)_tracks).GetEnumerator();
    }
}
