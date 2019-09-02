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

namespace TheraEditor
{
    /// <summary>
    /// Proxy that runs the engine in the game's domain.
    /// </summary>
    //[Serializable]
    public class EngineDomainProxyEditor : EngineDomainProxy
    {
        public ConcurrentDictionary<TypeProxy, TypeProxy> FullEditorTypes { get; private set; }
        public ConcurrentDictionary<TypeProxy, TypeProxy> InPlaceEditorTypes { get; private set; }
        /// <summary>
        /// Key is file type, Value is tree node wrapper type
        /// </summary>
        public ConcurrentDictionary<TypeProxy, TypeProxy> Wrappers { get; private set; }
        /// <summary>
        /// Key is lowercase extension, Value is tree node wrapper type
        /// </summary>
        public ConcurrentDictionary<string, TypeProxy> ThirdPartyWrappers { get; private set; }

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
        protected override void OnStarted()
        {
            //Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            //Editor.Instance.SetRenderTicking(true);
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            base.OnStarted();
        }
    }
}
