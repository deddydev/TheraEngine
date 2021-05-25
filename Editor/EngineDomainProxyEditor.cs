using Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEditor.Wrappers;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Actors.Types.Pawns;
using TheraEngine.Components;
using TheraEngine.Core;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Reflection;
using TheraEngine.GameModes;
using TheraEngine.Input;
using TheraEngine.Input.Devices;
using TheraEngine.Rendering;
using TheraEngine.Worlds;

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

        //public void AddRenderHandlerToEditorGameMode(IntPtr handle)
        //{
        //    //if (Contexts.ContainsKey(handle))
        //    //    EditorGameMode.TargetRenderHandlers.Add(Contexts[handle].Handler);
        //}

        public void SaveWorld() => SaveFile(World);
        public void SaveWorldAs() => SaveFileAs(World);

        //public void RemoveRenderHandlerFromEditorGameMode(IntPtr handle)
        //{
        //    if (Contexts.ContainsKey(handle))
        //        EditorGameMode.TargetRenderHandlers.Remove(Contexts[handle].Handler);
        //}

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

                Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Loading file wrappers.");

                await Task.Run(() => AppDomainHelper.ExportedTypes.ForEachParallelArray(EvaluateType));
                
                watch.Stop();

                Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] File wrappers loaded in {Math.Round((watch.ElapsedMilliseconds / 1000.0), 2).ToString()} seconds.");
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
                        Console.WriteLine($"{asmType.GetFriendlyName()} is derived from '{nameof(FileWrapper)}' and needs to specify a '{nameof(TreeFileTypeAttribute)}' attribute with {nameof(TreeFileTypeAttribute.ThirdPartyExtension)} set to a valid extension.");
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

        public async void CheckUpdates()
        {
            try
            {
                AssemblyName editorVer = Assembly.GetExecutingAssembly().GetName();
                await Github.Updater.TryInstallUpdate(editorVer);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
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
                Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Loading all editor types for property grid.");
                Stopwatch watch = Stopwatch.StartNew();
                Task propEditorsTask = Task.Run(AddPropControlEditorTypes);
                Task fullEditorsTask = Task.Run(AddFullEditorTypes);
                Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                     Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Finished loading all editor types for property grid in {Math.Round(watch.ElapsedMilliseconds / 1000.0, 2).ToString()} seconds."));
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
                    Console.WriteLine($"{propControlType.GetFriendlyName()} is the prop grid editor for {varType.GetFriendlyName()}.");
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
                Console.WriteLine($"{editorType.GetFriendlyName()} is the full editor for {varType.GetFriendlyName()}.");
                //else
                //    throw new Exception("Type " + varType.GetFriendlyName() + " already has editor " + editorType.GetFriendlyName() + " associated with it.");
            }
        }

        private string GetPlatform(bool x86) => x86 ? "x86" : "x64";
        //TODO: read sln and csproj files to get exact directory
        //in case build config changes in the future
        private string ResolveEnginePath(bool x86, string slnDir)
            => Path.Combine(slnDir, "Build", "Release", "Game", GetPlatform(x86), "TheraEngine.dll");
        private string ResolveEditorPath(bool x86, string slnDir)
            => Path.Combine(slnDir, "Build", "Release", "Editor", GetPlatform(x86), "TheraEditor.exe");

        public async void PostNewReleases(
            bool editor, bool engine, bool x86, string slnDir, string editorMsg, string engineMsg)
        {
            List<Task> tasks = new List<Task>();

            Github.ReleaseCreator creator = new Github.ReleaseCreator();
            string platform = x86 ? "x86" : "x64";

            if (editor)
            {
                string path = ResolveEditorPath(x86, slnDir);
                bool? check = CheckBuild(path, true);
                if (check != null && (check == true || ReleaseCreatorForm.Build("ReleaseEditor", platform, Path.Combine(slnDir, "Thera.sln"))))
                    tasks.Add(creator.New(path, editorMsg));
            }

            //if (engine)
            //{
            //    string path = ResolveEnginePath(x86, slnDir);
            //    bool? check = CheckBuild(path, false);
            //    if (check != null && (check == true || ReleaseCreatorForm.Build("ReleaseGame", platform, Path.Combine(slnDir, "Thera.sln"))))
            //        tasks.Add(creator.New(path, engineMsg));
            //}

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);
            else
                MessageBox.Show("Nothing was posted.");
        }
        private bool? CheckBuild(string path, bool isEditor)
        {
            if (File.Exists(path))
            {
                Type type = isEditor ? typeof(Editor) : typeof(Engine);

                FileVersionInfo fileInfo = FileVersionInfo.GetVersionInfo(path);
                string fileVersion = fileInfo.ProductVersion;

                string thisVersion = type.Assembly.GetName().Version.ToString();

                if (!string.Equals(fileVersion, thisVersion, StringComparison.InvariantCultureIgnoreCase))
                {
                    DialogResult result = MessageBox.Show(
                        $"File version {fileVersion} does not match current version {thisVersion}. Build before posting?",
                        "Build needed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                    switch (result)
                    {
                        default:
                        case DialogResult.Cancel:
                            return null;
                        case DialogResult.Yes:
                            return false;
                        case DialogResult.No:
                            return true;
                    }
                }

                return true;
            }
            else
            {
                DialogResult result = MessageBox.Show(
                        $"Build does not exist. Build before posting?",
                        "Build needed", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Exclamation);
                switch (result)
                {
                    default:
                    case DialogResult.Cancel:
                        return null;
                    case DialogResult.Yes:
                        return false;
                    case DialogResult.No:
                        return true;
                }
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

            if (wrappers != null && wrappers.TryGetValue(ext, out TypeProxy wrapperType))
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
            if (wrappers != null)
            {
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
                                Engine.Out(msg);
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
            Console.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] {(reloadNow ? "Regenerating" : "Clearing")} type caches");

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
                parent.ChildSockets.Add(newComp);
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

        public override void LoadWorld(string filePath)
        {
            if (filePath.IsExistingDirectoryPath() != false)
                return;

            RunOperationAsync(
                "Loading world from " + filePath, 
                "World loaded successfully.",
                async (p, c, a) => await TFileObject.LoadAsync<World>(filePath, p, c.Token),
                null).ContinueWith(t => SetWorld_Internal(t.Result));
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
                world.GameMode = CurrentGameMode = EditorGameMode;
        }
        public IGameMode CurrentGameMode { get; set; }
        public void SetGameplayMode()
        {
            IWorld world = Engine.World;
            if (world != null)
            {
                var mode = Engine.GetGameMode();
                CurrentGameMode = mode ?? new GameMode<FlyingCameraPawn, LocalPlayerController>();
                world.GameMode = CurrentGameMode;
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

            string filter = TFileObject.CreateFilter(file.GetType(), true, true, false, true);
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
            string filter = TFileObject.CreateFilter((Type)fileType, true, true, true, false);
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
            Engine.Out(statusBarMessage);

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

        #region Game Mode
        private void RegisterInput(InputInterface input)
        {
            input.RegisterKeyEvent(EKey.Escape, EButtonInputType.Pressed, EndGameplay, EInputPauseType.TickAlways);
        }
        private Rectangle _prevClip;
        private void CaptureMouse(Control panel)
        {
            //CursorManager.GlobalWrapCursorWithinClip = true;
            Engine.EditorState.InEditMode = false;
            panel.Focus();
            //panel.Capture = true;
            //_prevClip = Cursor.Clip;
            //Cursor.Clip = panel.RectangleToScreen(panel.ClientRectangle);
            //HideCursor();
        }
        private void ReleaseMouse()
        {
            //CursorManager.GlobalWrapCursorWithinClip = false;
            Engine.EditorState.InEditMode = true;
            //ShowCursor();
            //Cursor.Clip = _prevClip;
        }
        private EEditorGameplayState _gameState = EEditorGameplayState.Editing;
        public EEditorGameplayState GameState
        {
            get => _gameState;
            set
            {
                if (_gameState == value)
                    return;

                switch (value)
                {
                    case EEditorGameplayState.Attached: SetAttachedGameState(); break;
                    case EEditorGameplayState.Detached: SetDetachedGameState(); break;
                    case EEditorGameplayState.Editing: SetEditingGameState(); break;
                }

                _gameState = value;
            }
        }
        private void SetEditingGameState()
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke((Action)SetEditingGameState);
            //    return;
            //}

            //btnPlay.Text = "Play";
            //btnPlayDetached.Text = "Play Detached";

            var world = World;
            if (world is null)
                return;

            if (_gameState == EEditorGameplayState.Attached)
            {
                //Attached -> Editing

                ReleaseMouse();
            }
            else //Detached
            {
                //Detached -> Editing

                world.DespawnActor(FlyingCameraDetachedPawn);

                //Mouse is already released
            }

            //Both attached and detached use gameplay mode and are unpaused
            world.EndPlay();

            SetEditorGameMode();
            InputInterface.GlobalRegisters.Remove(RegisterInput);
            //ActorTreeForm.ClearMaps();

            world.BeginPlay();
            Engine.Pause(ELocalPlayerIndex.One, true);

            GameplayPawn = null;
        }

        public IPawn GameplayPawn { get; set; }

        private FlyingCameraPawn _flyingCameraPawn;
        public FlyingCameraPawn FlyingCameraDetachedPawn => _flyingCameraPawn ?? (_flyingCameraPawn = new FlyingCameraPawn());

        public EventList<RenderContext> BoundContexts => RenderContext.BoundContexts;

        private void SetDetachedGameState()
        {
            //if (InvokeRequired)
            //{
            //    BeginInvoke((Action)SetDetachedGameState);
            //    return;
            //}

            //btnPlay.Text = "Stop";
            //btnPlayDetached.Text = "Play Attached";

            var world = World;
            if (world is null)
                return;

            if (_gameState == EEditorGameplayState.Attached)
            {
                //Attached -> Detached

                ReleaseMouse();
            }
            else //Editing
            {
                //Editing -> Detached

                //Mouse already released in edit mode
                world.EndPlay();

                SetGameplayMode();
                InputInterface.GlobalRegisters.Add(RegisterInput);
                //ActorTreeForm.ClearMaps();

                world.BeginPlay();
                Engine.Unpause(ELocalPlayerIndex.One, true);
            }

            if (world.GameMode.LocalPlayers.Count > 0)
                GameplayPawn = world.GameMode.LocalPlayers[0].ControlledPawn;

            FlyingCameraDetachedPawn.EditorState.DisplayInActorTree = false;
            world.SpawnActor(FlyingCameraDetachedPawn);
            FlyingCameraDetachedPawn.ForcePossessionBy(ELocalPlayerIndex.One);
        }
        private void SetAttachedGameState()
        {
            var world = World;
            if (world is null)
                return;

            //if (InvokeRequired)
            //{
            //    BeginInvoke((Action)SetAttachedGameState);
            //    return;
            //}

            //btnPlay.Text = "Stop";
            //btnPlayDetached.Text = "Play Detached";

            //BaseRenderPanel renderPanel = (ActiveRenderForm as DockableWorldRenderPanel)?.RenderPanel ?? FocusViewport(0).RenderPanel;

            //Mouse is released in edit mode and detached mode
            //CaptureMouse(renderPanel);

            if (_gameState == EEditorGameplayState.Editing)
            {
                //Editing -> Attached

                world.EndPlay();

                SetGameplayMode();
                InputInterface.GlobalRegisters.Add(RegisterInput);
                //ActorTreeForm.ClearMaps();

                world.BeginPlay();
                Engine.Unpause(ELocalPlayerIndex.One, true);

                if (world.GameMode.LocalPlayers.Count > 0)
                    GameplayPawn = world.GameMode.LocalPlayers[0].ControlledPawn;
            }
            else //Detached
            {
                //Detached -> Attached

                GameplayPawn?.ForcePossessionBy(ELocalPlayerIndex.One);
                World.DespawnActor(FlyingCameraDetachedPawn);
            }
        }
        private void EndGameplay()
        {
            if (GameState == EEditorGameplayState.Detached)
                GameState = EEditorGameplayState.Editing;
            else
                GameState = EEditorGameplayState.Detached;
        }
        #endregion
    }
    public enum EEditorGameplayState
    {
        /// <summary>
        /// Gameplay is not simulating. Purely in edit mode.
        /// </summary>
        Editing,
        /// <summary>
        /// Gameplay is simulating, but the user is viewing from a third person flying camera.
        /// Editing is allowed.
        /// </summary>
        Detached,
        /// <summary>
        /// Gameplay is simulating and the user is playing it as it should be experienced.
        /// Editing is not allowed.
        /// </summary>
        Attached,
    }
}
