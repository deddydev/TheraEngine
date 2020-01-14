using System;

namespace KellermanSoftware.CompareNetObjects.TypeComparers
{
    /// <summary>
    /// Compare primitive types (long, int, short, byte etc.) and DateTime, decimal, and Guid
    /// </summary>
    public class SimpleTypeComparer : BaseTypeComparer
    {
        /// <summary>
        /// Constructor that takes a root comparer
        /// </summary>
        /// <param name="rootComparer"></param>
        public SimpleTypeComparer(RootComparer rootComparer) : base(rootComparer) { }

        /// <summary>
        /// Returns true if the type is a simple type
        /// </summary>
        /// <param name="type1">The type of the first object</param>
        /// <param name="type2">The type of the second object</param>
        /// <returns></returns>
        public override bool IsTypeMatch(Type type1, Type type2)
            => TypeHelper.IsSimpleType(type1) && TypeHelper.IsSimpleType(type2);

        /// <summary>
        /// Compare two simple types
        /// </summary>
        public override void CompareType(CompareParms parms)
        {
            //This should never happen, null check happens one level up
            if (parms.Object1 is null || parms.Object2 is null)
                return;

            if (!(parms.Object1 is IComparable valOne))
                throw new Exception("Expected value does not implement IComparable");

            if (valOne.CompareTo(parms.Object2) != 0)
                AddDifference(parms);
        }
    }
}
