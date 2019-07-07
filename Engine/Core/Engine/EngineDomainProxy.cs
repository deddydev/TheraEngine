using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Threading;
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
        private Lazy<Dictionary<ObjectSerializerFor, TypeProxy>> ObjectSerializers { get; set; }

        public EngineDomainProxy()
        {
            ClearObjectSerializerCache();
        }

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

        public Type CreateType(string typeDeclaration)
        {
            try
            {
                AssemblyQualifiedName asmQualName = new AssemblyQualifiedName(typeDeclaration);
                string asmName = asmQualName.AssemblyName;
                //var domains = Engine.EnumAppDomains();
                var assemblies = AppDomain.CurrentDomain.GetAssemblies(); //domains.SelectMany(x => x.GetAssemblies());

                return Type.GetType(typeDeclaration,
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
        public void Run(TGame game)
        {
            Engine.SetGame(game);
            //Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            Engine.Initialize();
            //Editor.Instance.SetRenderTicking(true);
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            ResetTypeCaches();
        }
        public virtual void Destroyed()
        {
            Engine.ShutDown();
            ResetTypeCaches(false);
            SponsorRef.Release = true;
        }
        /// <summary>
        /// Finds the class to use to read and write the given type.
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="mustAllowStringSerialize"></param>
        /// <param name="mustAllowBinarySerialize"></param>
        /// <returns></returns>
        public BaseObjectSerializer DetermineObjectSerializer(
            Type objectType, bool mustAllowStringSerialize = false, bool mustAllowBinarySerialize = false)
        {
            if (objectType == null)
            {
                Engine.LogWarning("Unable to create object serializer for null type.");
                return null;
            }

            var temp = ObjectSerializers.Value.Where(kv => 
                objectType?.IsAssignableTo(kv.Key.ObjectType) ?? false);

            if (mustAllowStringSerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsString);
            if (mustAllowBinarySerialize)
                temp = temp.Where(kv => kv.Key.CanSerializeAsBinary);

            TypeProxy[] types = temp.Select(x => x.Value).ToArray();

            TypeProxy serType;
            switch (types.Length)
            {
                case 0:
                    serType = typeof(CommonObjectSerializer);
                    break;
                case 1:
                    serType = types[0];
                    break;
                default:
                    {
                        int[] counts = types.Select(x => types.Count(x.IsSubclassOf)).ToArray();
                        int min = counts.Min();
                        int[] mins = counts.FindAllMatchIndices(x => x == min);
                        string msg = "Type " + objectType.GetFriendlyName() + " has multiple valid object serializers: " + types.ToStringList(", ", " and ", x => x.GetFriendlyName());
                        msg += ". Narrowed down to " + mins.Select(x => types[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                        Engine.PrintLine(msg);
                        serType = types[mins[0]];
                        break;
                    }
            }

            Debug.WriteLine($"AppDomain {AppDomain.CurrentDomain.FriendlyName} - Determined object serializer for {objectType.GetFriendlyName()}: {serType.GetFriendlyName()}");

            return serType.CreateInstance<BaseObjectSerializer>();
        }
        private Dictionary<ObjectSerializerFor, TypeProxy> GetObjectSerializers()
        {
            Debug.Print("Reloading object serializer cache.");
            Type baseObjSerType = typeof(BaseObjectSerializer);
            IEnumerable<TypeProxy> typeList = AppDomainHelper.FindTypes(type =>
                type.IsAssignableTo(baseObjSerType) &&
                type.HasCustomAttribute<ObjectSerializerFor>());

            var serializers = new Dictionary<ObjectSerializerFor, TypeProxy>();
            foreach (Type type in typeList)
            {
                var attrib = type.GetCustomAttribute<ObjectSerializerFor>();
                if (!serializers.ContainsKey(attrib))
                    serializers.Add(attrib, type);
            }
            Debug.Print("Done loading object serializer cache.");
            return serializers;
        }
        public void ClearObjectSerializerCache()
        {
            Debug.Print("Clearing object serializer cache.");
            ObjectSerializers = new Lazy<Dictionary<ObjectSerializerFor, TypeProxy>>(GetObjectSerializers, LazyThreadSafetyMode.PublicationOnly);
        }

        public virtual void ResetTypeCaches(bool reloadNow = true)
        {
            Engine.PrintLine($"{(reloadNow ? "Regenerating" : "Clearing")} type caches.");
            //FolderWrapper.LoadFileTypes();

            ClearObjectSerializerCache();
            if (reloadNow)
                _ = ObjectSerializers.Value;
            
            ClearThirdPartyTypeCache(reloadNow);
            Reset3rdPartyImportExportMethods(reloadNow);
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
