﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLArray<T> : GLVar where T : GLVar
    {
        public override GLTypeName TypeName { get { return _elementTypeName; } }
        public ArrayHandler<T> Value { get { return _value; } set { _value = value; } }
        public int Length { get { return _value.Length; } }
        public override IUniformable UniformValue { get { return _value; } }

        public GLTypeName _elementTypeName;
        private ArrayHandler<T> _value;

        public GLArray(ArrayHandler<T> defaultValue, GLTypeName elementType, string name, IGLVarOwner owner) 
            : base(name, owner) { _value = defaultValue; _elementTypeName = elementType; }
    }
    public class ArrayHandler<T> : IUniformableArray where T : GLVar
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
