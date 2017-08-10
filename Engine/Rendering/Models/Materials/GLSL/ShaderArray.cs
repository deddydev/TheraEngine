﻿using System;
using System.ComponentModel;

namespace TheraEngine.Rendering.Models.Materials
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class ShaderArray<T> : ShaderVar where T : ShaderVar
    {
        [Browsable(false)]
        public override ShaderVarType TypeName { get { return _elementTypeName; } }
        public ArrayHandler<T> Value { get => _value; set { _value = value; OnValueChanged(); } }
        public int Length { get { return _value.Length; } }
        internal override void SetProgramUniform(int programBindingId, int location)
        {
            throw new NotImplementedException();
            //Engine.Renderer.Uniform();
        }
        internal override string GetValueString() { return _value.ToString(); }

        public ShaderVarType _elementTypeName;
        private ArrayHandler<T> _value;

        public ShaderArray(ArrayHandler<T> defaultValue, ShaderVarType elementType, string name, IShaderVarOwner owner) 
            : base(name, owner) { _value = defaultValue; _elementTypeName = elementType; }
    }
    public class ArrayHandler<T> : IUniformableArray where T : ShaderVar
    {
        public int Length { get { return Values.Length; } }
        public IUniformable[] Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
