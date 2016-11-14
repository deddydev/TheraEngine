namespace CustomEngine.Input
{
    public class AIController : PawnController
    {
        public AIController() : base()
        {
            Engine.ActiveAI.Add(this);
        }
        ~AIController()
        {
            if (Engine.ActiveAI.Contains(this))
                Engine.ActiveAI.Remove(this);
        }
    }
}
