﻿using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Rendering;
using TheraEngine.Rendering.DirectX;
using TheraEngine.Rendering.OpenGL;
using TheraEngine.Rendering.Scene;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using static TheraEngine.Core.Files.TFileObject;
using static TheraEngine.Rendering.RenderContext;

namespace TheraEngine.Core
{
    public interface ISponsorableMarshalByRefObject
    {
        MarshalSponsor Sponsor { get; set; }
        AppDomain Domain { get; }
        bool IsSponsored { get; }

        object InitializeLifetimeService();
    }
    public class SponsorableMarshalByRefObject : MarshalByRefObject, ISponsorableMarshalByRefObject
    {
        [Browsable(false)]
        public MarshalSponsor Sponsor { get; set; }

        [Browsable(false)]
        public AppDomain Domain => AppDomain.CurrentDomain;

        [Browsable(false)]
        public bool IsSponsored => Sponsor != null;

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
    /// Proxy that runs code in the game's domain.
    /// </summary>
    //[Serializable]
    public class EngineDomainProxy : SponsorableMarshalByRefObject
    {
        public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyLoaders { get; private set; }

        public void ToggleVRActive(int worldManagerId)
        {
            if (EngineVR.IsActive)
                EngineVR.Shutdown();
            else
            {
                EngineVR.Initialize();
                EngineVR.LinkToWorldManager(worldManagerId);
            }
        }
        public void DisableVR()
        {
            if (EngineVR.IsActive)
                EngineVR.Shutdown();
        }
        public void EnableVR(int worldManagerId)
        {
            if (!EngineVR.IsActive)
            {
                EngineVR.Initialize();
                EngineVR.LinkToWorldManager(worldManagerId);
            }
        }

        //public Dictionary<string, Dictionary<TypeProxy, Delegate>> _3rdPartyExporters { get; private set; }

        public event Action Stopped;
        public event Action Started;
        //public event Action<string> DebugOutput;

        public event Action<bool> ReloadTypeCaches;

        protected virtual void OnStarted() => Started?.Invoke();
        protected virtual void OnStopped() => Stopped?.Invoke();

        public string GetVersionInfo() =>
            $".NET Version: {Environment.Version.ToString()}{Environment.NewLine}" +
            $"Assembly Location: {typeof(EngineDomainProxy).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\")}{Environment.NewLine}" +
            $"Assembly Directory: {Directory.GetCurrentDirectory()}{Environment.NewLine}" +
            $"ApplicationBase: {AppDomain.CurrentDomain.SetupInformation.ApplicationBase}{Environment.NewLine}" +
            $"AppDomain: {AppDomain.CurrentDomain.FriendlyName}{Environment.NewLine}";

        public ListProxy<TypeProxy> GetExportedTypes()
        {
            AppDomain domain = AppDomain.CurrentDomain;
            Assembly[] assemblies = domain.GetAssemblies();
            return new ListProxy<TypeProxy>(assemblies.Where(x => !x.IsDynamic).SelectMany(x => x.GetExportedTypes().Select(r => TypeProxy.Get(r))).Distinct());
        }

        public void SponsorObject(object obj) 
            => AppDomainHelper.Sponsor(obj);

        public void ReleaseSponsor(object obj)
            => AppDomainHelper.ReleaseSponsor(obj);

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
            Engine.UncacheSettings(true);

            Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Starting self.");

            AppDomainHelper.ResetCaches();
            ResetTypeCaches();

            AppDomain.CurrentDomain.AssemblyLoad += AppDomainHelper.CurrentDomain_AssemblyLoad;

            Engine.InputAwaiter = null;
            Engine.InputLibrary = EInputLibrary.OpenTK;

            Engine.Initialize();
            Engine.Run();
            if (gamePath.IsExistingDirectoryPath() == false)
            {
                TGame game = await LoadAsync<TGame>(gamePath);
                Engine.SetGame(game);
            }
            else
                Engine.SetGame(null);

            SetRenderTicking(true);
            OnStarted();
        }

        public bool GetGlobalFile(string absolutePath, out IFileObject file)
            => GlobalFileInstances.TryGetValue(absolutePath, out file);

        public void AddGlobalFile(string path, IFileObject file)
            => GlobalFileInstances.AddOrUpdate(path, file, (key, oldValue) => file);

        public void RemoveGlobalFile(string absolutePath)
            => GlobalFileInstances.TryRemove(absolutePath, out _);

        /// <summary>
        /// Instances of files that are loaded only once and are accessable by all global references to that file.
        /// </summary>
        public static ConcurrentDictionary<string, IFileObject> GlobalFileInstances { get; }
            = new ConcurrentDictionary<string, IFileObject>();

        public virtual void Stop()
        {
            Console.WriteLine($"Stopping self.");
            AppDomain.CurrentDomain.AssemblyLoad -= AppDomainHelper.CurrentDomain_AssemblyLoad;

            Engine.ShutDown();
            SetRenderTicking(false);
            ResetTypeCaches(false);

            Sponsor?.Release();

            OnStopped();
        }

        public bool IsRenderTicking { get; private set; }
        public void SetRenderTicking(bool isRendering)
        {
            if (isRendering && !IsRenderTicking)
            {
                IsRenderTicking = true;
                Engine.RegisterRenderTick(RenderTick, CollectVisibleTick, SwapBuffersTick);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterRenderTick(RenderTick, CollectVisibleTick, SwapBuffersTick);
            }
        }
        //public void Render(RenderContext ctx)
        //{
        //    if (ctx is null || ctx.IsContextDisposed())
        //        return;

        //    if (Monitor.TryEnter(ctx))
        //    {
        //        try
        //        {
        //            ctx.Render();
        //        }
        //        finally { Monitor.Exit(ctx); }
        //    }
        //}

        public IFileObject LoadRef(IFileRef fref) => fref.GetInstance();

        public RenderContext GetContext(EPanelType type)
        {
            return type switch
            {
                EPanelType.World => WorldPanel,
                EPanelType.Hovered => Hovered,
                EPanelType.Focused => Focused,
                EPanelType.Rendering => Captured,
                _ => null,
            };
        }

        private void CollectVisibleTick(object sender, FrameEventArgs e)
        {
            foreach (WorldManager m in WorldManagers)
            {
                if (m is null || m.AssociatedContexts.Count == 0)
                    continue;

                try
                {
                    m.GlobalCollectVisible(e.Time);
                    foreach (var ctx in m.AssociatedContexts)
                        ctx.CollectVisible();
                }
                catch { }
            }
        }
        private void SwapBuffersTick(object sender, FrameEventArgs e)
        {
            foreach (WorldManager m in WorldManagers)
            {
                if (m is null)
                    continue;

                m.SwapBuffers();

                if (m.AssociatedContexts.Count == 0)
                    continue;

                m.GlobalSwap(e.Time);

                try
                {
                    foreach (var ctx in m.AssociatedContexts)
                        ctx.SwapBuffers();
                }
                catch { }
            }
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            while (Engine.DisposingRenderContexts.TryDequeue(out RenderContext ctx))
                ctx?.Dispose();

            foreach (WorldManager m in WorldManagers)
            {
                if (m is null || m.AssociatedContexts.Count == 0)
                    continue;

                //m.AssociatedContexts[0].Capture(true);
                //m.GlobalPreRender(e.Time);

                try
                {
                    foreach (var ctx in m.AssociatedContexts)
                    {
                        ctx.Capture();
                        m.GlobalPreRender(e.Time);
                        ctx.Render();
                    }
                }
                catch { }
            }
        }
        public virtual void ResetTypeCaches(bool reloadNow = true)
        {
            //Trace.WriteLine($"{(reloadNow ? "Regenerating" : "Clearing")} type caches.");

            BaseObjectSerializer.ClearObjectSerializerCache();
            if (reloadNow)
                _ = BaseObjectSerializer.ObjectSerializers.Value;
            
            ClearThirdPartyTypeCache(reloadNow);
            Reset3rdPartyImportExportMethods(reloadNow);

            ReloadTypeCaches?.Invoke(reloadNow);

            Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Done {(reloadNow ? "regenerating" : "clearing")} type caches.");
        }

        public Delegate Get3rdPartyLoader(TypeProxy fileType, string extension)
        {
            if (_3rdPartyLoaders is null)
                Reset3rdPartyImportExportMethods();
            return Get3rdPartyMethod(_3rdPartyLoaders, fileType, extension);
        }
        //public Delegate Get3rdPartyExporter(TypeProxy fileType, string extension)
        //{
        //    if (_3rdPartyExporters is null)
        //        Reset3rdPartyImportExportMethods();
        //    return Get3rdPartyMethod(_3rdPartyExporters, fileType, extension);
        //}
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
            if (_3rdPartyLoaders is null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        //public void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethod<T> exportMethod) where T : class, IFileObject
        //{
        //    if (_3rdPartyExporters is null)
        //        Reset3rdPartyImportExportMethods();
        //    Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        //}
        public void Register3rdPartyLoader<T>(string extension, Del3rdPartyImportFileMethodAsync<T> loadMethod) where T : class, IFileObject
        {
            if (_3rdPartyLoaders is null)
                Reset3rdPartyImportExportMethods();
            Register3rdParty<T>(_3rdPartyLoaders, extension, loadMethod);
        }
        //public void Register3rdPartyExporter<T>(string extension, Del3rdPartyImportFileMethodAsync<T> exportMethod) where T : class, IFileObject
        //{
        //    if (_3rdPartyExporters is null)
        //        Reset3rdPartyImportExportMethods();
        //    Register3rdParty<T>(_3rdPartyExporters, extension, exportMethod);
        //}
        private static void Register3rdParty<T>(
            Dictionary<string, Dictionary<TypeProxy, Delegate>> methodDic,
            string extension,
            Delegate method)
            where T : class, IFileObject
        {
            extension = extension.ToLowerInvariant();

            if (methodDic is null)
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
                //_3rdPartyExporters = new Dictionary<string, Dictionary<TypeProxy, Delegate>>();
                try
                {
                    TypeProxy[] types = AppDomainHelper.FindTypes(t => t.IsSubclassOf(typeof(TFileObject)) && !t.IsAbstract).ToArray();
                    foreach (TypeProxy type in types)
                    {
                        TFileExt attrib = GetFileExtension(type);
                        if (attrib is null)
                            continue;

                        ReadLoaders(_3rdPartyLoaders, type, attrib.ImportableExtensions);
                        //ReadLoaders(_3rdPartyExporters, type, attrib.ExportableExtensions);
                    }
                }
                catch { }
            }
            else
            {
                _3rdPartyLoaders = null;
                //_3rdPartyExporters = null;
            }
        }
        private void ReadLoaders(IDictionary<string, Dictionary<TypeProxy, Delegate>> loaders, TypeProxy type, IEnumerable<string> extensions)
        {
            foreach (string ext3rd in extensions)
            {
                string ext = ext3rd.ToLowerInvariant();

                Dictionary<TypeProxy, Delegate> extensionLoaders;
                if (loaders.ContainsKey(ext))
                    extensionLoaders = loaders[ext];
                else
                    loaders.Add(ext, extensionLoaders = new Dictionary<TypeProxy, Delegate>());

                if (extensionLoaders.ContainsKey(type))
                    throw new Exception($"{type.GetFriendlyName()} has already been added to the third party loader list for {ext}");

                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
                MethodInfoProxy m = type.GetMethods(flags)?.FirstOrDefault(IsLoaderMethod(ext));

                if (m is null)
                    continue;

                bool async =
                    m.ReturnType.GenericTypeArguments.Length > 0 &&
                    m.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

                TypeProxy delGenType = async
                    ? typeof(Del3rdPartyImportFileMethodAsync<>)
                    : typeof(Del3rdPartyImportFileMethod<>);

                try
                {
                    TypeProxy delType = delGenType.MakeGenericType(m.DeclaringType);
                    Delegate d = delType.CreateDelegate(m);
                    extensionLoaders.Add(type, d);
                }
                catch
                {
                    Console.WriteLine($"Cannot use {m.GetFriendlyName()} as a third party loader for {m.DeclaringType.GetFriendlyName()}.");
                }
            }
        }

        private static Func<MethodInfoProxy, bool> IsLoaderMethod(string ext) 
            => method => string.Equals(ext,
                method.GetCustomAttribute<ThirdPartyLoader>()?.Extension,
                StringComparison.InvariantCultureIgnoreCase);

        public TypeProxy GetTypeFor(object o) => o?.GetType();
        public TypeProxy GetTypeFor<T>() => typeof(T);
        public TypeProxy GetTypeFor(string typeName) => Type.GetType(typeName);

        public void ExportFile(IFileObject file, string dir, EProprietaryFileFormat format)
        {
            file.Export(dir, file.Name, ESerializeFlags.Default, format);
        }

        public T CreateInstance<T>(params object[] args) where T : ISponsorableMarshalByRefObject
            => (T)Activator.CreateInstance(typeof(T), args);

        public void LostFocus(IntPtr handle)
        {
            if (!Contexts.ContainsKey(handle))
                return;

            var ctx = Contexts[handle];
            if (Focused == ctx)
                Focused = null;
            Contexts[handle].LostFocus();
        }
        public void GotFocus(IntPtr handle)
        {
            if (!Contexts.ContainsKey(handle))
                return;

            var ctx = Contexts[handle];
            Focused = ctx;
            ctx.GotFocus();
        }
        public void MouseLeave(IntPtr handle)
        {
            if (!Contexts.ContainsKey(handle))
                return;

            var ctx = Contexts[handle];
            if (Hovered == ctx)
                Hovered = null;
            ctx.MouseLeave();
        }
        public void MouseEnter(IntPtr handle)
        {
            if (!Contexts.ContainsKey(handle))
                return;

            var ctx = Contexts[handle];
            Hovered = ctx;
            ctx.MouseEnter();
        }

        //TODO: editor world manager, model editor world manager, UI editor world managers
        public ConsistentIndexList<WorldManager> WorldManagers { get; } = new ConsistentIndexList<WorldManager>();
        public ConcurrentDictionary<IntPtr, RenderContext> Contexts { get; } = new ConcurrentDictionary<IntPtr, RenderContext>();

        public RenderContext VRContext { get; protected set; }

        public WorldManager GetWorldManager(int id) => WorldManagers[id];
        public T RegisterAndGetWorldManager<T>(params object[] args) where T : WorldManager
        {
            T manager = (T)Activator.CreateInstance(typeof(T), args);
            int index = WorldManagers.Add(manager);
            manager.ID = index;
            return manager;
        }
        public int RegisterWorldManager<T>(params object[] args) where T : WorldManager
        {
            WorldManager manager = (WorldManager)Activator.CreateInstance(typeof(T), args);
            int index = WorldManagers.Add(manager);
            manager.ID = index;
            return index;
        }
        public void UnregisterWorldManager(int index)
        {
            WorldManagers.RemoveAt(index);
        }
        public void UnlinkRenderPanelFromWorldManager(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                UnlinkContextFromWorldManager(Contexts[handle]);
        }
        public void UnlinkVRFromWorldManager()
        {
            UnlinkContextFromWorldManager(VRContext);
        }

        private void UnlinkContextFromWorldManager(RenderContext ctx)
        {
            var handler = ctx?.Handler;
            if (handler is null)
                return;

            int id = handler.WorldManager?.ID ?? -1;
            handler.WorldManager = null;
            if (WorldManagers.HasValueAtIndex(id))
                WorldManagers[id].RemoveContext(ctx);
        }

        public void LinkRenderPanelToWorldManager(IntPtr handle, int worldManagerId)
        {
            if (!Contexts.ContainsKey(handle))
                return;

            if (!WorldManagers.HasValueAtIndex(worldManagerId))
                return;

            UnlinkRenderPanelFromWorldManager(handle);

            WorldManager worldManager = WorldManagers[worldManagerId];
            RenderContext ctx = Contexts[handle];

            if (worldManager.AssociatedContexts.Contains(ctx))
                return;

            worldManager.AddContext(ctx);
            ctx.Handler.WorldManager = worldManager;
            Engine.Out("Linked render panel to world manager successfully.");
        }
        public void LinkVRToWorldManager(int worldManagerId)
        {
            if (!WorldManagers.HasValueAtIndex(worldManagerId))
                return;

            UnlinkContextFromWorldManager(VRContext);

            WorldManager worldManager = WorldManagers[worldManagerId];
            if (worldManager.AssociatedContexts.Contains(VRContext))
                return;

            worldManager.AddContext(VRContext);
            VRContext.Handler.WorldManager = worldManager;
            Engine.Out("Linked VR to world manager successfully.");
        }
        public void RegisterVRContext()
        {
            VRContext?.QueueDisposeSelf();

            //VRContext = new VRWrapperContext();

            var handler = new VRRenderHandler();
            Engine.Out($"CREATED RENDER HANDLER : {nameof(VRRenderHandler)}");

            switch (handler.RenderLibrary)
            {
                case ERenderLibrary.OpenGL:
                    VRContext = new GLWindowContext(null) { Handler = handler };
                    break;
                case ERenderLibrary.Direct3D11:
                    VRContext = new DXWindowContext(null) { Handler = handler };
                    break;
                default:
                    return;
            }

            Engine.Out("Registered VR context.");
        }
        public void UnregisterVRContext()
        {
            VRContext?.QueueDisposeSelf();
            VRContext = null;
            Engine.Out("Unregistered VR context.");
        }
        public void RegisterRenderPanel<T>(IntPtr handle, params object[] handlerArgs)
            where T : class, IRenderHandler
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle]?.QueueDisposeSelf();

            Type t = typeof(T);
            var handler = Activator.CreateInstance(t, handlerArgs) as BaseRenderHandler;
            Engine.Out($"CREATED RENDER HANDLER : {t.GetFriendlyName()}");

            RenderContext ctx;
            switch (handler.RenderLibrary)
            {
                case ERenderLibrary.OpenGL:
                    Contexts[handle] = ctx = new GLWindowContext(handle) { Handler = handler };
                    break;
                case ERenderLibrary.Direct3D11:
                    Contexts[handle] = ctx = new DXWindowContext(handle) { Handler = handler };
                    break;
                default:
                    return;
            }
            if (ctx != null)
            {
                Engine.Out($"Registered render panel {handler.GetType().GetFriendlyName()}");
            }
        }
        public void UnregisterRenderPanel(IntPtr handle)
        {
            if (!Contexts.ContainsKey(handle))
                return;
            
            Contexts.TryRemove(handle, out var ctx);
            ctx?.QueueDisposeSelf();

            Engine.Out("Unregistered render panel.");
        }
        public void RenderPanelResized(IntPtr handle, int width, int height)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].Resize(width, height);
        }
        public void UpdateScreenLocation(IntPtr handle, Point point)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].ScreenLocation = point;
        }
        public IRenderHandler MarshalRenderHandler(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                return Contexts[handle].Handler;
            return null;
        }

        public async Task<T> GetInstanceAsync<T>(FileRef<T> fileRef) where T : class, IFileObject
        {
            return await fileRef.LoadNewInstanceAsync();
        }

        public async virtual void LoadWorld(string filePath)
        {
            World world = await LoadAsync<World>(filePath);
            Engine.SetCurrentWorld(world);
        }

        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        //public override object InitializeLifetimeService()
        //{
        //    return null;
        //}
    }
}
