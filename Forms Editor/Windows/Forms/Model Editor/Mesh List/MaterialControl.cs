using System;
using System.Reflection;
using System.Windows.Forms;
using TheraEditor.Windows.Forms.PropertyGrid;
using TheraEngine.Components.Scene.Lights;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEditor.Windows.Forms
{
    public partial class MaterialControl : UserControl
    {
        private float _cameraFovY = 90.0f;
        public float CameraFovY
        {
            get => _cameraFovY;
            set
            {
                _cameraFovY = value;
            }
        }

        public MaterialControl()
        {
            InitializeComponent();

            float camDist = 1.0f / TMath.Sindf(_cameraFovY * 0.5f);
            basicRenderPanel1.Camera = new PerspectiveCamera(
                new Vec3(0.0f, 0.0f, camDist), Rotator.GetZero(), 0.1f, 1000.0f, _cameraFovY, 1.0f);
        }

        DirectionalLightComponent _light;
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            basicRenderPanel1.RegisterTick();

            _light = new DirectionalLightComponent();
            _light.SetShadowMapResolution(256, 256);
            _light.WorldRadius = 1000.0f;
            _light.Rotation.Yaw = 45.0f;
            _light.Rotation.Pitch = -45.0f;
            basicRenderPanel1.Scene.Lights.Add(_light);
            basicRenderPanel1.PreRendered += BasicRenderPanel1_PreRendered;
        }

        private void BasicRenderPanel1_PreRendered()
        {
            _light.RenderShadowMap(basicRenderPanel1.Scene);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            basicRenderPanel1.UnregisterTick();
            base.OnHandleDestroyed(e);
        }

        private TMaterial _material;
        public TMaterial Material
        {
            get => _material;
            set
            {
                _material = value;

                tblUniforms.Controls.Clear();
                tblUniforms.RowStyles.Clear();
                tblUniforms.RowCount = 0;

                if (_material != null)
                {
                    PrimitiveRenderWrapper wrap = new PrimitiveRenderWrapper(
                        new PrimitiveManager(Sphere.SolidMesh(Vec3.Zero, 1.0f, 30), _material));
                    basicRenderPanel1.Scene.Add(wrap);

                    lblMaterialName.Text = _material.Name;
                    foreach (ShaderVar shaderVar in _material.Parameters)
                    {
                        Type valType = ShaderVar.AssemblyTypeAssociations[shaderVar.TypeName];
                        Type varType = shaderVar.GetType();

                        PropGridItem textCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            typeof(PropGridText),
                            varType.GetProperty(nameof(ShaderVar.Name)), 
                            shaderVar, UniformChanged);

                        PropGridItem valueCtrl = TheraPropertyGrid.InstantiatePropertyEditor(
                            TheraPropertyGrid.GetControlTypes(valType)[0],
                            varType.GetProperty("Value"), 
                            shaderVar, UniformChanged);

                        tblUniforms.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                        tblUniforms.RowCount = tblUniforms.RowStyles.Count;

                        tblUniforms.Controls.Add(textCtrl, 0, tblUniforms.RowCount - 1);
                        tblUniforms.Controls.Add(valueCtrl, 1, tblUniforms.RowCount - 1);
                    }
                    foreach (BaseTexRef tref in _material.Textures)
                    {

                    }
                }
            }
        }

        private void UniformChanged(object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            //btnSave.Visible = true;
            Editor.Instance.UndoManager.AddChange(Material.EditorState, oldValue, newValue, propertyOwner, propertyInfo);
        }
    }
}
