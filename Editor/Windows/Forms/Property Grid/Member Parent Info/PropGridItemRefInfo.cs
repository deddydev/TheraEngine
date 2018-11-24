using System;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    public abstract class PropGridItemRefInfo
    {
        /// <summary>
        /// The type that is expected of this instance's <see cref="MemberValue"/>.
        /// </summary>
        public abstract Type DataType { get; }
        /// <summary>
        /// False if the <see cref="MemberValue"/> can be set.
        /// </summary>
        public abstract bool IsReadOnly();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        /// <param name="dataChangeHandler"></param>
        internal protected abstract void SubmitStateChange(object oldValue, object newValue, IDataChangeHandler dataChangeHandler);
        /// <summary>
        /// Getter and setter for the member's value.
        /// </summary>
        public abstract object MemberValue { get; set; }
        /// <summary>
        /// The name of the member that's displayed to the user.
        /// </summary>
        public abstract string DisplayName { get; }
        /// <summary>
        /// Call to retrieve a boxed version of the member's current state from its owner.
        /// </summary>
        public abstract Func<object> GetOwner { get; set; }
    }
}
