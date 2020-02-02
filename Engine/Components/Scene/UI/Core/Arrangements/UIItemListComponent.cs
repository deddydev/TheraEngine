using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TheraEngine.Components;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public class ListPlacementInfo : UIParentAttachmentInfo
    {
        public SizingDefinition Size { get; set; }
    }
    public enum EOrientation
    {
        Vertical,
        Horizontal,
    }
    public class UIListComponent : UIBoundableComponent
    {
        private bool _reverseOrder = false;
        private EOrientation _orientation = EOrientation.Vertical;

        public UIListComponent()
        {

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

        protected override void OnResizeLayout(BoundingRectangleF parentRegion)
        {
            //Sizing priority: auto, fixed, proportional
            bool vertical = Orientation == EOrientation.Vertical;
            float remaining = vertical ? parentRegion.Height : parentRegion.Width;
            for (int i = 0; i < ChildComponents.Count; ++i)
            {
                ISceneComponent comp = ChildComponents[i];
                if (!(comp is IUIComponent uiComp))
                    continue;

                SizingDefinition def = (uiComp.ParentInfo as ListPlacementInfo)?.Size;
                switch (def.Value.Mode)
                {
                    case ESizingMode.Auto:
                        float auto = vertical ? uiComp.CalcAutoHeight() : uiComp.CalcAutoWidth();
                        break;
                    case ESizingMode.Fixed:

                        break;
                    case ESizingMode.Proportional:

                        break;
                }
            }
            for (int i = 0; i < ChildComponents.Count; ++i)
            {
                ISceneComponent component = ChildComponents[i];
                if (!(comp is IUIComponent uiComp))
                    continue;

            }

            var autoItems = ItemSizes.Where(x => x.Value.Mode == ESizingMode.Auto).ToArray();
            var fixedItems = ItemSizes.Where(x => x.Value.Mode == ESizingMode.Fixed).ToArray();
            var propItems = ItemSizes.Where(x => x.Value.Mode == ESizingMode.Proportional).ToArray();

            for (int i = 0; i < ItemSizes.Count; i++)
            {
                var size = ItemSizes[i];
            }

            var childComps = ChildComponents.Where(x => x is IUIComponent).Select(x => (IUIComponent)x);
            foreach (var row in autoItems)
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
                if (!(uic.ParentInfo is ListPlacementInfo info))
                    uic.ParentInfo = info = new ListPlacementInfo();

                //info.PropertyChanging += Info_PropertyChanging;
                info.PropertyChanged += Info_PropertyChanged;
            }

            base.OnChildAdded(item);
        }
        protected override void OnChildRemoved(ISceneComponent item)
        {
            if (item is IUIComponent uic)
            {
                if (uic.ParentInfo is ListPlacementInfo info)
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
    }
}