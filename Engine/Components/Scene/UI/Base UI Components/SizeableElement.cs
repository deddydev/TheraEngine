using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Rendering.UI
{
    public enum SizingMode
    {
        Ignore,
        Pixels,
        PercentageOfParent,
        Proportion,
    }
    public enum ParentBoundsInheritedValue
    {
        Width,
        Height,
    }
    public interface ISizeable
    {
        void Update(Vec2 parentBounds);
    }
    public class SizeableElement : ISizeable
    {
        internal bool IgnoreUserChanges { get; set; } = false;
        private float _currValue = 0.0f, _modValue = 0.0f, _resValue = 0.0f;
        private SizingMode _sizingMode = SizingMode.Ignore;
        private SizeableElement _propElem = null;
        private ParentBoundsInheritedValue _parentBoundsInherit = ParentBoundsInheritedValue.Width;
        private bool _smallerRelative = false;

        public float CurrentValue
        {
            get => _currValue;
            set
            {
                if (!IgnoreUserChanges)
                    _currValue = value;
            }
        }
        public float ModificationValue
        {
            get => _modValue;
            set
            {
                if (!IgnoreUserChanges)
                    _modValue = value;
            }
        }
        public float ResultingValue
        {
            get => _resValue;
            set
            {
                if (!IgnoreUserChanges)
                    _resValue = value;
            }
        }
        public SizingMode SizingOption
        {
            get => _sizingMode;
            set
            {
                if (!IgnoreUserChanges)
                    _sizingMode = value;
            }
        }
        public SizeableElement ProportionElement
        {
            get => _propElem;
            set
            {
                if (!IgnoreUserChanges)
                    _propElem = value;
            }
        }
        public ParentBoundsInheritedValue ParentBoundsInherited
        {
            get => _parentBoundsInherit;
            set
            {
                if (!IgnoreUserChanges)
                    _parentBoundsInherit = value;
            }
        }
        public bool SmallerRelative
        {
            get => _smallerRelative;
            set
            {
                if (!IgnoreUserChanges)
                    _smallerRelative = value;
            }
        }
        private float GetDim(Vec2 parentBounds)
        {
            switch (ParentBoundsInherited)
            {
                case ParentBoundsInheritedValue.Width:
                    return parentBounds.X;
                case ParentBoundsInheritedValue.Height:
                    return parentBounds.Y;
            }
            return 0.0f;
        }
        public float GetValue(Vec2 parentBounds)
        {
            switch (SizingOption)
            {
                default:
                case SizingMode.Ignore:
                    ResultingValue = CurrentValue;
                    break;
                case SizingMode.Pixels:
                    ResultingValue = _smallerRelative ? ModificationValue : GetDim(parentBounds) - ModificationValue;
                    break;
                case SizingMode.PercentageOfParent:
                    float size = GetDim(parentBounds);
                    float scaledValue = size * ModificationValue;
                    ResultingValue = _smallerRelative ? scaledValue : size - scaledValue;
                    break;
                case SizingMode.Proportion:
                    if (ProportionElement != null)
                    {
                        ResultingValue = ProportionElement.GetValue(parentBounds) * ModificationValue;
                        if (!_smallerRelative)
                            ResultingValue = GetDim(parentBounds) - ResultingValue;
                    }
                    break;
            }
            return ResultingValue;
        }

        public void Update(Vec2 parentBounds)
        {
            GetValue(parentBounds);
        }
    }
    public class SizeableElementQuad : ISizeable
    {
        public SizeableElement Left { get; set; }
        public SizeableElement Right { get; set; }
        public SizeableElement Top { get; set; }
        public SizeableElement Bottom { get; set; }

        public Vec4 GetLRTB(Vec2 parentBounds)
        {
            return new Vec4(
                Left.GetValue(parentBounds),
                Right.GetValue(parentBounds),
                Top.GetValue(parentBounds),
                Bottom.GetValue(parentBounds));
        }
        public void Update(Vec2 parentBounds)
        {
            GetLRTB(parentBounds);
        }
    }
}
