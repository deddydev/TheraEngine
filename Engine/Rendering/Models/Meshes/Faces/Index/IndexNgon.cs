﻿using System;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models
{
    public class IndexNgon : IndexPrimitive
    {
        public IndexNgon() { }
        public IndexNgon(params Point[] points)
        {

        }

        public override FaceType Type { get { return FaceType.Ngon; } }

        public override List<IndexTriangle> ToTriangles()
        {
            throw new Exception("");
        }
    }
}
