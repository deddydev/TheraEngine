using AppDomainToolkit;
using System.Linq;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Proxies;

namespace System
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
        public static ProxyList<AssemblyProxy> GetAssemblyProxies(this AppDomain domain)
            => RemoteFunc.Invoke(domain, () => new ProxyList<AssemblyProxy>(
                AppDomain.CurrentDomain.GetAssemblies().Select(x => AssemblyProxy.Get(x))));
        
    }
}
