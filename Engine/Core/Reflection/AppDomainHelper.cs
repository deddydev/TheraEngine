using mscoree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace TheraEngine.Core.Reflection
{
    /// <summary>
    /// Represents a <see cref="AppDomainManager"/> that is
    /// aware of the primary application AppDomain.
    /// </summary>
    public class AppDomainHelper : AppDomainManager
    {
        static AppDomainHelper()
        {
            ResetAppDomainCache();
            ResetTypeCache();
        }

        private static Lazy<AppDomain[]> _appDomainCache;
        private static Lazy<TypeProxy[]> _exportedTypesCache;

        public static AppDomain[] AppDomains => _appDomainCache.Value;
        public static TypeProxy[] ExportedTypes => _exportedTypesCache.Value;

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
            => AppDomains.FirstOrDefault(x => x.FriendlyName == Process.GetCurrentProcess().MainModule.ModuleName);
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
            => Engine.DomainProxy.GetExportedTypes().ToArray();
        
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

        public static event Action<AppDomain> GameDomainLoaded;
        public static event Action GameDomainUnloaded;

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
            TypeProxy[] types;

            if (assemblies != null && assemblies.Length > 0)
                types = ExportedTypes.Where(x => assemblies.Contains(x.Assembly)).ToArray();
            else
                types = ExportedTypes;

            ConcurrentDictionary<int, TypeProxy> matches = new ConcurrentDictionary<int, TypeProxy>();
            Parallel.For(0, types.Length, i =>
            {
                TypeProxy type = types[i];
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

        public static void OnGameDomainLoaded()
        {
            ResetAppDomainCache();
            ResetTypeCache();
            GameDomainLoaded?.Invoke(GetGameAppDomain());
        }
        public static void OnGameDomainUnloaded()
        {
            ResetAppDomainCache();
            ResetTypeCache();
            GameDomainUnloaded?.Invoke();
        }

        #endregion
    }
}
