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
    [FileClass("tkc", "Transform Key Collection", ManualXmlConfigSerialize = true)]
    public class TransformKeyCollection : FileObject, IEnumerable<KeyframeTrack<FloatKeyframe>>
    {
        public TransformKeyCollection() { }

        public float LengthInSeconds { get; private set; }
        public TransformOrder TransformOrder { get; set; } = TransformOrder.TRS;
        public RotationOrder EulerOrder { get; set; } = RotationOrder.RYP;

        public KeyframeTrack<FloatKeyframe> TranslationX => _tracks[0];
        public KeyframeTrack<FloatKeyframe> TranslationY => _tracks[1];
        public KeyframeTrack<FloatKeyframe> TranslationZ => _tracks[2];
        
        public KeyframeTrack<FloatKeyframe> RotationX => _tracks[3];
        public KeyframeTrack<FloatKeyframe> RotationY => _tracks[4];
        public KeyframeTrack<FloatKeyframe> RotationZ => _tracks[5];
        
        public KeyframeTrack<FloatKeyframe> ScaleX => _tracks[6];
        public KeyframeTrack<FloatKeyframe> ScaleY => _tracks[7];
        public KeyframeTrack<FloatKeyframe> ScaleZ => _tracks[8];

        public KeyframeTrack<FloatKeyframe> this[int index]
        {
            get => _tracks.IndexInRange(index) ? _tracks[index] : null;
            set
            {
                if (_tracks.IndexInRange(index))
                    _tracks[index] = value;
            }
        }

        private KeyframeTrack<FloatKeyframe>[] _tracks = new KeyframeTrack<FloatKeyframe>[]
        {
            new KeyframeTrack<FloatKeyframe>(), //tx
            new KeyframeTrack<FloatKeyframe>(), //ty
            new KeyframeTrack<FloatKeyframe>(), //tz
            new KeyframeTrack<FloatKeyframe>(), //rx
            new KeyframeTrack<FloatKeyframe>(), //ry
            new KeyframeTrack<FloatKeyframe>(), //rz
            new KeyframeTrack<FloatKeyframe>(), //sx
            new KeyframeTrack<FloatKeyframe>(), //sy
            new KeyframeTrack<FloatKeyframe>(), //sz
        };

        public void SetLength(float seconds, bool stretchAnimation)
        {
            LengthInSeconds = seconds;
            foreach (var track in _tracks)
                track.SetLength(seconds, stretchAnimation);
        }

        /// <summary>
        /// Retrieves the parts of the transform at the requested frame second.
        /// Uses the defaultTransform for tracks that have no keys.
        /// </summary>
        public unsafe void GetTransform(Transform bindState, float second, out Vec3 translation, out Rotator rotation, out Vec3 scale)
        {
            Vec3 t, r, s;
            Vec3
                bt = bindState.Translation.Raw,
                br = bindState.Rotation.RawPitchYawRoll,
                bs = bindState.Scale.Raw;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;
            for (int i = 0; i < 3; ++i)
            {
                var track = _tracks[i];
                *pt++ = track.First == null ? pbt[i] : track.First.Interpolate(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                *pr++ = track.First == null ? pbr[i] : track.First.Interpolate(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                *ps++ = track.First == null ? pbs[i] : track.First.Interpolate(second);
            }

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
            {
                var track = _tracks[i];
                *pt++ = track.First == null ? 0.0f : track.First.Interpolate(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                *pr++ = track.First == null ? 0.0f : track.First.Interpolate(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                *ps++ = track.First == null ? 1.0f : track.First.Interpolate(second);
            }

            translation = t;
            rotation = new Rotator(r, EulerOrder);
            scale = s;
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
                br = defaultTransform.Rotation.RawPitchYawRoll,
                bs = defaultTransform.Scale.Raw;
            float* pt = (float*)&t;
            float* pr = (float*)&r;
            float* ps = (float*)&s;
            float* pbt = (float*)&bt;
            float* pbr = (float*)&br;
            float* pbs = (float*)&bs;
            for (int i = 0; i < 3; ++i)
            {
                var track = _tracks[i];
                *pt++ = track.First == null ? pbt[i] : track.First.Interpolate(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                *pr++ = track.First == null ? pbr[i] : track.First.Interpolate(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                *ps++ = track.First == null ? pbs[i] : track.First.Interpolate(second);
            }

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
            {
                var track = _tracks[i];
                *pt++ = track.First == null ? 0.0f : track.First.Interpolate(second);
            }
            for (int i = 3; i < 6; ++i)
            {
                var track = _tracks[i];
                *pr++ = track.First == null ? 0.0f : track.First.Interpolate(second);
            }
            for (int i = 6; i < 9; ++i)
            {
                var track = _tracks[i];
                *ps++ = track.First == null ? 1.0f : track.First.Interpolate(second);
            }

            return new Transform(t, new Rotator(r, EulerOrder), s, TransformOrder);
        }

        /// <summary>
        /// Clears all keyframes and sets tracks to the proper length.
        /// </summary>
        public void ResetKeys()
        {
            foreach (var track in _tracks)
            {
                track.Clear();
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
            if (reader.BeginElement() && reader.Name.Equals("TransformKeyCollection", false))
            {
                if (reader.ReadAttribute() && reader.Name.Equals("LengthInSeconds", false))
                    LengthInSeconds = float.TryParse(reader.Value, out float length) ? length : 0.0f;

                ResetKeys();

                while (reader.BeginElement())
                {
                    int trackIndex = names.IndexOf(reader.Name);
                    if (_tracks.IndexInRange(trackIndex))
                    {
                        KeyframeTrack<FloatKeyframe> track = _tracks[trackIndex];
                        
                        int keyCount = 0;
                        if (reader.ReadAttribute() && reader.Name.Equals("Count", false) && !int.TryParse(reader.Value, out keyCount))
                            keyCount = 0;

                        if (keyCount > 0)
                        {
                            string[] seconds = null, inValues = null, outValues = null, inTans = null, outTans = null, interpolation = null;
                            while (reader.BeginElement())
                            {
                                switch ((string)reader.Name)
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
                            FloatKeyframe prev = null, first = null;
                            for (int i = 0; i < keyCount; ++i)
                            {
                                FloatKeyframe kf = new FloatKeyframe(
                                    float.Parse(seconds[i]),
                                    float.Parse(inValues[i]),
                                    float.Parse(outValues[i]),
                                    float.Parse(inTans[i]),
                                    float.Parse(outTans[i]),
                                    Enums.Parse<PlanarInterpType>(interpolation[i]));
                                if (prev != null)
                                    prev.Link(kf);
                                else
                                    first = kf;
                                prev = kf;
                            }
                            track.Add(first);
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

        protected internal override void Write(XmlWriter writer)
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
            writer.WriteStartElement("TransformKeyCollection");
            {
                writer.WriteAttributeString("LengthInSeconds", LengthInSeconds.ToString());
                for (int i = 0; i < 9; ++i)
                {
                    var track = _tracks[i];
                    if (track.KeyCount > 0)
                    {
                        writer.WriteStartElement(names[i]);
                        {
                            writer.WriteAttributeString("Count", track.KeyCount.ToString());
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

        public IEnumerator<KeyframeTrack<FloatKeyframe>> GetEnumerator()
            => ((IEnumerable<KeyframeTrack<FloatKeyframe>>)_tracks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<KeyframeTrack<FloatKeyframe>>)_tracks).GetEnumerator();
    }
}
