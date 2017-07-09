using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
