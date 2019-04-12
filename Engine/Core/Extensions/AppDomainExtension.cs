using AppDomainToolkit;
using System.Linq;
using TheraEngine.Core.Reflection;
using TheraEngine.Core.Reflection.Proxies;

namespace System
{
    public static unsafe class AppDomainExtension
    {
        public static ProxyList<AssemblyProxy> GetAssemblyProxies(this AppDomain domain)
            => RemoteFunc.Invoke(domain, () => new ProxyList<AssemblyProxy>(
                AppDomain.CurrentDomain.GetAssemblies().Select(x => AssemblyProxy.Get(x))));
        
    }
}
