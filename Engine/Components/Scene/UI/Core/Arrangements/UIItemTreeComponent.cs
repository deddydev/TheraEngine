using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public class UIItemTreeComponent : UIBoundableComponent
    {
        private EventList<UISizingDefinition> _rows;
        private EventList<UISizingDefinition> _columns;
        private List<int>[,] _indices;
        
        public UIItemTreeComponent()
        {
            _rows = new EventList<UISizingDefinition>();
            _rows.CollectionChanged += CollectionChanged;

            _columns = new EventList<UISizingDefinition>();
            _columns.CollectionChanged += CollectionChanged;
        }

        //Jagged array indexed by (row,col) of int lists.
        //Each int list contains indices of child UI components that reside in the cell specified by (row,col).
        [Browsable(false)]
        public List<int>[,] Indices
        {
            get
            {
                if (_indices is null)
                    RegenerateIndices();
                return _indices;
            }
            set => _indices = value;
        }

        public bool InvertY { get; set; }

        public EventList<UISizingDefinition> Rows
        {
            get => _rows;
            set
            {
                if (Set(ref _rows, value,
                    () => _rows.CollectionChanged -= CollectionChanged,
                    () => _rows.CollectionChanged += CollectionChanged, false))
                    OnChildrenChanged();
            }
        }
        public EventList<UISizingDefinition> Columns
        {
            get => _columns;
            set
            {
                if (Set(ref _columns, value,
                    () => _columns.CollectionChanged -= CollectionChanged,
                    () => _columns.CollectionChanged += CollectionChanged, false))
                    OnChildrenChanged();
            }
        }

        private void CollectionChanged(object sender, TCollectionChangedEventArgs<UISizingDefinition> e)
            => OnChildrenChanged();

        private void OnChildrenChanged()
        {
            Indices = null;
            InvalidateLayout();
        }

        public List<IUIComponent> GetComponentsInRow(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= Rows.Count)
                return null;

            List<IUIComponent> list = new List<IUIComponent>();
            for (int i = 0; i < Columns.Count; ++i)
            {
                var quadrant = Indices[rowIndex, i];
                foreach (var index in quadrant)
                    if (ChildComponents.IndexInRange(index))
                        list.Add(ChildComponents[index] as IUIComponent);
            }

            return list;
        }
        public List<IUIComponent> GetComponentsInColumn(int colIndex)
        {
            if (colIndex < 0 || colIndex >= Columns.Count)
                return null;

            List<IUIComponent> list = new List<IUIComponent>();
            for (int i = 0; i < Rows.Count; ++i)
            {
                var quadrant = Indices[i, colIndex];
                foreach (var index in quadrant)
                    if (ChildComponents.IndexInRange(index))
                        list.Add(ChildComponents[index] as IUIComponent);
            }

            return list;
        }
        private float GetRowAutoHeight(IEnumerable<IUIComponent> comps)
        {
            float height = 0.0f;
            foreach (var comp in comps)
                if (comp is IUIBoundableComponent bc)
                    height = Math.Max(bc.CalcAutoHeight(), height);
            return height;
        }
        private float GetColAutoWidth(IEnumerable<IUIComponent> comps)
        {
            float width = 0.0f;
            foreach (var comp in comps)
                if (comp is IUIBoundableComponent bc)
                    width = Math.Max(bc.CalcAutoWidth(), width);
            return width;

        }

        private void RegenerateIndices()
        {
            _indices = new List<int>[Rows.Count, Columns.Count];
            for (int r = 0; r < Rows.Count; ++r)
                for (int c = 0; c < Columns.Count; ++c)
                    _indices[r, c] = new List<int>();

            for (int i = 0; i < ChildComponents.Count; ++i)
                if (ChildComponents[i] is IUIComponent uic && uic.ParentInfo is GridPlacementInfo info)
                    _indices[info.Row, info.Column].Add(i);
        }

        protected override void OnResizeChildComponents(BoundingRectangleF parentRegion)
        {
            //Set to fixed values or initialize to zero for auto calculation
            float rowPropDenom = 0.0f;
            float colPropDenom = 0.0f;

            List<IUIComponent> autoComps = new List<IUIComponent>();

            //Pre-pass: 
            //calculate initial values,
            //grab any components affected by auto sizing,
            //add proportional values for use later
            foreach (var row in Rows)
            {
                switch (row.Value.Mode)
                {
                    case ESizingMode.Auto:
                        row.CalculatedValue = 0.0f;
                        foreach (var comp in row.AttachedControls)
                            if (!autoComps.Contains(comp.UIComponent))
                                autoComps.Add(comp.UIComponent);
                        break;
                    case ESizingMode.Fixed:
                        row.CalculatedValue = row.Value.Value;
                        break;
                    case ESizingMode.Proportional:
                        row.CalculatedValue = 0.0f;
                        rowPropDenom += row.Value.Value;
                        break;
                }
            }
            foreach (var col in Columns)
            {
                switch (col.Value.Mode)
                {
                    case ESizingMode.Auto:
                        col.CalculatedValue = 0.0f;
                        foreach (var comp in col.AttachedControls)
                            if (!autoComps.Contains(comp.UIComponent))
                                autoComps.Add(comp.UIComponent);
                        break;
                    case ESizingMode.Fixed:
                        col.CalculatedValue = col.Value.Value;
                        break;
                    case ESizingMode.Proportional:
                        col.CalculatedValue = 0.0f;
                        colPropDenom += col.Value.Value;
                        break;
                }
            }
            //Auto sizing pass, only calculate auto size for components that are affected
            foreach (IUIComponent comp in autoComps)
            {
                if (!(comp?.ParentInfo is GridPlacementInfo info))
                    continue;

                bool hasCalcAutoHeight = false;
                bool hasCalcAutoWidth = false;
                float autoHeight = 0.0f;
                float autoWidth = 0.0f;

                //Calc height through one or more rows
                foreach (int rowIndex in info.AssociatedRowIndices)
                {
                    var row = Rows[rowIndex];
                    switch (row.Value.Mode)
                    {
                        case ESizingMode.Auto:
                            if (!hasCalcAutoHeight)
                            {
                                hasCalcAutoHeight = true;
                                autoHeight = info.UIComponent?.CalcAutoHeight() ?? 0.0f;
                            }
                            row.CalculatedValue = Math.Max(row.CalculatedValue, autoHeight);
                            break;
                    }
                }

                //Calc width through one or more cols
                foreach (int colIndex in info.AssociatedColumnIndices)
                {
                    var col = Columns[colIndex];
                    switch (col.Value.Mode)
                    {
                        case ESizingMode.Auto:
                            if (!hasCalcAutoWidth)
                            {
                                hasCalcAutoWidth = true;
                                autoWidth = info.UIComponent?.CalcAutoWidth() ?? 0.0f;
                            }
                            col.CalculatedValue = Math.Max(col.CalculatedValue, autoWidth);
                            break;
                    }
                }
            }

            float remainingRowHeight = parentRegion.Height;
            float remainingColWidth = parentRegion.Width;

            foreach (var row in Rows)
            {
                if (row.Value.Mode != ESizingMode.Proportional)
                    remainingRowHeight -= row.CalculatedValue;
            }

            foreach (var col in Columns)
            {
                if (col.Value.Mode != ESizingMode.Proportional)
                    remainingColWidth -= col.CalculatedValue;
            }

            //Clamp remaining to zero
            if (remainingRowHeight < 0.0f)
                remainingRowHeight = 0.0f;
            if (remainingColWidth < 0.0f)
                remainingColWidth = 0.0f;

            //Post-pass: actually size each row and col, and resize each component
            float heightOffset = 0.0f;
            for (int r = 0; r < Rows.Count; ++r)
            {
                var row = Rows[r];

                //Calculate the proportional value now that the fixed and auto values have been processed
                if (row.Value.Mode == ESizingMode.Proportional)
                    row.CalculatedValue = rowPropDenom <= 0.0f ? 0.0f : row.Value.Value / rowPropDenom * remainingRowHeight;

                float height = row.CalculatedValue;

                float widthOffset = 0.0f;
                for (int c = 0; c < Columns.Count; ++c)
                {
                    var col = Columns[c];

                    //Calculate the proportional value now that the fixed and auto values have been processed
                    if (col.Value.Mode == ESizingMode.Proportional)
                        col.CalculatedValue = colPropDenom <= 0.0f ? 0.0f : col.Value.Value / colPropDenom * remainingColWidth;

                    float width = col.CalculatedValue;

                    List<int> indices = Indices[r, c];
                    if (indices is null)
                        Indices[r, c] = indices = new List<int>();
                    foreach (var index in indices)
                    {
                        ISceneComponent comp = ChildComponents[index];
                        if (!(comp is IUIComponent uiComp))
                            continue;

                        float x = parentRegion.X;
                        float y = parentRegion.Y;
                        y += parentRegion.Height - heightOffset - height;
                        x += widthOffset;

                        AdjustByMargin(
                            uiComp as IUIBoundableComponent,
                            ref widthOffset, ref heightOffset,
                            ref x, ref y,
                            ref width, ref height);

                        uiComp.ResizeLayout(new BoundingRectangleF(x, y, width, height));
                    }

                    widthOffset += width;
                }

                heightOffset += height;
            }
        }

        private void AdjustByMargin(IUIBoundableComponent uibComp, ref float widthOffset, ref float heightOffset, ref float x, ref float y, ref float width, ref float height)
        {
            var margins = uibComp?.Margins;
            if (margins is null)
                return;

            float left = margins.X;
            float bottom = margins.Y;
            float right = margins.Z;
            float top = margins.W;

            float temp = bottom + top;
            width -= left + right;
            height -= temp;
            heightOffset += temp;

            y += bottom;
            x += left;

            temp = left + right;
            height -= bottom + top;
            width -= temp;
            widthOffset += temp;

            x += left;
            y += bottom;
        }

        protected override void OnChildAdded(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                //Add parent info
                if (!(uic.ParentInfo is GridPlacementInfo info))
                    uic.ParentInfo = info = new GridPlacementInfo() { ParentUIComponent = this };

                //Register events
                info.PropertyChanging += Info_PropertyChanging;
                info.PropertyChanged += Info_PropertyChanged;

                //Add row/col associations
                AddControlToRows(info);
                AddControlToColumns(info);
            }

            base.OnChildAdded(item);

            //Regenerate row/col indices and invalidate layout for next render
            OnChildrenChanged();
        }

        protected override void OnChildRemoved(ISceneComponent item)
        {
            if (item is IUIComponent uic && uic.ParentInfo is GridPlacementInfo info)
            {
                //Unregister events
                info.PropertyChanging -= Info_PropertyChanging;
                info.PropertyChanged -= Info_PropertyChanged;

                //Remove row/col associations
                RemoveControlFromRows(info);
                RemoveControlFromColumns(info);

                //Remove parent info
                if (info.ParentUIComponent == this)
                {
                    info.ParentUIComponent = null;
                    uic.ParentInfo = null;
                }
            }

            base.OnChildRemoved(item);

            //Regenerate row/col indices and invalidate layout for next render
            OnChildrenChanged();
        }

        private void Info_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            if (!(sender is GridPlacementInfo info))
                return;

            switch (e.PropertyName)
            {
                case nameof(GridPlacementInfo.RowSpan):
                case nameof(GridPlacementInfo.Row):
                    RemoveControlFromRows(info);
                    break;

                case nameof(GridPlacementInfo.ColumnSpan):
                case nameof(GridPlacementInfo.Column):
                    RemoveControlFromColumns(info);
                    break;
            }

            //TODO: don't fully regenerate every time,
            //Just update the indices of this element
            OnChildrenChanged();
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is GridPlacementInfo info))
                return;

            switch (e.PropertyName)
            {
                case nameof(GridPlacementInfo.RowSpan):
                case nameof(GridPlacementInfo.Row):
                    AddControlToRows(info);
                    break;

                case nameof(GridPlacementInfo.ColumnSpan):
                case nameof(GridPlacementInfo.Column):
                    AddControlToColumns(info);
                    break;
            }

            //TODO: don't fully regenerate every time,
            //Just update the indices of this element
            OnChildrenChanged();
        }

        private void AddControlToRows(GridPlacementInfo info)
        {
            int startIndex = info.Row;
            for (int rowIndex = startIndex; rowIndex < startIndex + info.RowSpan; ++rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    continue;

                var list = Rows[rowIndex]?.AttachedControls;
                if (list != null && !list.Contains(info))
                {
                    list.Add(info);
                    info.AssociatedRowIndices.Add(rowIndex);
                }
            }
        }
        private void AddControlToColumns(GridPlacementInfo info)
        {
            int startIndex = info.Column;
            for (int colIndex = startIndex; colIndex < startIndex + info.ColumnSpan; ++colIndex)
            {
                if (colIndex < 0 || colIndex >= Columns.Count)
                    continue;

                var list = Columns[colIndex]?.AttachedControls;
                if (list != null && !list.Contains(info))
                {
                    list.Add(info);
                    info.AssociatedColumnIndices.Add(colIndex);
                }
            }
        }
        private void RemoveControlFromRows(GridPlacementInfo info)
        {
            int startIndex = info.Row;
            for (int rowIndex = startIndex; rowIndex < startIndex + info.RowSpan; ++rowIndex)
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    continue;

                var list = Rows[rowIndex]?.AttachedControls;
                if (list != null && list.Contains(info))
                {
                    list.Remove(info);
                    info.AssociatedRowIndices.Remove(rowIndex);
                }
            }
        }
        private void RemoveControlFromColumns(GridPlacementInfo info)
        {
            int startIndex = info.Column;
            for (int colIndex = startIndex; colIndex < startIndex + info.ColumnSpan; ++colIndex)
            {
                if (colIndex < 0 || colIndex >= Columns.Count)
                    continue;

                var list = Columns[colIndex]?.AttachedControls;
                if (list != null && list.Contains(info))
                {
                    list.Remove(info);
                    info.AssociatedColumnIndices.Remove(colIndex);
                }
            }
        }
    }
}