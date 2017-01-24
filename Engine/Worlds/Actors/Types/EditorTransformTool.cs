using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Types
{
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate
    }
    public class EditorTransformTool : Actor
    {
        public EditorTransformTool(SceneComponent modified)
        {
            _modified = modified;
        }

        private TransformType _mode = TransformType.Translate;
        private SceneComponent _modified = null;
        
        public TransformType Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        public SceneComponent ModifiedComponent
        {
            get { return _modified; }
            set { _modified = value; }
        }
    }
}
