using Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Editor;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialControl2 : UserControl, ValueChangeHandler
    {
        public MaterialControl2()
        {
            InitializeComponent();
            comboBox1.DataSource = Enum.GetNames(typeof(EGLSLType));
            comboBox2.DataSource = Enum.GetNames(typeof(ETextureType));
        }

        public class ShaderVarMemberWrapper : IPropGridMemberOwner
        {
            public MaterialControl2 Control { get; set; }
            public int ParameterIndex { get; set; }

            public object Value => Control.Material.Parameters[ParameterIndex];
            public bool ReadOnly => false;
            public PropGridMemberInfo MemberInfo => throw new NotImplementedException();
        }
        private ShaderVarMemberWrapper[] _paramWrappers;

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
                    
                    theraPropertyGrid1.TargetObject = _material.RenderParamsRef;

                    int count = _material.Parameters.Length;
                    _paramWrappers = new ShaderVarMemberWrapper[count];
                    for (int i = 0; i < count; ++i)
                    {
                        _paramWrappers[i] = new ShaderVarMemberWrapper() { Control = this, ParameterIndex = i };

                        ShaderVar shaderVar = _material.Parameters[i];

                        Type valType = ShaderVar.AssemblyTypeAssociations[shaderVar.TypeName];
                        Type varType = shaderVar.GetType();

                        PropGridItem textCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            typeof(PropGridText), new PropGridMemberInfoProperty(_paramWrappers[i], varType.GetProperty(nameof(ShaderVar.Name))), null, this);
                        textCtrl.ValueChanged += RedrawPreview;

                        PropGridItem valueCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            TheraPropertyGrid.GetControlTypes(valType)[0], new PropGridMemberInfoProperty(_paramWrappers[i], varType.GetProperty("Value")), null, this);
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
                        tref.Renamed += Tref_Renamed;
                    }
                    
                    foreach (var shaderRef in _material.Shaders)
                    {
                        string text = string.Empty;
                        if (!string.IsNullOrWhiteSpace(shaderRef.Path.Path))
                            text = Path.GetFileNameWithoutExtension(shaderRef.Path.Path) + " ";
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

        private void Tref_Renamed(TObject node, string oldName)
        {
            if (!(node is BaseTexRef tref))
                return;
            int index = Material.Textures.IndexOf(tref);
            if (index < 0 || index >= lstTextures.Items.Count || lstTextures.Items[index].Tag != tref)
                return;
            lstTextures.Items[index].Text = string.Format("{0} [{1}]", tref.Name, tref.GetType().GetFriendlyName());
        }

        private void RedrawPreview()
        {
            //basicRenderPanel1.UpdateTick(null, null);
            //basicRenderPanel1.SwapBuffers();
            //basicRenderPanel1.Invalidate();
        }

        private void lblMatName_Click(object sender, EventArgs e)
        {
            DockContent form = FindForm() as DockContent;
            DockPanel p = form?.DockPanel ?? Editor.Instance.DockPanel;
            Form f = p.FindForm();

            if (f is ModelEditorForm modelForm)
            {
                modelForm.MatPreviewForm.SetMaterial(_material);
            }
            else if (f is MaterialEditorForm matForm)
            {
                //matForm.MatPreviewForm.SetMaterial(_material);
            }
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
            //lblMatName.Text = _material.Name;
        }

        public void HandleChange(params LocalValueChange[] changes)
            => Editor.Instance.UndoManager.AddChange(Material.EditorState, changes);

        //public void HandleChange(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        //{
        //    Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        //}
        //public void IListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex)
        //{
        //    Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, listOwner, listIndex);
        //}

        private void lstTextures_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private Dictionary<GLSLScript, DockableTextEditor> _textEditors = new Dictionary<GLSLScript, DockableTextEditor>();

        private void lstShaders_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstShaders.SelectedItems.Count == 0)
                return;

            GlobalFileRef<GLSLScript> fileRef = lstShaders.SelectedItems[0].Tag as GlobalFileRef<GLSLScript>;
            GLSLScript file = fileRef.File;

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
                textEditor.TargetFile = file;
                //textEditor.Saved += M_Saved;
                textEditor.FormClosed += TextEditor_FormClosed;

                _textEditors.Add(file, textEditor);
            }
        }

        private void TextEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            DockableTextEditor editor = sender as DockableTextEditor;
            GlobalFileRef<GLSLScript> fileRef = editor.Tag as GlobalFileRef<GLSLScript>;
            _textEditors.Remove(fileRef.File);
        }
        
        //private void M_Saved(DockableTextEditor editor)
        //{
        //    GlobalFileRef<GLSLShaderFile> fileRef = editor.Tag as GlobalFileRef<GLSLShaderFile>;
        //    fileRef.File.Text = editor.GetText();
        //    Editor.Instance.ContentTree.WatchProjectDirectory = false;
        //    //WfileRef.ExportReference();
        //    Editor.Instance.ContentTree.WatchProjectDirectory = true;
        //}
        //public void IDictionaryObjectChanged(object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey)
        //{
        //    Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, dicOwner, key, isKey);
        //}

        private void lstTextures_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstTextures.SelectedItems.Count == 0)
                return;

            if (!(lstTextures.SelectedItems[0].Tag is BaseTexRef tref))
                return;

            DockContent form = FindForm() as DockContent;
            DockPanel p = form?.DockPanel ?? Editor.Instance.DockPanel;
            ModelEditorForm editor = p.FindForm() as ModelEditorForm;
            editor.TexRefForm.texRefControl1.SetTexRef(tref);
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            //btnAdd.Visible = btnRemove.Visible =
            //    tabControl1.SelectedTab.Name == "tabShaders" && lstShaders.SelectedIndices.Count > 0;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (lstShaders.SelectedIndices.Count > 0)
            {
                int index = lstShaders.SelectedIndices[0];
                _material.Shaders.RemoveAt(index);
                lstShaders.Items.RemoveAt(index);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            GLSLScript f = new GLSLScript((EGLSLType)((int)Math.Pow(2, comboBox1.SelectedIndex)));

            GlobalFileRef<GLSLScript> shaderRef = f;

            string text = string.Empty;
            if (!string.IsNullOrWhiteSpace(shaderRef.Path.Path))
                text = Path.GetFileNameWithoutExtension(shaderRef.Path.Path) + " ";
            else if (!string.IsNullOrWhiteSpace(shaderRef.File?.Name))
                text = Path.GetFileNameWithoutExtension(shaderRef.File.Name) + " ";
            text += "[" + shaderRef.File.Type.ToString() + "]";

            ListViewItem item = new ListViewItem(text) { Tag = shaderRef };
            if (lstShaders.SelectedIndices.Count == 0)
            {
                _material.Shaders.Add(f);
                lstShaders.Items.Add(item);
            }
            else
            {
                int index = lstShaders.SelectedIndices[0];
                _material.Shaders.Insert(index, f);
                lstShaders.Items.Insert(index, item);
            }
        }

        private void lstShaders_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnRemove.Enabled = lstShaders.SelectedIndices.Count != 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            BaseTexRef tref = BaseTexRef.CreateTexRef((ETextureType)comboBox2.SelectedIndex);
            tref.Renamed += Tref_Renamed;

            var item = new ListViewItem(string.Format("{0} [{1}]",
               tref.Name, tref.GetType().GetFriendlyName())) { Tag = tref };

            if (lstTextures.SelectedIndices.Count == 0)
            {
                int index = _material.Textures.Length;
                _material.Textures = _material.Textures.Resize(index + 1);
                _material.Textures[index] = tref;
                lstTextures.Items.Add(item);
            }
            else
            {
                int index = lstTextures.SelectedIndices[0];
                _material.Textures = _material.Textures.Resize(_material.Textures.Length + 1);
                for (int i = index + 1; i < _material.Textures.Length; ++i)
                    _material.Textures[i] = _material.Textures[i - 1];
                _material.Textures[index] = tref;
                lstTextures.Items.Insert(index, item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (lstTextures.SelectedIndices.Count > 0)
            {
                int index = lstTextures.SelectedIndices[0];
                int length = _material.Textures.Length;
                for (int i = index; i < _material.Textures.Length - 1; ++i)
                    _material.Textures[i] = _material.Textures[i + 1];
                _material.Textures = _material.Textures.Resize(length - 1);
                lstTextures.Items.RemoveAt(index);
            }
        }
    }
}