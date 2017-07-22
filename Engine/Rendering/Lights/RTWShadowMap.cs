using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public class RTWShadowMap : MaterialFrameBuffer
    {
        RenderProgram _program;
        Matrix4 _lightViewToCameraView;
        Matrix4 _cameraViewToLightView;
        float _tessSize = 0.05f;

        public RTWShadowMap() : base() { }

        public float TesselationSize { get => _tessSize; set => _tessSize = value; }

        public void Calculate()
        {
            GenerateSurface();
            GenerateImportance();
            GenerateWarpMap();
            GenerateShadowMap();
        }

        private void GenerateShadowMap()
        {
            //tess_effect.GetParameter("TESS_SIZE").SetParameter1f(tess_size);
            //tess_effect.GetParameter("TEXUNIT5").SetTextureParameter(GetWarpMap().GetTexID());

            //sm->EnableFBO(true);
            //glClearColor(1, 1, 1, 1);
            //glClear(GL_DEPTH_BUFFER_BIT | GL_COLOR_BUFFER_BIT);
            //glEnable(GL_DEPTH_TEST);

            //oglWidgets::glSetMatrix(GL_PROJECTION, lv_mvp.data);
            //oglWidgets::glSetMatrix(GL_MODELVIEW, 0);

            //if (dynamic_tess == 0)
            //{
            //    CG_RUN_TECHNIQUE(tess_effect.GetTechnique("RTW_Tessellator_Pass_Thru"), scene->DrawTriangles(time));
            //}
            //else
            //{
            //    CG_RUN_TECHNIQUE(tess_effect.GetTechnique("RTW_Tessellator"), scene->DrawPatches(time));
            //}
            //sm->DisableFBO();
        }

        private void GenerateWarpMap()
        {
            throw new NotImplementedException();
        }

        private void GenerateImportance()
        {
            throw new NotImplementedException();
        }

        private void GenerateSurface()
        {
            SetDefaultUniforms();
        }

        private void SetDefaultUniforms()
        {
            //Engine.Renderer.ProgramUniform(_program.BindingId, "lv_2_dv", )
            //effect.GetParameter("lv_2_dv").SetMatrixParameterfc((dv_mvp * lv_mvp.Inverse()).data);
            //effect.GetParameter("dv_2_lv").SetMatrixParameterfc((lv_mvp * dv_mvp.Inverse()).data);

            //effect.GetParameter("DESIRED_VIEW").SetMatrixParameterfc(dv_view.data);

            //effect.GetParameter("DD_TEXEL_SIZE").SetParameter1f(1.0f / (float)GetImportanceSize());

            //effect.GetParameter("NORMAL_SCALING_BONUS").SetParameter1f(normal_scaling ? 2.0f : 0.0f);

            //effect.GetParameter("TEXUNIT1").SetTextureParameter(di_fbo.GetDepthTexture()->GetTexID());
            //effect.GetParameter("TEXUNIT5").SetTextureParameter(GetWarpMap().GetTexID());
            //effect.GetParameter("TEXUNIT6").SetTextureParameter(sm->GetShadowMapDepth().GetTexID());
            //effect.GetParameter("TEXUNIT8").SetTextureParameter(di_norm.GetTexID());
            //effect.GetParameter("TEXUNIT9").SetTextureParameter(di_fbo.GetColorTexture(0)->GetTexID());

            //effect.GetParameter("TEXEL_SIZE").SetParameter2f(1.0f / (float)sm->GetWidth(), 1.0f / (float)sm->GetHeight());
        }

        private float GetImportanceSize()
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            //impt_map_res = importance_map_resolution;

            //di_norm.TexImage2D(GL_LUMINANCE16F_ARB, di_w, di_h, GL_LUMINANCE, GL_FLOAT, 0);
            //di_norm.SetMinMagFilter(GL_NEAREST, GL_NEAREST);

            //di_fbo.Init(di_w, di_h);
            //di_fbo.InsertColorAttachment(0);
            //di_fbo.GetColorTexture(0)->SetMinMagFilter(GL_NEAREST, GL_NEAREST);
            //di_fbo.InsertColorAttachment(1, &di_norm);
            //di_fbo.InsertDepthAttachment();
            //di_fbo.GetDepthTexture()->SetMinMagFilter(GL_NEAREST, GL_NEAREST);

            //warp_tex[0].TexImage2D(GL_LUMINANCE32F_ARB, impt_map_res, 2, GL_LUMINANCE, GL_FLOAT, 0);
            //warp_tex[0].SetMinMagFilter(GL_NEAREST, GL_NEAREST);
            //warp_fbo[0].Init(impt_map_res, 2);
            //warp_fbo[0].InsertColorAttachment(0, &warp_tex[0]);

            //warp_tex[1].TexImage2D(GL_LUMINANCE32F_ARB, impt_map_res, 2, GL_LUMINANCE, GL_FLOAT, 0);
            //warp_tex[1].SetMinMagFilter(GL_NEAREST, GL_NEAREST);
            //warp_fbo[1].Init(impt_map_res, 2);
            //warp_fbo[1].InsertColorAttachment(0, &warp_tex[1]);

            //warp_tex[2].TexImage2D(GL_LUMINANCE32F_ARB, impt_map_res + 1, 2, GL_LUMINANCE, GL_FLOAT, 0);
            //warp_tex[2].SetMinMagFilter(GL_LINEAR, GL_LINEAR);
            //warp_tex[2].SetWrapST(GL_CLAMP, GL_CLAMP);
            //warp_fbo[2].Init(impt_map_res + 1, 2);
            //warp_fbo[2].InsertColorAttachment(0, &warp_tex[2]);

            //impt_tex.TexImage2D(GL_LUMINANCE32F_ARB, impt_map_res, impt_map_res, GL_LUMINANCE, GL_FLOAT, 0);
            //impt_tex.SetMinMagFilter(GL_NEAREST, GL_NEAREST);
            //impt_fbo.Init(impt_map_res, impt_map_res);
            //impt_fbo.InsertColorAttachment(0, &impt_tex);
            //impt_fbo.InsertDepthAttachment();


            //impt_effect.Load("../shaders/rtw_map.cgfx");
            //tess_effect.Load("../shaders/rtw_tessellator.cgfx");

            Material m = new Material()
            {
                Requirements = Material.UniformRequirements.None
            };

            Material = m;
        }
    }
}
