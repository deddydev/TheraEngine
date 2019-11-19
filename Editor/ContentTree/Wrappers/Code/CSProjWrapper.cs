﻿using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Windows.Forms;
using TheraEngine.Core.Files.XML;
using TheraEngine.ThirdParty;

namespace TheraEditor.Wrappers
{
    [TreeFileType("csproj")]
    public class CSProjWrapper : FileWrapper
    {
        public CSProjWrapper()
        {
            Menu = new TMenu()
            {
                TMenuOption.Rename,
                TMenuOption.Explorer,
                new TMenuOption("Co&mpile", Compile, Keys.Control | Keys.B),
                TMenuOption.Edit,
                TMenuOption.EditRaw,
                TMenuDivider.Instance,
                TMenuOption.Cut,
                TMenuOption.Copy,
                TMenuOption.Paste,
                TMenuOption.Delete,
            };
        }

        public MSBuild.Project Project { get; set; }

        public async void Compile()
        {
            var project = Editor.Instance.Project;
            if (project is null)
                return;

            await Editor.RunOperationAsync(
                "Compiling project...",
                "Finished compiling project.",
                async (p, c) => await project.CompileAsync());
        }
        public override async void Edit()
        {
            if (Project is null)
                Project = await XMLSchemaDefinition<MSBuild.Project>.ImportAsync(FilePath);
            
            Editor.Instance.MSBuildTreeForm.SetProject(Project);
        }
    }
}