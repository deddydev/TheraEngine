using System.Collections.Generic;
using KellermanSoftware.CompareNetObjects.TypeComparers;


namespace KellermanSoftware.CompareNetObjects
{
    /// <summary>
    /// Factory to create a root comparer
    /// </summary>
    public static class RootComparerFactory
    {
        #region Class Variables
        private static readonly object _locker = new object();
        private static RootComparer _rootComparer;
        #endregion

        #region Methods

        /// <summary>
        /// Get the current root comparer
        /// </summary>
        /// <returns></returns>
        public static RootComparer GetRootComparer()
        {
            lock(_locker)
                if (_rootComparer == null)
                    _rootComparer= BuildRootComparer();

            return _rootComparer;
        }

        private static RootComparer BuildRootComparer()
        {
            _rootComparer = new RootComparer();

            _rootComparer.TypeComparers = new List<BaseTypeComparer>
            {
                new RuntimeTypeComparer(_rootComparer),
#if !NETSTANDARD
                new FontComparer(_rootComparer),
                new IpEndPointComparer(_rootComparer),
                new DatasetComparer(_rootComparer),
                new DataTableComparer(_rootComparer),
                new DataRowComparer(_rootComparer),
                new DataColumnComparer(_rootComparer),
#endif
                new EnumerableComparer(_rootComparer),
                new ByteArrayComparer(_rootComparer),
                new DictionaryComparer(_rootComparer),
                new ListComparer(_rootComparer),
                new HashSetComparer(_rootComparer),
                new CollectionComparer(_rootComparer),
                new EnumComparer(_rootComparer),
                new PointerComparer(_rootComparer),
                new UriComparer(_rootComparer),
                new StringBuilderComparer(_rootComparer),
                new StringComparer(_rootComparer),
                new DateComparer(_rootComparer),
                new DateTimeOffSetComparer(_rootComparer),
                new DoubleComparer(_rootComparer),
                new DecimalComparer(_rootComparer),
                new SimpleTypeComparer(_rootComparer),
                new ClassComparer(_rootComparer),
                new TimespanComparer(_rootComparer),
                new StructComparer(_rootComparer),
                new ImmutableArrayComparer(_rootComparer)
            };
            return _rootComparer;
        }
        #endregion
    }
}
