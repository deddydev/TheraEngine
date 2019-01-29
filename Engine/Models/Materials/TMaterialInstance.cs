using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public class TMaterialInstance : TMaterialBase
    {
        public TMaterialInstance()
        {
            _material = new GlobalFileRef<TMaterial>();
            _material.RegisterLoadEvent(MaterialLoaded);
        }

        [TSerialize]
        private GlobalFileRef<TMaterial> _material;
        
        public GlobalFileRef<TMaterial> InheritedMaterial
        {
            get => _material;
            set
            {
                _material.UnregisterLoadEvent(MaterialLoaded);
                _material = value ?? new GlobalFileRef<TMaterial>();
                _material.RegisterLoadEvent(MaterialLoaded);
            }
        }
        private void MaterialLoaded(TMaterial mat)
        {

        }

        protected override void OnSetUniforms(RenderProgram programBindingId)
        {

        }
    }
}
