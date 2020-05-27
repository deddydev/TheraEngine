using System.Collections.Generic;
using TheraEngine.Rendering.Models.Materials;
using Extensions;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Rendering
{
    public abstract class RenderPipeline : TObjectSlim
    {
        private List<ViewportRenderCommand> RenderCommands { get; } = new List<ViewportRenderCommand>();
        private Dictionary<string, BaseTexRef> Textures { get; } = new Dictionary<string, BaseTexRef>();
        private Dictionary<string, FrameBuffer> FBOs { get; } = new Dictionary<string, FrameBuffer>();

        public T GetTexture<T>(string name) where T : BaseTexRef
        {
            T texture = null;
            if (Textures.ContainsKey(name))
                texture = Textures[name] as T;
            if (texture is null)
                Engine.Out($"Render pipeline texture {name} of type {typeof(T).GetFriendlyName()} was not found.");
            return texture;
        }
        public void SetTexture(BaseTexRef texture)
        {
            string name = texture.Name;
            if (Textures.ContainsKey(name))
            {
                Textures[name] = texture;
                Engine.Out($"Render pipeline texture {name} was overwritten.");
            }
            else
                Textures.Add(name, texture);
        }
        public T GetFBO<T>(string name) where T : FrameBuffer
        {
            T fbo = null;
            if (FBOs.ContainsKey(name))
                fbo = FBOs[name] as T;
            if (fbo is null)
                Engine.Out($"Render pipeline FBO {name} of type {typeof(T).GetFriendlyName()} was not found.");
            return fbo;
        }
        public void SetFBO(string name, FrameBuffer fbo)
        {
            if (FBOs.ContainsKey(name))
            {
                FBOs[name] = fbo;
                Engine.Out($"Render pipeline FBO {name} was overwritten.");
            }
            else
                FBOs.Add(name, fbo);
        }

        public void GenerateCommandChain(Viewport viewport)
        {
            DestroyFBOs();
            FBOs.Clear();
            Textures.Clear();
            RenderCommands.Clear();
            GenerateCommandChain(RenderCommands, viewport);
        }
        protected abstract void GenerateCommandChain(List<ViewportRenderCommand> commands, Viewport viewport);
        public void ExecuteCommandChain(RenderPasses renderingPasses, IScene scene, ICamera camera, Viewport viewport, FrameBuffer target)
        {
            if (RenderCommands is null)
                return;

            foreach (ViewportRenderCommand command in RenderCommands)
                command?.Execute(renderingPasses, scene, camera, viewport, target);
        }

        public virtual void DestroyFBOs()
        {
            if (RenderCommands != null)
                foreach (var rc in RenderCommands)
                    rc?.DestroyFBOs();
        }
        public virtual void GenerateFBOs(Viewport viewport)
        {
            if (RenderCommands != null)
                foreach (var rc in RenderCommands)
                    rc?.GenerateFBOs(viewport);
        }
    }
}
