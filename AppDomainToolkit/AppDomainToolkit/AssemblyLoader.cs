﻿namespace AppDomainToolkit
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    /// <summary>
    /// Used to determine which load context assemblies should be loaded into by the loader.
    /// </summary>
    public enum ELoadMethod
    {
        /// <summary>
        /// Loads the assembly into the LoadFrom context, which enables the assembly and all it's references to be discovered
        /// and loaded into the target application domain. Despite it's penchant for DLL hell, this is probably the way to go by
        /// default as long as you make sure to pass the base directory of the application to an AssemblyResolver instance such
        /// that references can be properly resolved. This also allows for multiple assemblies of the same name to be loaded while
        /// maintaining separate file names. This is the recommended way to go.
        /// </summary>
        LoadFrom,

        /// <summary>
        /// Loads an assembly into memory using the raw file name. This loads the assembly anonymously, so it won't have
        /// a load context. Use this if you want the bits loaded, but make sure to pass the directory where this file lives to an 
        /// AssemblyResolver instance so you can find it again. This is similar to LoadFrom except you don't get the free 
        /// lookups for already existing assembly names via fusion. Use this for more control over assembly file loads.
        /// </summary>
        LoadFile,

        /// <summary>
        /// Loads the bits of the target assembly into memory using the raw file name. This is, in essence, a dynamic assembly
        /// for all the CLR cares. You won't ever be able to find this with an assembly resolver, so don't use this unless you look
        /// for it by name. Be careful with this one.
        /// </summary>
        LoadBits
    }

    /// <summary>
    /// This class will load assemblies into whatever application domain it's loaded into. It's just a simple convenience
    /// wrapper around the static Assembly.Load* methods, with the main benefit being the ability to load an assembly
    /// anonymously bitwise. When you load the assembly this way, there won't be any locking of the DLL file.
    /// </summary>
    public class AssemblyLoader : MarshalByRefObject, IAssemblyLoader
    {
        #region Public Methods

        /// <inheritdoc /> 
        /// <remarks>
        /// If the LoadMethod for this instance is set to LoadBits and the path to the PDB file is unspecified then we will attempt to guess
        /// the path to the PDB and load it.  Note that if an assembly is loaded into memory without it's debugging symbols then an
        /// image exception will be thrown. Be wary of this. Loading an Assembly with the LoadBits method will not lock
        /// the DLL file because the entire assembly is loaded into memory and the file handle is closed. Note that, however,
        /// Assemblies loaded in this manner will not have a location associated with them--so you must then key the Assembly
        /// by it's strong name. This can cause problems when loading multiple versions of the same assembly into a single
        /// application domain.
        /// </remarks>
        public Assembly LoadAssembly(ELoadMethod loadMethod, string assemblyPath, string pdbPath = null)
        {
            Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] {loadMethod} {assemblyPath}");

            Assembly assembly;
            switch (loadMethod)
            {
                case ELoadMethod.LoadFrom:
                    assembly = Assembly.LoadFrom(assemblyPath);
                    break;
                case ELoadMethod.LoadFile:
                    assembly = Assembly.LoadFile(assemblyPath);
                    break;
                case ELoadMethod.LoadBits:

                    // Attempt to load the PDB bits along with the assembly to avoid image exceptions.
                    pdbPath = string.IsNullOrEmpty(pdbPath) ? Path.ChangeExtension(assemblyPath, "pdb") : pdbPath;

                    // Only load the PDB if it exists--we may be dealing with a release assembly.
                    if (File.Exists(pdbPath))
                    {
                        assembly = Assembly.Load(
                            File.ReadAllBytes(assemblyPath),
                            File.ReadAllBytes(pdbPath));
                    }
                    else
                    {
                        assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
                    }

                    break;
                default:
                    // In case we upadate the enum but forget to update this logic.
                    throw new NotSupportedException("The target load method isn't supported!");
            }

            return assembly;
        }

        /// <inheritdoc /> 
        /// <remarks>
        /// LoadMethod.LoadFile is not supported. See LoadAssembly for more details.
        /// </remarks>
        public Assembly ReflectionOnlyLoadAssembly(ELoadMethod loadMethod, string assemblyPath)
        {
            Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] [ReflectionOnly] {loadMethod} {assemblyPath}");

            Assembly assembly;
            switch (loadMethod)
            {
                case ELoadMethod.LoadFrom:
                    assembly = Assembly.ReflectionOnlyLoadFrom(assemblyPath);
                    break;
                case ELoadMethod.LoadFile:
                    throw new NotSupportedException("The target load method isn't supported!");
                case ELoadMethod.LoadBits:
                    assembly = Assembly.ReflectionOnlyLoad(File.ReadAllBytes(assemblyPath));
                    break;
                default:
                    // In case we upadate the enum but forget to update this logic.
                    throw new NotSupportedException("The target load method isn't supported!");
            }

            return assembly;
        }
        /// <inheritdoc />
        /// <remarks>
        /// This implementation will perform a best-effort load of the target assembly and it's required references
        /// into the current application domain. The .NET framework pins us in on which call we're allowed to use
        /// when loading these assemblies, so we'll need to rely on the AssemblyResolver instance attached to the
        /// AppDomain in order to load the way we want.
        /// </remarks>
        public IList<Assembly> LoadAssemblyWithReferences(ELoadMethod loadMethod, string assemblyPath)
        {
            var list = new List<Assembly>();
            var assembly = LoadAssembly(loadMethod, assemblyPath);
            list.Add(assembly);

            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
            {
                Trace.WriteLine($"[{AppDomain.CurrentDomain.FriendlyName}] Loading reference from {reference}");

                if (File.Exists(reference.CodeBase))
                    list.Add(LoadAssembly(loadMethod, reference.CodeBase));
                else
                    list.Add(Assembly.Load(reference));
            }

            return list;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Just a simple call to AppDomain.CurrentDomain.GetAssemblies(), nothing more.
        /// </remarks>
        public Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Just a simple call to AppDomain.CurrentDomain.GetAssemblies(), nothing more.
        /// </remarks>
        public Assembly[] ReflectionOnlyGetAssemblies()
        {
            return AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();
        }

        #endregion
    }
}
