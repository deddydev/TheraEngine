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

        public void ReloadEditorTypes()
        {
            if (Engine.DesignMode)
                return;

            InPlaceEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());
            FullEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>(new TypeProxy.EqualityComparer());

            Engine.PrintLine("Loading all editor types to property grid in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            Task propEditorsTask = Task.Run(AddPropControlEditorTypes);
            Task fullEditorsTask = Task.Run(AddFullEditorTypes);
            Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                Engine.PrintLine("Finished loading all editor types to property grid in AppDomain " + AppDomain.CurrentDomain.FriendlyName));
        }
        private void AddPropControlEditorTypes()
        {
            var propControls = AppDomainHelper.FindTypes(x =>
                !x.IsAbstract &&
                x.IsSubclassOf(typeof(PropGridItem)),
                Assembly.GetExecutingAssembly());

            Parallel.ForEach(propControls, AddPropControlEditorType);
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

            Parallel.ForEach(fullEditors, AddFullEditorType);
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
            ReloadEditorTypes();
            base.ResetTypeCaches(reloadNow);
        }
        //public override void Start(string gamePath, bool isUIDomain)
        //{
        //    base.Start(gamePath, isUIDomain);
        //}
        protected override void OnStarted()
        {
            Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            Editor.Instance.SetRenderTicking(true);
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            base.OnStarted();
        }
    }
}
