using System;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;

namespace TheraEngine.Core
{
    public class MarshalSponsor : MarshalByRefObject, ISponsor, IDisposable
    {
        public static readonly TimeSpan RenewalTimeSpan = TimeSpan.FromMinutes(10.0);

        public ILease Lease { get; private set; }
        public bool WantsRelease { get; set; } = false;
        public bool IsReleased { get; private set; } = false;
        public ISponsorableMarshalByRefObject Object { get; private set; }
        public DateTime LastRenewalTime { get; private set; }

        public TimeSpan Renewal(ILease lease)
        {
            IsReleased = lease is null || lease.CurrentState == LeaseState.Expired || WantsRelease;
            //string fn = Object.GetTypeProxy().GetFriendlyName();
            if (IsReleased)
            {
                //Engine.PrintLine($"Released lease for {fn}.");
                return TimeSpan.Zero;
            }
            //if (lease.CurrentState == LeaseState.Renewing)
            {
                //TimeSpan span = DateTime.Now - LastRenewalTime;
                //double sec = Math.Round(span.TotalSeconds, 1);
                //Engine.PrintLine($"Renewed lease for {fn}. {sec} seconds elapsed since last renewal.");
                LastRenewalTime = DateTime.Now;
                return RenewalTimeSpan;
            }
            //return TimeSpan.Zero;
        }

        public MarshalSponsor(ISponsorableMarshalByRefObject mbro)
        {
            Object = mbro;
            Lease = mbro.InitializeLifetimeService() as ILease;
            Lease?.Register(this);
            LastRenewalTime = DateTime.Now;
        }

        public void Dispose()
        {
            Lease?.Unregister(this);
            Lease = null;
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService() => null;

        public void Release() => WantsRelease = true;
    }
}
