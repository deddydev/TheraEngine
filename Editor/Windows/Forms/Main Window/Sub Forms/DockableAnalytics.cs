using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine.Rendering;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableGPUAnalytics : DockContent
    {
        public DockableGPUAnalytics()
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

            EventList<RenderContext> ctxList = Editor.DomainProxy.BoundContexts;
            ctxList.CollectionChanged += Ctx_CollectionChanged;
            foreach (RenderContext ctx in ctxList)
                Add(ctx);
        }

        private ConcurrentDictionary<BaseRenderObject, ListViewItem> ObjItemDic { get; }
            = new ConcurrentDictionary<BaseRenderObject, ListViewItem>();

        private void Add(RenderContext renderContext)
        {
            var states = renderContext?.States;
            if (states is null)
                return;

            foreach (BaseRenderObject.ContextBind bind in states)
            {
                BaseRenderObject state = bind.ParentState;
                string[] cols = new string[]
                {
                    state.Type.ToString(),
                    bind.BindingId.ToString(),
                    bind.GenerationTime.ToString()
                };
                ListViewItem item = new ListViewItem(cols) { Tag = bind };
                lstRenderObjects.Items.Add(item);
            }
        }
        private void Remove(RenderContext renderContext)
        {
            var states = renderContext?.States;
            if (states is null)
                return;

            foreach (BaseRenderObject.ContextBind bind in states)
            {
                BaseRenderObject state = bind.ParentState;
                if (!ObjItemDic.ContainsKey(state))
                    return;

                ListViewItem item = ObjItemDic[state];
                if (item != null)
                    lstRenderObjects.Items.Remove(item);
            }
        }

        private void Ctx_CollectionChanged(object sender, TheraEngine.Core.TCollectionChangedEventArgs<RenderContext> e)
        {
            switch (e.Action)
            {
                case TheraEngine.Core.ECollectionChangedAction.Add:
                    foreach (var ctx in e.NewItems)
                        Add(ctx);
                    break;
                case TheraEngine.Core.ECollectionChangedAction.Remove:
                    foreach (var ctx in e.OldItems)
                        Remove(ctx);
                    break;
                case TheraEngine.Core.ECollectionChangedAction.Clear:
                    lstRenderObjects.Items.Clear();
                    break;
                case TheraEngine.Core.ECollectionChangedAction.Replace:
                    foreach (var ctx in e.OldItems)
                        Remove(ctx);
                    foreach (var ctx in e.NewItems)
                        Add(ctx);
                    break;
            }
        }
        private void lstRenderObjects_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var bind = e.Item.Tag as BaseRenderObject.ContextBind;
            richTextBox1.Text = bind?.GenerationStackTrace;
            theraPropertyGrid1.TargetObject = bind?.ParentState;
        }
    }
}
