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
    public class TransformTool : Actor
    {
        public TransformTool(Actor modified)
        {
            _modified = modified;
        }

        private TransformType _mode = TransformType.Translate;
        private Actor _modified = null;

        public TransformType Mode { get { return _mode; } set { _mode = value; } }
        public Actor ModifiedActor { get { return _modified; } set { _modified = value; } }
    }
}
