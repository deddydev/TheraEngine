namespace AppDomainToolkit
{
    using System;
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// This class exists to prevent DLL hell. Assemblies must be loaded into specific application domains
    /// without crossing those boundaries. We cannot simply remote an AssemblyLoader into a remote 
    /// domain and load assemblies to use in the current domain. Instead, we introduct a tiny, serializable
    /// implementation of the AssemblyTarget class that handles comunication between the foreign app
    /// domain and the default one. This class is simply a wrapper around an assembly loader that translates
    /// Assembly to AssemblyTarget instances before shipping them back to the parent domain.
    /// </summary>
    public class AssemblyTargetLoader : MarshalByRefObject, IAssemblyTargetLoader
    {
        #region Fields & Constants

        private readonly IAssemblyLoader _loader;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the RemotableAssemblyLoader class. This parameterless ctor is
        /// required for remoting.
        /// </summary>
        public AssemblyTargetLoader()
        {
            _loader = new AssemblyLoader();
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public IAssemblyTarget LoadAssembly(ELoadMethod loadMethod, string assemblyPath, string pdbPath = null)
        {
            IAssemblyTarget target;
            var assembly = _loader.LoadAssembly(loadMethod, assemblyPath, pdbPath);
            if (loadMethod == ELoadMethod.LoadBits)
            {
                // Assemlies loaded by bits will have the codebase set to the assembly that loaded it. Set it to the correct path here.
                var codebaseUri = new Uri(assemblyPath);
                target = AssemblyTarget.FromPath(codebaseUri, assembly.Location, assembly.FullName);
            }
            else
            {
                target = AssemblyTarget.FromAssembly(assembly);
            }

            return target;
        }

        /// <inheritdoc/>
        public IAssemblyTarget ReflectionOnlyLoadAssembly(ELoadMethod loadMethod, string assemblyPath)
        {
            IAssemblyTarget target;
            var assembly = _loader.ReflectionOnlyLoadAssembly(loadMethod, assemblyPath);
            if (loadMethod == ELoadMethod.LoadBits)
            {
                // Assemlies loaded by bits will have the codebase set to the assembly that loaded it. Set it to the correct path here.
                var codebaseUri = new Uri(assemblyPath);
                target = AssemblyTarget.FromPath(codebaseUri, assembly.Location, assembly.FullName);
            }
            else
            {
                target = AssemblyTarget.FromAssembly(assembly);
            }

            return target;
        }

        /// <inheritdoc/>
        public IList<IAssemblyTarget> LoadAssemblyWithReferences(ELoadMethod loadMethod, string assemblyPath)
        {
            return _loader.LoadAssemblyWithReferences(loadMethod, assemblyPath).Select(x => AssemblyTarget.FromAssembly(x)).ToList();
        }

        /// <inheritdoc />
        public IAssemblyTarget[] GetAssemblies()
        {
            var assemblies = _loader.GetAssemblies();
            return assemblies.Select(x => AssemblyTarget.FromAssembly(x)).ToArray();
        }

        /// <inheritdoc />
        public IAssemblyTarget[] ReflectionOnlyGetAssemblies()
        {
            var assemblies = _loader.ReflectionOnlyGetAssemblies();
            return assemblies.Select(x => AssemblyTarget.FromAssembly(x)).ToArray();
        }

        #endregion
    }
}
