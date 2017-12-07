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

                display.DataSources.Clear();
                display.PanelLayout = GraphLib.PlotterGraphPaneEx.LayoutMode.NORMAL;

                if (_animation == null)
                    return;

                display.SetDisplayRangeX(0.0f, _animation.LengthInSeconds);

                var source = new GraphLib.DataSource();
                source.Name = "Interpolation";
                _animation.GetMinMax(out float min, out float max);
                source.SetDisplayRangeY(min, max);
                source.SetGridDistanceY((max - min) / 10.0f);
                source.OnRenderXAxisLabel += RenderXLabel;
                source.OnRenderYAxisLabel += RenderYLabel;

                display.DataSources.Add(source);
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
