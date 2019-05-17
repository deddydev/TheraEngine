using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Reflection;

namespace TheraEditor
{
    /// <summary>
    /// Proxy that runs methods in the game's domain.
    /// </summary>
    public class ProjectDomainProxy : MarshalByRefObject
    {
        public ConcurrentDictionary<TypeProxy, TypeProxy> FullEditorTypes { get; private set; }
        public ConcurrentDictionary<TypeProxy, TypeProxy> InPlaceEditorTypes { get; private set; }

        public TProject Project { get; private set; }
        public Sponsor SponsorRef { get; } = new Sponsor();

        public string GetVersionInfo() =>

            ".NET Version: "        + Environment.Version.ToString() 
            + Environment.NewLine +
            "Assembly Location: "   + typeof(ProjectDomainProxy).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\") 
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
        public void ReloadEditorTypes()
        {
            if (Engine.DesignMode)
                return;

            InPlaceEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>();
            FullEditorTypes = new ConcurrentDictionary<TypeProxy, TypeProxy>();

            Engine.PrintLine("Loading all editor types to property grid in AppDomain " + AppDomain.CurrentDomain.FriendlyName);
            Task propEditorsTask = Task.Run(() =>
            {
                var propControls = AppDomainHelper.FindTypes(x =>
                    !x.IsAbstract &&
                    x.IsSubclassOf(typeof(PropGridItem)),
                    Assembly.GetExecutingAssembly());

                Parallel.ForEach(propControls, AddPropControlEditorType);
            });
            Task fullEditorsTask = Task.Run(() =>
            {
                var fullEditors = AppDomainHelper.FindTypes(x =>
                    !x.IsAbstract &&
                    x.IsSubclassOf(typeof(Form)) &&
                    x.HasCustomAttribute<EditorForAttribute>(),
                    Assembly.GetExecutingAssembly());

                Parallel.ForEach(fullEditors, AddFullEditorType);
            });
            Task.WhenAll(propEditorsTask, fullEditorsTask).ContinueWith(t =>
                Engine.PrintLine("Finished loading all editor types to property grid."));
        }
        private void AddPropControlEditorType(TypeProxy propControlType)
        {
            var attribs = propControlType.GetCustomAttributes<PropGridControlForAttribute>();
            if (attribs.Count > 0)
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
        public void Created(TProject project)
        {
            Engine.SetGame(project);
            Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            Engine.Initialize();
            Editor.Instance.SetRenderTicking(true);
            Engine.SetPaused(true, ELocalPlayerIndex.One, true);

            Engine.PrintLine("Resetting type caches.");
            Editor.ResetTypeCaches();
            Engine.PrintLine("Type caches reset.");
        }
        public void Destroyed()
        {
            Engine.ShutDown();
            SponsorRef.Release = true;
        }
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
