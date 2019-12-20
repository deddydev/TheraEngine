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
    public interface ISizeable
    {
        //void Update(Vec2 parentBounds);
    }
    public class UIFloat : TObject, ISizeable
    {
        internal bool IgnoreUserChanges { get; set; } = false;

        private float
            _modValue = 0.0f,
            _resValue = 0.0f;
        private ESizingMode _sizingMode = ESizingMode.Pixels;

        private UIFloat
            _propElem = null,
            _minSize,
            _maxSize,
            _origin;

        private bool _smallerRelative = true;
        private UIFloat _valueRange;

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
        internal void SetModificationValueNoUpdate(float value)
        {
            if (IgnoreUserChanges)
                return;

            _modValue = value;
            InvalidateAbsoluteValue();
        }
        internal void SetResultingValueNoUpdate(float value)
        {
            if (IgnoreUserChanges)
                return;

            _resValue = value;
            CalcModValue();
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
                    Set(ref _smallerRelative, value, null, InvalidateAbsoluteValue, true);
            }
        }
        public ESizingMode SizingOption
        {
            get => _sizingMode;
            set
            {
                if (!IgnoreUserChanges)
                    Set(ref _sizingMode, value, null, InvalidateAbsoluteValue, true);
            }
        }

        private void UnReg(UIFloat obj)
        {
            obj.PropertyChanged -= AnyDependentPropertyChanged;
        }
        private void Reg(UIFloat obj)
        {
            obj.PropertyChanged += AnyDependentPropertyChanged;
            InvalidateAbsoluteValue();
        }

        public UIFloat ProportionElement
        {
            get => _propElem;
            set
            {
                if (!IgnoreUserChanges)
                    SetBackingField2(ref _propElem, value, UnReg, Reg);
            }
        }
        public UIFloat Minimum
        {
            get => _minSize;
            set
            {
                if (!IgnoreUserChanges)
                    SetBackingField2(ref _minSize, value, UnReg, Reg);
            }
        }
        public UIFloat Maximum
        {
            get => _maxSize;
            set
            {
                if (!IgnoreUserChanges)
                    SetBackingField2(ref _maxSize, value, UnReg, Reg);
            }
        }
        public UIFloat Origin
        {
            get => _origin;
            set
            {
                if (!IgnoreUserChanges)
                    SetBackingField2(ref _origin, value, UnReg, Reg);
            }
        }
        public UIFloat Range
        {
            get => _valueRange;
            set
            {
                if (!IgnoreUserChanges)
                    SetBackingField2(ref _valueRange, value, UnReg, Reg);
            }
        }

        private void AnyDependentPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) 
            => InvalidateAbsoluteValue();

        public float Value
        {
            get
            {
                if (_resValueInvalidated)
                {
                    _resValueInvalidated = false;
                    CalcResValue();
                }
                return _resValue;
            }
            set
            {
                if (!IgnoreUserChanges && Set(ref _resValue, value))
                    CalcModValue();
            }
        }
        public float RelativeValue
        {
            get => _modValue;
            set
            {
                if (!IgnoreUserChanges && Set(ref _modValue, value))
                    CalcResValue();
            }
        }

        public void InvalidateAbsoluteValue() 
            => Set(ref _resValueInvalidated, true, null, null, false, nameof(Value));

        private bool _resValueInvalidated = false;
        private void CalcResValue()
        {
            float range = Range?.Value ?? 0.0f;
            float newValue = Origin?.Value ?? 0.0f;
            switch (SizingOption)
            {
                default:
                case ESizingMode.Pixels:
                    newValue += _modValue;
                    break;
                case ESizingMode.PercentageOfParent:
                    newValue += range * _modValue;
                    break;
                case ESizingMode.ProportionalToElement:
                    if (ProportionElement != null)
                        newValue += ProportionElement.Value * _modValue;
                    break;
            }

            newValue = _smallerRelative ? newValue : range - newValue;

            if (_minSize != null)
                newValue = newValue.ClampMin(_minSize.Value);

            if (_maxSize != null)
                newValue = newValue.ClampMax(_maxSize.Value);

            _resValue = newValue;
        }

        /// <summary>
        /// Converts the resulting value back into a modification value relative to the current properties.
        /// </summary>
        public void CalcModValue()
        {
            float origin = Origin?.Value ?? 0.0f;
            float size = Range?.Value ?? 0.0f;

            float newValue = _resValue;

            if (_minSize != null)
                newValue = newValue.ClampMin(_minSize.Value);

            if (_maxSize != null)
                newValue = newValue.ClampMax(_maxSize.Value);

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
                        float dim = ProportionElement.Value;
                        if (dim != 0.0f)
                            newValue /= dim;
                        else
                            newValue = 0.0f;
                    }
                    break;
            }

            _modValue = newValue;
        }

        public static implicit operator UIFloat(float value) => Pixels(value, true);

        #region Sizing modes
        public static UIFloat PercentageOfParent(float percentage, bool smallerRelative = true)
        {
            UIFloat e = new UIFloat
            {
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
        public static UIFloat Proportioned(UIFloat proportionalElement, float ratio, bool smallerRelative = true)
        {
            UIFloat e = new UIFloat
            {
                SmallerRelative = smallerRelative
            };
            e.SetSizingProportioned(proportionalElement, ratio);
            return e;
        }
        public void SetSizingProportioned(UIFloat proportionalElement, float ratio)
        {
            SizingOption = ESizingMode.ProportionalToElement;
            ProportionElement = proportionalElement;
            ModificationValue = ratio;
        }
        public static UIFloat Pixels(float pixels, bool smallerRelative = true)
        {
            UIFloat e = new UIFloat
            {
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
    }
    public class SizeableElementQuad : ISizeable
    {
        public UIFloat Left { get; set; } = new UIFloat();
        public UIFloat Right { get; set; } = new UIFloat();
        public UIFloat Top { get; set; } = new UIFloat();
        public UIFloat Bottom { get; set; } = new UIFloat();

        public Vec4 GetLRTB() => new Vec4(
            Left.Value,
            Right.Value,
            Top.Value,
            Bottom.Value);
    }
}
