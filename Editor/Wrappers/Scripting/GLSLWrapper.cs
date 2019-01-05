using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(GLSLScript), nameof(Resources.GenericFile))]
    public class GLSLWrapper : FileWrapper<GLSLScript>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static GLSLWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }
        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            GLSLWrapper w = GetInstance<GLSLWrapper>();
        }
        #endregion
        
        public GLSLWrapper() : base() { }

        private RenderShader _shader;
        public override void EditResource()
        {
            _shader = new RenderShader(ResourceRef.File);
            _shader.Generate();

            var textEditor = DockableTextEditor.ShowNew(Editor.Instance.DockPanel, DockState.Document, ResourceRef.File, M_Saved);
            textEditor.CompileGLSL = M_CompileGLSL;
            textEditor.FormClosed += TextEditor_FormClosed;
        }
        
        private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            _shader.Dispose();
            _shader = null;
        }

        private (bool, string) M_CompileGLSL(string text, DockableTextEditor editor)
        {
            string ext = Path.GetExtension(ResourceRef.Path.Absolute).Substring(1);
            EGLSLType mode = EGLSLType.Fragment;
            switch (ext)
            {
                default:
                case "fs":
                case "frag":
                    mode = EGLSLType.Fragment;
                    break;
                case "vs":
                case "vert":
                    mode = EGLSLType.Vertex;
                    break;
                case "gs":
                case "geom":
                    mode = EGLSLType.Geometry;
                    break;
                case "tcs":
                case "tesc":
                    mode = EGLSLType.TessControl;
                    break;
                case "tes":
                case "tese":
                    mode = EGLSLType.TessEvaluation;
                    break;
                case "cs":
                case "comp":
                    mode = EGLSLType.Compute;
                    break;
            }
            _shader.SetSource(text, mode, false);
            bool success = _shader.Compile(out string info, false);
            return (success, info);
        }
        
        private async void M_Saved(DockableTextEditor obj)
        {
            ResourceRef.File.Text = obj.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            int op = Editor.Instance.BeginOperation("Saving text...", out Progress<float> progress, out System.Threading.CancellationTokenSource cancel);
            await ResourceRef.File.ExportAsync(ResourceRef.Path.Absolute, ESerializeFlags.Default, progress, cancel.Token);
            Editor.Instance.EndOperation(op);
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
    }
}