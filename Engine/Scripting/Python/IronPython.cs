using System;
using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System.IO;
using TheraEngine.Core.Files;
using Microsoft.Scripting;

namespace TheraEngine.Scripting
{
    public static class IronPython
    {
        private static EventRaisingStreamWriter _writer;
        private static ScriptEngine _engine;

        public static ScriptScope GlobalScope { get; private set; }
        public static ScriptEngine Engine
        {
            get
            {
                if (_engine == null)
                    Initialize();
                return _engine;
            }
        }

        public static void Initialize()
        {
            MemoryStream stream = new MemoryStream();

            _writer = new EventRaisingStreamWriter(stream);
            _writer.StringWritten += new EventHandler<EventArgs<string>>(OutputUpdate);
            
            _engine = Python.CreateEngine();
            if (TheraEngine.Engine.Game?.DirectoryPath != null)
                _engine.SetSearchPaths(new string[] { TheraEngine.Engine.Game.DirectoryPath });
            GlobalScope = Engine.CreateScope();
            Engine.Runtime.IO.SetOutput(stream, _writer);
            Engine.Runtime.IO.SetErrorOutput(stream, _writer);
        }

        internal static void Execute(string text, string methodName, object[] args)
        {
            
        }

        public static dynamic Execute(string sourceText)
            => Engine.Execute(sourceText, GlobalScope);
        public static T Execute<T>(string sourceText)
            => Engine.Execute<T>(sourceText, GlobalScope);
        public static dynamic Execute(string sourceText, ScriptScope scope)
            => Engine.Execute(sourceText, scope);
        public static T Execute<T>(string sourceText, ScriptScope scope)
            => Engine.Execute<T>(sourceText, scope);

        private static void OutputUpdate(object sender, EventArgs<string> e)
            => e.Value.Print();
    }
    public class PythonCompileErrorListener : ErrorListener
    {
        public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            Engine.PrintLine(message);
        }
    }
}
