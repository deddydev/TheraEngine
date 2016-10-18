using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class PrimitiveManager
    {
        public List<Vertex> _vertices;
        public List<Triangle> _faces;

        public bool _isWeighted;
        public bool _hasNormals;
        public int _texCoordChannelCount = 0;
        public int _colorChannelCount = 0;

        public void PrepareStream()
        {
            CalcStride();
            int bufferSize = _renderStride * _faces.Count * 3;

            //Dispose of buffer if size doesn't match
            if (_graphicsBuffer != null && _graphicsBuffer.Length != bufferSize)
            {
                _graphicsBuffer.Dispose();
                _graphicsBuffer = null;
            }

            //Create data buffer
            if (_graphicsBuffer == null)
            {
                _graphicsBuffer = new UnsafeBuffer(bufferSize);
                for (int i = 0; i < 12; i++)
                    _dirty[i] = true;
            }

            //Update streams before binding
            for (int i = 0; i < 12; i++)
                if (_dirty[i])
                    UpdateStream(i);
        }

        public unsafe void BindStream()
        {
            byte* pData = (byte*)_graphicsBuffer.Address;
            for (int i = 0; i < 12; i++)
                if (_faceData[i] != null)
                    switch (i)
                    {
                        case 0:
                            GL.EnableClientState(ArrayCap.VertexArray);
                            GL.VertexPointer(3, VertexPointerType.Float, _renderStride, (IntPtr)pData);
                            pData += 12;
                            break;
                        case 1:
                            GL.EnableClientState(ArrayCap.NormalArray);
                            GL.NormalPointer(NormalPointerType.Float, _renderStride, (IntPtr)pData);
                            pData += 12;
                            break;
                        case 2:
                            GL.EnableClientState(ArrayCap.ColorArray);
                            GL.ColorPointer(4, ColorPointerType.UnsignedByte, _renderStride, (IntPtr)pData);
                            pData += 4;
                            break;
                        case 3:
                            GL.EnableClientState(ArrayCap.SecondaryColorArray);
                            GL.SecondaryColorPointer(4, ColorPointerType.UnsignedByte, _renderStride, (IntPtr)pData);
                            pData += 4;
                            break;
                        default:
                            pData += 8;
                            break;
                    }
        }

        internal unsafe void DetachStreams()
        {
            for (int i = 0; i < 8; i++)
            {
                GL.ActiveTexture(TextureUnit.Texture0 + i);
                GL.ClientActiveTexture(TextureUnit.Texture0 + i);
                GL.DisableClientState(ArrayCap.TextureCoordArray);
                GL.Disable(EnableCap.Texture2D);
            }

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.ClientActiveTexture(TextureUnit.Texture0);
            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.NormalArray);
            GL.DisableClientState(ArrayCap.ColorArray);
        }
    }
}
