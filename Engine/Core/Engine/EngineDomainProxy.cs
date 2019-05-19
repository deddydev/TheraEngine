using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using static TheraEngine.Core.Files.TFileObject;

namespace TheraEngine.Core
{
    /// <summary>
    /// Proxy that runs methods in the game's domain.
    /// </summary>
    public class EngineDomainProxy : MarshalByRefObject
    {
        public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyLoaders { get; private set; }
        public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyExporters { get; private set; }

        public Sponsor SponsorRef { get; } = new Sponsor();

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

        public TypeProxy CreateType(string typeDeclaration)
        {
            try
            {
                AssemblyQualifiedName asmQualName = new AssemblyQualifiedName(typeDeclaration);
                string asmName = asmQualName.AssemblyName;
                //var domains = Engine.EnumAppDomains();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies(); //domains.SelectMany(x => x.GetAssemblies());

                return TypeProxy.GetType(typeDeclaration,
                    name => assemblies.FirstOrDefault(assembly => assembly.GetName().Name.EqualsInvariantIgnoreCase(name.Name)),
                    null,
                    true);
            }
            catch// (Exception ex)
            {
                //Engine.LogException(ex);
            }
            return null;
        }
        public void Created(TGame game)
        {
            Engine.SetGame(game);
            //Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            Engine.Initialize();
            //Editor.Instance.SetRenderTicking(true);
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            Engine.PrintLine("Resetting type caches.");
            ResetTypeCaches();
            Engine.PrintLine("Type caches reset.");
        }
        public void Destroyed()
        {
            Engine.ShutDown();
            SponsorRef.Release = true;
        }

        public virtual void ResetTypeCaches()
        {
            //FolderWrapper.LoadFileTypes();
            BaseObjectSerializer.ResetObjectSerializerCache(true);
            ClearThirdPartyTypeCache(true);
            Reset3rdPartyImportExportMethods();
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

        public override object InitializeLifetimeService() => base.InitializeLifetimeService();
        public class Sponsor : MarshalByRefObject, ISponsor
        {
            public bool Release { get; set; } = false;

            public TimeSpan Renewal(ILease lease)
            {
                // if any of these cases is true
                if (lease == null || lease.CurrentState != LeaseState.Renewing || Release)
                    return TimeSpan.Zero; // don't renew
                return TimeSpan.FromSeconds(1); // renew for a second, or however long u want
            }
        }
    }
}
