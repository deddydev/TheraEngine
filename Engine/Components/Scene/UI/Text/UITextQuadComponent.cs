using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI
{
    public class UITextQuadComponent : UIInteractableComponent
    {
        private Font _font;
        private string _text;

        public string Text
        {
            get => _text;
            set 
            {
                if (Set(ref _text, value))
                {
                    InvalidateLayout();
                }
            }
        }

        public Font Font
        {
            get => _font;
            set
            {
                if (Set(ref _font, value))
                {
                    GenerateFontTexture();
                    InvalidateLayout();
                }
            }
        }

        [TSerialize(nameof(TextDrawer))]
        private TextRasterizer _textDrawer;
        public TextRasterizer TextDrawer => _textDrawer;

        private void GenerateFontTexture()
        {

        }

        private RenderCommandMesh3D Command { get; }
        private PrimitiveManager CharQuad => Command.Mesh;

        public UITextQuadComponent() : base(null)
        {
            var mat = TMaterial.CreateUnlitAlphaTextureMaterialForward(
                new TexRef2D("CharTex", 64, 64, PixelFormat.Format32bppArgb));

            var data = PrimitiveData.FromQuads(
                VertexShaderDesc.PosTex(),
                VertexQuad.PosZQuad(1.0f, true, 0.0f, true));

            //TODO: resize buffer length if text length reaches capacity
            int textLengthCapacity = 256;
            var positionsBuffer = data.AddBuffer(new Vec4[textLengthCapacity], new VertexAttribInfo(EBufferType.Other, 0), false, false, true, 1);
            var textureRegionsBuffer = data.AddBuffer(new Vec4[textLengthCapacity], new VertexAttribInfo(EBufferType.Other, 1), false, false, true, 1);

            positionsBuffer.Location = 1;
            textureRegionsBuffer.Location = 2;

            Command = new RenderCommandMesh3D(ERenderPass.TransparentForward) { Mesh = new PrimitiveManager(data, mat) };
        }

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            base.OnResizeLayout(parentRegion);

            int totalChars = 0;
            if (Text != null)
                for (int i = 0; i < Text.Length; ++i)
                {

                }

            CharQuad.Instances = totalChars;
        }

        public override void AddRenderables(RenderPasses passes, ICamera camera)
        {
            base.AddRenderables(passes, camera);

            passes.Add(Command);
        }
    }
}
