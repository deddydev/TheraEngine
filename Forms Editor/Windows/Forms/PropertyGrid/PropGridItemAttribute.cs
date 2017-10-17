using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemAttribute : Attribute
    {
        public Type[] Types { get; set; }
        public PropGridItemAttribute(params Type[] types)
        {
            Types = types;
        }
    }
}
