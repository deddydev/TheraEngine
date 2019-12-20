using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Components;

namespace TheraEngine.Rendering.UI
{
    public class UIGridComponent : UIBoundableComponent
    {
        private EventList<RowDefinition> _rows = new EventList<RowDefinition>();
        private EventList<ColumnDefinition> _columns = new EventList<ColumnDefinition>();

        public UIGridComponent()
        {

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

        public override void ArrangeChildren(Vec2 translation, Vec2 parentBounds)
        {
            var autoRows = Rows.Select(x => x.AnyAuto ? x : null).ToArray();
            var autoCols = Columns.Select(x => x.AnyAuto ? x : null).ToArray();
            

            float y = 0.0f;
            for (int r = 0; r < Rows.Count; ++r)
            {
                var row = Rows[r];
                float height = row.Value.Value;

                float x = 0.0f;
                for (int c = 0; c < Columns.Count; ++c)
                {
                    float width = Columns[c].Width.Value;

                    List<int> indices = Indices[r, c];
                    foreach (var index in indices)
                    {
                        ISceneComponent comp = ChildComponents[index];
                        if (comp is IUIComponent uic)
                        {
                            if (comp is IUIBoundableComponent bc)
                            {
                                bc.ArrangeChildren(new Vec2(x, y), new Vec2(width, height));
                            }
                            else
                            {

                            }
                        }
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
                get => Get(ref _row);
                set => Set(ref _row, value);
            }
            [Category("Grid")]
            public int Column
            {
                get => Get(ref _column);
                set => Set(ref _column, value);
            }
            [Category("Grid")]
            public int RowSpan
            {
                get => Get(ref _rowSpan);
                set => Set(ref _rowSpan, value);
            }
            [Category("Grid")]
            public int ColumnSpan
            {
                get => Get(ref _columnSpan);
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
                get => Get(ref _mode);
                set => Set(ref _mode, value);
            }
            public float Value
            {
                get => Get(ref _value);
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
                get => Get(ref _value);
                set => Set(ref _value, value);
            }
            public SizingValue Min
            {
                get => Get(ref _min);
                set => Set(ref _min, value);
            }
            public SizingValue Max
            {
                get => Get(ref _max);
                set => Set(ref _max, value);
            }
        }
    }
}