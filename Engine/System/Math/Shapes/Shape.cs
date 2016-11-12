using CustomEngine;
using CustomEngine.Files;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomEngine.Rendering.Models;

namespace System
{
    public abstract class Shape : FileObject, IPrimitive
    {
        public abstract bool Contains(Vec3 point);
        public abstract List<PrimitiveData> GetPrimitives();
        
        public void OnDespawned()
        {
            throw new NotImplementedException();
        }

        public void OnSpawned()
        {
            throw new NotImplementedException();
        }
    }
}
