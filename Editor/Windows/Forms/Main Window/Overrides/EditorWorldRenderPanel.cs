namespace TheraEngine.Windows.Forms
{
    public class EditorWorldRenderPanel : WorldRenderPanel
    {
        protected override void GlobalPreRender()
        {
            //Prerender will be done by the editor before rendering any other viewport
            OnPreRender();
            //base.GlobalPreRender();
        }
    }
}
