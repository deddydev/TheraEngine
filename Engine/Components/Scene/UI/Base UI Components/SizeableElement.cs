using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheraEngine.Core.Maths.Transforms;

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

        public event Action ParameterChanged;

        private float
            _currValue = 0.0f,
            _modValue = 0.0f,
            _resValue = 0.0f;
        private SizingMode _sizingMode = SizingMode.Ignore;
        private SizeableElement
            _propElem = null,
            _minSize,
            _maxSize;
        private ParentBoundsInheritedValue _parentBoundsInherit = ParentBoundsInheritedValue.Width;
        private bool _smallerRelative = true;

        public float CurrentValue
        {
            get => _currValue;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _currValue = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public float ModificationValue
        {
            get => _modValue;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _modValue = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public float ResultingValue
        {
            get => _resValue;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _resValue = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public SizingMode SizingOption
        {
            get => _sizingMode;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _sizingMode = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public SizeableElement ProportionElement
        {
            get => _propElem;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _propElem = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public SizeableElement Minimum
        {
            get => _minSize;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _minSize = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public SizeableElement Maximum
        {
            get => _maxSize;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _maxSize = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public ParentBoundsInheritedValue ParentBoundsInherited
        {
            get => _parentBoundsInherit;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _parentBoundsInherit = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        /// <summary>
        /// If the resulting value should be calculated relative to the left/bottom (smaller value) or right/top (larger value).
        /// </summary>
        public bool SmallerRelative
        {
            get => _smallerRelative;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _smallerRelative = value;
                    ParameterChanged?.Invoke();
                }
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

            if (Minimum != null)
                ResultingValue = ResultingValue.ClampMin(Minimum.GetValue(parentBounds));

            if (Maximum != null)
                ResultingValue = ResultingValue.ClampMax(Maximum.GetValue(parentBounds));

            return ResultingValue;
        }

        #region Sizing modes
        public static SizeableElement PercentageOfParent(float percentage, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SizeableElement e = new SizeableElement();
            e.SetSizingPercentageOfParent(percentage, smallerRelative, parentDim);
            return e;
        }
        public void SetSizingPercentageOfParent(float percentage, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SmallerRelative = smallerRelative;
            ParentBoundsInherited = parentDim;
            SizingOption = SizingMode.PercentageOfParent;
            ModificationValue = percentage;
        }
        public static SizeableElement Proportioned(SizeableElement proportionalElement, float ratio, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SizeableElement e = new SizeableElement();
            e.SetSizingProportioned(proportionalElement, ratio, smallerRelative, parentDim);
            return e;
        }
        public void SetSizingProportioned(SizeableElement proportionalElement, float ratio, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SmallerRelative = smallerRelative;
            ParentBoundsInherited = parentDim;
            SizingOption = SizingMode.Proportion;
            ProportionElement = proportionalElement;
            ModificationValue = ratio;
        }
        public static SizeableElement Pixels(float pixels, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SizeableElement e = new SizeableElement();
            e.SetSizingPixels(pixels, smallerRelative, parentDim);
            return e;
        }
        /// <summary>
        /// This element's value will be set to specific value.
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="smallerRelative"></param>
        /// <param name="parentDim"></param>
        public void SetSizingPixels(float pixels, bool smallerRelative, ParentBoundsInheritedValue parentDim)
        {
            SmallerRelative = smallerRelative;
            ParentBoundsInherited = parentDim;
            SizingOption = SizingMode.Pixels;
            ModificationValue = pixels;
        }
        public void SetSizingIgnored()
        {
            SizingOption = SizingMode.Ignore;
        }
        #endregion

        public void Update(Vec2 parentBounds)
        {
            GetValue(parentBounds);
        }
    }
    public class SizeableElementQuad : ISizeable
    {
        public SizeableElement Left { get; set; } = new SizeableElement();
        public SizeableElement Right { get; set; } = new SizeableElement();
        public SizeableElement Top { get; set; } = new SizeableElement();
        public SizeableElement Bottom { get; set; } = new SizeableElement();

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
