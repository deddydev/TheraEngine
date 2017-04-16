using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Animation
{
    public class ModelAnimation : FileObject
    {

    }
    public class BoneAnimation
    {
        public string _name;
        public KeyframeTrack<RotatorKeyframe> _rotation;
        public KeyframeTrack<Vec3Keyframe> _translation;
        public KeyframeTrack<Vec3Keyframe> _scale;
    }
}
