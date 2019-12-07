using System;
using TheraEngine.Components;

namespace TheraEngine.Rendering.UI
{
    public class UIGridComponent : UIDockableComponent
    {
        public UIGridComponent()
        {

        }

        public override Vec2 OnResize(Vec2 parentBounds)
        {
            Vec2 size = base.OnResize(parentBounds);

            return size;
        }

        protected override void OnChildAdded(ISceneComponent item) => base.OnChildAdded(item);
        protected override void OnChildRemoved(ISceneComponent item) => base.OnChildRemoved(item);

        public class RowDefinition
        {
            public SizeableElement Height { get; set; }
        }
        public class ColumnDefinition
        {
            public SizeableElement Width { get; set; }
        }
    }
}