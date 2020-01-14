#if !NETSTANDARD

using System;
using System.Data;
using System.Globalization;
using System.Linq;

namespace KellermanSoftware.CompareNetObjects.TypeComparers
{
    /// <summary>
    /// Compare all rows in a data table
    /// </summary>
    public class DataTableComparer : BaseTypeComparer
    {
        /// <summary>
        /// Constructor that takes a root comparer
        /// </summary>
        /// <param name="rootComparer"></param>
        public DataTableComparer(RootComparer rootComparer)
            : base(rootComparer) { }

        /// <summary>
        /// Returns true if both objects are of type DataTable
        /// </summary>
        /// <param name="type1">The type of the first object</param>
        /// <param name="type2">The type of the second object</param>
        /// <returns></returns>
        public override bool IsTypeMatch(Type type1, Type type2)
            => TypeHelper.IsDataTable(type1) && TypeHelper.IsDataTable(type2);

        /// <summary>
        /// Compare two datatables
        /// </summary>
        public override void CompareType(CompareParms parms)
        {
            //This should never happen, null check happens one level up
            if (!(parms.Object1 is DataTable dataTable1) || !(parms.Object2 is DataTable dataTable2))
                return;

            //Only compare specific table names
            if (parms.Config.MembersToInclude.Count > 0 && !parms.Config.MembersToInclude.Contains(dataTable1.TableName))
                return;

            //If we should ignore it, skip it
            if (parms.Config.MembersToInclude.Count == 0 && parms.Config.MembersToIgnore.Contains(dataTable1.TableName))
                return;

            //There must be the same amount of rows in the datatable
            if (dataTable1.Rows.Count != dataTable2.Rows.Count)
            {
                Difference difference = new Difference
                {
                    ParentObject1 = parms.ParentObject1,
                    ParentObject2 = parms.ParentObject2,
                    PropertyName = parms.BreadCrumb,
                    Object1Value = dataTable1.Rows.Count.ToString(CultureInfo.InvariantCulture),
                    Object2Value = dataTable2.Rows.Count.ToString(CultureInfo.InvariantCulture),
                    ChildPropertyName = "Rows.Count",
                    Object1 = parms.Object1,
                    Object2 = parms.Object2
                };

                AddDifference(parms.Result, difference);

                if (parms.Result.ExceededDifferences)
                    return;
            }

            if (!ColumnsDifferent(parms))
                CompareEachRow(parms);
        }

        private bool ColumnsDifferent(CompareParms parms)
        {
            if (!(parms.Object1 is DataTable dataTable1))
                throw new ArgumentException("parms.Object1");

            if (!(parms.Object2 is DataTable dataTable2))
                throw new ArgumentException("parms.Object2");

            if (dataTable1.Columns.Count != dataTable2.Columns.Count)
            {
                Difference difference = new Difference
                {
                    ParentObject1 = parms.ParentObject1,
                    ParentObject2 = parms.ParentObject2,
                    PropertyName = parms.BreadCrumb,
                    Object1Value = dataTable1.Columns.Count.ToString(CultureInfo.InvariantCulture),
                    Object2Value = dataTable2.Columns.Count.ToString(CultureInfo.InvariantCulture),
                    ChildPropertyName = "Columns.Count",
                    Object1 = parms.Object1,
                    Object2 = parms.Object2
                };

                AddDifference(parms.Result, difference);

                if (parms.Result.ExceededDifferences)
                    return true;
            }

            foreach (var i in Enumerable.Range(0, dataTable1.Columns.Count))
            {
                string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Columns", string.Empty, i);

                CompareParms childParms = new CompareParms
                {
                    Result = parms.Result,
                    Config = parms.Config,
                    ParentObject1 = parms.Object1,
                    ParentObject2 = parms.Object2,
                    Object1 = dataTable1.Columns[i],
                    Object2 = dataTable2.Columns[i],
                    BreadCrumb = currentBreadCrumb
                };

                RootComparer.Compare(childParms);

                if (parms.Result.ExceededDifferences)
                    return true;
            }

            return false;
        }

        private void CompareEachRow(CompareParms parms)
        {
            if (!(parms.Object1 is DataTable dataTable1))
                throw new ArgumentException("parms.Object1");

            if (!(parms.Object2 is DataTable dataTable2))
                throw new ArgumentException("parms.Object2");

            for (int i = 0; i < Math.Min(dataTable1.Rows.Count, dataTable2.Rows.Count); i++)
            {
                string currentBreadCrumb = AddBreadCrumb(parms.Config, parms.BreadCrumb, "Rows", string.Empty, i);

                CompareParms childParms = new CompareParms
                {
                    Result = parms.Result,
                    Config = parms.Config,
                    ParentObject1 = parms.Object1,
                    ParentObject2 = parms.Object2,
                    Object1 = dataTable1.Rows[i],
                    Object2 = dataTable2.Rows[i],
                    BreadCrumb = currentBreadCrumb
                };

                RootComparer.Compare(childParms);

                if (parms.Result.ExceededDifferences)
                    return;
            }
        }
    }
}

#endif
