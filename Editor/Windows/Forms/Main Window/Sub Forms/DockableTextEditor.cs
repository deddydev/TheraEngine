using FastColoredTextBoxNS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public enum ETextEditorMode
    {
        Text,
        Python,
        Lua,
        CSharp,
        GLSL,
    }
    public partial class DockableTextEditor : DockContent
    {
        public Func<string, DockableTextEditor, (bool, string)> CompileGLSL;

        public DockableTextEditor()
        {
            InitializeComponent();
            cboMode.Items.AddRange(Enum.GetNames(typeof(ETextEditorMode)));
            cboMode.SelectedIndex = 0;
            TextBox.CustomAction += TextBox_CustomAction;
            TextBox.HotkeysMapping.Add(Keys.Control | Keys.S, FCTBAction.CustomAction1);
            TextBox.AutoCompleteBrackets = true;
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Renderer = new TheraForm.TheraToolstripRenderer();
            TextBox.AllowSeveralTextStyleDrawing = true;
            _errorBrush = new SolidBrush(Color.FromArgb(TextBox.BackColor.R + 40, TextBox.BackColor.G, TextBox.BackColor.B));
        }
        public static void ShowNew(DockPanel dockPanel, DockState document, string text, string formTitle, ETextEditorMode mode, Action<DockableTextEditor> defaultSaveText)
        {
            DockableTextEditor m = new DockableTextEditor();
            m.Show(dockPanel, document);
            m.InitText(text, formTitle, mode);
            m.Saved += defaultSaveText;
        }

        private void TextBox_CustomAction(object sender, CustomActionEventArgs e)
        {
            switch (e.Action)
            {
                case FCTBAction.CustomAction1:
                    btnSave_Click_1(null, null);
                    break;
            }
        }

        public void InitText(string text, string formTitle, ETextEditorMode mode = ETextEditorMode.Text)
        {
            Text = string.IsNullOrWhiteSpace(formTitle) ? "Text Editor" : formTitle;
            Mode = mode;
            TextBox.Text = text;
            TextBox.IsChanged = false;
        }

        TextStyle _errorStyle = new TextStyle(new SolidBrush(Color.Black), new SolidBrush(Color.Red), FontStyle.Underline);
        private ETextEditorMode _mode = ETextEditorMode.Text;
        public ETextEditorMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value)
                    return;
                _updating = true;
                switch (_mode)
                {
                    case ETextEditorMode.Python:
                        TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_Python;
                        break;
                    case ETextEditorMode.GLSL:
                        TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_GLSL;
                        break;
                }
                _mode = value;
                TextBox.ClearStyle(StyleIndex.All);
                cboMode.SelectedIndex = (int)_mode;
                //System.Windows.Forms.TextBox.HighlightDescriptors.Clear();
                switch (_mode)
                {
                    case ETextEditorMode.Text:
                        TextBox.Language = Language.Custom;
                        TextBox.DescriptionFile = null;
                        break;
                    case ETextEditorMode.Python:
                        TextBox.Language = Language.Custom;
                        TextBox.CommentPrefix = "#";
                        TextBox.AutoCompleteBrackets = true;
                        TextBox.AutoIndent = true;
                        TextBox.DescriptionFile = Engine.EngineScriptsPath("PythonHighlighting.xml");
                        TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_Python;
                        break;
                    case ETextEditorMode.CSharp:
                        TextBox.Language = Language.CSharp;
                        TextBox.DescriptionFile = null;
                        TextBox.AutoIndent = true;
                        TextBox.AutoIndentChars = true;
                        break;
                    case ETextEditorMode.Lua:
                        TextBox.Language = Language.Lua;
                        TextBox.DescriptionFile = null;
                        break;
                    case ETextEditorMode.GLSL:
                        TextBox.Language = Language.Custom;
                        TextBox.CommentPrefix = "//";
                        TextBox.DescriptionFile = Engine.EngineScriptsPath("GLSLHighlighting.xml");
                        TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_GLSL;
                        TextBox.AutoIndent = true;
                        TextBox.AutoIndentChars = true;
                        TextBox.AutoCompleteBrackets = true;
                        break;
                }
                TextBox.AddStyle(_errorStyle);
                _updating = false;
            }
        }
        private void TextBox_AutoIndentNeeded_GLSL(object sender, AutoIndentEventArgs e)
        {
            string line = e.LineText.Trim();
            if (line.EndsWith("{"))
            {
                e.ShiftNextLines = e.TabLength;
                return;
            }
            else if (line.EndsWith("}"))
            {
                e.ShiftNextLines = -e.TabLength;
                return;
            }
        }
        private void TextBox_AutoIndentNeeded_Python(object sender, AutoIndentEventArgs e)
        {
            string line = e.LineText.Trim();
            if (line.EndsWith(":"))
            {
                e.ShiftNextLines = e.TabLength;
                return;
            }
        }

        public string GetText() => TextBox.Text;
        public event Action<DockableTextEditor> Saved;

        private static char[] GLSLSyntax = { '(', ')', '[', ']', '{', '}', ',', ';' };
        private static string[] GLSLKeywords = { "attribute", "const", "uniform", "varying", "buffer", "shared", "coherent", "volatile", "restrict", "readonly", "writeonly", "atomic_uint", "layout", "centroid", "flat", "smooth", "noperspective", "patch", "sample", "break", "continue", "do", "for", "while", "switch", "case", "default", "if", "else", "subroutine", "in", "out", "inout", "float", "double", "int", "void", "bool", "true", "false", "invariant", "precise", "discard", "return", "mat2", "mat3", "mat4", "dmat2", "dmat3", "dmat4", "mat2x2", "mat2x3", "mat2x4", "dmat2x2", "dmat2x3", "dmat2x4", "mat3x2", "mat3x3", "mat3x4", "dmat3x2", "dmat3x3", "dmat3x4", "mat4x2", "mat4x3", "mat4x4", "dmat4x2", "dmat4x3", "dmat4x4", "vec2", "vec3", "vec4", "ivec2", "ivec3", "ivec4", "bvec2", "bvec3", "bvec4", "dvec2", "dvec3", "dvec4", "uint", "uvec2", "uvec3", "uvec4", "lowp", "mediump", "highp", "precision", "sampler1D", "sampler2D", "sampler3D", "samplerCube", "sampler1DShadow", "sampler2DShadow", "samplerCubeShadow", "sampler1DArray", "sampler2DArray", "sampler1DArrayShadow", "sampler2DArrayShadow", "isampler1D", "isampler2D", "isampler3D", "isamplerCube", "isampler1DArray", "isampler2DArray", "usampler1D", "usampler2D", "usampler3D", "usamplerCube", "usampler1DArray", "usampler2DArray", "sampler2DRect", "sampler2DRectShadow", "isampler2DRect", "usampler2DRect", "samplerBuffer", "isamplerBuffer", "usamplerBuffer", "sampler2DMS", "isampler2DMS", "usampler2DMS", "sampler2DMSArray", "isampler2DMSArray", "usampler2DMSArray", "samplerCubeArray", "samplerCubeArrayShadow", "isamplerCubeArray", "usamplerCubeArray", "image1D", "iimage1D", "uimage1D", "image2D", "iimage2D", "uimage2D", "image3D", "iimage3D", "uimage3D", "image2DRect", "iimage2DRect", "uimage2DRect", "imageCube", "iimageCube", "uimageCube", "imageBuffer", "iimageBuffer", "uimageBuffer", "image1DArray", "iimage1DArray", "uimage1DArray", "image2DArray", "iimage2DArray", "uimage2DArray", "imageCubeArray", "iimageCubeArray", "uimageCubeArray", "image2DMS", "iimage2DMS", "uimage2DMS", "image2DMSArray", "iimage2DMSArray", "uimage2DMSArray", "struct" };
        private static string[] GLSLBuiltInMethods = { "abs", "acos", "acosh", "all", "any", "asin", "asinh", "atan", "atanh", "atomicAdd", "atomicAnd", "atomicCompSwap", "atomicCounter", "atomicCounterDecrement", "atomicCounterIncrement", "atomicExchange", "atomicMax", "atomicMin", "atomicOr", "atomicXor", "barrier", "bitCount", "bitfieldExtract", "bitfieldInsert", "bitfieldReverse", "ceil", "clamp", "cos", "cosh", "cross", "degrees", "determinant", "dFdx", "dFdxCoarse", "dFdxFine", "dFdy", "dFdyCoarse", "dFdyFine", "distance", "dot", "EmitStreamVertex", "EmitVertex", "EndPrimitive", "EndStreamPrimitive", "equal", "exp", "exp2", "faceforward", "findLSB", "findMSB", "floatBitsToInt", "floatBitsToUint", "floor", "fma", "fract", "frexp", "fwidth", "fwidthCoarse", "fwidthFine", "gl_ClipDistance", "gl_CullDistance", "gl_FragCoord", "gl_FragDepth", "gl_FrontFacing", "gl_GlobalInvocationID", "gl_HelperInvocation", "gl_InstanceID", "gl_InvocationID", "gl_Layer", "gl_LocalInvocationID", "gl_LocalInvocationIndex", "gl_NumSamples", "gl_NumWorkGroups", "gl_PatchVerticesIn", "gl_PointCoord", "gl_PointSize", "gl_Position", "gl_PrimitiveID", "gl_PrimitiveIDIn", "gl_SampleID", "gl_SampleMask", "gl_SampleMaskIn", "gl_SamplePosition", "gl_TessCoord", "gl_TessLevelInner", "gl_TessLevelOuter", "gl_VertexID", "gl_ViewportIndex", "gl_WorkGroupID", "gl_WorkGroupSize", "greaterThan", "greaterThanEqual", "groupMemoryBarrier", "imageAtomicAdd", "imageAtomicAnd", "imageAtomicCompSwap", "imageAtomicExchange", "imageAtomicMax", "imageAtomicMin", "imageAtomicOr", "imageAtomicXor", "imageLoad", "imageSamples", "imageSize", "imageStore", "imulExtended", "intBitsToFloat", "interpolateAtCentroid", "interpolateAtOffset", "interpolateAtSample", "inverse", "inversesqrt", "isinf", "isnan", "ldexp", "length", "lessThan", "lessThanEqual", "log", "log2", "matrixCompMult", "max", "memoryBarrier", "memoryBarrierAtomicCounter", "memoryBarrierBuffer", "memoryBarrierImage", "memoryBarrierShared", "min", "mix", "mod", "modf", "noise", "noise1", "noise2", "noise3", "noise4", "normalize", "not", "notEqual", "outerProduct", "packDouble2x32", "packHalf2x16", "packSnorm2x16", "packSnorm4x8", "packUnorm", "packUnorm2x16", "packUnorm4x8", "pow", "radians", "reflect", "refract", "round", "roundEven", "sign", "sin", "sinh", "smoothstep", "sqrt", "step", "tan", "tanh", "texelFetch", "texelFetchOffset", "texture", "textureGather", "textureGatherOffset", "textureGatherOffsets", "textureGrad", "textureGradOffset", "textureLod", "textureLodOffset", "textureOffset", "textureProj", "textureProjGrad", "textureProjGradOffset", "textureProjLod", "textureProjLodOffset", "textureProjOffset", "textureQueryLevels", "textureQueryLod", "textureSamples", "textureSize", "transpose", "trunc", "uaddCarry", "uintBitsToFloat", "umulExtended", "unpackDouble2x32", "unpackHalf2x16", "unpackSnorm2x16", "unpackSnorm4x8", "unpackUnorm", "unpackUnorm2x16", "unpackUnorm4x8", "usubBorrow" };
        private static string[] PythonKeywords = { "False", "class", "finally", "is", "return", "None", "continue", "for", "lambda", "try", "True", "def" , "from", "nonlocal", "while", "and", "del", "global", "not", "with", "as", "elif", "if", "or", "yield", "assert", "else", "import", "pass" , "break", "except", "in", "raise" };

        private bool _updating = false;
        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
                Mode = (ETextEditorMode)cboMode.SelectedIndex;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            Font prevFont = TextBox.Font;
            Color prevColor = TextBox.ForeColor;
            using (FontDialog fd = new FontDialog()
            {
                FixedPitchOnly = true,
                Font = TextBox.Font,
                ShowHelp = false,
                ShowApply = true,
                ShowEffects = false,
                ShowColor = true,
                Color = TextBox.ForeColor,
                AllowScriptChange = false,
            })
            {
                fd.Apply += Fd_Apply;
                if (fd.ShowDialog(this) == DialogResult.OK)
                {
                    prevFont = fd.Font;
                    prevColor = fd.Color;
                }
            }
            TextBox.Font = prevFont;
            TextBox.ForeColor = prevColor;
            btnFont.Text = string.Format("{0} {1} pt", TextBox.Font.Name, Math.Round(TextBox.Font.SizeInPoints));
        }

        private void Fd_Apply(object sender, EventArgs e)
        {
            FontDialog fd = sender as FontDialog;
            TextBox.Font = fd.Font;
            TextBox.ForeColor = fd.Color;
            btnFont.Text = string.Format("{0} {1} pt", fd.Font.Name, Math.Round(fd.Font.SizeInPoints));
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<TextFile>(false, true, true) + "|All files (*.*)|*.*",
                Multiselect = true
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    string text = "";
                    foreach (string path in ofd.FileNames)
                        text += File.ReadAllText(path, TextFile.GetEncoding(path));
                    TextBox.Text = text;
                    TextBox.IsChanged = false;
                }
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (TextBox.IsChanged)
            {
                Saved?.Invoke(this);
                TextBox.IsChanged = false;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = TFileObject.GetFilter<TextFile>(false, true, true) + "|All files (*.*)|*.*",
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, TextBox.Text);
                }
            }
        }

        private void btnSelectPaths_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = true
            })
            {
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    TextBox.Text = TextBox.Text.Insert(TextBox.SelectionStart, string.Join(Environment.NewLine, ofd.FileNames));
                }
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Range sel = TextBox.Selection;
            //if (sel.Start.iChar < TextBox.Text.Length)
            //{
            //    char c = TextBox.Text[i];
            //    if (c == '\r')
            //        TextBox.Selection = new FastColoredTextBoxNS.Range(TextBox, new Place())
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool processed = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == Keys.Insert)
            {
                TextBox_SelectionChanged(null, null);
            }
            return processed;
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            bool insert = IsKeyLocked(Keys.Insert);
            Range r = TextBox.Selection;
            StatusText.Text = string.Format("Ln {0} Col {1} {2}", 
                r.Start.iLine + 1, r.Start.iChar + 1, insert ? "OVR" : "INS");

            bool open = AutoCompleteOpen;
            if (open)
            {
                string prevAuto = _autoCompleteStr;
                FindAutocompleteString(TextBox.Selection.Start);
                if (string.IsNullOrWhiteSpace(_autoCompleteStr) ||
                    !string.Equals(_autoCompleteStr, prevAuto))
                {
                    open = false;
                }
            }
                        
            //Range sel = TextBox.Selection;
            //Place start = sel.Start;
            //FindAutocompleteString(start);
            //int selCount = RemakeAutoCompleteSelections(_autoCompleteStr);
            //if (selCount > 1)
            //{
            //    open = true;
            //}

            if (AutoCompleteOpen != open)
                AutoCompleteOpen = open;
        }

        private bool AutoCompleteOpen
        {
            get => pnlAutocomplete.Visible;
            set => pnlAutocomplete.Visible = value;
        }
        private int RemakeAutoCompleteSelections(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            int i = lstAutocomplete.SelectedIndex;
            var matches = 
                GLSLKeywords.Where(x => x.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) >= 0).Union(
                GLSLBuiltInMethods.Where(x => x.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) >= 0)).ToArray();
            Array.Sort(matches);
            lstAutocomplete.Items.Clear();
            if (matches.Length == 0)
                return 0;
            lstAutocomplete.Items.AddRange(matches);
            int match = Array.FindIndex(matches, x => string.Equals(str, x, StringComparison.InvariantCultureIgnoreCase));
            if (match >= 0)
                lstAutocomplete.SelectedIndex = match;
            else
            {
                match = Array.FindIndex(matches, x => x.StartsWith(str, StringComparison.InvariantCultureIgnoreCase));
                if (match >= 0)
                    lstAutocomplete.SelectedIndex = match;
                else
                    lstAutocomplete.SelectedIndex = i.Clamp(0, lstAutocomplete.Items.Count - 1);
            }
            pnlAutocomplete.Height = (matches.Length * lstAutocomplete.ItemHeight).Clamp(0, 264);
            return matches.Length;
        }
        private void FillAutoCompleteSelection()
        {
            TextBox.Selection = _autoCompleteStrRange;
            TextBox.ClearSelected();
            string str = (string)lstAutocomplete.SelectedItem;
            TextBox.InsertText(str);
            _autoCompleteStrRange.End = new Place(_autoCompleteStrRange.Start.iChar + str.Length, _autoCompleteStrRange.End.iLine);
            _autoCompleteStr = str;
            AutoCompleteOpen = false;
        }

        private string _autoCompleteStr = null;
        private Range _autoCompleteStrRange = null;
        private void FindAutocompleteString(Place p)
        {
            //int pos = TextBox.PlaceToPosition(p), start = pos, end = pos;
            //bool onBadChar = true;
            //char c;
            //if (pos < TextBox.Text.Length && pos >= 0)
            //{
            //    c = TextBox.Text[pos];
            //    onBadChar = char.IsWhiteSpace(c) || GLSLSyntax.Contains(c);
            //}
            //for (; start > 0;)
            //{
            //    c = TextBox.Text[start - 1];
            //    if (char.IsWhiteSpace(c) || GLSLSyntax.Contains(c))
            //        break;
            //    --start;
            //}
            //if (!onBadChar)
            //{
            //    for (; end < TextBox.Text.Length - 1;)
            //    {
            //        c = TextBox.Text[end + 1];
            //        if (char.IsWhiteSpace(c) || GLSLSyntax.Contains(c))
            //            break;
            //        ++end;
            //    }
            //}
            //else
            //{
            //    if (start == pos)
            //    {
            //        _autoCompleteStr = null;
            //        return;
            //    }
            //    else
            //    {
            //        --end;
            //    }
            //}
            //_autoCompleteStr = "";
            //for (int i = start; i <= end; ++i)
            //    _autoCompleteStr += TextBox.Text[i].ToString();
            //_autoCompleteStrRange = new Range(TextBox, TextBox.PositionToPlace(start), TextBox.PositionToPlace(end + 1));
        }

        private void lstAutocomplete_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            FillAutoCompleteSelection();
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Modifiers != Keys.None)
                return;

            char c = (char)e.KeyData;
            if (AutoCompleteOpen)
            {
                if (e.KeyData == Keys.Up)
                {
                    lstAutocomplete.SelectedIndex = (lstAutocomplete.SelectedIndex - 1).ClampMin(0);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Down)
                {
                    lstAutocomplete.SelectedIndex = (lstAutocomplete.SelectedIndex + 1).ClampMax(lstAutocomplete.Items.Count - 1);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Return)
                {
                    FillAutoCompleteSelection();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }
        }
        SolidBrush _errorBrush;
        private List<Line> _errorLines = new List<Line>();
        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            foreach (Line line in _errorLines)
            {
                line.BackgroundBrush = TextBox.BackBrush;
                int startRemove = line.Text.IndexOf(" // error");
                if (startRemove >= 0)
                    line.RemoveRange(startRemove, line.Count - startRemove);
                startRemove = line.Text.IndexOf(" // warning");
                if (startRemove >= 0)
                    line.RemoveRange(startRemove, line.Count - startRemove);
            }

            if (e.Modifiers != Keys.None)
                return;

            char c = (char)e.KeyData;
            if (AutoCompleteOpen)
            {
                if (e.KeyData == Keys.Up)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Down)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Return)
                {
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
                else if (e.KeyData == Keys.Left ||
                    e.KeyData == Keys.Right)
                {
                    string prevAuto = _autoCompleteStr;
                    Range sel = TextBox.Selection;
                    Place start = sel.Start;
                    FindAutocompleteString(start);
                    if (string.IsNullOrWhiteSpace(_autoCompleteStr) ||
                        !string.Equals(_autoCompleteStr, prevAuto))
                    {
                        AutoCompleteOpen = false;
                    }
                    e.Handled = true;
                }
                else if (e.KeyData == Keys.Back ||
                    e.KeyData == Keys.Delete ||
                    char.IsLetterOrDigit(c))
                {
                    //e.Handled = false;
                    Range sel = TextBox.Selection;
                    Place start = sel.Start;
                    FindAutocompleteString(start);
                    int selCount = RemakeAutoCompleteSelections(_autoCompleteStr);
                    if (selCount == 0)
                    {
                        AutoCompleteOpen = false;
                        lstAutocomplete.Items.Clear();
                    }
                }
                else if (char.IsWhiteSpace(c) || GLSLSyntax.Contains(c))
                {
                    AutoCompleteOpen = false;
                    lstAutocomplete.Items.Clear();
                }
            }
            else
            {
                if (char.IsLetterOrDigit(c))
                {
                    Range sel = TextBox.Selection;
                    Place start = sel.Start;
                    FindAutocompleteString(start);
                    int selCount = RemakeAutoCompleteSelections(_autoCompleteStr);
                    if (selCount > 0)
                    {
                        AutoCompleteOpen = true;
                        --start.iChar;
                        Point p = TextBox.PlaceToPoint(start);
                        p = PointToClient(TextBox.PointToScreen(p));
                        p.Y += TextBox.CharHeight;
                        //p.X -= TextBox.CharWidth;
                        pnlAutocomplete.Location = p;
                        //e.Handled = true;
                    }
                }
            }

            if (Mode == ETextEditorMode.GLSL)
            {
                var result = CompileGLSL?.Invoke(TextBox.Text, this);
                if (result != null && !result.Value.Item1)
                {
                    string errors = result.Value.Item2;
                    int[] errorLines = errors.FindAllOccurrences(0, ") : ");
                    foreach (int i in errorLines)
                    {
                        int start = errors.FindFirstReverse(i - 1, '(');
                        int lineIndex = int.Parse(errors.Substring(start + 1, i - start - 1)) - 1;
                        if (lineIndex >= 0 && lineIndex < TextBox.LinesCount)
                        {
                            Line lineInfo = TextBox[lineIndex];
                            _errorLines.Add(lineInfo);
                            lineInfo.BackgroundBrush = _errorBrush;

                            string line = lineInfo.Text;
                            int errorStart = i + 4;//errors.FindFirst(i + 3, ':') + 2;
                            int errorEnd = errors.FindFirst(errorStart, "\n");
                            string errorMsg = errors.Substring(errorStart, errorEnd - errorStart);

                            lineInfo.AddRange((" // " + errorMsg).Select(x => new FastColoredTextBoxNS.Char(x)));
                        }
                        //Match m = Regex.Match(errorMsg, "(?<= \").*(?=\")");
                        //int tokenStart = line.IndexOf(m.Value);
                        //if (tokenStart < 0)
                        //    continue;
                        //Place px;
                        //for (int x = 0; x < m.Length; ++x)
                        //{
                        //    px = new Place(tokenStart + x, lineIndex);
                        //    FastColoredTextBoxNS.Char cx = TextBox[px];
                        //    cx.style = TextBox.GetStyleIndexMask(new Style[] { _errorStyle });
                        //    TextBox[px] = cx;
                        //}
                        //0(line#) : error CXXXX: <message>
                        //at token "<token>"
                    }
                }
            }
        }
    }
}