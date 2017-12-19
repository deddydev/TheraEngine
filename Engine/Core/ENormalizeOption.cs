using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Core
{
    public enum ENormalizeOption
    {
        /// <summary>
        /// No normalization is done to the vector.
        /// </summary>
        None,
        /// <summary>
        /// The vector is checked that it can be normalized safely, then is normalized with the regular algorithm.
        /// </summary>
        Safe,
        /// <summary>
        /// Regular normalization is done to the vector.
        /// </summary>
        Unsafe,
        /// <summary>
        /// Normalization is done to the vector using the Newton-Raphson approximation method. No square root is taken, but may be less accurate.
        /// </summary>
        FastSafe,
        /// <summary>
        /// The vector first is checked that it can be normalized safely. If not, does nothing.
        /// Normalization is done to the vector using the Newton-Raphson approximation method. No square root is taken, but may be less accurate.
        /// </summary>
        FastUnsafe,
    }
}
