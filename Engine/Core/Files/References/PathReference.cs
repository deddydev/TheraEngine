using Extensions;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection.Attributes;

namespace TheraEngine.Core.Files
{
    /// <summary>
    /// Contains the origin types for a given path.
    /// </summary>
    public enum EPathType
    {
        /// <summary>
        /// Stores a path relative to a specific drive.
        /// Not recommended as this makes the path specific to that computer.
        /// </summary>
        Absolute,
        /// <summary>
        /// Path is relative to the path of the current file.
        /// </summary>
        FileRelative,
        /// <summary>
        /// Path is relative to the path of the Thera Engine dll.
        /// </summary>
        EngineRelative,
        /// <summary>
        /// Path is relative to the path of the game's exe.
        /// </summary>
        GameRelative,
    }
    /// <summary>
    /// Stores a file/folder path relative to a certain origin.
    /// </summary>
    [TFileDef("Path Reference", "Stores a file/folder path relative to a certain origin.")]
    [TFileExt("path")]
    public class PathReference : TFileObject
    {
        public delegate void DelPathChange(string oldPath, string newPath);
        public event DelPathChange AbsolutePathChanged;
        public void OnAbsolutePathChanged(string oldPath)
            => AbsolutePathChanged?.Invoke(oldPath, Path);

        [TSerialize(nameof(Type), Order = 0, NodeType = ENodeType.Attribute)]
        public EPathType _type = EPathType.FileRelative;

        [TSerialize(nameof(Path), Order = 1, NodeType = ENodeType.Attribute)]
        public string _path;

        public PathReference() { }
        public PathReference(string path, EPathType type)
        {
            Path = path;
            _type = type;
        }

        [Category("File Reference")]
        public EPathType Type
        {
            get => _type;
            set => _type = value;
        }

        [CustomMemberSerializeMethod(nameof(Path))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by reflection.")]
        private object SerializePath()
        {
            string path = Path;
            if (!path.IsValidPath())
                return path;
            path = System.IO.Path.GetFullPath(Path);
            switch (Type)
            {
                case EPathType.FileRelative:
                {
                    string dir = DirectoryPath;
                    path = path.MakeAbsolutePathRelativeTo(dir);
                    break;
                }
                case EPathType.EngineRelative:
                {
                    string relPath = Assembly.GetExecutingAssembly().Location;
                    string dir = System.IO.Path.GetDirectoryName(relPath);
                    path = path.MakeAbsolutePathRelativeTo(dir);
                    break;
                }
                case EPathType.GameRelative:
                {
                    string dir = Engine.Game?.DirectoryPath;
                    if (!string.IsNullOrWhiteSpace(dir))
                        path = path.MakeAbsolutePathRelativeTo(dir);
                    break;
                }
            }
            return path;
        }
        [CustomMemberDeserializeMethod(nameof(Path))]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by reflection.")]
        private void DeserializePath(SerializeAttribute node)
        {
            try
            {
                if (!node.GetObjectAs(out string path))
                {
                    _path = null;
                    return;
                }
                if (!path.IsAbsolutePath())
                {
                    switch (Type)
                    {
                        case EPathType.FileRelative:
                            {
                                string dir = System.IO.Path.GetDirectoryName(node.Parent.Manager.FilePath);
                                if (dir.EndsWith(@"\"))
                                    dir = dir.Remove(dir.Length - 1);
                                if (!path.StartsWith(@"\"))
                                    path = @"\" + path;
                                path = dir + path;
                                break;
                            }
                        case EPathType.EngineRelative:
                            {
                                string relPath = Assembly.GetExecutingAssembly().Location;
                                string dir = System.IO.Path.GetDirectoryName(relPath);
                                if (dir.EndsWith(@"\"))
                                    dir = dir.Remove(dir.Length - 1);
                                if (!path.StartsWith(@"\"))
                                    path = @"\" + path;
                                path = dir + path;
                                break;
                            }
                        case EPathType.GameRelative:
                            {
                                string dir = Engine.Game?.DirectoryPath;
                                if (!string.IsNullOrWhiteSpace(dir))
                                {
                                    if (dir.EndsWith(@"\"))
                                        dir = dir.Remove(dir.Length - 1);
                                    if (!path.StartsWith(@"\"))
                                        path = @"\" + path;
                                    path = dir + path;
                                }
                                break;
                            }
                    }
                }
                _path = System.IO.Path.GetFullPath(path);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
                _path = null;
            }
        }

        [TString(false, true)]
        [Category("File Reference")]
        public virtual string Path
        {
            get => _path;
            set
            {
                string oldPath = _path;
                _path = value.IsValidPath() ? System.IO.Path.GetFullPath(value) : value;
                OnAbsolutePathChanged(oldPath);
            }
        }

        //[TString(false, true, false)]
        //[Category("File Reference")]
        //[TSerialize("Path", Order = 1, NodeType = ENodeType.Attribute)]
        //public virtual string Relative
        //{
        //    get => _relative;
        //    set
        //    {
        //        //string oldAbsolute = _absolute;
        //        //string oldRelative = _relative;

        //        //if (value != null)
        //        //{
        //            _relative = value;
        //        //    bool isAbsolute = _relative.IsAbsolutePath();

        //        //    if (!isAbsolute)
        //        //        _relative = "\\" + _relative;
        //        //    //else
        //        //    //    _pathType = EPathType.Absolute;

        //        //    if (Type == EPathType.Absolute)
        //        //    {
        //        //        _absolute = Path.GetFullPath(_relative);
        //        //    }
        //        //    else
        //        //    {
        //        //        bool fileRelative = Type == EPathType.FileRelative;
        //        //        if (fileRelative)
        //        //        {
        //        //            if (string.IsNullOrWhiteSpace(RootFile.DirectoryPath))
        //        //            {
        //        //                _absolute = Path.GetFileName(_relative);
        //        //            }
        //        //            else
        //        //            {
        //        //                string combinedPath = RootFile.DirectoryPath + _relative;
        //        //                _absolute = Path.GetFullPath(combinedPath);
        //        //            }
        //        //        }
        //        //        else
        //        //        {
        //        //            string relPath = _relative.MakeAbsolutePathRelativeTo(Application.StartupPath);
        //        //            string combinedPath = Path.Combine(Application.StartupPath, _relative);
        //        //            _absolute = Path.GetFullPath(combinedPath);
        //        //        }
        //        //    }
        //        //}
        //        //else
        //        //{
        //        //    _relative = null;
        //        //    _absolute = null;
        //        //}

        //        //OnRelativePathChanged(oldRelative);
        //        //OnAbsolutePathChanged(oldAbsolute);
        //    }
        //}

        /// <summary>
        /// Returns true if a file exists at the path that this reference points to.
        /// </summary>
        [Browsable(false)]
        public virtual bool FileExists => Path.IsAbsolutePath() && File.Exists(Path);

        public static implicit operator PathReference(string path)
            => new PathReference(path, EPathType.FileRelative);
    }
}
