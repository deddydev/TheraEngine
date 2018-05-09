//using ComponentOwl.BetterListView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using TheraEditor.Core;
using TheraEditor.Core.Extensions;
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

            //_backPen = new Pen(betterListView1.BackColor);
            //_forePen = new Pen(betterListView1.ForeColor);
            //_backBrush = new SolidBrush(betterListView1.BackColor);
            //_foreBrush = new SolidBrush(betterListView1.ForeColor);
            
            ////betterListView1.DrawColumnHeaderBackground += BetterListView1_DrawColumnHeaderBackground;
            ////betterListView1.DrawColumnHeader += BetterListView1_DrawColumnHeader;
            //betterListView1.BeforeDrag += BetterListView1_BeforeDrag;

            //betterListView1.Items.Clear();
            //Type matFunc = typeof(MaterialFunction);
            //var funcs = Engine.FindAllTypes(t => !t.IsAbstract && matFunc.IsAssignableFrom(t));
            //foreach (Type t in funcs)
            //{
            //    FunctionDefinition def = t.GetCustomAttributeExt<FunctionDefinition>();
            //    if (def != null)
            //    {
            //        foreach (string keyword in def.Keywords)
            //            if (_funcDic.ContainsKey(keyword))
            //                _funcDic[keyword].Add(_funcs.Count);
            //            else
            //                _funcDic.Add(keyword, new List<int>() { _funcs.Count });

            //        BetterListViewGroup grp = betterListView1.Groups[def.Category];
            //        if (grp == null)
            //        {
            //            grp = new BetterListViewGroup(def.Category, def.Category);
            //            betterListView1.Groups.Add(grp);
            //        }

            //        if (t.ContainsGenericParameters)
            //        {
            //            foreach (Type arg in t.GetGenericArguments())
            //            {
            //                arg.GetGenericParameterConstraints(out GenericVarianceFlag gvf, out TypeConstraintFlag tcf);
            //                bool test(Type type)
            //                {
            //                    return !(

            //                    //Base type isn't requested base type?
            //                    (arg.BaseType != null && !arg.BaseType.IsAssignableFrom(type)) ||

            //                    //Doesn't fit constraints?
            //                    !type.FitsConstraints(gvf, tcf)// ||

            //                    //Has no default constructor?
            //                    //type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null) == null
            //                    );
            //                }

            //                foreach (Type r in Engine.FindAllTypes(x => test(x)))
            //                {
            //                    MatFuncInfo info = new MatFuncInfo(t.MakeGenericType(r), def);

            //                    betterListView1.Items.Add(new BetterListViewItem(
            //                        new string[] { def.Name + "[" + r.GetFriendlyName() + "]", def.Description }, grp)
            //                    {
            //                        Tag = _funcs.Count
            //                    });

            //                    _funcs.Add(info);
            //                }
            //            }
            //        }
            //        else
            //        {
            //            MatFuncInfo info = new MatFuncInfo(t, def);

            //            betterListView1.Items.Add(new BetterListViewItem(
            //                new string[] { def.Name, def.Description }, grp)
            //            {
            //                Tag = _funcs.Count
            //            });

            //            _funcs.Add(info);
            //        }
            //    }
            //}
        }

        private bool _isDragging = false;
        //private void BetterListView1_BeforeDrag(object sender, BetterListViewBeforeDragEventArgs eventArgs)
        //{
        //    //Cancel the blocking drag operation
        //    eventArgs.Cancel = true;
        //    if (_isDragging)
        //        return;
        //    //Use drag drop filter system instead
        //    DragDropFilter f = DragDropFilter.Initialize(eventArgs.Data, System.Windows.Forms.DragDropEffects.Move);
        //    if (f != null)
        //    {
        //        _isDragging = true;
        //        f.Done += _filter_Done;
        //        Editor.Instance.Invoke((Action)(() => f.BeginFiltering()));
        //    }
        //}

        //private void betterListView1_ItemDrag(object sender, BetterListViewItemDragEventArgs eventArgs)
        //{

        //}

        //private void _filter_Done(object sender, EventArgs e)
        //{
        //    _isDragging = false;
        //}

        //private void BetterListView1_DrawColumnHeader(object sender, BetterListViewDrawColumnHeaderEventArgs e)
        //{
        //    Graphics g = e.Graphics;
        //    g.Clear(betterListView1.BackColor);
        //    g.DrawRectangle(_backPen, e.ColumnHeaderBounds.BoundsInner);
        //    g.DrawString(e.ColumnHeader.Text, e.ColumnHeader.Font, _foreBrush, e.ColumnHeaderBounds.BoundsText.Location);
        //}

        //private void BetterListView1_DrawColumnHeaderBackground(object sender, BetterListViewDrawColumnHeaderBackgroundEventArgs e)
        //{
        //    Graphics g = e.Graphics;
        //    g.Clear(betterListView1.BackColor);
        //    g.DrawRectangle(_backPen, e.ColumnHeaderBounds.BoundsOuter);
        //}

        internal BindingList<MatFuncInfo> _funcs = new BindingList<MatFuncInfo>();
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

            public MaterialFunction CreateNew()
            {
                return (MaterialFunction)Activator.CreateInstance(_materialFuncType);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text))
            {
                //betterListView1.RemoveSearchHighlight();
            }
            else
            {
                //var items = betterListView1.FindItemsWithText(textBox1.Text, new BetterListViewSearchSettings(BetterListViewSearchMode.PrefixOrSubstring, BetterListViewSearchOptions.CaseSensitive | BetterListViewSearchOptions.UpdateSearchHighlight));
                //foreach (var item in items)
                //{

                //}
            }
        }
    }
}
