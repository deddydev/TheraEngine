using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Components;
using Extensions;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public class UIGridComponent : UIBoundableComponent
    {
        private EventList<SizingDefinition> _rows;
        private EventList<SizingDefinition> _columns;

        public UIGridComponent()
        {
            Rows = new EventList<SizingDefinition>();
            Columns = new EventList<SizingDefinition>();
        }

        //Jagged array indexed by (row,col) of int lists.
        //Each int list contains indices of child UI components that reside in the cell specified by (row,col).
        public List<int>[,] Indices { get; set; }
        public bool InvertY { get; set; }

        public EventList<SizingDefinition> Rows
        {
            get => _rows;
            set
            {
                if (Set(ref _rows, value,
                    () => _rows.CollectionChanged -= CollectionChanged,
                    () => _rows.CollectionChanged += CollectionChanged, false))
                    RegenerateIndices();
            }
        }
        public EventList<SizingDefinition> Columns
        {
            get => _columns;
            set
            {
                if (Set(ref _columns, value,
                    () => _columns.CollectionChanged -= CollectionChanged,
                    () => _columns.CollectionChanged += CollectionChanged, false))
                    RegenerateIndices();
            }
        }
        private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) => RegenerateIndices();
        private void RegenerateIndices()
        {
            Indices = new List<int>[Rows.Count, Columns.Count];
            Indices.Initialize();

            for (int i = 0; i < ChildComponents.Count; ++i)
                if (ChildComponents[i] is IUIComponent uic && uic.ParentInfo is GridPlacementInfo info)
                    Indices[info.Row, info.Column].Add(i);
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
        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            //Sizing priority: auto, fixed, proportional

            var autoRows = Rows.Select(x => x.AnyAuto ? x : null).ToArray();
            var autoCols = Columns.Select(x => x.AnyAuto ? x : null).ToArray();

            for (int i = 0; i < Rows.Count; i++)
            {
                var def = Rows[i];
                var comps = GetComponentsInRow(i);
                float rowHeight = 0.0f;
                foreach (var comp in comps)
                    if (comp is IUIBoundableComponent bc)
                        rowHeight = Math.Max(bc.ActualHeight, rowHeight);

            }
            for (int i = 0; i < Columns.Count; i++)
            {
                var def = Rows[i];
                var comps = GetComponentsInColumn(i);
                float colWidth = 0.0f;
                foreach (var comp in comps)
                    if (comp is IUIBoundableComponent bc)
                        colWidth = Math.Max(bc.ActualWidth, colWidth);

            }

            var allRows = Rows.ToList();
            var allCols = Columns.ToList();
            var childComps = ChildComponents.Where(x => x is IUIComponent).Select(x => (IUIComponent)x);
            foreach (var row in autoRows)
            {

                allRows.Remove(row);
            }
            foreach (var col in autoCols)
            {

                allCols.Remove(col);
            }

            float y = 0.0f;
            for (int r = 0; r < Rows.Count; ++r)
            {
                var row = Rows[r];
                float height = row.Value.Value;

                float x = 0.0f;
                for (int c = 0; c < Columns.Count; ++c)
                {
                    var col = Columns[c];
                    float width = col.Value.Value;

                    List<int> indices = Indices[r, c];
                    foreach (var index in indices)
                    {
                        ISceneComponent comp = ChildComponents[index];
                        if (comp is IUIComponent uic)
                            uic.ResizeLayout(new BoundingRectangleF(x, y, width, height));
                    }

                    x += width;
                }

                y += height;
            }
        }

        protected override void OnChildAdded(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                if (!(uic.ParentInfo is GridPlacementInfo info))
                    uic.ParentInfo = info = new GridPlacementInfo();

                //info.PropertyChanging += Info_PropertyChanging;
                info.PropertyChanged += Info_PropertyChanged;
            }

            base.OnChildAdded(item);
        }
        protected override void OnChildRemoved(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                if (uic.ParentInfo is GridPlacementInfo info)
                {
                    //info.PropertyChanging -= Info_PropertyChanging;
                    info.PropertyChanged -= Info_PropertyChanged;
                }

                uic.ParentInfo = null;
            }

            base.OnChildRemoved(item);
        }

        private void Info_PropertyChanging(object sender, PropertyChangingEventArgs e)
        {
            //GridPlacementInfo info = sender as GridPlacementInfo;
            //switch (e.PropertyName)
            //{
            //    case nameof(GridPlacementInfo.Row):

            //        break;
            //    case nameof(GridPlacementInfo.Column):

            //        break;
            //}
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //TODO: don't fully regenerate every time
            RegenerateIndices();

            //GridPlacementInfo info = sender as GridPlacementInfo;
            //switch (e.PropertyName)
            //{
            //    case nameof(GridPlacementInfo.Row):

            //        break;
            //    case nameof(GridPlacementInfo.Column):

            //        break;
            //}
        }

        public class GridPlacementInfo : UIParentAttachmentInfo
        {
            private int _row = 0;
            private int _column = 0;
            private int _rowSpan = 1;
            private int _columnSpan = 1;

            [Category("Grid")]
            public int Row
            {
                get => _row;
                set => Set(ref _row, value);
            }
            [Category("Grid")]
            public int Column
            {
                get => _column;
                set => Set(ref _column, value);
            }
            [Category("Grid")]
            public int RowSpan
            {
                get => _rowSpan;
                set => Set(ref _rowSpan, value);
            }
            [Category("Grid")]
            public int ColumnSpan
            {
                get => _columnSpan;
                set => Set(ref _columnSpan, value);
            }
        }
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

            public bool AnyAuto =>
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
}