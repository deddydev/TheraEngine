using System.ComponentModel;

namespace TheraEngine.Rendering.UI
{
    public enum ESizingMode
    {
        Auto,
        Fixed,
        Proportional,
    }
    public class SizingValue : TObject
    {
        private float _value = 0.0f;
        private ESizingMode _mode = ESizingMode.Auto;

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
    public class SizingDefinition : TObject
    {
        private SizingValue _value = null;
        private SizingValue _min = null;
        private SizingValue _max = null;

        [Browsable(false)]
        public bool NeedsAutoSizing =>
            _value != null && _value.Mode == ESizingMode.Auto ||
            _min != null && _min.Mode == ESizingMode.Auto ||
            _max != null && _max.Mode == ESizingMode.Auto;

        public SizingValue Value
        {
            get => _value;
            set => Set(ref _value, value);
        }
        public SizingValue Min
        {
            get => _min;
            set => Set(ref _min, value);
        }
        public SizingValue Max
        {
            get => _max;
            set => Set(ref _max, value);
        }
    }
}
