using AppDomainToolkit;
using System;
using System.Collections.Generic;
using System.Reflection;
using TheraEngine.Core.Reflection;

namespace Extensions
{
    public static unsafe class AppDomainExtension
    {
        /// <summary>
        /// Creates a new instance of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of object to create.</typeparam>
        /// <param name="appDomain">The app domain.</param>
        /// <returns>A proxy for the new object.</returns>
        public static T CreateInstanceAndUnwrap<T>(this AppDomain appDomain)
        {
            Type t = typeof(T);
            var res = (T)appDomain.CreateInstanceAndUnwrap(t.Assembly.FullName, t.FullName);
            return res;
        }
        public static List<Assembly> GetAssemblies(this AppDomain domain)
            => RemoteFunc.Invoke(domain, () => new List<Assembly>(
                AppDomain.CurrentDomain.GetAssemblies()));
        public static bool IsPrimaryDomain(this AppDomain domain)
            => AppDomainHelper.GetPrimaryAppDomain() == domain;
#if EDITOR
        public static bool IsGameDomain(this AppDomain domain)
            => AppDomainHelper.GetGameAppDomain() == domain;
#endif
    }
}
