using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEditor.Core;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.UI.Functions;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMatFuncList : DockContent
    {
        Pen _backPen, _forePen;
        Brush _backBrush, _foreBrush;

        public DockableMatFuncList()
        {
            InitializeComponent();

            _backPen = new Pen(betterListView1.BackColor);
            _forePen = new Pen(betterListView1.ForeColor);
            _backBrush = new SolidBrush(betterListView1.BackColor);
            _foreBrush = new SolidBrush(betterListView1.ForeColor);
            
            betterListView1.DrawColumnHeaderBackground += BetterListView1_DrawColumnHeaderBackground;
            betterListView1.DrawColumnHeader += BetterListView1_DrawColumnHeader;
            betterListView1.BeforeDrag += BetterListView1_BeforeDrag;

            betterListView1.Items.Clear();
            Type matFunc = typeof(MaterialFunction);
            var funcs = Engine.FindTypes(t => !t.IsAbstract && matFunc.IsAssignableFrom(t));
            foreach (Type t in funcs)
            {
                FunctionDefinition def = t.GetCustomAttributeExt<FunctionDefinition>();
                if (def != null)
                {
                    foreach (string keyword in def.Keywords)
                        if (_funcDic.ContainsKey(keyword))
                            _funcDic[keyword].Add(_funcs.Count);
                        else
                            _funcDic.Add(keyword, new List<int>() { _funcs.Count });

                    BetterListViewGroup grp = betterListView1.Groups[def.Category];
                    if (grp == null)
                    {
                        grp = new BetterListViewGroup(def.Category, def.Category);
                        betterListView1.Groups.Add(grp);
                    }

                    MatFuncInfo info = new MatFuncInfo(t, def);

                    betterListView1.Items.Add(new BetterListViewItem(new string[] { def.Name, def.Description }, grp) { Tag = _funcs.Count });

                    _funcs.Add(info);
                }
            }
        }

        DragDropFilter _filter;
        private void BetterListView1_BeforeDrag(object sender, BetterListViewBeforeDragEventArgs eventArgs)
        {
            //Use drag drop filter system instead
            _filter = new DragDropFilter(eventArgs.Data, System.Windows.Forms.DragDropEffects.Move);
            _filter.Done += _filter_Done;
            _filter.BeginFiltering();

            //Cancel the blocking drag operation
            eventArgs.Cancel = true;
        }

        private void betterListView1_ItemDrag(object sender, BetterListViewItemDragEventArgs eventArgs)
        {

        }

        private void _filter_Done(object sender, EventArgs e)
        {
            _filter = null;
        }

        private void BetterListView1_DrawColumnHeader(object sender, BetterListViewDrawColumnHeaderEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(betterListView1.BackColor);
            g.DrawRectangle(_backPen, e.ColumnHeaderBounds.BoundsInner);
            g.DrawString(e.ColumnHeader.Text, e.ColumnHeader.Font, _foreBrush, e.ColumnHeaderBounds.BoundsText.Location);
        }

        private void BetterListView1_DrawColumnHeaderBackground(object sender, BetterListViewDrawColumnHeaderBackgroundEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(betterListView1.BackColor);
            g.DrawRectangle(_backPen, e.ColumnHeaderBounds.BoundsOuter);
        }

        private BindingList<MatFuncInfo> _funcs = new BindingList<MatFuncInfo>();
        private Dictionary<string, List<int>> _funcDic = new Dictionary<string, List<int>>();

        public class MatFuncInfo
        {
            private Type _materialFuncType;
            private FunctionDefinition _funcDef;

            public MatFuncInfo(Type materialFuncType, FunctionDefinition def)
            {
                _materialFuncType = materialFuncType;
                _funcDef = def;
            }

            public override string ToString()
            {
                return _funcDef.Name;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                betterListView1.RemoveSearchHighlight();
            }
            else
            {
                var items = betterListView1.FindItemsWithText(textBox1.Text, new BetterListViewSearchSettings(BetterListViewSearchMode.PrefixOrSubstring, BetterListViewSearchOptions.CaseSensitive | BetterListViewSearchOptions.UpdateSearchHighlight));
                //foreach (var item in items)
                //{

                //}
            }
        }
    }
}
