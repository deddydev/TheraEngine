namespace TheraEngine.Windows.Forms
{
    public class EditorWorldRenderPanel : WorldRenderPanel
    {
        protected override void PreRender()
        {
            //Prerender will be done by the editor before rendering any other viewport
            OnPreRender();
        }
    }
}
