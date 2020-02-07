using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using TheraEngine.Core.Files;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Threading;
using TheraEngine.ComponentModel;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("cs", "csx")]
    [TFileDef("C# Script")]
    public class ScriptCSharp : ScriptFile
    {
        public ScriptCSharp() : base() { }
        public ScriptCSharp(string path) : base(path) { }
        public static new ScriptCSharp FromText(string text)
            => new ScriptCSharp() { Text = text };

        public ScriptState<object> ExecutionState;
        public CancellationTokenSource ExecutionCancel;

        public async void Execute()
        {
            ScriptOptions ops = ScriptOptions.Default.WithAllowUnsafe(true);
            ExecutionState = await CSharpScript.RunAsync(Text, ops, null, null, ExecutionCancel.Token);

            var script = CSharpScript.Create(Text, ops);
            var comp = script.GetCompilation();
            
        }

        public void Analyze()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(Text);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            SyntaxWalker walker = new SyntaxWalker();

        }

        private class SyntaxWalker : CSharpSyntaxWalker
        {
            public override void Visit(SyntaxNode node)
            {
                SyntaxKind kind = node.Kind();
                Location location = node.GetLocation();
                var span = location.GetLineSpan();
                LinePositionSpan lineSpan = span.Span;
                Engine.Out($"{kind}: {lineSpan}");
            }
        }
    }
}
