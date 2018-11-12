using System;
using System.ComponentModel;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefNullableInfo : PropGridItemRefInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => _parentInfo.DisplayName;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Type DataType { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object MemberValue { get => _parentInfo.MemberValue; set => _parentInfo.MemberValue = value; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override bool IsReadOnly() => _parentInfo.IsReadOnly();
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override Func<object> GetOwner
        {
            get => _parentInfo.GetOwner;
            set => _parentInfo.GetOwner = value;
        }
        internal protected override void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler)
            => _parentInfo.SubmitStateChange(oldValue, newValue, dataChangeHandler);
        private readonly PropGridItemRefInfo _parentInfo;

        public PropGridItemRefNullableInfo(PropGridItemRefInfo parentInfo, Type valueType)
        {
            if (parentInfo?.DataType == null || 
                !parentInfo.DataType.IsGenericType || 
                parentInfo.DataType.GetGenericTypeDefinition() != typeof(Nullable<>))
                throw new Exception();

            _parentInfo = parentInfo;
            DataType = valueType;
        }
    }
}
