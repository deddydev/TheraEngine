using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core;
using TheraEngine.Core.Reflection;
using Extensions;
using TheraEditor.Wrappers;
using System.Diagnostics;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;
using TheraEngine.GameModes;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Input;

namespace TheraEditor
{
    /// <summary>
    /// Proxy that runs the engine in the game's domain.
    /// </summary>
    //[Serializable]
    public class EngineDomainProxyEditor : EngineDomainProxy
    {
        public UndoManager UndoManager { get; } = new UndoManager();

        public EditorGameMode EditorGameMode { get; set; } = new EditorGameMode();
        public IWorld World
        {
            get => Engine.World;
            set => SetWorld_Internal(value);
        }

        public event Action PostWorldChanged;
        public event Action PreWorldChanged;

        private void Engine_PreWorldChanged() => PreWorldChanged?.Invoke();
        private void Engine_PostWorldChanged() => PostWorldChanged?.Invoke();

        public ConcurrentDictionary<TypeProxy, TypeProxy> FullEditorTypes { get; private set; }
        public ConcurrentDictionary<TypeProxy, TypeProxy> InPlaceEditorTypes { get; private set; }

        public void AddRenderHandlerToEditorGameMode(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                EditorGameMode.TargetRenderHandlers.Add(Contexts[handle].Handler);
        }
        public void RemoveRenderHandlerFromEditorGameMode(IntPtr handle)
        {
            if (Contexts.ContainsKey(handle))
                EditorGameMode.TargetRenderHandlers.Remove(Contexts[handle].Handler);
        }

        /// <summary>
        /// Key is file type, Value is tree node wrapper type
        /// </summary>
        public ConcurrentDictionary<TypeProxy, TypeProxy> Wrappers { get; private set; }
        /// <summary>
        /// Key is lowercase extension, Value is tree node wrapper type
        /// </summary>
        public ConcurrentDictionary<string, TypeProxy> ThirdPartyWrappers { get; private set; }

        public bool CanUndo => UndoManager.CanUndo;
        public bool CanRedo => UndoManager.CanRedo;
        public void Undo(int count = 1) => UndoManager.Undo(count);
        public void Redo(int count = 1) => UndoManager.Redo(count);

        public async void ReloadNodeWrapperTypes()
        {
            Wrappers = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            ThirdPartyWrappers = new ConcurrentDictionary<string, TypeProxy>();

            Stopwatch watch = Stopwatch.StartNew();
            Engine.PrintLine(EOutputVerbosity.Verbose, "Loading file wrappers.");
            await Task.Run(() => Parallel.ForEach(AppDomainHelper.ExportedTypes, EvaluateType)).ContinueWith(t =>
            {
                watch.Stop();
                Engine.PrintLine(EOutputVerbosity.Verbose, $"File wrappers loaded in {Math.Round((watch.ElapsedMilliseconds / 1000.0), 2).ToString()} seconds.");
            });
        }
        private void EvaluateType(TypeProxy asmType)
        {
            if (IsValidWrapperClassBase(asmType))
                return;

            if (asmType.AnyBaseTypeMatches(IsValidWrapperClassBase, out TypeProxy match))
            {
                if (match == typeof(ThirdPartyFileWrapper))
                {
                    NodeWrapperAttribute wrapper = asmType.GetCustomAttribute<NodeWrapperAttribute>();
                    string ext = wrapper?.ThirdPartyExtension;
                    if (!string.IsNullOrWhiteSpace(ext))
                        ThirdPartyWrappers[ext] = asmType;
                    else
                        Engine.LogWarning($"{asmType.GetFriendlyName()} is derived from '{nameof(ThirdPartyFileWrapper)}' and needs to specify a '{nameof(NodeWrapperAttribute)}' attribute with {nameof(NodeWrapperAttribute.ThirdPartyExtension)} set to a valid extension.");
                }
                else
                {
                    TypeProxy fileType = match.GetGenericArguments()[0];
                    Wrappers[fileType] = asmType;
                }
            }
            //else
            //    throw new InvalidOperationException($"{nameof(NodeWrapperAttribute)} must be an attribute on a class that inherits from {nameof(FileWrapper<IFileObject>)} or {nameof(ThirdPartyFileWrapper)}.");
        }

        private static bool IsValidWrapperClassBase(TypeProxy type)
            => (type == typeof(ThirdPartyFileWrapper)) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FileWrapper<>));

        public void ReloadEditorTypes()
        {
            if (Engine.DesignMode)
                return;

            InPlaceEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            FullEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());

            Engine.PrintLine(EOutputVerbosity.Verbose, "Loading all editor types for property grid.");
            Stopwatch watch = Stopwatch.StartNew();
            Task propEditorsTask = Task.Run(AddPropControlEditorTypes);
            Task fullEditorsTask = Task.Run(AddFullEditorTypes);
            Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                Engine.PrintLine(EOutputVerbosity.Verbose, $"Finished loading all editor types for property grid in {Math.Round(watch.ElapsedMilliseconds / 1000.0, 2).ToString()} seconds."));
        }
        private void AddPropControlEditorTypes()
        {
            var propControls = AppDomainHelper.FindTypes(x =>
                !x.IsAbstract &&
                x.IsSubclassOf(typeof(PropGridItem)),
                Assembly.GetExecutingAssembly());

            propControls.ForEachParallel(AddPropControlEditorType);
        }
        private void AddPropControlEditorType(TypeProxy propControlType)
        {
            var attribs = propControlType.GetCustomAttributes<PropGridControlForAttribute>().ToArray();
            if (attribs.Length > 0)
            {
                PropGridControlForAttribute a = attribs[0];
                foreach (Type varType in a.Types)
                {
                    //if (!_inPlaceEditorTypes.ContainsKey(varType))
                    InPlaceEditorTypes.AddOrUpdate(varType, propControlType, (x, y) => propControlType);
                    Engine.PrintLine(EOutputVerbosity.Verbose, $"{propControlType.GetFriendlyName()} is the editor for {varType.GetFriendlyName()}.");
                    //else
                    //    throw new Exception("Type " + varType.GetFriendlyName() + " already has control " + propControlType.GetFriendlyName() + " associated with it.");
                }
            }
        }
        private void AddFullEditorTypes()
        {
            var fullEditors = AppDomainHelper.FindTypes(x =>
                !x.IsAbstract &&
                x.IsSubclassOf(typeof(Form)) &&
                x.HasCustomAttribute<EditorForAttribute>(),
                Assembly.GetExecutingAssembly());

            fullEditors.ForEachParallel(AddFullEditorType);
        }
        private void AddFullEditorType(TypeProxy editorType)
        {
            var attrib = editorType.GetCustomAttribute<EditorForAttribute>();
            foreach (TypeProxy varType in attrib.DataTypes)
            {
                //if (!_fullEditorTypes.ContainsKey(varType))
                FullEditorTypes.AddOrUpdate(varType, editorType, (x, y) => editorType);
                //else
                //    throw new Exception("Type " + varType.GetFriendlyName() + " already has editor " + editorType.GetFriendlyName() + " associated with it.");
            }
        }
        public override void ResetTypeCaches(bool reloadNow = true)
        {
            ReloadNodeWrapperTypes();
            ReloadEditorTypes();
            base.ResetTypeCaches(reloadNow);
        }
        //public override void Start(string gamePath, bool isUIDomain)
        //{
        //    base.Start(gamePath, isUIDomain);
        //}
        public override void Stop()
        {
            Engine.PostWorldChanged -= Engine_PostWorldChanged;
            Engine.PreWorldChanged -= Engine_PreWorldChanged;

            base.Stop();
        }
        protected override void OnStarted()
        {
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            Engine.PostWorldChanged += Engine_PostWorldChanged;
            Engine.PreWorldChanged += Engine_PreWorldChanged;

            base.OnStarted();
        }

        public async override void SetWorld(string filePath)
        {
            World world = await TFileObject.LoadAsync<World>(filePath);
            SetWorld_Internal(world);
        }
        public void CreateNewWorld()
        {
            World world = new World();
            SetWorld_Internal(world);
        }
        private void SetWorld_Internal(IWorld world)
        {
            Engine.SetCurrentWorld(world);
            if (world != null)
                world.CurrentGameMode = EditorGameMode;
        }
        public void SetEditorGameMode()
        {
            IWorld world = Engine.World;
            if (world != null)
                world.CurrentGameMode = CurrentGameMode = EditorGameMode;
        }
        public IGameMode CurrentGameMode { get; set; }
        public void SetGameplayMode()
        {
            IWorld world = Engine.World;
            if (world != null)
            {
                CurrentGameMode = Engine.GetGameMode() ?? new GameMode<FlyingCameraPawn, LocalPlayerController>();
                world.CurrentGameMode = CurrentGameMode;
            }
        }
    }
}
