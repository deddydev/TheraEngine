using System.Collections.Concurrent;

namespace TheraEngine.Rendering.HUD
{
    public class DebugPrintHudComponent : TextHudComponent
    {
        private ConcurrentQueue<string> _messages = new ConcurrentQueue<string>();
        public void PrintMessage(string message)
        {
            _messages.Enqueue(message);
        }
    }
}
