using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
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
    [TFileDef("Path Reference", "Stores a path relative to a certain origin.")]
    [TFileExt("path")]
    public class PathReference : TFileObject
    {
        public delegate void DelPathChange(string oldPath, string newPath);
        public event DelPathChange AbsolutePathChanged;
        public event DelPathChange RelativePathChanged;
        public void OnAbsolutePathChanged(string oldPath)
            => AbsolutePathChanged?.Invoke(oldPath, Absolute);
        public void OnRelativePathChanged(string oldPath)
            => RelativePathChanged?.Invoke(oldPath, Relative);

        [TSerialize(nameof(Type), Order = 0, NodeType = ENodeType.Attribute)]
        protected EPathType _pathType = EPathType.FileRelative;
        protected string _relative;
        protected string _absolute;

        [Category("File Reference")]
        public EPathType Type
        {
            get => _pathType;
            set
            {
                if (_pathType == value)
                    return;

                _pathType = value;

                string oldRelative = _relative;

                if (_pathType == EPathType.Absolute)
                    _relative = _absolute;
                else if (!string.IsNullOrWhiteSpace(_absolute) && _absolute.IsValidExistingPath())
                {
                    if (Type == EPathType.EngineRelative)
                        _relative = _absolute.MakeAbsolutePathRelativeTo(Application.StartupPath);
                    else
                    {
                        if (!string.IsNullOrEmpty(RootFile.DirectoryPath))
                            _relative = _absolute.MakeAbsolutePathRelativeTo(RootFile.DirectoryPath);
                        else
                            _relative = Path.GetFileName(_absolute);
                    }
                }
                else
                    _relative = null;

                OnRelativePathChanged(oldRelative);
            }
        }

        [TString(false, true, false)]
        [Category("File Reference")]
        public virtual string Absolute
        {
            get => _absolute;
            set
            {
                string oldAbsolute = _absolute;
                string oldRelative = _relative;

                if (value != null)
                {
                    bool validPath = value.IsExistingDirectoryPath() == false;
                    if (validPath)
                    {
                        _absolute = Path.GetFullPath(value);
                        if (Type == EPathType.Absolute)
                            _relative = _absolute;
                        else
                        {
                            string root = Path.GetPathRoot(value);
                            int colonIndex = root.IndexOf(":");
                            if (colonIndex > 0)
                                root = root.Substring(0, colonIndex);
                            else
                                root = string.Empty;

                            if (Type == EPathType.EngineRelative)
                            {
                                string root2 = Path.GetPathRoot(Application.StartupPath);
                                colonIndex = root2.IndexOf(":");
                                if (colonIndex > 0)
                                    root2 = root2.Substring(0, colonIndex);
                                else
                                    root2 = string.Empty;
                                if (!string.Equals(root, root2))
                                {
                                    Type = EPathType.Absolute;
                                    _relative = _absolute;
                                }
                                else
                                    _relative = _absolute.MakeAbsolutePathRelativeTo(Application.StartupPath);
                            }
                            else //Absolute or File Relative
                            {
                                if (!string.IsNullOrEmpty(RootFile.DirectoryPath))
                                {
                                    string root2 = Path.GetPathRoot(RootFile.DirectoryPath);
                                    colonIndex = root2.IndexOf(":");
                                    if (colonIndex > 0)
                                        root2 = root2.Substring(0, colonIndex);
                                    else
                                        root2 = string.Empty;
                                    if (!string.Equals(root, root2))
                                    {
                                        Type = EPathType.Absolute;
                                        _relative = _absolute;
                                    }
                                    else
                                        _relative = _absolute.MakeAbsolutePathRelativeTo(RootFile.DirectoryPath);
                                }
                                else
                                    _relative = null;
                            }
                        }
                    }
                    else
                    {
                        _absolute = value;
                        _relative = value;
                    }
                }
                else
                {
                    _relative = null;
                    _absolute = null;
                }

                OnRelativePathChanged(oldRelative);
                OnAbsolutePathChanged(oldRelative);
            }
        }
        [TString(false, true, false)]
        [Category("File Reference")]
        [TSerialize(Order = 1, NodeType = ENodeType.Attribute)]
        public virtual string Relative
        {
            get => _relative;
            set
            {
                string oldAbsolute = _absolute;
                string oldRelative = _relative;

                if (value != null)
                {
                    _relative = value;
                    bool isAbsolute = _relative.Contains(":");

                    if (!isAbsolute && !_relative.StartsWith("\\"))
                        _relative = "\\" + _relative;
                    else
                        _pathType = EPathType.Absolute;

                    if (Type == EPathType.Absolute)
                    {
                        _absolute = Path.GetFullPath(_relative);
                    }
                    else
                    {
                        bool fileRelative = Type == EPathType.FileRelative;
                        if (fileRelative)
                        {
                            if (string.IsNullOrWhiteSpace(RootFile.DirectoryPath))
                            {
                                _absolute = Path.GetFileName(_relative);
                            }
                            else
                            {
                                string combinedPath = RootFile.DirectoryPath + _relative;
                                _absolute = Path.GetFullPath(combinedPath);
                            }
                        }
                        else
                        {
                            string relPath = _relative.MakeAbsolutePathRelativeTo(Application.StartupPath);
                            string combinedPath = Path.Combine(Application.StartupPath, _relative);
                            _absolute = Path.GetFullPath(combinedPath);
                        }
                    }
                }
                else
                {
                    _relative = null;
                    _absolute = null;
                }

                OnRelativePathChanged(oldRelative);
                OnAbsolutePathChanged(oldAbsolute);
            }
        }

        /// <summary>
        /// Returns true if a file exists at the path that this reference points to.
        /// </summary>
        [Browsable(false)]
        public virtual bool FileExists
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Absolute))
                    return false;
                if (!File.Exists(Absolute))
                    return false;
                return true;
            }
        }
    }
}
