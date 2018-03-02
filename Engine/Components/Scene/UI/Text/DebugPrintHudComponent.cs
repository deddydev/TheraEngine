using System.Collections.Concurrent;

namespace TheraEngine.Rendering.UI
{
    public class DebugPrintHudComponent : UITextComponent
    {
        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        public void PrintMessage(string message)
        {
            _messages.Enqueue(message);
        }
    }
}
