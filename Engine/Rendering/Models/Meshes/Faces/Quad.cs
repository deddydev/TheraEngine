using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models
{
    public class Quad : ObjectBase
    {
        public Quad() { }
        /// <summary>
        /// Clockwise winding
        ///2---3          3         2---3 
        ///|   |  =      /|   and   |  / 
        ///|   |       /  |         |/ 
        ///0---1      0---1         0 
        /// </summary>
        /// <param name="tri013"></param>
        /// <param name="tri032"></param>
        public Quad(Triangle tri013, Triangle tri032)
        {
            _tri013 = tri013;
            _tri032 = tri032;
        }

        Triangle _tri013, _tri032;

        public List<int> Points { get { return _tri} }
    }
}
