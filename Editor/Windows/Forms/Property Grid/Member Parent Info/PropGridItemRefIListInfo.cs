using Extensions;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Core.Reflection;
using TheraEngine.Editor;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public class PropGridMemberInfoIList : PropGridMemberInfo
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string MemberAccessor => "." + Owner.MemberInfo.DisplayName + DisplayName;
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string DisplayName => string.Format("[{0}]", Index);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public int Index { get; set; }
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override TypeProxy DataType => List == null ? ListElementType : (Index >= 0 && Index < List.Count ? List[Index]?.GetTypeProxy() ?? ListElementType : ListElementType);
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public TypeProxy ListElementType { get; private set; }

        public IList List => Owner.Value as IList;

        public PropGridMemberInfoIList(IPropGridMemberOwner owner, int index) : base(owner)
        {
            Index = index;
            ListElementType = List?.DetermineElementTypeProxy();
        }

        public override bool IsReadOnly() 
            => base.IsReadOnly() || List == null || List.IsReadOnly;
        
        internal protected override void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler)
            => dataChangeHandler?.HandleChange(new LocalValueChangeIList(oldValue, newValue, List, Index));
        
        public override object MemberValue
        {
            get
            {
                if (List == null || Index < 0 || Index >= List.Count)
                    return DataType.GetDefaultValue();
                return List[Index];
            }
            set
            {
                if (List != null && Index >= 0 && Index < List.Count)
                    List[Index] = value;
            }
        }
    }
}
