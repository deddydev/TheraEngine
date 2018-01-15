using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    public static class IronPython
    {
        public static ScriptRuntime Runtime;

        private static EventRaisingStreamWriter writer;
        public static void Initialize()
        {
            MemoryStream stream = new MemoryStream();

            writer = new EventRaisingStreamWriter(stream);
            writer.StringWritten += new EventHandler<EventArgs<string>>(OutputUpdate);

            Runtime = Python.CreateRuntime();
            Runtime.IO.SetOutput(stream, writer);
            Runtime.IO.SetErrorOutput(stream, writer);

            //Run test script
            //dynamic test = Runtime.UseFile(Path.Combine(Engine.Settings.ScriptsFolder, "Test.py"));
            //test.Test();
        }
        private static void OutputUpdate(object sender, EventArgs<string> e)
        {
            e.Value.Print();
        }
    }
}
