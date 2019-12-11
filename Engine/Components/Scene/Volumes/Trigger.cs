using System.Collections.Generic;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Physics.ContactTesting;
using Extensions;

namespace TheraEngine.Components.Scene.Volumes
{
    public delegate void DelOnOverlapEnter(TCollisionObject actor);
    public delegate void DelOnOverlapLeave(TCollisionObject actor);
    public class TriggerVolumeComponent : BoxComponent
    {
        public DelOnOverlapEnter Entered;
        public DelOnOverlapLeave Left;

        protected virtual void OnEntered(TCollisionObject obj)
        {
            Entered?.Invoke(obj);
        }
        protected virtual void OnLeft(TCollisionObject obj)
        {
            Left?.Invoke(obj);
        }

        public class TriggerContactInfo
        {
            public TriggerContactInfo() { }
            public TriggerContactInfo(TContactInfo contact, bool isB)
            {
                Contact = contact;
                IsObjectB = isB;
            }

            public TContactInfo Contact { get; set; }
            public bool IsObjectB { get; set; }
        }

        public Dictionary<TCollisionObject, TriggerContactInfo> Contacts =
           new Dictionary<TCollisionObject, TriggerContactInfo>();

        public TriggerVolumeComponent() : this(1.0f) { }
        public TriggerVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, new TGhostBodyConstructionInfo()
            {
                CollidesWith = (ushort)ETheraCollisionGroup.DynamicWorld,
                CollisionGroup = (ushort)ETheraCollisionGroup.StaticWorld,
                CollisionEnabled = false,
                SimulatePhysics = false,
            }) { }

        public override void OnSpawned()
        {
            base.OnSpawned();
            //_collisionObject?.Despawn(OwningWorld);
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            base.OnDespawned();
        }
        private ContactTestMulti _test = new ContactTestMulti(null, 0, 0);
        private void Tick(float delta)
        {
            if (!(CollisionObject is TGhostBody ghost))
                return;

            //var list = ghost.CollectOverlappingPairs();

            ushort group = CollisionObject.CollisionGroup;
            ushort with = CollisionObject.CollidesWith;

            _test.Object = CollisionObject;
            _test.CollisionGroup = group;
            _test.CollidesWith = with;
            _test.Test(OwningWorld);

            List<TCollisionObject> remove = new List<TCollisionObject>(Contacts.Count);
            foreach (var kv in Contacts)
            {
                int matchIndex = _test.Results.FindIndex(x => x.CollisionObject == kv.Key);
                if (_test.Results.IndexInRange(matchIndex))
                {
                    var match = _test.Results[matchIndex];
                    _test.Results.RemoveAt(matchIndex);

                    kv.Value.Contact = match.Contact;
                    kv.Value.IsObjectB = match.IsObjectB;
                }
                else
                {
                    remove.Add(kv.Key);
                }
            }
            foreach (var obj in remove)
            {
                Contacts.Remove(obj);
                OnLeft(obj);
                Engine.PrintLine($"TRIGGER OBJECT LEFT: {Contacts.Count} contacts total");
            }
            foreach (var result in _test.Results)
            {
                var info = new TriggerContactInfo(result.Contact, result.IsObjectB);
                var obj = result.CollisionObject;

                if (Contacts.ContainsKey(obj))
                    Contacts[obj] = info;
                else
                {
                    Contacts.Add(obj, info);
                    OnEntered(obj);
                    Engine.PrintLine($"TRIGGER OBJECT ENTERED: {Contacts.Count} contacts total");
                }
                //RigidBodyCollision.OnOverlapped(result.CollisionObject, result.Contact, result.IsObjectB);
                //result.CollisionObject.OnOverlapped(RigidBodyCollision, result.Contact, !result.IsObjectB);
            }
        }
    }
}
