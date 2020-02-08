using System.Collections.Generic;
using System.ComponentModel;

namespace TheraEngine.Rendering.UI
{
    public enum ESizingMode
    {
        Auto,
        Fixed,
        Proportional,
    }
    public class UISizingValue : TObject
    {
        private float _value = 0.0f;
        private ESizingMode _mode = ESizingMode.Fixed;

        public ESizingMode Mode
        {
            get => _mode;
            set => Set(ref _mode, value);
        }
        public float Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
    }
    public class UISizingDefinition : TObject
    {
        private UISizingValue _value = new UISizingValue();
        private UISizingValue _min = null;
        private UISizingValue _max = null;

        internal float CalculatedValue { get; set; }
        internal List<UIParentAttachmentInfo> AttachedControls { get; } = new List<UIParentAttachmentInfo>();

        [Browsable(false)]
        public bool NeedsAutoSizing =>
            _value != null && _value.Mode == ESizingMode.Auto ||
            _min != null && _min.Mode == ESizingMode.Auto ||
            _max != null && _max.Mode == ESizingMode.Auto;

        public UISizingValue Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
        public UISizingValue Min
        {
            get => _min;
            set => Set(ref _min, value);
        }
        public UISizingValue Max
        {
            get => _max;
            set => Set(ref _max, value);
        }
    }
}
