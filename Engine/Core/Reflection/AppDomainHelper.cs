using Extensions;
using mscoree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Threading;

namespace TheraEngine.Core.Reflection
{
    /// <summary>
    /// Represents a <see cref="AppDomainManager"/> that is
    /// aware of the primary application AppDomain.
    /// </summary>
    public class AppDomainHelper : AppDomainManager
    {
        private static Lazy<AppDomain[]> _appDomainCache = new Lazy<AppDomain[]>(GetAppDomains, LazyThreadSafetyMode.PublicationOnly);
        private static Lazy<TypeProxy[]> _exportedTypesCache = new Lazy<TypeProxy[]>(GetExportedTypes, LazyThreadSafetyMode.PublicationOnly);

        public static AppDomain[] AppDomains => _appDomainCache.Value;
        public static TypeProxy[] ExportedTypes => _exportedTypesCache.Value;

        public static string AppDomainStringList => string.Join(", ", AppDomains.Select(x => x.FriendlyName));

        /// <summary>
        /// Determines whether this is the primary domain.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is the primary domain; otherwise, <see langword="false"/>.
        /// </value>
        public static bool IsPrimaryDomain
            => GetPrimaryAppDomain() == AppDomain.CurrentDomain;
        public static bool IsGameDomain
        {
            get
            {
                var gameDomain = GetGameAppDomain();
                var primaryDomain = GetPrimaryAppDomain();
                return gameDomain == AppDomain.CurrentDomain && gameDomain != primaryDomain;
            }
        }

        public static bool IsGameDomainAlsoPrimary => GetGameAppDomain() == GetPrimaryAppDomain();
        /// <summary>
        /// Returns the primary application domain.
        /// </summary>
        /// <returns>The primary application domain.</returns>
        public static AppDomain GetPrimaryAppDomain()
        {
            return AppDomains.FirstOrDefault(x => Test(x));
        }

        private static bool Test(AppDomain x)
        {
            Process p = Process.GetCurrentProcess();
            ProcessModule m = p.MainModule;
            string n = m.ModuleName;
            string fn = x.FriendlyName;
            return string.Equals(fn, n, StringComparison.InvariantCulture);
        }

        public static AppDomain GetGameAppDomain()
        {
            //There will only ever be two AppDomains loaded
            AppDomain primary = GetPrimaryAppDomain();
            return AppDomains.FirstOrDefault(x => x != primary) ?? primary;
        }

        public static void ResetAppDomainCache()
            => _appDomainCache = new Lazy<AppDomain[]>(GetAppDomains, LazyThreadSafetyMode.PublicationOnly);
        public static void ResetTypeCache()
            => _exportedTypesCache = new Lazy<TypeProxy[]>(GetExportedTypes, LazyThreadSafetyMode.PublicationOnly);

        private static AppDomain[] GetAppDomains()
            => EnumAppDomains().ToArray();
        private static TypeProxy[] GetExportedTypes()
            => Engine.DomainProxy.GetExportedTypes()?.ToArray() ?? new TypeProxy[0];

        public static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            string name = args.LoadedAssembly.GetName().Name;
            string path = args.LoadedAssembly.Location;
            Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] LOADED {name} FROM {path}");
        }

        /// <summary>
        /// The default AppDomain.
        /// </summary>
        private static readonly Lazy<AppDomain> _defaultAppDomain = new Lazy<AppDomain>(() =>
        {
            IntPtr enumHandle = IntPtr.Zero;
            ICorRuntimeHost host = null;
            object ret = null;

            try
            {
                host = new CorRuntimeHost();
                host.GetDefaultDomain(out ret);
            }
            finally
            {
                if (host != null)
                {
                    if (enumHandle != IntPtr.Zero)
                        host.CloseEnum(enumHandle);

                    Marshal.ReleaseComObject(host);
                }
            }
            return ret as AppDomain;

        }, LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Gets the default AppDomain. This property caches the resulting value.
        /// </summary>
        public static AppDomain DefaultAppDomain => _defaultAppDomain.Value;

        /// <summary>
        /// Enumerates all AppDomains in the process.
        /// </summary>
        public static IEnumerable<AppDomain> EnumAppDomains()
        {
            IntPtr enumHandle = IntPtr.Zero;
            ICorRuntimeHost host = null;

            try
            {
                host = new CorRuntimeHost();
                host.EnumDomains(out IntPtr enumeration);

                while (true)
                {
                    host.NextDomain(enumeration, out object domain);
                    if (domain is null)
                        yield break;
                    yield return (AppDomain)domain;
                }
            }
            finally
            {
                if (host != null)
                {
                    if (enumHandle != IntPtr.Zero)
                        host.CloseEnum(enumHandle);
                    
                    Marshal.ReleaseComObject(host);
                }
            }
        }
        /// <summary>
        /// Helper to collect all types from all loaded assemblies that match the given predicate.
        /// </summary>
        /// <param name="matchPredicate">What determines if the type is a match or not.</param>
        /// <param name="resetTypeCache">If true, recollects all assembly types manually and re-caches them.</param>
        /// <returns>All types that match the predicate.</returns>
        public static IEnumerable<TypeProxy> FindTypes(Predicate<TypeProxy> matchPredicate, params AssemblyProxy[] assemblies)
        {
            //TODO: search all appdomains, return marshalbyrefobject list containing typeproxies
            TypeProxy[] types;

            if (assemblies != null && assemblies.Length > 0)
                types = ExportedTypes.Where(x => assemblies.Contains(x.Assembly)).ToArray();
            else
                types = ExportedTypes;

            Dictionary<int, TypeProxy> matches = new Dictionary<int, TypeProxy>();
            //Parallel.For(0, types.Length, i =>
            for (int i = 0; i < types.Length; ++i)
            {
                TypeProxy type = types[i];
                if (matchPredicate(type))
                    matches.Add(i, type);
            }//);

            return matches.Values.OrderBy(x => x.Name);
        }

        internal static void DomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Trace.WriteLine($"LOADED {args.LoadedAssembly.Location}");
            //string assemblyName = args.LoadedAssembly.GetName().Name;
            //string domainName = AppDomain.CurrentDomain.FriendlyName;
            //Debug.Print($"{nameof(AppDomain)} {domainName} loaded assembly {assemblyName}");
        }

        #region Method Execution

        //https://stackoverflow.com/questions/2008691/pass-and-execute-delegate-in-separate-appdomain
        //https://stackoverflow.com/questions/1510466/replacing-process-start-with-appdomains

        /// <summary>
        /// Executes a method in a separate AppDomain.  This should serve as a simple replacement
        /// of running code in a separate process via a console app.
        /// </summary>
        public T RunInAppDomain<T>(AppDomain domain, Func<T> func)
        {
            domain.DomainUnload += (sender, e) =>
            {
                // this empty event handler fixes the unit test, but I don't know why
            };

            try
            {
                AppDomainDelegateWrapper wrapper = new AppDomainDelegateWrapper(domain, func);
                domain.DoCallBack(wrapper.Invoke);

                return (T)domain.GetData(wrapper.ResultID);
            }
            finally
            {
                AppDomain.Unload(domain);
            }
        }
        public void RunInAppDomain(AppDomain domain, Action func)
        {
            RunInAppDomain(domain, () => { func(); return 0; });
        }

        /// <summary>
        /// Provides a serializable wrapper around a delegate.
        /// </summary>
        [Serializable]
        private class AppDomainDelegateWrapper : MarshalByRefObject
        {
            private readonly AppDomain _domain;
            private readonly Delegate _delegate;

            public string ResultID { get; }

            public AppDomainDelegateWrapper(AppDomain domain, Delegate func)
            {
                _domain = domain;
                _delegate = func;

                ResultID = func.GetHashCode().ToString();
            }

            public void Invoke()
            {
                _domain.SetData(ResultID, _delegate.DynamicInvoke());
            }
        }

        public static void ResetCaches()
        {
            ResetAppDomainCache();
            ResetTypeCache();
            ResetProxyCache();
            ReleaseSponsors();
        }

        private static void ResetProxyCache()
        {
            TypeProxy.Proxies.Clear();
            FieldInfoProxy.Proxies.Clear();
            PropertyInfoProxy.Proxies.Clear();
            //MemberInfoProxy.Proxies.Clear();
            MethodInfoProxy.Proxies.Clear();
            ParameterInfoProxy.Proxies.Clear();
            //MethodBaseProxy.Proxies.Clear();
            EventInfoProxy.Proxies.Clear();
            ConstructorInfoProxy.Proxies.Clear();
            AssemblyProxy.Proxies.Clear();
            MethodBodyProxy.Proxies.Clear();
            //ReflectionProxy.Proxies.Clear();
        }

        public static void ReleaseSponsors()
        {
            while (SponsoredObjects.TryTake(out ISponsorableMarshalByRefObject sobj))
                sobj.Sponsor.Release();
            foreach (var sponsor in ExternalSponsoredObjects.Values)
                sponsor.Release();
            ExternalSponsoredObjects.Clear();
        }

        public static ConcurrentBag<ISponsorableMarshalByRefObject> SponsoredObjects { get; } = new ConcurrentBag<ISponsorableMarshalByRefObject>();
        public static ConcurrentDictionary<MarshalByRefObject, MarshalExternalSponsor> ExternalSponsoredObjects { get; } = new ConcurrentDictionary<MarshalByRefObject, MarshalExternalSponsor>();

        public static void Sponsor(object obj)
        {
            try
            {
                if (obj is null || !RemotingServices.IsTransparentProxy(obj))
                    return;

                if (obj is ISponsorableMarshalByRefObject sponsorableObject)
                {
                    if (AppDomain.CurrentDomain == sponsorableObject.Domain || sponsorableObject.IsSponsored)
                        return;

                    sponsorableObject.Sponsor = new MarshalSponsor(sponsorableObject);
                    //Engine.PrintLine($"Sponsored {sponsorableObject.ToString()} from AppDomain {sponsorableObject.Domain.FriendlyName}.");

                    SponsoredObjects.Add(sponsorableObject);
                }
                else if (obj is MarshalByRefObject marshalObject)
                {
                    if (!ExternalSponsoredObjects.ContainsKey(marshalObject))
                    {
                        ExternalSponsoredObjects.TryAdd(marshalObject, new MarshalExternalSponsor(marshalObject));
                        //Engine.PrintLine($"Sponsored {marshalObject.ToString()}.");
                    }
                }
            }
            catch (AppDomainUnloadedException)
            {

            }
        }

        public static void ReleaseSponsor(object obj)
        {

        }

        public class MarshalExternalSponsor : MarshalByRefObject, ISponsor, IDisposable
        {
            public static readonly TimeSpan RenewalTimeSpan = TimeSpan.FromSeconds(1.0);

            public ILease Lease { get; private set; }
            public bool WantsRelease { get; set; } = false;
            public bool IsReleased { get; private set; } = false;
            public MarshalByRefObject Object { get; private set; }
            public DateTime LastRenewalTime { get; private set; }
            
            public TimeSpan Renewal(ILease lease)
            {
                // if any of these cases is true
                IsReleased = lease is null || lease.CurrentState == LeaseState.Expired || WantsRelease;
                string fn = Object.GetTypeProxy().GetFriendlyName();
                if (IsReleased)
                {
                    //Engine.PrintLine($"Released lease for {fn}.");
                    return TimeSpan.Zero;
                }
                TimeSpan span = DateTime.Now - LastRenewalTime;
                double sec = Math.Round(span.TotalSeconds, 1);
                //Engine.PrintLine($"Renewed lease for {fn}. {sec} seconds elapsed since last renewal.");
                LastRenewalTime = DateTime.Now;
                return TimeSpan.FromMinutes(10.0);
            }

            public MarshalExternalSponsor(MarshalByRefObject mbro)
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

        #endregion
    }
}
