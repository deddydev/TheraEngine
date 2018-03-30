using System;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Rendering;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableAnalytics : DockContent
    {
        public DockableAnalytics()
        {
            InitializeComponent();
            theraPropertyGrid1.Enabled = true;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            lstRenderObjects.View = View.Details;
            lstRenderObjects.Sorting = SortOrder.Ascending;
            lstRenderObjects.Columns.Add(new ColumnHeader() { Text = "Type" });
            lstRenderObjects.Columns.Add(new ColumnHeader() { Text = "Binding ID" });
            lstRenderObjects.Columns.Add(new ColumnHeader() { Text = "Context Index" });
            lstRenderObjects.Columns.Add(new ColumnHeader() { Text = "Generation Time" });
            RenderContext.BoundContextsChanged += RenderContext_BoundContextsChanged;
            for (int r = 0; r < RenderContext.BoundContexts.Count; ++r)
            {
                var states = RenderContext.BoundContexts[r].States;
                for (int i = 0; i < states.Count; ++i)
                {
                    BaseRenderState.ContextBind bind = states[i];
                    BaseRenderState state = bind.ParentState;
                    string[] cols = new string[]
                    {
                        state.Type.ToString(),
                        bind.BindingId.ToString(),
                        bind.ContextIndex.ToString(),
                        bind.GenerationTime.ToString()
                    };
                    ListViewItem item = new ListViewItem(cols) { Tag = bind };
                    lstRenderObjects.Items.Add(item);
                }
            }
        }

        private void RenderContext_BoundContextsChanged(RenderContext context, bool added)
        {

        }

        private void lstRenderObjects_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var bind = e.Item.Tag as BaseRenderState.ContextBind;
            richTextBox1.Text = bind?.GenerationStackTrace;
            theraPropertyGrid1.SubObject = bind?.ParentState;
        }
    }
}
