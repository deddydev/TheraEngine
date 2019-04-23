using System;
using TheraEngine.Core.Reflection;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public interface IPropGridMemberOwner
    {
        object Value { get; }
        bool ReadOnly { get; }
        PropGridMemberInfo MemberInfo { get; }
    }
    public abstract class PropGridMemberInfo : MarshalByRefObject
    {
        public PropGridMemberInfo(IPropGridMemberOwner owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// The type that is expected of this instance's <see cref="MemberValue"/>.
        /// </summary>
        public abstract TypeProxy DataType { get; }
        /// <summary>
        /// False if the <see cref="MemberValue"/> can be set.
        /// </summary>
        public virtual bool IsReadOnly() => Owner?.ReadOnly ?? false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="dataChangeHandler"></param>
        internal protected abstract void SubmitStateChange(object oldValue, object newValue, ValueChangeHandler dataChangeHandler);
        /// <summary>
        /// Getter and setter for the member's value.
        /// </summary>
        public abstract object MemberValue { get; set; }
        /// <summary>
        /// The name of the member that's displayed to the user.
        /// </summary>
        public abstract string DisplayName { get; }
        /// <summary>
        /// The accessor that is written as code to get to this member.
        /// </summary>
        public abstract string MemberAccessor { get; }
        /// <summary>
        /// This is the object that owns this member.
        /// </summary>
        public IPropGridMemberOwner Owner { get; set; }
    }
}
