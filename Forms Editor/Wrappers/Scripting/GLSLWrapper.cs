using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Properties;
using TheraEditor.Windows.Forms;
using TheraEngine;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(GLSLShaderFile), nameof(Resources.GenericFile))]
    public class GLSLWrapper : FileWrapper<GLSLShaderFile>
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
            DockableTextEditor textEditor = new DockableTextEditor();
            textEditor.Show(Editor.Instance.DockPanel, DockState.Document);
            _shader = new RenderShader(ResourceRef.File);
            _shader.Generate();
            textEditor.InitText(ResourceRef.File.Text, Path.GetFileName(ResourceRef.ReferencePathAbsolute), ETextEditorMode.GLSL);
            textEditor.Saved += M_Saved;
            textEditor.CompileGLSL = M_CompileGLSL;
            textEditor.FormClosed += TextEditor_FormClosed;
        }
        
        private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            _shader.Dispose();
            _shader = null;
        }

        private (bool, string) M_CompileGLSL(string text)
        {
            _shader.SetSource(text, false);
            bool success = _shader.Compile(out string info, false);
            return (success, info);
        }
        
        private void M_Saved(DockableTextEditor obj)
        {
            ResourceRef.File.Text = obj.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            ResourceRef.ExportReference();
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
    }
}