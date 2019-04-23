using mscoree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Proxies;

namespace TheraEngine.Core.Reflection
{
    /// <summary>
    /// Represents a <see cref="AppDomainManager"/> that is
    /// aware of the primary application AppDomain.
    /// </summary>
    public class AppDomainHelper : AppDomainManager
    {
        /// <summary>
        /// Gets the primary domain.
        /// </summary>
        /// <value>The primary domain.</value>
        //public static AppDomain PrimaryDomain { get; private set; }

        ///// <summary>
        ///// Sets the primary domain.
        ///// </summary>
        ///// <param name="primaryDomain">The primary domain.</param>
        //private void SetPrimaryDomain(AppDomain primaryDomain)
        //    => PrimaryDomain = primaryDomain;
        
        ///// <summary>
        ///// Sets the primary domain to self.
        ///// </summary>
        //private void SetPrimaryDomainToSelf()
        //    => PrimaryDomain = AppDomain.CurrentDomain;
        
        /// <summary>
        /// Determines whether this is the primary domain.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if this instance is the primary domain; otherwise, <see langword="false"/>.
        /// </value>
        public static bool IsPrimaryDomain 
            => GetPrimaryAppDomain() == AppDomain.CurrentDomain;

        ///// <summary>
        ///// Creates the initial domain.
        ///// </summary>
        ///// <param name="friendlyName">Name of the friendly.</param>
        ///// <param name="securityInfo">The security info.</param>
        ///// <param name="appDomainInfo">The AppDomain setup info.</param>
        ///// <returns></returns>
        //public static AppDomain CreateInitialDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
        //{
        //    if (AppDomain.CurrentDomain.DomainManager is PrimaryAppDomainManager)
        //        return null;

        //    appDomainInfo = appDomainInfo ?? new AppDomainSetup();

        //    Type t = typeof(PrimaryAppDomainManager);
        //    appDomainInfo.AppDomainManagerAssembly = t.Assembly.FullName;
        //    appDomainInfo.AppDomainManagerType = t.FullName;

        //    var appDomain = CreateDomainHelper(friendlyName, securityInfo, appDomainInfo);
        //    ((PrimaryAppDomainManager)appDomain.DomainManager).SetPrimaryDomainToSelf();
        //    PrimaryDomain = appDomain;

        //    appDomain.AssemblyLoad += DomainAssemblyLoad;
        //    return appDomain;
        //}

        ///// <summary>
        ///// Returns a new or existing application domain.
        ///// </summary>
        ///// <param name="friendlyName">The friendly name of the domain.</param>
        ///// <param name="securityInfo">An object that contains evidence mapped through the security policy to establish a top-of-stack permission set.</param>
        ///// <param name="appDomainInfo">An object that contains application domain initialization information.</param>
        ///// <returns>A new or existing application domain.</returns>
        ///// <PermissionSet>
        /////     <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="ControlEvidence, ControlAppDomain, Infrastructure"/>
        ///// </PermissionSet>
        //public override AppDomain CreateDomain(string friendlyName, Evidence securityInfo, AppDomainSetup appDomainInfo)
        //{
        //    appDomainInfo = appDomainInfo ?? new AppDomainSetup();
        //    appDomainInfo.AppDomainManagerAssembly = typeof(PrimaryAppDomainManager).Assembly.FullName;
        //    appDomainInfo.AppDomainManagerType = typeof(PrimaryAppDomainManager).FullName;

        //    var appDomain = base.CreateDomain(friendlyName, securityInfo, appDomainInfo);
        //    ((PrimaryAppDomainManager)appDomain.DomainManager).SetPrimaryDomain(PrimaryDomain);

        //    return appDomain;
        //}

        /// <summary>
        /// Returns the primary application domain.
        /// </summary>
        /// <returns>The primary application domain.</returns>
        public static AppDomain GetPrimaryAppDomain()
            => AppDomains.FirstOrDefault(x => x.FriendlyName == Process.GetCurrentProcess().MainModule.ModuleName);
        public static AppDomain GetGameAppDomain()
            => AppDomains.FirstOrDefault(x => x.FriendlyName == "Puyo");

        private static AppDomain[] _appDomainCache;
        public static AppDomain[] AppDomains => _appDomainCache ?? (_appDomainCache = EnumAppDomains().ToArray());
        public static void ClearAppDomainCache() => _appDomainCache = null;

        /// <summary>
        /// The default AppDomain.
        /// </summary>
        private static readonly Lazy<AppDomain> LazyDefaultAppDomain = new Lazy<AppDomain>(() =>
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
        public static AppDomain DefaultAppDomain => LazyDefaultAppDomain.Value;

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
                    if (domain == null)
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
            IEnumerable<AssemblyProxy> search;

            if (assemblies != null && assemblies.Length > 0)
                search = assemblies;
            else
            {
                //search = AppDomain.CurrentDomain.GetAssemblyProxies();
                ////PrintLine("FindTypes; returning assemblies from domains:");
                var domains = AppDomains;
                search = domains.SelectMany(x =>
                {
                    //PrintLine(x.FriendlyName);
                    try
                    {
                        return x.GetAssemblyProxies();
                    }
                    catch //(Exception ex)
                    {
                        Debug.Print($"Unable to load assemblies from {nameof(AppDomain)} {x.FriendlyName}");
                        return new ProxyList<AssemblyProxy>();
                    }
                });
            }

            search = search.Where(x => !x.IsDynamic).Distinct(new AssemblyProxy.EqualityComparer());

            //if (includeEngineAssembly)
            //{
            //    Assembly engine = Assembly.GetExecutingAssembly();
            //    if (!search.Contains(engine))
            //        search = search.Append(engine);
            //}

            var allTypes = search.SelectMany(x => x.GetExportedTypes()).Distinct(new TypeProxy.EqualityComparer()).ToArray();
            ConcurrentDictionary<int, TypeProxy> matches = new ConcurrentDictionary<int, TypeProxy>();
            Parallel.For(0, allTypes.Length, i =>
            {
                TypeProxy type = allTypes[i];
                if (matchPredicate(type))
                    matches.AddOrUpdate(i, type, (x, y) => type);
            });
            return matches.Values.OrderBy(x => x.Name);
        }
        
        internal static void DomainAssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            string assemblyName = args.LoadedAssembly.GetName().Name;
            string domainName = AppDomain.CurrentDomain.FriendlyName;
            Debug.Print($"{nameof(AppDomain)} {domainName} loaded assembly {assemblyName}");
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

        #endregion
    }
}
