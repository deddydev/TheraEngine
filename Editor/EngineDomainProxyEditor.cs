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
using System.Threading;
using System.Collections.Generic;
using TheraEngine.Core.Maths;
using TheraEngine.Timers;
using System.Linq;
using TheraEngine.Core.Files.Serialization;
using System.IO;
using Microsoft.Build.Execution;
using Microsoft.Build.Evaluation;
using static TheraEditor.TProject;
using Microsoft.Build.Framework;
using TheraEngine.ThirdParty;
using static TheraEngine.ThirdParty.MSBuild;
using TheraEngine.Core.Files.XML;
using static TheraEngine.ThirdParty.MSBuild.Item;
using System.Runtime.Remoting;
using TheraEngine.Components;

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

        public void SaveWorld() => SaveFile(World);
        public void SaveWorldAs() => SaveFileAs(World);

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

        public async void ReloadNodeWrapperTypes(bool reloadNow = true)
        {
            Wrappers = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            ThirdPartyWrappers = new ConcurrentDictionary<string, TypeProxy>();

            if (reloadNow)
            {
                Stopwatch watch = Stopwatch.StartNew();

                Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Loading file wrappers.");

                await Task.Run(() => AppDomainHelper.ExportedTypes.ForEachParallelArray(EvaluateType));
                
                watch.Stop();

                Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] File wrappers loaded in {Math.Round((watch.ElapsedMilliseconds / 1000.0), 2).ToString()} seconds.");
            }
        }
        private void EvaluateType(TypeProxy asmType)
        {
            if (IsValidWrapperClassBase(asmType))
                return;

            if (asmType.AnyBaseTypeMatches(IsValidWrapperClassBase, out TypeProxy match))
            {
                //TODO: Third party wrapper?
                if (match == typeof(FileWrapper))
                {
                    TreeFileTypeAttribute wrapper = asmType.GetCustomAttribute<TreeFileTypeAttribute>();
                    string ext = wrapper?.ThirdPartyExtension;
                    if (!string.IsNullOrWhiteSpace(ext))
                        ThirdPartyWrappers[ext] = asmType;
                    else
                        Engine.LogWarning($"{asmType.GetFriendlyName()} is derived from '{nameof(FileWrapper)}' and needs to specify a '{nameof(TreeFileTypeAttribute)}' attribute with {nameof(TreeFileTypeAttribute.ThirdPartyExtension)} set to a valid extension.");
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

        public async void NewFile(TypeProxy type, string dirPath)
        {
            object o = Editor.UserCreateInstanceOf(type, true);
            if (!(o is IFileObject file))
                return;

            string typeName = type.GetFriendlyName();
            await RunOperationAsync(
                $"Exporting new {typeName} file to folder {dirPath}",
                $"Successfully exported new {typeName} file to folder {dirPath}", 
                async (p, c) => await file.ExportAsync(
                    dirPath, file.Name, ESerializeFlags.Default, EProprietaryFileFormat.XML, p, c.Token));

            //if (Serializer.PreExport(file, dir, file.Name, EProprietaryFileFormat.XML, null, out string path))
            //{
            //    int op = Editor.Instance.BeginOperation($"Exporting {path}...", $"Export to {path} completed.", out Progress<float> progress, out CancellationTokenSource cancel);
            //    string name = file.Name;
            //    name = name.Replace("<", "[");
            //    name = name.Replace(">", "]");
            //    await Serializer.ExportXMLAsync(file, dir, name, ESerializeFlags.Default, progress, cancel.Token);
            //    Editor.Instance.EndOperation(op);
            //}
        }

        private static bool IsValidWrapperClassBase(TypeProxy type)
            => (type == typeof(FileWrapper)) || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(FileWrapper<>));

        public void ReloadEditorTypes(bool reloadNow = true)
        {
            if (Engine.DesignMode)
                return;

            InPlaceEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            FullEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());

            if (reloadNow)
            {
                Engine.PrintLine(EOutputVerbosity.Verbose, "Loading all editor types for property grid.");
                Stopwatch watch = Stopwatch.StartNew();
                Task propEditorsTask = Task.Run(AddPropControlEditorTypes);
                Task fullEditorsTask = Task.Run(AddFullEditorTypes);
                Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                    Engine.PrintLine(EOutputVerbosity.Verbose, $"Finished loading all editor types for property grid in {Math.Round(watch.ElapsedMilliseconds / 1000.0, 2).ToString()} seconds."));
            }
        }
        private void AddPropControlEditorTypes()
        {
            var propControls = AppDomainHelper.FindTypes(x =>
                !x.IsAbstract &&
                x.IsSubclassOf(typeof(PropGridItem)));

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
                x.HasCustomAttribute<EditorForAttribute>());

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

        /// <summary>
        /// Generates a wrapper that houses a right click menu 
        /// with various options for editing the file or folder at the given path.
        /// </summary>
        /// <param name="path">The path to wrap.</param>
        /// <returns>The wrapper.</returns>
        public IBasePathWrapper TryWrapPath(string path)
        {
            IBasePathWrapper typeWrapper;
            if (path.IsExistingDirectoryPath() == true)
                typeWrapper = new FolderWrapper();
            else
            {
                string ext = Path.GetExtension(path);

                if (ext.Length > 0)
                    ext = ext.Substring(1).ToLowerInvariant();

                typeWrapper = TryWrap3rdPartyExt(ext);
                if (typeWrapper is null)
                {
                    TypeProxy type = !string.IsNullOrWhiteSpace(path) && File.Exists(path) ? TFileObject.DetermineType(path, out _) : null;
                    typeWrapper = TryWrapProprietaryType(type);
                    if (typeWrapper is null)
                        typeWrapper = new UnknownFileWrapper();
                }
            }
            typeWrapper.FilePath = path;
            return typeWrapper;
        }
        private IBasePathWrapper TryWrap3rdPartyExt(string ext)
        {
            var wrappers = ThirdPartyWrappers;

            if (wrappers.TryGetValue(ext, out TypeProxy wrapperType))
                return wrapperType.CreateInstance() as IBasePathWrapper;

            return null;
        }
        private IBasePathWrapper TryWrapProprietaryType(TypeProxy type)
        {
            if (type is null)
                return null;

            IBasePathWrapper wrapper = null;

            //Try to find wrapper for type or any inherited type, in order
            var wrappers = Wrappers;
            TypeProxy currentType = type;
            while (!(currentType is null) && wrapper is null)
            {
                if (wrappers.TryGetValue(currentType, out TypeProxy wrapperType))
                    wrapper = wrapperType.CreateInstance() as IBasePathWrapper;
                else
                {
                    TypeProxy[] interfaces = currentType.GetInterfaces();
                    var validInterfaces = interfaces.Where(interfaceType => wrappers.Keys.Any(wrapperKeyType => wrapperKeyType == interfaceType)).ToArray();
                    if (validInterfaces.Length > 0)
                    {
                        TypeProxy interfaceType;

                        //TODO: find best interface to use if multiple matches?
                        if (validInterfaces.Length > 1)
                        {
                            int[] numAssignableTo = validInterfaces.Select(match => validInterfaces.Count(other => other != match && other.IsAssignableTo(match))).ToArray();
                            int min = numAssignableTo.Min();
                            int[] mins = numAssignableTo.FindAllMatchIndices(x => x == min);
                            string msg = "File of type " + type.GetFriendlyName() + " has multiple valid interface wrappers: " + validInterfaces.ToStringList(", ", " and ", x => x.GetFriendlyName());
                            msg += ". Narrowed down wrappers to " + mins.Select(x => validInterfaces[x]).ToArray().ToStringList(", ", " and ", x => x.GetFriendlyName());
                            Engine.PrintLine(msg);
                            interfaceType = validInterfaces[mins[0]];
                        }
                        else
                            interfaceType = validInterfaces[0];

                        if (wrappers.TryGetValue(interfaceType, out wrapperType))
                            wrapper = wrapperType.CreateInstance() as IBasePathWrapper;
                    }
                }

                currentType = currentType.BaseType;
            }

            if (wrapper is null)
            {
                //Make wrapper for whatever file type this is
                wrapper = new FileWrapper() { FileType = type };
                //TypeProxy genericFileWrapper = TypeProxy.Get(typeof(FileWrapper<>)).MakeGenericType(t);
                //w = Activator.CreateInstance((Type)genericFileWrapper) as BaseFileWrapper;
            }

            return wrapper;
        }

        public override void ResetTypeCaches(bool reloadNow = true)
        {
            Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] {(reloadNow ? "Regenerating" : "Clearing")} type caches");

            ReloadNodeWrapperTypes(reloadNow);
            ReloadEditorTypes(reloadNow);
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

        public void NewSceneComponentChild(ISceneComponent parent)
        {
            ISceneComponent newComp = Editor.UserCreateInstanceOf<ISceneComponent>(true, Editor.Instance);
            if (newComp != null)
                parent.ChildComponents.Add(newComp);
        }

        public T UserCreateInstanceOf<T>(bool allowDerivedTypes = true)
            => Editor.UserCreateInstanceOf<T>(allowDerivedTypes, Editor.Instance);
        public object UserCreateInstanceOf(TypeProxy dataType, bool allowDerivedTypes = true)
            => Editor.UserCreateInstanceOf(dataType, allowDerivedTypes, Editor.Instance);

        protected override void OnStarted()
        {
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            Engine.PostWorldChanged += Engine_PostWorldChanged;
            Engine.PreWorldChanged += Engine_PreWorldChanged;

            base.OnStarted();
        }

        public async override void LoadWorld(string filePath)
        {
            if (filePath.IsExistingDirectoryPath() != false)
                return;

            World world = await RunOperationAsync(
                "Loading world from " + filePath, 
                "World loaded successfully.",
                async (p, c, a) => await TFileObject.LoadAsync<World>(filePath, p, c.Token),
                null);

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
            SetEditorGameMode();
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

        public bool TryCloseWorld()
        {
            if (World?.EditorState?.HasChanges ?? false)
            {
                DialogResult result = MessageBox.Show(null,
                    "Save changes to current world?", "Save changes?",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);

                switch (result)
                {
                    default:
                    case DialogResult.Cancel:
                        return false;
                    case DialogResult.Yes:
                        Task.Run(() => SaveFile(World)).ContinueWith(t => World.EditorState = null);
                        return true;
                }
            }

            World = null;
            return true;
        }

        public void SaveFile(IFileObject file)
        {
            if (file is null)
                return;

            if (string.IsNullOrEmpty(file.FilePath))
                SaveFileAs(file);
            else
                SaveFileAs(file, file.FilePath);
        }
        public void SaveFileAs(IFileObject file)
        {
            if (file is null)
                return;

            string filter = TFileObject.GetFilter(file.GetType(), true, true, false, true);
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = filter })
            {
                if (sfd.ShowDialog(Editor.Instance) == DialogResult.OK)
                    SaveFileAs(file, sfd.FileName);
            }
        }
        public void SaveFileAs(IFileObject file, string filePath, ESerializeFlags flags = ESerializeFlags.Default) 
            => SaveFileAs(file, filePath, "Saving file to " + filePath, "File successfully saved to " + filePath, flags);
        public async void SaveFileAs(IFileObject file, string filePath, string beginMessage, string finishMessage, ESerializeFlags flags = ESerializeFlags.Default)
        {
            if (!filePath.IsValidPath())
                return;

            await RunOperationAsync(
                beginMessage,
                finishMessage,
                async (p, c) => await file.ExportAsync(filePath, flags, p, c.Token));
        }

        private List<OperationInfo> Operations { get; } = new List<OperationInfo>();
        public int OperationCount => Operations.Count;
        public int ProgressMinValue { get; set; } = 0;
        public int ProgressMaxValue { get; set; } = 100000;

        public async void ImportFile(TypeProxy fileType, string dir)
        {
            if (fileType.ContainsGenericParameters)
            {
                using (GenericsSelector gs = new GenericsSelector(fileType))
                {
                    if (gs.ShowDialog(Editor.Instance) == DialogResult.OK)
                        fileType = gs.FinalClassType;
                    else
                        return;
                }
            }

            string path, name;
            string filter = TFileObject.GetFilter((Type)fileType, true, true, true, false);
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = filter,
                Title = "Import File"
            })
            {
                DialogResult r = ofd.ShowDialog(Editor.Instance);
                if (r != DialogResult.OK)
                    return;

                name = Path.GetFileNameWithoutExtension(ofd.FileName);
                path = ofd.FileName;
            }

            object file = await RunOperationAsync(
                $"Importing '{path}'...",
                $"Import from '{path}' completed.", 
                async (p, c) => await TFileObject.LoadAsync(
                    (Type)fileType, path, p, c.Token));

            if (!(file is IFileObject iobj))
                return;

            await RunOperationAsync(
                $"Saving file...", 
                $"Saved file successfully.", 
                async (p, c) => await iobj.ExportAsync(
                    dir, name, ESerializeFlags.Default, EProprietaryFileFormat.XML, p, c.Token));
        }

        public int TargetOperationValue { get; private set; }

        public async void RunOperationAsync2<T>(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, object[], Task<T>> task,
            TimeSpan? maxOperationTime,
            params object[] args)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out MarshalProgress<float> progress, out CancellationTokenSource cancel,
                maxOperationTime);

            await task(progress, cancel, args);

            EndOperation(index);
        }

        public async Task<T> RunOperationAsync<T>(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, object[], Task<T>> task,
            TimeSpan? maxOperationTime,
            params object[] args)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out MarshalProgress<float> progress, out CancellationTokenSource cancel,
                maxOperationTime);

            T value = await task(progress, cancel, args);

            EndOperation(index);

            return value;
        }
        public async Task<T> RunOperationAsync<T>(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, Task<T>> task)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out MarshalProgress<float> progress, out CancellationTokenSource cancel);

            T value = await task(progress, cancel);

            EndOperation(index);

            return value;
        }
        public async Task RunOperationAsync(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, object[], Task> task,
            TimeSpan? maxOperationTime,
            params object[] args)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out MarshalProgress<float> progress, out CancellationTokenSource cancel,
                maxOperationTime);

            await task(progress, cancel, args);

            EndOperation(index);
        }
        public async Task RunOperationAsync(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, Task> task)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out MarshalProgress<float> progress, out CancellationTokenSource cancel);

            await task(progress, cancel);

            EndOperation(index);
        }

        public int BeginOperation(
            string statusBarMessage, string finishedMessage,
            out MarshalProgress<float> progress, out CancellationTokenSource cancel,
            TimeSpan? maxOperationTime = null)
        {
            Engine.PrintLine(statusBarMessage);

            bool firstOperationAdded = Operations.Count == 0;
            int index = Operations.Count;

            progress = new MarshalProgress<float>();
            cancel = maxOperationTime is null ? new CancellationTokenSource() : new CancellationTokenSource(maxOperationTime.Value);

            Operations.Add(new OperationInfo(progress, cancel, OnOperationProgressUpdate, index, statusBarMessage, finishedMessage));

            Editor.Instance.UpdateUI(firstOperationAdded, statusBarMessage, Operations.Any(x => x != null && x.CanCancel));

            return index;
        }

        private void OnOperationProgressUpdate(int operationIndex)
        {
            float avgProgress = 0.0f;
            int valid = 0;
            for (int i = 0; i < Operations.Count; ++i)
            {
                OperationInfo info = Operations[i];
                if (info != null)
                {
                    avgProgress += info.ProgressValue;
                    if (info.IsComplete)
                        EndOperation(i--);
                    else
                        ++valid;
                }
            }

            if (valid == 0)
                return;

            avgProgress /= valid;

            int maxValue = ProgressMaxValue;
            int minValue = ProgressMinValue;

            int value = (int)Math.Round(Interp.Lerp(minValue, maxValue, avgProgress));
            TargetOperationValue = value;
            //toolStripProgressBar1.ProgressBar.Value = TargetOperationValue;
        }

        public void EndOperation(int index)
        {
            if (Operations.IndexInRange(index))
            {
                var info = Operations[index];
                Operations[index] = null;

                double sec = Math.Round(info.OperationDuration.TotalSeconds, 2, MidpointRounding.AwayFromZero);
                string completeTime = " (Completed in ";
                if (sec < 1.0)
                    completeTime += (sec * 1000.0).ToString() + " ms)";
                else
                    completeTime += sec.ToString() + " sec)";

                string message = info.FinishedMessage + completeTime;
                Editor.Instance.SetOperationMessage(message);
            }

            if (Operations.Count == 0 || Operations.All(x => x is null))
            {
                Operations.Clear();
                Editor.Instance.OperationsEnded();
                TargetOperationValue = 0;
            }
            else if (Operations.Count(x => x != null) > 1)
            {
                //toolStripStatusLabel1.Text = "Waiting for multiple operations to finish...";
            }
            else
            {
                var op = Operations.FirstOrDefault(x => x != null);
            }
        }

        public void CancelOperations()
        {
            for (int i = 0; i < Operations.Count; ++i)
                Operations[i]?.Cancel();

            EndOperation(-1);
        }

        private class OperationInfo
        {
            private readonly Action<int> _updated;
            private CancellationTokenSource _token;

            public int Index { get; }
            public DateTime StartTime { get; }
            public TimeSpan OperationDuration => DateTime.Now - StartTime;
            public MarshalProgress<float> Progress { get; }
            public float ProgressValue { get; private set; } = 0.0f;
            public bool IsComplete => ProgressValue >= 0.99f;
            public bool CanCancel => _token != null && _token.Token.CanBeCanceled;
            public string StatusBarMessage { get; set; }
            public string FinishedMessage { get; set; }

            public OperationInfo(MarshalProgress<float> progress, CancellationTokenSource cancel, Action<int> updated, int index, string statusBarMessage, string finishedMessage)
            {
                _updated = updated;
                Progress = progress;
                if (Progress != null)
                    Progress.ProgressChanged += Progress_ProgressChanged;
                _token = cancel;
                StartTime = DateTime.Now;
                Index = index;
                StatusBarMessage = statusBarMessage;
                FinishedMessage = finishedMessage;
            }
            private void Progress_ProgressChanged(object sender, float progressValue)
            {
                ProgressValue = progressValue;
                _updated(Index);
            }
            public void Cancel()
            {
                Editor.DomainProxy.Operations[Index] = null;
                _token?.Cancel();
                if (Progress != null)
                    Progress.ProgressChanged -= Progress_ProgressChanged;
            }
        }
    }
}
