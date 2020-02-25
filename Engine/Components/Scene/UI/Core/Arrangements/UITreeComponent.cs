namespace TheraEngine.Rendering.UI
{
    public class UITreeComponent : UIGridComponent
    {
        public UITreeComponent()
        {
            
        }
    }
    public class UITreeItemComponent : UIBoundableComponent
    {
        public UITreeComponent OwningTreeComponent { get; set; }
        public int TreeDepth { get; set; }

        public UITreeItemComponent()
        {

        }
    }
}