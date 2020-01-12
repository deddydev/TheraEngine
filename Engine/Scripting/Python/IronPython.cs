using Extensions;
using IronPython.Hosting;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Core.Files;

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
                if (_engine is null)
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
            => e.Value.PrintLine();
    }
    public class IronPythonCompileLogger : ErrorListener
    {
        public class ReportInfo
        {
            public ReportInfo(ScriptSource source, string message, SourceSpan span, int errorCode)
            {
                Source = source;
                Message = message;
                Span = span;
                ErrorCode = errorCode;
            }

            public ScriptSource Source { get; set; }
            public string Message { get; set; }
            public SourceSpan Span { get; set; }
            public int ErrorCode { get; set; }
        }
        public List<ReportInfo> Errors { get; set; } = new List<ReportInfo>();
        public List<ReportInfo> Warnings { get; set; } = new List<ReportInfo>();
        public List<ReportInfo> FatalErrors { get; set; } = new List<ReportInfo>();
        public List<ReportInfo> Ignored { get; set; } = new List<ReportInfo>();
        public override void ErrorReported(ScriptSource source, string message, SourceSpan span, int errorCode, Severity severity)
        {
            ReportInfo info = new ReportInfo(source, message, span, errorCode);
            switch (severity)
            {
                case Severity.Error:
                    Errors.Add(info);
                    break;
                case Severity.Warning:
                    Warnings.Add(info);
                    break;
                case Severity.FatalError:
                    FatalErrors.Add(info);
                    break;
                case Severity.Ignore:
                    Ignored.Add(info);
                    break;
            }
            Engine.Out($"{severity} {errorCode}: {message} (line {span.Start.Line} col {span.Start.Column})");
        }
    }
}
