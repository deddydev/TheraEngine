﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Renderer
{
    public enum ShaderType
    {
        Fragment,
        Vertex,
    }
    public class Shader
    {
        private string _code;
        private ShaderType _type;
        public Shader(ShaderType type)
        {
            _type = type;
        }
        public void SetCode(string code)
        {
            _code = code;
        }
        public void Compile() { }

    }
}
