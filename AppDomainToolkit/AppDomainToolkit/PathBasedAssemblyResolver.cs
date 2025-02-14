﻿using System.Linq;

namespace AppDomainToolkit
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Handles resolving assemblies in application domains. This class is helpful when attempting to load a
    /// particular assembly into an application domain and the assembly you're looking for doesn't exist in the
    /// main application bin path. This 'feature' of the .NET framework makes assembly loading very, very
    /// irritating, but this little helper class should alleviate much of the pains here. Note that it extends 
    /// MarshalByRefObject, so it can be remoted into another application domain. Paths to directories containing
    /// assembly files that you wish to load should be added to an instance of this class, and then the Resolve
    /// method should be assigned to the AssemblyResolve event on the target application domain.
    /// </summary>
    public class PathBasedAssemblyResolver : MarshalByRefObject, IAssemblyResolver
    {
        #region Fields & Constants

        private readonly HashSet<string> _probePaths;
        private readonly IAssemblyLoader _loader;

        #endregion

        #region Constructors & Destructors

        /// <summary>
        /// Initializes a new instance of the PathBasedAssemblyResolver class. Exists for MarshalByRefObject
        /// remoting into app domains.
        /// </summary>
        public PathBasedAssemblyResolver()
            : this(null, ELoadMethod.LoadFrom) { }

        /// <summary>
        /// Initializes a new instance of the AssemblyResolver class. A default instance of this class will resolve
        /// assemblies into the LoadFrom context.
        /// </summary>
        /// <param name="loader">
        /// The loader to use when loading assemblies. Default is null, which will create and use an instance
        /// of the RemotableAssemblyLoader class.
        /// </param>
        /// <param name="loadMethod">
        /// The load method to use when loading assemblies. Defaults to LoadMethod.LoadFrom.
        /// </param>
        public PathBasedAssemblyResolver(
            IAssemblyLoader loader = null,
            ELoadMethod loadMethod = ELoadMethod.LoadFrom)
        {
            _probePaths = new HashSet<string>();
            _loader = loader is null ? new AssemblyLoader() : loader;
            LoadMethod = loadMethod;
        }

        #endregion

        #region Properties

        /// <inheritdoc />
        public ELoadMethod LoadMethod { get; set; } = ELoadMethod.LoadFrom;

        /// <inheritdoc />
        private string _applicationBase;

        /// <inheritdoc />
        public string ApplicationBase
        {
            get => _applicationBase;
            set
            {
                _applicationBase = value;
                AddProbePath(value);
            }
        }

        /// <inheritdoc />
        private string _privateBinPath;

        /// <inheritdoc />
        public string PrivateBinPath
        {
            get => _privateBinPath;
            set
            {
                _privateBinPath = value;
                AddProbePath(value);
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc />
        public void AddProbePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            
            if (path.Contains(";"))
            {
                var paths = path.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                AddProbePaths(paths);
            }
            else
                AddProbePaths(path);
        }

        /// <inheritdoc />
        public void AddProbePaths(params string[] paths)
        {
            foreach (var path in paths)
            {
                if (string.IsNullOrEmpty(path))
                    continue;
                
                var dir = new DirectoryInfo(path);
                if (!_probePaths.Contains(dir.FullName))
                    _probePaths.Add(dir.FullName);
            }
        }

        /// <summary>
        /// Removes the given probe path or semicolon separated list of probe paths from the assembly loader.
        /// </summary>
        /// <param name="path">The path to remove.</param>
        public void RemoveProbePath(string path)
        {
            if (String.IsNullOrEmpty(path))
                return;

            if (path.Contains(";"))
            {
                var paths = path.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries);
                RemoveProbePaths(paths);
            }
            else
                RemoveProbePaths(path);
        }

        /// <summary>
        /// Removes the given probe paths from the assembly loader.
        /// </summary>
        /// <param name="paths">The paths to remove.</param>
        public void RemoveProbePaths(params string[] paths)
        {
            var e = from path in paths
                    where !string.IsNullOrEmpty(path)
                    select new DirectoryInfo(path);

            foreach (var dir in e)
                _probePaths.Remove(dir.FullName);
        }

        /// <inheritdoc />
        public Assembly Resolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);
            foreach (var path in _probePaths)
            {
                var dllPath = Path.Combine(path, string.Format("{0}.dll", name.Name));
                if (File.Exists(dllPath))
                    return _loader.LoadAssembly(LoadMethod, dllPath);
                
                var exePath = Path.ChangeExtension(dllPath, "exe");
                if (File.Exists(exePath))
                    return _loader.LoadAssembly(LoadMethod, exePath);
            }

            // Not found.
            return null;
        }

        #endregion
    }
}
