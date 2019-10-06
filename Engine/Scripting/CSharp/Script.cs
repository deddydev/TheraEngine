using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("cs")]
    [TFileDef("C# Script")]
    public class CSharpScript : ScriptFile
    {
        public CSharpScript() : base() { }
        public CSharpScript(string path) : base(path) { }
        public static new CSharpScript FromText(string text)
            => new CSharpScript() { Text = text };

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
                Engine.PrintLine($"{kind}: {lineSpan}");
            }
        }
    }
}
