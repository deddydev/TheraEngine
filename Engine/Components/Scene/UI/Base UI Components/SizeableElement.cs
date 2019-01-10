using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public enum SizingMode
    {
        /// <summary>
        /// 
        /// </summary>
        //Ignore,
        /// <summary>
        /// 
        /// </summary>
        Pixels,
        /// <summary>
        /// 
        /// </summary>
        PercentageOfParent,
        /// <summary>
        /// 
        /// </summary>
        Proportion,
    }
    public enum EParentBoundsInheritedValue
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
        private SizingMode _sizingMode = SizingMode.Pixels;
        private SizeableElement
            _propElem = null,
            _minSize,
            _maxSize,
            _origin;
        private EParentBoundsInheritedValue _parentBoundsInherit = EParentBoundsInheritedValue.Width;
        private bool _smallerRelative = true;

        //public float CurrentValue
        //{
        //    get => _currValue;
        //    set
        //    {
        //        if (!IgnoreUserChanges)
        //        {
        //            _currValue = value;
        //            ParameterChanged?.Invoke();
        //        }
        //    }
        //}
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
        public SizeableElement Origin
        {
            get => _origin;
            set
            {
                if (!IgnoreUserChanges)
                {
                    _origin = value;
                    ParameterChanged?.Invoke();
                }
            }
        }
        public EParentBoundsInheritedValue ParentBoundsInherited
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
                case EParentBoundsInheritedValue.Width:
                    return parentBounds.X;
                case EParentBoundsInheritedValue.Height:
                    return parentBounds.Y;
            }
            return 0.0f;
        }
        public float GetValue(Vec2 parentBounds)
        {
            float origin = Origin?.GetValue(parentBounds) ?? 0.0f;
            float size = GetDim(parentBounds);
            float newValue = origin;
            switch (SizingOption)
            {
                default:
                case SizingMode.Pixels:
                    newValue += ModificationValue;
                    break;
                case SizingMode.PercentageOfParent:
                    newValue += size * ModificationValue;
                    break;
                case SizingMode.Proportion:
                    if (ProportionElement != null)
                        newValue += ProportionElement.GetValue(parentBounds) * ModificationValue;
                    break;
            }

            ResultingValue = _smallerRelative ? newValue : size - newValue;

            if (Minimum != null)
                ResultingValue = ResultingValue.ClampMin(Minimum.GetValue(parentBounds));

            if (Maximum != null)
                ResultingValue = ResultingValue.ClampMax(Maximum.GetValue(parentBounds));

            return ResultingValue;
        }

        #region Sizing modes
        public static SizeableElement PercentageOfParent(float percentage, bool smallerRelative, EParentBoundsInheritedValue parentBoundsInherited)
        {
            SizeableElement e = new SizeableElement
            {
                ParentBoundsInherited = parentBoundsInherited,
                SmallerRelative = smallerRelative
            };
            e.SetSizingPercentageOfParent(percentage);
            return e;
        }
        public void SetSizingPercentageOfParent(float percentage)
        {
            SizingOption = SizingMode.PercentageOfParent;
            ModificationValue = percentage;
        }
        public static SizeableElement Proportioned(SizeableElement proportionalElement, float ratio, bool smallerRelative, EParentBoundsInheritedValue parentBoundsInherited)
        {
            SizeableElement e = new SizeableElement
            {
                ParentBoundsInherited = parentBoundsInherited,
                SmallerRelative = smallerRelative
            };
            e.SetSizingProportioned(proportionalElement, ratio);
            return e;
        }
        public void SetSizingProportioned(SizeableElement proportionalElement, float ratio)
        {
            SizingOption = SizingMode.Proportion;
            ProportionElement = proportionalElement;
            ModificationValue = ratio;
        }
        public static SizeableElement Pixels(float pixels, bool smallerRelative, EParentBoundsInheritedValue parentBoundsInherited)
        {
            SizeableElement e = new SizeableElement
            {
                ParentBoundsInherited = parentBoundsInherited,
                SmallerRelative = smallerRelative
            };
            e.SetSizingPixels(pixels);
            return e;
        }
        /// <summary>
        /// This element's value will be set to specific value.
        /// </summary>
        /// <param name="pixels"></param>
        /// <param name="smallerRelative"></param>
        /// <param name="parentDim"></param>
        public void SetSizingPixels(float pixels)
        {
            SizingOption = SizingMode.Pixels;
            ModificationValue = pixels;
        }
        //public void SetSizingIgnored()
        //{
        //    SizingOption = SizingMode.Ignore;
        //}
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
