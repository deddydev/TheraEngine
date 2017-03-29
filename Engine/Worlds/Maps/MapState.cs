using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Maps
{
    public class MapState
    {
        private bool _visible;
        
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
    }
}
