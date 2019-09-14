using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridItemRefNullableInfo : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => _parentInfo.MemberAccessor + ".Value";
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => _parentInfo.DisplayName;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TypeProxy DataType { get; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override object MemberValue { get => _parentInfo.MemberValue; set => _parentInfo.MemberValue = value; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override bool IsReadOnly() => base.IsReadOnly() || _parentInfo.IsReadOnly();
        //[EditorBrowsable(EditorBrowsableState.Never)]
        //[Browsable(false)]
        //public override PropGridItem Owner
        //{
        //    get => _parentInfo.Owner;
        //    set => _parentInfo.Owner = value;
        //}

        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler)
            => _parentInfo.SubmitStateChange(oldValue, newValue, dataChangeHandler);
        private readonly PropGridMemberInfo _parentInfo;

        public PropGridItemRefNullableInfo(IPropGridMemberOwner owner, PropGridMemberInfo parentInfo, TypeProxy valueType) : base(owner)
        {
            if (parentInfo?.DataType is null || 
                !parentInfo.DataType.IsGenericType || 
                parentInfo.DataType.GetGenericTypeDefinition() != typeof(Nullable<>))
                throw new Exception();
            
            _parentInfo = parentInfo;
            DataType = valueType;
        }
    }
}
