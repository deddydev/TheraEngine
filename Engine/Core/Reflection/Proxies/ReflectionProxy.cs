using System;
using System.Security.Permissions;

namespace TheraEngine.Core.Reflection
{
    public abstract class ReflectionProxy : MarshalByRefObject
    {
        //public MarshalSponsor Sponsor { get; set; }
        protected ReflectionProxy()
        {
            //Sponsor = new MarshalSponsor(this);
        }
        //~ReflectionProxy()
        //{
        //    Sponsor?.Dispose();
        //    Sponsor = null;
        //}
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
