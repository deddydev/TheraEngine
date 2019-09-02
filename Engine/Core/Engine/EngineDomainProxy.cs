using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using TheraEngine.Core.Files;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;
using TheraEngine.Rendering;
using TheraEngine.Rendering.DirectX;
using TheraEngine.Rendering.OpenGL;
using TheraEngine.Timers;
using static TheraEngine.Core.Files.TFileObject;
using static TheraEngine.Rendering.RenderContext;

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

        public RenderContext WorldPanel { get; set; }
        public RenderContext HoveredPanel { get; set; }
        public RenderContext FocusedPanel { get; set; }
        public RenderContext RenderingPanel { get; set; }

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

            SetRenderTicking(true);
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

        public bool IsRenderTicking { get; private set; }
        public void SetRenderTicking(bool isRendering)
        {
            if (isRendering && !IsRenderTicking)
            {
                IsRenderTicking = true;
                Engine.RegisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
            else if (!isRendering && IsRenderTicking)
            {
                IsRenderTicking = false;
                Engine.UnregisterTick(RenderTick, UpdateTick, SwapBuffers);
            }
        }
        //public void Render(RenderContext ctx)
        //{
        //    if (ctx == null || ctx.IsContextDisposed())
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
        public RenderContext GetContext(EPanelType type)
        {
            switch (type)
            {
                case EPanelType.World: return WorldPanel;
                case EPanelType.Hovered: return HoveredPanel;
                case EPanelType.Focused: return FocusedPanel;
                case EPanelType.Rendering: return RenderingPanel;
            }
            return null;
        }

        private void UpdateTick(object sender, FrameEventArgs e)
        {
            Engine.Instance.GlobalUpdate();
            foreach (var ctx in Contexts.Values)
                ctx.Update();
        }
        private void SwapBuffers()
        {
            Engine.Instance.GlobalSwap();
            foreach (var ctx in Contexts.Values)
                ctx.SwapBuffers();
        }
        private void RenderTick(object sender, FrameEventArgs e)
        {
            WorldPanel?.Capture();
            Engine.Instance.GlobalPreRender();

            //TODO: group contexts by world.
            //Do a global pre-render per world.
            foreach (var ctx in Contexts.Values)
                ctx.Render();
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

        public void LostFocus(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].LostFocus();
        }
        public void GotFocus(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].GotFocus();
        }
        public void MouseLeave(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].MouseLeave();
        }
        public void MouseEnter(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle].MouseEnter();
        }

        public ConcurrentDictionary<IntPtr, RenderContext> Contexts { get; } = new ConcurrentDictionary<IntPtr, RenderContext>();
        public void RegisterRenderPanel<T>(IntPtr handle, params object[] handlerArgs)
            where T : class, IRenderHandler
        {
            if (Contexts.ContainsKey(handle))
                Contexts[handle]?.Dispose();
            RenderContext ctx;
            var rh = Activator.CreateInstance(typeof(T), handlerArgs) as BaseRenderHandler;
            switch (Engine.RenderLibrary)
            {
                case ERenderLibrary.OpenGL:
                    Contexts[handle] = ctx = new GLWindowContext(handle) { Handler = rh };
                    break;
                case ERenderLibrary.Direct3D11:
                    Contexts[handle] = ctx = new DXWindowContext(handle) { Handler = rh };
                    break;
                default:
                    return;
            }
            if (ctx != null)
            {
                ctx.Capture(true);
                ctx.Initialize();

                Engine.PrintLine("Registered render panel " + rh.GetType().GetFriendlyName());
            }
        }
        public void UnregisterRenderPanel(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
            {
                Contexts[handle]?.Dispose();
                Contexts.TryRemove(handle, out _);
            }
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

        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        //public override object InitializeLifetimeService()
        //{
        //    return null;
        //}
    }
}
