﻿using mscoree;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading.Tasks;
using TheraEngine.Core.Reflection.Proxies;

namespace TheraEngine.Core.Reflection
{
    /// <summary>
    /// Represents a <see cref="AppDomainManager"/> that is
    /// aware of the primary application AppDomain.
    /// </summary>
    public class PrimaryAppDomainManager : AppDomainManager
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
        /// Returns an enumerable containing all AppDomains in the process.
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<AppDomain> EnumAppDomains()
        {
            IntPtr enumHandle = IntPtr.Zero;
            ICorRuntimeHost host = null;

            try
            {
                host = new CorRuntimeHost();
                host.EnumDomains(out enumHandle);

                host.NextDomain(enumHandle, out object domain);
                while (domain != null)
                {
                    yield return (AppDomain)domain;
                    host.NextDomain(enumHandle, out domain);
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

            search = search.Where(x => !x.IsDynamic).Distinct();

            //if (includeEngineAssembly)
            //{
            //    Assembly engine = Assembly.GetExecutingAssembly();
            //    if (!search.Contains(engine))
            //        search = search.Append(engine);
            //}

            var allTypes = search.SelectMany(x => x.GetExportedTypes()).Distinct().ToArray();
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
    }
}
