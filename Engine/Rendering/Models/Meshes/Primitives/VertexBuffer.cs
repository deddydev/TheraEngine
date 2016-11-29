﻿using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CustomEngine.Rendering.Models
{
    public enum BufferType
    {
        Position,
        Normal,
        //Binormal,
        //Tangent,
        TexCoord,
        Color,
    }
    public class VertexBuffer : BaseRenderState, IDisposable
    {
        public static string GetBufferName(BufferType type, int index = -1, bool morph = false)
        {
            string s = (morph ? "Morph" : "Base") + type.ToString();
            if (morph = false && (type == BufferType.Position || type == BufferType.Normal))
                return s;
            return s + index.ToString();
        }
        public bool IsType(BufferType type, int index = -1, bool morph = false)
        {
            return Name.StartsWith(morph ? "Morph" : "Base") && 
                Name.Contains(type.ToString()) &&
                ((!morph && (type == BufferType.Position || type == BufferType.Normal)) || index < 0 ? true : Name.EndsWith(index.ToString()));
        }
        
        public enum BufferUsage
        {
            StreamDraw = 0,
            StreamRead = 1,
            StreamCopy = 2,
            StaticDraw = 4,
            StaticRead = 5,
            StaticCopy = 6,
            DynamicDraw = 8,
            DynamicRead = 9,
            DynamicCopy = 10
        }
        public enum ComponentType
        {
            SByte = 0,
            Byte = 1,
            Short = 2,
            UShort = 3,
            Int = 4,
            UInt = 5,
            Float = 6,
            Double = 7
        }
        private ComponentType _componentType;
        private int _componentCount;
        private int _elementCount;
        private bool _normalize;

        private DataSource _data;
        
        private int _index;
        private BufferTarget _target;

        public int Index { get { return _index; } }

        public VertexBuffer(
            int index,
            string name,
            BufferTarget target,
            int elementCount,
            ComponentType componentType,
            int componentCount,
            bool normalize) : base(GenType.Buffer)
        {
            _index = index;
            _target = target;
            _name = name;

            _componentType = componentType;
            _componentCount = componentCount;
            _elementCount = elementCount;
            _normalize = normalize;

            _data = DataSource.Allocate(DataLength);
        }
        public VertexBuffer(
            int index, 
            string name,
            BufferTarget target) : base(GenType.Buffer)
        {
            _index = index;
            _target = target;
            _name = name;
        }

        public VoidPtr Data { get { return _data.Address; } }
        public int DataLength { get { return _elementCount * Stride; } }
        public int Stride { get { return _componentCount * ElementSize; } }
        private int ElementSize
        {
            get
            {
                switch (_componentType)
                {
                    case ComponentType.SByte:
                    case ComponentType.Byte:
                        return 1;
                    case ComponentType.Short:
                    case ComponentType.UShort:
                        return 2;
                    case ComponentType.Int:
                    case ComponentType.UInt:
                    case ComponentType.Float:
                        return 4;
                    case ComponentType.Double:
                        return 8;
                }
                return -1;
            }
        }
        public int Initialize()
        {
            Generate();
            Bind();
            BindVertexAttrib();
            PushData();
            return BindingId;
        }
        /// <summary>
        /// Determines how the currently bound buffer should be read.
        /// Only call during initialization, unless the buffer is changed.
        /// </summary>
        public void BindVertexAttrib()
        {
            GL.EnableVertexAttribArray(_index);
            GL.VertexAttribPointer(_index, _componentCount, VertexAttribPointerType.Byte + (int)_componentType, _normalize, 0, 0);
            GL.VertexAttribFormat(_index, _componentCount, VertexAttribType.Byte + (int)_componentType, _normalize, 0);
            GL.VertexAttribBinding(_index, _index);
        }
        public void Bind() { GL.BindBuffer(_target, BindingId); }
        /// <summary>
        /// Pushes this buffer's data to the currently bound buffer 
        /// Call Bind() before this to set the current buffer.
        /// </summary>
        public void PushData(BufferUsage usage = BufferUsage.StaticDraw)
        {
            GL.BufferData(_target, (IntPtr)_data.Length, _data.Address, BufferUsageHint.StreamDraw + (int)usage);
        }
        public T Get<T>(int offset) where T : struct
        {
            T value = default(T);
            Marshal.PtrToStructure(_data.Address + offset, value);
            return value;
        }
        public unsafe Remapper SetDataNumeric<T>(IList<T> list, bool remap = false) where T : struct
        {
            if (typeof(T) == typeof(sbyte))
                _componentType = ComponentType.SByte;
            else if (typeof(T) == typeof(byte))
                _componentType = ComponentType.Byte;
            else if (typeof(T) == typeof(short))
                _componentType = ComponentType.Short;
            else if (typeof(T) == typeof(ushort))
                _componentType = ComponentType.UShort;
            else if (typeof(T) == typeof(int))
                _componentType = ComponentType.Int;
            else if (typeof(T) == typeof(uint))
                _componentType = ComponentType.UInt;
            else if (typeof(T) == typeof(float))
                _componentType = ComponentType.Float;
            else if (typeof(T) == typeof(double))
                _componentType = ComponentType.Double;
            else
                throw new InvalidOperationException("Not a proper numeric data type.");

            Console.WriteLine("\nSetting numeric vertex data for buffer " + Index + " - " + Name);

            _componentCount = 1;
            _normalize = false;
            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                int elementSize = ElementSize;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[remapper.ImplementationTable[i]];
                    Console.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                Console.WriteLine();
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                int elementSize = ElementSize;
                for (int i = 0; i < list.Count; ++i)
                {
                    VoidPtr addr = _data.Address[i, stride];
                    T value = list[i];
                    Console.Write(value.ToString() + " ");
                    Marshal.StructureToPtr(value, addr, true);
                }
                Console.WriteLine("\n");
                return null;
            }
        }
        public Remapper SetData<T>(IList<T> list, bool remap = false) where T : IBufferable
        {
            Console.WriteLine("\nSetting vertex data for buffer " + Index + " - " + Name);

            IBufferable d = default(T);
            _componentType = d.ComponentType;
            _componentCount = d.ComponentCount;
            _normalize = d.Normalize;

            if (remap)
            {
                Remapper remapper = new Remapper();
                remapper.Remap(list, null);
                _elementCount = remapper.ImplementationLength;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                for (int i = 0; i < remapper.ImplementationLength; ++i)
                {
                    IBufferable b = list[remapper.ImplementationTable[i]];
                    Console.WriteLine(b.ToString());
                    b.Write(_data.Address[i, stride]);
                }
                return remapper;
            }
            else
            {
                _elementCount = list.Count;
                _data = DataSource.Allocate(DataLength);
                int stride = Stride;
                for (int i = 0; i < list.Count; ++i)
                {
                    Console.WriteLine(list[i].ToString());
                    list[i].Write(_data.Address[i, stride]);
                }
                return null;
            }
        }
        public void Dispose()
        {
            if (_data != null)
            {
                _data.Dispose();
                _data = null;
            }
            Destroy();
        }
        ~VertexBuffer() { Dispose(); }
        public static implicit operator VoidPtr(VertexBuffer b) { return b.Data; }
        public override string ToString()
        {
            return _name;
        }
    }
}
