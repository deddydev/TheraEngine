using System;
using TheraEngine;
using TheraEngine.Animation;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    [EditorFor(typeof(PropAnimFloat))]
    public partial class PropAnimFloatEditor : DockContent
    {
        private UIMaterialEditor _hud;
        public PropAnimFloatEditor()
        {
            InitializeComponent();
            display.DataSources.Clear();
            display.PanelLayout = GraphLib.PlotterGraphPaneEx.LayoutMode.NORMAL;
            var source = new GraphLib.DataSource() { Name = "Interpolation", };
            source.OnRenderXAxisLabel += RenderXLabel;
            source.OnRenderYAxisLabel += RenderYLabel;
            display.DataSources.Add(source);
        }
        public PropAnimFloatEditor(PropAnimFloat anim) : this()
        {
            Animation = anim;
        }
        private PropAnimFloat _animation;
        public PropAnimFloat Animation
        {
            get => _animation;
            set
            {
                _animation = value;
                
                if (_animation == null)
                    return;

                RegenerateSamples();
            }
        }
        public void RegenerateSamples()
        {
            if (_animation == null)
                return;

            display.SetDisplayRangeX(0.0f, _animation.LengthInSeconds);
            GraphLib.DataSource source = display.DataSources[0];

            _animation.GetMinMax(out float min, out float max);
            source.SetDisplayRangeY(min, max);
            source.SetGridDistanceY((max - min) / 10.0f);

            int precision = 3;
            float mult = (float)Math.Pow(10.0, precision);
            float invMult = 1.0f / mult;
            float dist = 0.0f;
            source.Length = (int)(Math.Round(_animation.LengthInSeconds, precision, MidpointRounding.AwayFromZero) * mult);
            for (int i = 0; i < source.Length; i++, dist += invMult)
            {
                source.Samples[i].x = dist;
                source.Samples[i].y = _animation.GetValue(dist);
            }

        }
        private string RenderXLabel(GraphLib.DataSource s, int idx)
        {
            if (s.AutoScaleX)
            {
                int Value = (int)(s.Samples[idx].x);
                return "" + Value;
            }
            else
            {
                int Value = (int)(s.Samples[idx].x / 200);
                String Label = "" + Value + "\"";
                return Label;
            }
        }

        private String RenderYLabel(GraphLib.DataSource s, float value)
        {
            return String.Format("{0:0.0}", value);
        }
    }
}
