using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialControl : UserControl, IDataChangeHandler
    {
        public MaterialControl()
        {
            InitializeComponent();
        }

        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;

                if (Engine.DesignMode)
                    return;

                tblUniforms.Controls.Clear();
                tblUniforms.RowStyles.Clear();
                tblUniforms.RowCount = 0;
                lstTextures.Clear();
                lstShaders.Clear();

                if (_material != null)
                {
                    lblMatName.Text = _material.Name;
                    
                    theraPropertyGrid1.TargetFileObject = _material.RenderParamsRef.File;

                    foreach (ShaderVar shaderVar in _material.Parameters)
                    {
                        Type valType = ShaderVar.AssemblyTypeAssociations[shaderVar.TypeName];
                        Type varType = shaderVar.GetType();

                        PropGridItem textCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            typeof(PropGridText),
                            varType.GetProperty(nameof(ShaderVar.Name)), 
                            shaderVar, this);
                        textCtrl.ValueChanged += RedrawPreview;

                        PropGridItem valueCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            TheraPropertyGrid.GetControlTypes(valType)[0],
                            varType.GetProperty("Value"), 
                            shaderVar, this);
                        valueCtrl.ValueChanged += RedrawPreview;

                        tblUniforms.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        tblUniforms.RowCount = tblUniforms.RowStyles.Count;

                        tblUniforms.Controls.Add(textCtrl, 0, tblUniforms.RowCount - 1);
                        tblUniforms.Controls.Add(valueCtrl, 1, tblUniforms.RowCount - 1);
                    }

                    foreach (BaseTexRef tref in _material.Textures)
                    {
                        var item = new ListViewItem(string.Format("{0} [{1}]",
                            tref.Name, tref.GetType().GetFriendlyName())) { Tag = tref };
                        lstTextures.Items.Add(item);
                    }
                    
                    foreach (var shaderRef in _material.Shaders)
                    {
                        string text = string.Empty;
                        if (!string.IsNullOrWhiteSpace(shaderRef.ReferencePathAbsolute))
                            text = Path.GetFileNameWithoutExtension(shaderRef.ReferencePathAbsolute) + " ";
                        else if (!string.IsNullOrWhiteSpace(shaderRef.File?.Name))
                            text = Path.GetFileNameWithoutExtension(shaderRef.File.Name) + " ";

                        text += "[" + shaderRef.File.Type.ToString() + "]";

                        ListViewItem item = new ListViewItem(text) { Tag = shaderRef };

                        lstShaders.Items.Add(item);
                    }
                }
                else
                {
                    lblMatName.Text = "<null>";
                }

                RedrawPreview();
            }
        }

        private void RedrawPreview()
        {
            //basicRenderPanel1.UpdateTick(null, null);
            //basicRenderPanel1.SwapBuffers();
            //basicRenderPanel1.Invalidate();
        }

        private void lblMatName_Click(object sender, EventArgs e)
        {
            //panel2.Visible = !panel2.Visible;
            theraPropertyGrid1.TargetFileObject = _material;

        }

        private void lblMatName_MouseEnter(object sender, EventArgs e)
        {
            lblMatName.BackColor = Color.FromArgb(42, 53, 60);
        }

        private void lblMatName_MouseLeave(object sender, EventArgs e)
        {
            lblMatName.BackColor = Color.FromArgb(32, 43, 50);
        }

        private void txtMatName_TextChanged(object sender, EventArgs e)
        {
            //_material.Name = txtMatName.Text;
            lblMatName.Text = _material.Name;
        }
        
        public void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }
        public void IListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex)
        {
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, listOwner, listIndex);
        }

        private void lstTextures_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstTextures.SelectedItems.Count > 0)
            {
                BaseTexRef tref = lstTextures.SelectedItems[0].Tag as BaseTexRef;
                
            }
            else
                theraPropertyGrid1.TargetFileObject = null;
        }

        private Dictionary<GLSLShaderFile, DockableTextEditor> _textEditors = new Dictionary<GLSLShaderFile, DockableTextEditor>();

        private void lstShaders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstShaders.SelectedItems.Count == 0)
                return;

            GlobalFileRef<GLSLShaderFile> fileRef = lstShaders.SelectedItems[0].Tag as GlobalFileRef<GLSLShaderFile>;
            GLSLShaderFile file = fileRef.File;

            if (file == null)
                return;

            if (_textEditors.ContainsKey(file))
            {
                _textEditors[file].Focus();
            }
            else
            {
                DockContent form = FindForm() as DockContent;
                DockPanel p = form?.DockPanel ?? Editor.Instance.DockPanel;

                DockableTextEditor textEditor = new DockableTextEditor
                {
                    Tag = fileRef
                };
                textEditor.Show(p, DockState.DockLeft);
                textEditor.InitText(fileRef.File.Text, Path.GetFileName(fileRef.ReferencePathAbsolute), ETextEditorMode.GLSL);
                textEditor.Saved += M_Saved;
                textEditor.CompileGLSL = M_CompileGLSL;
                textEditor.FormClosed += TextEditor_FormClosed;

                _textEditors.Add(file, textEditor);
            }
        }

        private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            DockableTextEditor editor = sender as DockableTextEditor;
            GlobalFileRef<GLSLShaderFile> fileRef = editor.Tag as GlobalFileRef<GLSLShaderFile>;
            _textEditors.Remove(fileRef.File);
        }

        private (bool, string) M_CompileGLSL(string text, DockableTextEditor editor)
        {
            GlobalFileRef<GLSLShaderFile> fileRef = editor.Tag as GlobalFileRef<GLSLShaderFile>;
            EShaderMode mode = fileRef.File.Type;
            fileRef.File.Text = text;
            //bool success = _shader.Compile(out string info, false);
            return (true, null);
        }

        private void M_Saved(DockableTextEditor editor)
        {
            GlobalFileRef<GLSLShaderFile> fileRef = editor.Tag as GlobalFileRef<GLSLShaderFile>;
            fileRef.File.Text = editor.GetText();
            Editor.Instance.ContentTree.WatchProjectDirectory = false;
            fileRef.ExportReference();
            Editor.Instance.ContentTree.WatchProjectDirectory = true;
        }
        public void IDictionaryObjectChanged(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey)
        {
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, dicOwner, key, isKey);
        }
    }
}
