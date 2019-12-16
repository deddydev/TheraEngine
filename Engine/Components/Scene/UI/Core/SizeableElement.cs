using Extensions;
using System;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public enum ESizingMode
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
        ProportionalToElement,
        /// <summary>
        /// 
        /// </summary>
        Ratio,
    }
    public enum EParentBoundsInheritedValue
    {
        Width,
        Height,
    }
    public interface ISizeable
    {
        //void Update(Vec2 parentBounds);
    }
    public class SizeableElement : TObject, ISizeable
    {
        internal bool IgnoreUserChanges { get; set; } = false;

        public event Action ParameterChanged;

        private float
            _modValue = 0.0f,
            _resValue = 0.0f;
        private ESizingMode _sizingMode = ESizingMode.Pixels;
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
        //[ReadOnly(true)]
        //public float ResultingValue
        //{
        //    get => _resValue;
        //    //set
        //    //{
        //    //    if (!IgnoreUserChanges)
        //    //    {
        //    //        _resValue = value;
        //    //        ParameterChanged?.Invoke();
        //    //    }
        //    //}
        //}
        public void SetResultingValue(float value, Vec2 parentBounds)
        {
            if (IgnoreUserChanges)
                return;

            _resValue = value;
            GetModificationValue(parentBounds);
            ParameterChanged?.Invoke();
        }
        internal void SetModificationValueNoUpdate(float value)
        {
            if (!IgnoreUserChanges)
                _modValue = value;
        }
        internal void SetResultingValueNoUpdate(float value, Vec2 parentBounds)
        {
            if (IgnoreUserChanges)
                return;

            _resValue = value;
            GetModificationValue(parentBounds);
        }
        public ESizingMode SizingOption
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
        public float GetResultingValue(Vec2 parentBounds)
        {
            float origin = Origin?.GetResultingValue(parentBounds) ?? 0.0f;
            float size = GetDim(parentBounds);
            float newValue = origin;
            switch (SizingOption)
            {
                default:
                case ESizingMode.Pixels:
                    newValue += _modValue;
                    break;
                case ESizingMode.PercentageOfParent:
                    newValue += size * _modValue;
                    break;
                case ESizingMode.ProportionalToElement:
                    if (ProportionElement != null)
                        newValue += ProportionElement.GetResultingValue(parentBounds) * _modValue;
                    break;
            }

            newValue = _smallerRelative ? newValue : size - newValue;

            if (Minimum != null)
                newValue = newValue.ClampMin(Minimum.GetResultingValue(parentBounds));

            if (Maximum != null)
                newValue = newValue.ClampMax(Maximum.GetResultingValue(parentBounds));

            return _resValue = newValue;
        }
        public float GetModificationValue(Vec2 parentBounds)
        {
            float origin = Origin?.GetResultingValue(parentBounds) ?? 0.0f;
            float size = GetDim(parentBounds);

            float newValue = _resValue;

            if (Minimum != null)
                newValue = newValue.ClampMin(Minimum.GetResultingValue(parentBounds));

            if (Maximum != null)
                newValue = newValue.ClampMax(Maximum.GetResultingValue(parentBounds));

            if (!_smallerRelative)
                newValue = size - newValue;

            newValue -= origin;
            switch (SizingOption)
            {
                default:
                case ESizingMode.Pixels:
                    break;
                case ESizingMode.PercentageOfParent:
                    if (size != 0.0f)
                        newValue /= size;
                    else
                        newValue = 0.0f;
                    break;
                case ESizingMode.ProportionalToElement:
                    if (ProportionElement != null)
                    {
                        float dim = ProportionElement.GetResultingValue(parentBounds);
                        if (dim != 0.0f)
                            newValue /= dim;
                        else
                            newValue = 0.0f;
                    }
                    break;
            }
            
            return _modValue = newValue;
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
            SizingOption = ESizingMode.PercentageOfParent;
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
            SizingOption = ESizingMode.ProportionalToElement;
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
            SizingOption = ESizingMode.Pixels;
            ModificationValue = pixels;
        }
        //public void SetSizingIgnored()
        //{
        //    SizingOption = SizingMode.Ignore;
        //}
        #endregion

        //public void Update(Vec2 parentBounds)
        //{
        //    GetValue(parentBounds);
        //}
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
                Left.GetResultingValue(parentBounds),
                Right.GetResultingValue(parentBounds),
                Top.GetResultingValue(parentBounds),
                Bottom.GetResultingValue(parentBounds));
        }
        public void Update(Vec2 parentBounds)
        {
            GetLRTB(parentBounds);
        }
    }
}
