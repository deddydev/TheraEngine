using CustomEngine.Rendering.Models.Meshes;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class Vector3Extension
    {
        public static bool IsGequalTo(this Vector3 value, Vector3 other)
        {
            return value.X >= other.X && value.Y >= other.Y && value.Z >= other.Z;
        }
        public static bool IsLequalTo(this Vector3 value, Vector3 other)
        {
            return value.X <= other.X && value.Y <= other.Y && value.Z <= other.Z;
        }
        public static void SetGequalTo(this Vector3 value, Vector3 other)
        {
            if (value.X < other.X)
                value.X = other.X;
            if (value.Y < other.Y)
                value.Y = other.Y;
            if (value.Z < other.Z)
                value.Z = other.Z;
        }
        public static void SetLequalTo(this Vector3 value, Vector3 other)
        {
            if (value.X > other.X)
                value.X = other.X;
            if (value.Y > other.Y)
                value.Y = other.Y;
            if (value.Z > other.Z)
                value.Z = other.Z;
        }
    }
}
