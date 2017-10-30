using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Animation
{
    public class TransformKeyCollection : IEnumerable<KeyframeTrack<FloatKeyframe>>
    {
        public TransformKeyCollection()
        {

        }

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
            foreach (var track in _tracks)
                track.SetLength(seconds, stretchAnimation);
        }

        /// <summary>
        /// Retrieves the parts of the transform for this bone at the requested frame second.
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

        public IEnumerator<KeyframeTrack<FloatKeyframe>> GetEnumerator()
            => ((IEnumerable<KeyframeTrack<FloatKeyframe>>)_tracks).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable<KeyframeTrack<FloatKeyframe>>)_tracks).GetEnumerator();
    }
}
