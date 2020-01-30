using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Components;
using Extensions;
using TheraEngine.Core.Shapes;
using TheraEngine.Core;

namespace TheraEngine.Rendering.UI
{
    public enum EOrientation
    {
        Vertical,
        Horizontal,
    }
    public class UIListComponent : UIBoundableComponent
    {
        private EventList<SizingDefinition> _itemSizes;
        private bool _reverseOrder = false;
        private EOrientation _orientation = EOrientation.Vertical;

        public UIListComponent()
        {
            _itemSizes = new EventList<SizingDefinition>();
        }

        [TSerialize]
        [Category("List")]
        public EOrientation Orientation
        {
            get => _orientation;
            set => Set(ref _orientation, value);
        }
        [TSerialize]
        [Category("List")]
        public bool ReverseOrder 
        {
            get => _reverseOrder;
            set => Set(ref _reverseOrder, value);
        }

        public EventList<SizingDefinition> ItemSizes
        {
            get => _itemSizes;
            set => Set(ref _itemSizes, value);
        }

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            //Sizing priority: auto, fixed, proportional
            float xSize = parentRegion.Width;
            var autoRows = ItemSizes.Select(x => x.NeedsAutoSizing ? x : null).ToArray();
            for (int i = 0; i < ItemSizes.Count; i++)
            {
                var size = ItemSizes[i];
            }

            var childComps = ChildComponents.Where(x => x is IUIComponent).Select(x => (IUIComponent)x);
            foreach (var row in autoRows)
            {

                allItems.Remove(row);
            }

            float parentSize = ActualWidth;
            float totalItemSize = 0.0f;
            float size;

            for (int i = 0; i < ChildComponents.Count; ++i)
            {
                if (!(ChildComponents[i] is IUIComponent child))
                    continue;

                var childSize = ItemSizes[i];
                if (childSize.NeedsAutoSizing)
                {
                    size = child.CalcAutoHeight();
                }
                float size = childSize.Value.Value;

                ISceneComponent comp = ChildComponents[i];
                if (comp is IUIComponent uic)
                    uic.ResizeLayout(new BoundingRectangleF(x, totalItemSize, width, size));

                totalItemSize += size;
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

            [Browsable(false)]
            public bool NeedsAutoSizing =>
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