using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core.Reflection
{
    public abstract class ReflectionProxy : MarshalByRefObject
    {
        public MarshalSponsor Sponsor { get; set; }
        protected ReflectionProxy()
        {
            Sponsor = new MarshalSponsor(this);
        }
        ~ReflectionProxy()
        {
            Sponsor?.Dispose();
            Sponsor = null;
        }
    }
}
