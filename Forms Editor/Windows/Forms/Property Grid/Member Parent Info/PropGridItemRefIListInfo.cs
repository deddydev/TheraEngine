using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefIListInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", Index);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int Index { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Func<object> GetOwner { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public IList OwnerIList => GetOwner() as IList;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType => GetOwner() == null ? null : (Index >= 0 && Index < OwnerIList.Count ? OwnerIList[Index]?.GetType() ?? _dataType : _dataType);
        private readonly Type _dataType;

        public PropGridItemRefIListInfo(Func<object> owner, int index)
        {
            GetOwner = owner;
            Index = index;
            _dataType = OwnerIList?.DetermineElementType();
        }

        public override bool IsReadOnly()
        {
            return OwnerIList == null || OwnerIList.IsReadOnly;
        }
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
        {
            dataChangeHandler?.IListObjectChanged(oldValue, newValue, OwnerIList, Index);
        }
        public override object MemberValue
        {
            get
            {
                if (OwnerIList == null || Index < 0 || Index >= OwnerIList.Count)
                    return DataType.GetDefaultValue();
                return OwnerIList[Index];
            }
            set
            {
                if (OwnerIList != null && Index >= 0 && Index < OwnerIList.Count)
                    OwnerIList[Index] = value;
            }
        }
    }
}
