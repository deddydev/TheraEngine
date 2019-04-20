using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using TheraEngine;
using TheraEngine.Core.Reflection;

namespace TheraEditor
{
    /// <summary>
    /// Proxy that runs methods in the game's domain.
    /// </summary>
    public class ProjectDomainProxy : MarshalByRefObject
    {
        public TProject Project { get; private set; }
        public Sponsor SponsorRef { get; } = new Sponsor();

        public string GetVersionInfo()
        {
            return ".NET Version: " + Environment.Version.ToString() + "\r\n" +
            "Assembly Location: " + typeof(ProjectDomainProxy).Assembly.CodeBase.Replace("file:///", "").Replace("/", "\\") + "\r\n" +
            "Assembly Cur Dir: " + Directory.GetCurrentDirectory() + "\r\n" +
            "ApplicationBase: " + AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\r\n" +
            "App Domain: " + AppDomain.CurrentDomain.FriendlyName + "\r\n";
        }
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
        public void Created()
        {
            //Project = project;
            //Engine.ShutDown();
            //Engine.SetGame(Project);
            ////Engine.SetWorldPanel(Editor.Instance.RenderForm1.RenderPanel, false);
            //Engine.Initialize();
            ////Editor.Instance.SetRenderTicking(true);
            //Engine.SetPaused(true, ELocalPlayerIndex.One, true);
        }
        public void Destroyed()
        {
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
