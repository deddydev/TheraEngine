using System;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLArgument<T> : BaseGLArgument where T : GLVar
    {
        public GLArgument(string name) : base(name) { }

        public override Type GetArgType() { return default(T).TypeName; }
        public override Type[] GetPossibleArgTypes() { return new Type[] { GetArgType() }; }
    }
}
