using System;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialControl : UserControl, IDataChangeHandler
    {
        private float _cameraFovY = 45.0f;
        public float CameraFovY
        {
            get => _cameraFovY;
            set
            {
                _cameraFovY = value;
                if (basicRenderPanel1.Camera is PerspectiveCamera cam)
                {
                    cam.VerticalFieldOfView = _cameraFovY;
                    float camDist = 1.0f / TMath.Tandf(_cameraFovY * 0.5f);
                    cam.LocalPoint.Z = camDist;
                }
            }
        }

        public MaterialControl()
        {
            InitializeComponent();
            basicRenderPanel1.MouseDown += BasicRenderPanel1_MouseDown;
            basicRenderPanel1.MouseUp += BasicRenderPanel1_MouseUp;
            basicRenderPanel1.MouseMove += BasicRenderPanel1_MouseMove;
            pnlMatInfo.MouseEnter += PnlMatInfo_MouseEnter;
            pnlMatInfo.MouseLeave += PnlMatInfo_MouseLeave;
            pnlMatInfo.MouseDown += PnlMatInfo_MouseDown;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            float camDist = 1.0f / TMath.Tandf(_cameraFovY * 0.5f);
            basicRenderPanel1.Camera = new PerspectiveCamera(
                new Vec3(0.0f, 0.0f, camDist), Rotator.GetZero(), 0.1f, 100.0f, _cameraFovY, 1.0f);
            if (_light == null)
            {
                _light = new DirectionalLightComponent();
                _light.SetShadowMapResolution(1, 1);
                _light.WorldRadius = 100.0f;
                _light.Rotation.Yaw = 45.0f;
                _light.Rotation.Pitch = -45.0f;
                basicRenderPanel1.Scene.Lights.Add(_light);
                basicRenderPanel1.PreRendered += BasicRenderPanel1_PreRendered;
                RedrawPreview();
            }
        }

        private int _prevX, _prevY;
        private bool _dragging = false;
        private void BasicRenderPanel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {
                int dx = e.X - _prevX;
                int dy = e.Y - _prevY;
                _prevX = e.X;
                _prevY = e.Y;
                
                if (dx == 0 && dy == 0)
                    return;

                //up is negative, left is negative

                Quat lightRotation = _light.Rotation.GetMatrix().ExtractRotation(false);

                Vec2 vec = new Vec2(dx, dy);
                Vec3 axis = Vec3.Zero;

                if (dx == 0)
                {
                    if (dy < 0)
                        axis = -Vec3.UnitX;
                    else
                        axis = Vec3.UnitX;
                }
                else if (dy == 0)
                {
                    if (dx < 0)
                        axis = -Vec3.UnitY;
                    else
                        axis = Vec3.UnitY;
                }
                //else
                //{
                //    axis = new Vec3(1.0f / vec, 0.0f).Normalized();
                //}
                float vecLen = vec.Length;
                Quat rot = Quat.FromAxisAngle(axis, vecLen);
                lightRotation = rot * lightRotation;
                Rotator r = lightRotation.ToYawPitchRoll();
                _light.Rotation.SetRotations(r);

                RedrawPreview();
            }
        }

        private void BasicRenderPanel1_MouseUp(object sender, MouseEventArgs e)
        {
            _dragging = false;
        }

        private void BasicRenderPanel1_MouseDown(object sender, MouseEventArgs e)
        {
            _prevX = e.X;
            _prevY = e.Y;
            _dragging = true;
        }

        private void PnlMatInfo_MouseDown(object sender, MouseEventArgs e)
        {
            //tblUniforms.Visible = !tblUniforms.Visible;
        }

        private void PnlMatInfo_MouseLeave(object sender, EventArgs e)
        {
            pnlMatInfo.BackColor = Color.FromArgb(62, 83, 90);
        }

        private void PnlMatInfo_MouseEnter(object sender, EventArgs e)
        {
            pnlMatInfo.BackColor = Color.FromArgb(42, 63, 70);
        }
        
        private void BasicRenderPanel1_PreRendered()
        {
            _light.RenderShadowMap(basicRenderPanel1.Scene);
        }
        
        private DirectionalLightComponent _light;
        private PrimitiveRenderWrapper _spherePrim;
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

                if (_material != null)
                {
                    lblMatName.Text = _material.Name;

                    if (_spherePrim == null)
                    {
                        basicRenderPanel1.Scene.Clear(BoundingBox.FromHalfExtentsTranslation(2.0f, 0.0f));
                        _spherePrim = new PrimitiveRenderWrapper( //0.8f instead of 1.0f for border padding
                            new PrimitiveManager(Sphere.SolidMesh(Vec3.Zero, 0.8f, 30), _material));
                        basicRenderPanel1.Scene.Add(_spherePrim);
                    }
                    else
                        _spherePrim.Material = _material;

                    theraPropertyGrid1.TargetFileObject = _material.RenderParamsRef;

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
                }
                else
                {
                    lblMatName.Text = "<null>";
                }
            }
        }

        private void RedrawPreview()
        {
            basicRenderPanel1.UpdateTick(null, null);
            basicRenderPanel1.SwapBuffers();
            basicRenderPanel1.Invalidate();
        }

        public void PropertyObjectChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            //btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }

        private void lblMatName_Click(object sender, EventArgs e)
        {
            //panel2.Visible = !panel2.Visible;
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

        private void chkDepthTest_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        public void ListObjectChanged(object oldValue, object newValue, IList listOwner, int listIndex)
        {
            //btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, listOwner, listIndex);
        }
    }
}
