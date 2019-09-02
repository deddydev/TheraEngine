using System.Collections.Generic;
using System.Linq;
using TheraEngine.Worlds;

namespace TheraEngine.Physics.ContactTesting
{
    /// <summary>
    /// Provides information about a ray intersection.
    /// </summary>
    public class ContactTestResult
    {
        public TCollisionObject CollisionObject { get; internal set; }
        public bool IsObjectB { get; internal set; }
        public TContactInfo Contact { get; internal set; }
        
        internal ContactTestResult() { }
        internal ContactTestResult(TContactInfo contact, TCollisionObject obj, bool isB)
        {
            CollisionObject = obj;
            Contact = contact;
            IsObjectB = isB;
        }
    }
    /// <summary>
    /// Contains properties and methods for projecting a ray in the world and testing for intersections with collision objects.
    /// </summary>
    public abstract class ContactTest
    {
        public TCollisionObject Object { get; set; }
        public ushort CollisionGroup { get; set; }
        public ushort CollidesWith { get; set; }
        public TCollisionObject[] Ignored { get; set; }

        public abstract bool HasContact { get; }
        
        public ContactTest(
            TCollisionObject obj,
            ushort collisionGroup,
            ushort collidesWith,
            params TCollisionObject[] ignored)
        {
            Object = obj;
            CollisionGroup = collisionGroup;
            CollidesWith = collidesWith;
            Ignored = ignored;
        }

        protected bool CanAddCommon(TCollisionObject obj)
            => !Ignored.Contains(obj);

        internal protected abstract void AddResult(TContactInfo contact, TCollisionObject otherObject, bool isOtherB);
        //internal protected virtual bool TestApproxCollision(int uniqueID, ushort collisionGroup, ushort collidesWith, Vec3 aabbMin, Vec3 aabbMax, object clientObject)
        //{
        //    //if (Ignored.Any(x => x.UniqueID == uniqueID))
        //    //    return false;
            
        //    //I believe this algorithm is faster.
        //    if (Collision.RayIntersectsAABBDistance(StartPointWorld, dir, aabbMin, aabbMax, out float distance) && distance * distance < dir.LengthSquared)
        //    //if (Collision.SegmentIntersectsAABB(Start, End, aabbMin, aabbMax, out Vec3 enterPoint, out Vec3 exitPoint))
        //    {
        //        bool rayIntersectsOther = (CollisionGroup & collidesWith) == CollisionGroup;
        //        bool otherIntersectsRay = (collisionGroup & CollidesWith) == collisionGroup;
        //        if (rayIntersectsOther && otherIntersectsRay)
        //            return true;
        //    }
        //    return false;
        //}

        /// <summary>
        /// Performs the test in the world and returns true if there are any collision results.
        /// </summary>
        public bool Test(IWorld world)
            => Engine.ContactTest(this, world);

        internal abstract void Reset();
    }
    /// <summary>
    /// Returns all intersected objects that specify collision with this ray.
    /// </summary>
    public class ContactTestMulti : ContactTest
    {
        public override bool HasContact => Results.Count != 0;
        
        public List<ContactTestResult> Results { get; } = new List<ContactTestResult>();
        internal override void Reset() => Results.Clear();
        
        public ContactTestMulti(TCollisionObject obj, ushort collisionGroupFlags, ushort collidesWithFlags, params TCollisionObject[] ignored) 
            : base(obj, collisionGroupFlags, collidesWithFlags, ignored) { }

        protected internal override void AddResult(TContactInfo contact, TCollisionObject otherObject, bool isOtherB)
        {
            if (!CanAddCommon(otherObject))
                return;
            
            Results.Add(new ContactTestResult(contact, otherObject, isOtherB));
        }
    }
}
