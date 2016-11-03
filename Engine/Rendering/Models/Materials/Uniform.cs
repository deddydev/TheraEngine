using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class Uniform<T> : GLGlobalVar where T : IUniformable
    {
        T _data;

        public Uniform(GLTypeName type, string name, int index, T data) : base(type, name, index, EQualifier._uniform)
        {
            _data = data;
        }
    }
}
