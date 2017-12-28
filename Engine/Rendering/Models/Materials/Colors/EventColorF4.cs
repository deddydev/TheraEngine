using TheraEngine;
using TheraEngine.Rendering.Models;
using System.Drawing;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Globalization;
using System;

namespace TheraEngine.Rendering.Models.Materials
{
    public class EventColorF4 : IByteColor
    {
        public event Action RedChanged;
        public event Action GreenChanged;
        public event Action BlueChanged;
        public event Action AlphaChanged;
        public event FloatChange RedValueChanged;
        public event FloatChange GreenValueChanged;
        public event FloatChange BlueValueChanged;
        public event FloatChange AlphaValueChanged;
        public event Action Changed;

        ColorF4 _raw;
        private float _oldR, _oldG, _oldB, _oldA;
        
        public EventColorF4(ColorF4 rgba)
        {
            _raw = rgba;
        }

        public void SetRawNoUpdate(ColorF4 raw)
        {
            _raw = raw;
        }

        [Browsable(false)]
        public ColorF4 Raw
        {
            get => _raw;
            set => _raw = value;
        }

        private void BeginUpdate()
        {
            _oldR = R;
            _oldG = G;
            _oldB = B;
            _oldA = A;
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
            if (A != _oldA)
            {
                changed = true;
                AlphaChanged?.Invoke();
                AlphaValueChanged?.Invoke(A, _oldA);
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
        public float A
        {
            get => _raw.A;
            set
            {
                BeginUpdate();
                _raw.A = value;
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

        public override string ToString() => _raw.ToString();

        [Browsable(false)]
        public Color Color { get => _raw.Color; set => _raw.Color = value; }

        public static implicit operator ColorF4(EventColorF4 v) => v._raw;
        public static implicit operator EventColorF4(ColorF4 v) => new EventColorF4(v);
    }
}
