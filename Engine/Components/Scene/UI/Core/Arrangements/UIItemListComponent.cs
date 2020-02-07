using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.ComponentModel;
using TheraEngine.Components;
using TheraEngine.Core.Shapes;

namespace TheraEngine.Rendering.UI
{
    public class ListPlacementInfo : UIParentAttachmentInfo
    {
        private SizingDefinition _size = new SizingDefinition();
        public SizingDefinition Size
        {
            get => _size;
            set => _size = value ?? new SizingDefinition();
        }
    }
    public enum EOrientation
    {
        Vertical,
        Horizontal,
    }
    public class UIListComponent : UIBoundableComponent
    {
        private bool _reverseItemOrder = false;
        private bool _arrangeBackward = false;
        private EOrientation _orientation = EOrientation.Vertical;
        private IComparer<ISceneComponent> _sorter;

        public UIListComponent() { }

        [TSerialize]
        [Category("List")]
        public EOrientation Orientation
        {
            get => _orientation;
            set
            {
                if (Set(ref _orientation, value))
                    InvalidateLayout();
            }
        }
        [TSerialize]
        [Category("List")]
        public bool ReverseItemOrder
        {
            get => _reverseItemOrder;
            set
            {
                if (Set(ref _reverseItemOrder, value))
                    InvalidateLayout();
            }
        }
        [TSerialize]
        [Category("List")]
        public bool ArrangeBackward
        {
            get => _arrangeBackward;
            set
            {
                if (Set(ref _arrangeBackward, value))
                    InvalidateLayout();
            }
        }
        [TSerialize]
        [Category("List")]
        public IComparer<ISceneComponent> Sorter
        {
            get => _sorter;
            set
            {
                if (Set(ref _sorter, value))
                    InvalidateLayout();
            }
        }

        protected override void OnResizeChildComponents(BoundingRectangleF parentRegion)
        {
            if (Sorter != null && ChildrenChanged)
                ChildComponents.Sort(Sorter);

            //Sizing priority: auto, fixed, proportional
            bool vertical = Orientation == EOrientation.Vertical;
            float remaining = vertical ? parentRegion.Height : parentRegion.Width;
            float totalProportional = 0.0f;

            ResizeChildrenPrepass(vertical, ref remaining, ref totalProportional);
            ResizeChildrenPostpass(parentRegion, vertical, remaining, totalProportional);

            ChildrenChanged = false;
        }

        /// <summary>
        /// Actually resizes child items using prepass information.
        /// </summary>
        /// <param name="vertical">If the list box displays items left to right (false) or top to bottom (true)</param>
        /// <param name="remaining">How much room is left after calculating the size of fixed and auto sized items.</param>
        /// <param name="propDenom">The denominator to use to calculate proportional item sizes.</param>
        private void ResizeChildrenPrepass(bool vertical, ref float remaining, ref float propDenom)
        {
            for (int i = 0; i < ChildComponents.Count; ++i)
            {
                int index = ReverseItemOrder ? ChildComponents.Count - 1 - i : i;

                ISceneComponent comp = ChildComponents[index];
                if (!(comp is IUIComponent uiComp))
                    continue;

                float calc = 0.0f;
                SizingDefinition def = (uiComp.ParentInfo as ListPlacementInfo)?.Size;
                if (def?.Value != null)
                {
                    switch (def.Value.Mode)
                    {
                        default:
                        case ESizingMode.Fixed:
                            {
                                calc = def.Value.Value;
                                break;
                            }
                        case ESizingMode.Auto:
                            {
                                calc = vertical ? uiComp.CalcAutoHeight() : uiComp.CalcAutoWidth();
                                break;
                            }
                        case ESizingMode.Proportional:
                            {
                                calc = 0.0f;
                                propDenom += def.Value.Value;
                                break;
                            }
                    };
                    def.CalculatedValue = calc;
                }
                remaining -= calc;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentRegion"></param>
        /// <param name="vertical"></param>
        /// <param name="remaining"></param>
        /// <param name="totalProportional"></param>
        private void ResizeChildrenPostpass(BoundingRectangleF parentRegion, bool vertical, float remaining, float totalProportional)
        {
            float maxExtent = vertical ? parentRegion.Height : parentRegion.Width;

            //If negative, the total size of all items added together has overflowed past the maximum extent.
            if (remaining < 0.0f)
                remaining = 0.0f;

            float offset = 0.0f;
            for (int i = 0; i < ChildComponents.Count; ++i)
            {
                int index = ReverseItemOrder ? ChildComponents.Count - 1 - i : i;

                if (!(ChildComponents[index] is IUIComponent uiComp))
                    continue;

                SizingDefinition def = (uiComp.ParentInfo as ListPlacementInfo)?.Size;
                if (def?.Value is null)
                    continue;

                if (def.Value.Mode == ESizingMode.Proportional)
                    def.CalculatedValue = totalProportional <= 0.0f ? 0.0f : def.Value.Value / totalProportional * remaining;

                GetItemRegion(parentRegion, vertical, maxExtent, ref offset, def,
                    out float width, out float height, out float x, out float y);

                AdjustByMargin(vertical, uiComp as IUIBoundableComponent,
                    ref offset, ref x, ref y, ref width, ref height);

                uiComp.ResizeLayout(new BoundingRectangleF(x, y, width, height));
            }
        }

        /// <summary>
        /// Determines the allowed region for the element to occupy.
        /// </summary>
        /// <param name="parentRegion"></param>
        /// <param name="vertical"></param>
        /// <param name="maxExtent"></param>
        /// <param name="offset"></param>
        /// <param name="def"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void GetItemRegion(
            BoundingRectangleF parentRegion,
            bool vertical,
            float maxExtent,
            ref float offset,
            SizingDefinition def,
            out float width,
            out float height,
            out float x,
            out float y)
        {
            x = parentRegion.X;
            y = parentRegion.Y;

            if (vertical)
            {
                width = parentRegion.Width;
                height = def.CalculatedValue;
                y += ArrangeBackward ? offset : maxExtent - offset - height;
            }
            else
            {
                width = def.CalculatedValue;
                height = parentRegion.Height;
                x += ArrangeBackward ? maxExtent - offset - width : offset;
            }

            offset += def.CalculatedValue;
        }

        private void AdjustByMargin(
            bool vertical,
            IUIBoundableComponent uibComp,
            ref float offset,
            ref float x,
            ref float y,
            ref float width,
            ref float height)
        {
            var margins = uibComp?.Margins;
            if (margins is null)
                return;

            float left = margins.X;
            float bottom = margins.Y;
            float right = margins.Z;
            float top = margins.W;

            if (vertical)
            {
                float temp = bottom + top;
                width -= left + right;
                height -= temp;
                offset += temp;

                y += bottom;
                x += left;
            }
            else
            {
                float temp = left + right;
                height -= bottom + top;
                width -= temp;
                offset += temp;

                x += left;
                y += bottom;
            }
        }

        private bool ChildrenChanged { get; set; } = false;

        protected override void OnChildAdded(ISceneComponent item)
        {
            ChildrenChanged = true;

            if (item is IUIComponent uic)
            {
                if (!(uic.ParentInfo is ListPlacementInfo info))
                    uic.ParentInfo = info = new ListPlacementInfo();

                info.PropertyChanged += Info_PropertyChanged;
            }

            base.OnChildAdded(item);
        }
        protected override void OnChildRemoved(ISceneComponent item)
        {
            ChildrenChanged = true;

            if (item is IUIComponent uic)
            {
                if (uic.ParentInfo is ListPlacementInfo info)
                {
                    info.PropertyChanged -= Info_PropertyChanged;
                }

                uic.ParentInfo = null;
            }

            base.OnChildRemoved(item);
        }

        private void Info_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            InvalidateLayout();
        }
    }
}