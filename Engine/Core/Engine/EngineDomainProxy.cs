using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security.Permissions;
using System.Threading;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using static TheraEngine.Core.Files.TFileObject;

namespace TheraEngine.Core
{
    public interface ISponsorableMarshalByRefObject
    {
        MarshalSponsor Sponsor { get; set; }
        AppDomain Domain { get; }

        object InitializeLifetimeService();
    }
    public class SponsorableMarshalByRefObject : MarshalByRefObject, ISponsorableMarshalByRefObject
    {
        [Browsable(false)]
        public MarshalSponsor Sponsor { get; set; }

        [Browsable(false)]
        public AppDomain Domain => AppDomain.CurrentDomain;

        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        //public override object InitializeLifetimeService()
        //{
        //    ILease lease = (ILease)base.InitializeLifetimeService();
        //    if (lease.CurrentState == LeaseState.Initial)
        //    {
        //        lease.InitialLeaseTime = TimeSpan.FromSeconds(10);
        //        lease.SponsorshipTimeout = TimeSpan.FromSeconds(10);
        //        lease.RenewOnCallTime = TimeSpan.FromSeconds(10);
        //    }
        //    return lease;
        //}
    }
    /// <summary>
    /// Proxy that runs the engine in the game's domain.
    /// </summary>
    //[Serializable]
    public class EngineDomainProxy : SponsorableMarshalByRefObject
    {
        public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyLoaders { get; private set; }
        public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyExporters { get; private set; }

        public event Action Stopped;
        public event Action Started;
        public event Action<string> DebugOutput;
        public event Action<bool> ReloadTypeCaches;

        public string GetVersionInfo() =>

            ".NET Version: "        + Environment.Version.ToString() 
            + Environment.NewLine +
            "Assembly Location: "   + typeof(EngineDomainProxy).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\") 
            + Environment.NewLine +
            "Assembly Directory: "  + Directory.GetCurrentDirectory() 
            + Environment.NewLine +
            "ApplicationBase: "     + AppDomain.CurrentDomain.SetupInformation.ApplicationBase 
            + Environment.NewLine +
            "AppDomain: "           + AppDomain.CurrentDomain.FriendlyName
            + Environment.NewLine;

        public ProxyList<TypeProxy> GetExportedTypes()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();
            return new ProxyList<TypeProxy>(assemblies.Where(x => !x.IsDynamic).SelectMany(x => x.GetExportedTypes().Select(r => TypeProxy.Get(r))).Distinct());
        }

        //public Type CreateType(string typeDeclaration)
        //{
        //    try
        //    {
        //        AssemblyQualifiedName asmQualName = new AssemblyQualifiedName(typeDeclaration);
        //        string asmName = asmQualName.AssemblyName;
        //        //var domains = Engine.EnumAppDomains();
        //        var assemblies = AppDomain.CurrentDomain.GetAssemblies(); //domains.SelectMany(x => x.GetAssemblies());

        //        return Type.GetType(typeDeclaration,
        //            name => assemblies.FirstOrDefault(assembly => assembly.GetName().Name.EqualsInvariantIgnoreCase(name.Name)),
        //            null,
        //            true);
        //    }
        //    catch// (Exception ex)
        //    {
        //        //Engine.LogException(ex);
        //    }
        //    return null;
        //}
        public async virtual void Start(string gamePath, bool isUIDomain)
        {
            Engine.PrintLine($"Starting domain proxy.");
            AppDomainHelper.OnGameDomainLoaded();

            Engine.Initialize();
            ResetTypeCaches();

            Engine.Run();
            if (gamePath.IsExistingDirectoryPath() == false)
            {
                TGame game = await LoadAsync<TGame>(gamePath);
                Engine.SetGame(game);
            }
            else
                Engine.SetGame(null);

            OnStarted();
        }
        protected virtual void OnStarted() => Started?.Invoke();
        public virtual void Stop()
        {
            Engine.Stop();
            Engine.ShutDown();
            ResetTypeCaches(false);
            Sponsor?.Release();
            Stopped?.Invoke();
        }
        public virtual void ResetTypeCaches(bool reloadNow = true)
        {
            Engine.PrintLine($"{(reloadNow ? "Regenerating" : "Clearing")} type caches.");

            BaseObjectSerializer.ClearObjectSerializerCache();
            if (reloadNow)
                _ = BaseObjectSerializer.ObjectSerializers.Value;
            
            ClearThirdPartyTypeCache(reloadNow);
            Reset3rdPartyImportExportMethods(reloadNow);

            ReloadTypeCaches?.Invoke(reloadNow);

            Engine.PrintLine($"Done {(reloadNow ? "regenerating" : "clearing")} type caches.");
        }

        public Delegate Get3rdPartyLoader(Type fileType, string extension)
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            return Get3rdPartyMethod(_3rdPartyLoaders, fileType, extension);
        }
        public Delegate Get3rdPartyExporter(Type fileType, string extension)
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            return Get3rdPartyMethod(_3rdPartyExporters, fileType, extension);
        }
        private Delegate Get3rdPartyMethod(Dictionary<string, Dictionary<TypeProxy, Delegate>> methodDic, TypeProxy fileType, string extension)
        {
            extension = extension.ToLowerInvariant();
            if (methodDic != null && methodDic.ContainsKey(extension))
            {
                var t = methodDic[extension];
                if (t.ContainsKey(fileType))
                    return t[fileType];
            }
            return null;
        }

        public void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethod<T> loadMethod) where T : class, IFileObject
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethod<T> exportMethod) where T : class, IFileObject
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        public void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethodAsync<T> loadMethod) where T : class, IFileObject
        {
            if (_3rdPartyLoaders == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        public void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethodAsync<T> exportMethod) where T : class, IFileObject
        {
            if (_3rdPartyExporters == null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        }
        private static void Register3rdParty<T>(
            Dictionary<string, Dictionary<TypeProxy, Delegate>> methodDic,
            string extension,
            Delegate method)
            where T : class, IFileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic == null)
                methodDic = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();

            Dictionary<TypeProxy, Delegate> typesforExt;
            if (!methodDic.ContainsKey(extension))
                methodDic.Add(extension, typesforExt = new Dictionary<TypeProxy, Delegate>());
            else
                typesforExt = methodDic[extension];

            Type fileType = typeof(T);
            if (!typesforExt.ContainsKey(fileType))
                typesforExt.Add(fileType, method);
            else
                throw new Exception("Registered " + extension + " for " + fileType.GetFriendlyName() + " too many times.");
        }

        private void Reset3rdPartyImportExportMethods(bool reloadNow = true)
        {
            if (reloadNow)
            {
                _3rdPartyLoaders = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();
                _3rdPartyExporters = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();
                try
                {
                    TypeProxy[] types = AppDomainHelper.FindTypes(t => t.IsSubclassOf(typeof(TFileObject)) && !t.IsAbstract).ToArray();
                    foreach (TypeProxy type in types)
                    {
                        TFileExt attrib = GetFileExtension(type);
                        if (attrib == null)
                            continue;

                        ReadLoaders(_3rdPartyLoaders, type, attrib.ImportableExtensions);
                        ReadLoaders(_3rdPartyExporters, type, attrib.ExportableExtensions);
                    }
                }
                catch { }
            }
            else
            {
                _3rdPartyLoaders = null;
                _3rdPartyExporters = null;
            }
        }
        private void ReadLoaders(IDictionary<string, Dictionary<TypeProxy, Delegate>> loaders, TypeProxy type, IEnumerable<string> extensions)
        {
            foreach (string ext3rd in extensions)
            {
                string extLower = ext3rd.ToLowerInvariant();
                Dictionary<TypeProxy, Delegate> extensionLoaders;
                if (loaders.ContainsKey(extLower))
                    extensionLoaders = loaders[extLower];
                else
                    loaders.Add(extLower, extensionLoaders = new Dictionary<TypeProxy, Delegate>());

                if (extensionLoaders.ContainsKey(type))
                    throw new Exception(type.GetFriendlyName() + " has already been added to the third party loader list for " + extLower);

                MethodInfoProxy[] methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static)
                    .Where(x => string.Equals(x.GetCustomAttribute<ThirdPartyLoader>()?.Extension, extLower, StringComparison.InvariantCultureIgnoreCase))
                    .ToArray();

                if (methods.Length <= 0)
                    continue;

                MethodInfoProxy m = methods[0];
                ThirdPartyLoader loader = m.GetCustomAttribute<ThirdPartyLoader>();
                bool async = loader.Async;
                TypeProxy delGenType = async ? typeof(Del3rdPartyImportFileMethodAsync<>) : typeof(Del3rdPartyImportFileMethod<>);

                try
                {
                    TypeProxy delType = delGenType.MakeGenericType(m.DeclaringType);
                    Delegate d = delType.CreateDelegate(m);
                    extensionLoaders.Add(type, d);
                }
                catch
                {
                    Engine.LogWarning($"Cannot use {m.GetFriendlyName()} as a third party loader for {m.DeclaringType.GetFriendlyName()}.");
                }
            }
        }

        public TypeProxy GetTypeFor(string typeName)
        {
            //Engine.PrintLine("Getting type proxy for " + typeName);
            TypeProxy proxy = Type.GetType(typeName);
            return proxy;
        }

        public async void ExportFile(IFileObject file, string dir, EProprietaryFileFormat format)
        {
            await file.ExportAsync(dir, file.Name, ESerializeFlags.Default, format, null, CancellationToken.None);
        }

        public T CreateInstance<T>(params object[] args) where T : ISponsorableMarshalByRefObject
            => (T)Activator.CreateInstance(typeof(T), args);

        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        //public override object InitializeLifetimeService()
        //{
        //    return null;
        //}
    }
}
