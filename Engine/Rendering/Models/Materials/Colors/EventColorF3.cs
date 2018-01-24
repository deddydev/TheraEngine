using System.Drawing;
using System.ComponentModel;
using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public class EventColorF3 : IByteColor
    {
        public event Action RedChanged;
        public event Action GreenChanged;
        public event Action BlueChanged;
        public event FloatChange RedValueChanged;
        public event FloatChange GreenValueChanged;
        public event FloatChange BlueValueChanged;
        public event Action Changed;
        
        ColorF3 _raw;
        private float _oldR, _oldG, _oldB;

        public EventColorF3(ColorF3 rgb)
        {
            _raw = rgb;
        }

        public void SetRawNoUpdate(ColorF3 raw)
        {
            _raw = raw;
        }

        [Browsable(false)]
        public ColorF3 Raw
        {
            get => _raw;
            set => _raw = value;
        }

        private void BeginUpdate()
        {
            _oldR = R;
            _oldG = G;
            _oldB = B;
        }
        private void EndUpdate()
        {
            bool changed = false;
            if (R != _oldR)
            {
                changed = true;
                RedChanged?.Invoke();
                RedValueChanged?.Invoke(R, _oldR);
            }
            if (G != _oldG)
            {
                changed = true;
                GreenChanged?.Invoke();
                GreenValueChanged?.Invoke(G, _oldG);
            }
            if (B != _oldB)
            {
                changed = true;
                BlueChanged?.Invoke();
                BlueValueChanged?.Invoke(B, _oldB);
            }
            if (changed)
                Changed?.Invoke();
        }

        public float R
        {
            get => _raw.R;
            set
            {
                BeginUpdate();
                _raw.R = value;
                EndUpdate();
            }
        }
        public float G
        {
            get => _raw.G;
            set
            {
                BeginUpdate();
                _raw.G = value;
                EndUpdate();
            }
        }
        public float B
        {
            get => _raw.B;
            set
            {
                BeginUpdate();
                _raw.B = value;
                EndUpdate();
            }
        }
        public string HexCode
        {
            get => _raw.HexCode;
            set
            {
                BeginUpdate();
                _raw.HexCode = value;
                EndUpdate();
            }
        }

        public override string ToString()
        {
            return _raw.ToString();
        }

        [Browsable(false)]
        public Color Color { get => _raw.Color; set => _raw.Color = value; }

        public static implicit operator ColorF3(EventColorF3 v) { return v._raw; }
        public static implicit operator EventColorF3(ColorF3 v) { return new EventColorF3(v); }
    }
}
