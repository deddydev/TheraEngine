using System.Collections.Generic;
using TheraEngine.Components.Scene.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Physics.ContactTesting;

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

        public Dictionary<TCollisionObject, (TContactInfo info, bool isB)> Contacts =
           new Dictionary<TCollisionObject, (TContactInfo info, bool isB)>();

        public TriggerVolumeComponent() : this(1.0f) { }
        public TriggerVolumeComponent(Vec3 halfExtents)
            : base(halfExtents, new TGhostBodyConstructionInfo()
            {
                CollidesWith = (ushort)ETheraCollisionGroup.All,
                CollisionGroup = (ushort)ETheraCollisionGroup.DynamicWorld,
            }) { }

        public override void OnSpawned()
        {
            base.OnSpawned();
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
            if (CollisionObject is null)
                return;

            ushort group = CollisionObject.CollisionGroup;
            ushort with = CollisionObject.CollidesWith;

            _test.Object = CollisionObject;
            _test.CollisionGroup = group;
            _test.CollidesWith = with;

            _test.Test(OwningWorld);

            List<TCollisionObject> remove = new List<TCollisionObject>(Contacts.Count);
            foreach (var key in Contacts.Keys)
            {
                int matchIndex = _test.Results.FindIndex(x => x.CollisionObject == key);
                if (matchIndex < 0)
                {
                    remove.Add(key);
                }
                else
                {
                    var match = _test.Results[matchIndex];
                    _test.Results.RemoveAt(matchIndex);
                    //Contacts[key] = (match.Contact, match.IsObjectB);
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
                var info = (result.Contact, result.IsObjectB);
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
