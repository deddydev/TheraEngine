using BulletSharp;
using System.ComponentModel;

namespace TheraEngine.Physics.Bullet
{
    public class BulletGhostBody : TGhostBody
    {
        private PairCachingGhostObject _body;
        [TSerialize(Order = 0)]
        public PairCachingGhostObject Body
        {
            get => _body;
            set
            {
                if (_body != null)
                    _body.UserObject = null;

                _body = value ?? new PairCachingGhostObject();

                _body.UserObject = this;
            }
        }

        public BulletGhostBody(TGhostBodyConstructionInfo info, TCollisionShape shape)
        {

        }
    }
}