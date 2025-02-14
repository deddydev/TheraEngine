﻿using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Wrappers
{
    public interface IBaseFileWrapper : IBasePathWrapper
    {
        TypeProxy FileType { get; }
        bool IsLoaded { get; }
        bool AlwaysReload { get; set; }
        bool ExternallyModified { get; set; }
        IFileRef FileRefGeneric { get; }
        
        IFileObject GetFileGeneric();
        Task<IFileObject> GetFileGenericAsync();
        Task<IFileObject> GetFileGenericAsync(IProgress<float> progress, CancellationToken cancel);

        void Reload();
        void Edit();
        void EditRaw();
    }
    public abstract class BaseFileWrapper : BasePathWrapper, IBaseFileWrapper
    {
        public BaseFileWrapper() => Menu = DefaultMenu(this);

        public void Reload()
        {
            var fref = FileRefGeneric;
            if (fref is null)
                return;

            bool wasLoaded = fref.IsLoaded;
            if (wasLoaded)
                fref.Unload();
            fref.IsLoaded = wasLoaded;
        }

        public bool IsLoaded => FileRefGeneric?.IsLoaded ?? false;
        public bool AlwaysReload { get; set; } = false;
        public bool ExternallyModified { get; set; } = false;

        public IFileObject GetFileGeneric() 
            => FileRefGeneric?.GetInstance();
        public async Task<IFileObject> GetFileGenericAsync()
        {
            if (FileRefGeneric is null)
                return null;

            return await FileRefGeneric.GetInstanceAsync();
        }
        public async Task<IFileObject> GetFileGenericAsync(IProgress<float> progress, CancellationToken cancel)
        {
            if (FileRefGeneric is null)
                return null;

            return await FileRefGeneric.GetInstanceAsync(progress, cancel);
        }

        public TypeProxy FileType 
        {
            get => FileRefGeneric?.FileType;
            set
            {
                if (FileRefGeneric != null)
                    FileRefGeneric.FileType = value;
            }
        }

        private string _filePath;
        public override string FilePath
        {
            get => FileRefGeneric?.Path?.Path ?? _filePath;
            set
            {
                _filePath = value;
                FileType = TFileObject.DetermineType(FilePath, out _);
                var fref = FileRefGeneric;
                if (fref?.Path != null)
                    fref.Path.Path = value;
            }
        }

        public abstract IFileRef FileRefGeneric { get; }

        public virtual async void Edit()
        {
            var file = await GetFileGenericAsync();

            if (file is null)
                Engine.Out($"Can't open file at {FilePath}.");
            else
            {
                Engine.Out($"Editing file at {FilePath}.");

                //TODO: pre-resolve editor type
                TypeProxy editorType = ResolveEditorType(FileType);
                if (editorType != null)
                    Editor.Instance.ShowEditor(editorType, file);
                else
                    Editor.SetPropertyGridObject(file);
            }
        }

        public virtual void EditRaw() => Editor.Instance.UserEditText(FilePath);

        public static TypeProxy ResolveEditorType(TypeProxy fileType)
        {
            if (fileType is null)
                return null;

            EngineDomainProxyEditor proxy = Engine.DomainProxy as EngineDomainProxyEditor;
            var editorTypes = proxy.FullEditorTypes;

            if (editorTypes != null && TypeProxy.AnyBaseTypeMatches(fileType, type => editorTypes.ContainsKey(type), out TypeProxy match, true))
                return editorTypes[match];

            return null;
        }

        //protected async void DefaultSaveText(DockableTextEditor obj)
        //{
        //    if (!(Resource is ITextSource source))
        //        return;

        //    source.Text = obj.GetText();

        //    string path = ResourceRef.Path.Absolute;

        //    Editor.Instance.ContentTree.BeginFileSaveWithProgress(path, "Saving text...", out Progress<float> progress, out CancellationTokenSource cancel);
        //    await ResourceRef.File.ExportAsync(path, ESerializeFlags.Default, progress, cancel.Token);
        //    Editor.Instance.ContentTree.EndFileSave(path);
        //}
        
        public override void Explorer() => Explorer(false);
        public void Explorer(bool openDirect)
        {
            string path = FilePath;

            if (string.IsNullOrEmpty(path))
                return;

            if (openDirect)
                Process.Start(path);
            else
            {
                string dir = Path.GetDirectoryName(path);

                if (string.IsNullOrEmpty(dir))
                    return;

                Process.Start("explorer.exe", dir);
            }
        }

    }
}
