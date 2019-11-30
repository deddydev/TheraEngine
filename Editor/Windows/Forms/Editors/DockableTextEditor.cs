using Extensions;
using FastColoredTextBoxNS;
using Svg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Reflection;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public enum ETextEditorMode
    {
        Text,
        Python,
        Lua,
        [Description("C#")]
        CSharp,
        GLSL,
        XML,
        JSON,
        //C,
        //[Description("C++")]
        //CPlusPlus,
    }
    [EditorFor(typeof(TextFile))]
    public partial class DockableTextEditor : DockContent
    {
        public DockableTextEditor()
        {
            InitializeComponent();

            foreach (ETextEditorMode e in Enum.GetValues(typeof(ETextEditorMode)))
                cboMode.Items.Add(e.GetDescription());
            cboMode.SelectedIndex = 0;

            TextBox.CustomAction += TextBox_CustomAction;
            TextBox.HotkeysMapping.Add(Keys.Control | Keys.S, FCTBAction.CustomAction1);

            TextBox.AutoCompleteBrackets = true;
            TextBox.AllowSeveralTextStyleDrawing = true;
            TextBox.AutoIndent = true;
            TextBox.AutoIndentChars = true;
            TextBox.AutoCompleteBrackets = true;

            TextBox.Language = Language.Custom;
            TextBox.AddStyle(SameWordsStyle);
            //TextBox.AddStyle(PreprocessorStyle);
            //TextBox.AddStyle(KeywordStyle);
            //TextBox.AddStyle(ClassNameStyle);

            stripTextDisplay.RenderMode = ToolStripRenderMode.Professional;
            stripMain.RenderMode = ToolStripRenderMode.Professional;
            stripSearch.RenderMode = ToolStripRenderMode.Professional;
            stripTextEdit.RenderMode = ToolStripRenderMode.Professional;
            ctxRightClick.RenderMode = ToolStripRenderMode.Professional;

            stripTextDisplay.Renderer =
            stripMain.Renderer =
            stripSearch.Renderer =
            stripTextEdit.Renderer =
            ctxRightClick.Renderer = new TheraForm.TheraToolStripRenderer();

            dgvObjectExplorer.RowPrePaint += DgvObjectExplorer_RowPrePaint;
        }
        public DockableTextEditor(TextFile file) : this() => TargetFile = file;

        private void DgvObjectExplorer_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            dgvObjectExplorer.Rows[e.RowIndex].DefaultCellStyle.BackColor = dgvObjectExplorer.BackgroundColor;
        }
        
        private AutocompleteMenu AutoCompleteMenu { get; set; }
        
        //private Range HoveredWordRange { get; set; }
        private Style InvisibleCharsStyle { get; } = new InvisibleCharsRenderer(Pens.Gray);
        private Color CurrentLineColor { get; } = Color.FromArgb(255, 80, 95, 90);
        private Color ChangedLineColor { get; } = Color.FromArgb(200, Color.Yellow);
        //private MarkerStyle HoveredWordStyle { get; } = new MarkerStyle(new SolidBrush(Color.FromArgb(255, 60, 60, 40)));
        private TextStyle KeywordStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(86, 156, 214)), null, FontStyle.Regular);
        private TextStyle ClassNameStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(78, 201, 176)), null, FontStyle.Regular);
        private TextStyle PreprocessorStyle { get; } = new TextStyle(Brushes.Gray, null, FontStyle.Regular);
        private TextStyle NumberStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(181, 206, 168)), null, FontStyle.Regular);
        private TextStyle CommentStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(86, 166, 74)), null, FontStyle.Regular);
        private TextStyle StringStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(214, 157, 133)), null, FontStyle.Regular);
        private TextStyle MethodStyle { get; } = new TextStyle(new SolidBrush(Color.FromArgb(200, 107, 83)), null, FontStyle.Regular);
        private TextStyle MaroonStyle { get; } = new TextStyle(Brushes.Maroon, null, FontStyle.Regular);
        private MarkerStyle SameWordsStyle { get; } = new MarkerStyle(new SolidBrush(Color.FromArgb(150, Color.Teal)));

        public static Dictionary<string, DockableTextEditor> TextEditorInstances { get; } = new Dictionary<string, DockableTextEditor>();

        public static DockableTextEditor ShowNew(
            DockPanel dockPanel,
            DockState document,
            TextFile file,
            DelTextSavedHandler onSavedOverride = null)
        {
            DockableTextEditor editor;
            if (file.RootFile == file && !string.IsNullOrWhiteSpace(file?.FilePath))
            {
                if (TextEditorInstances.ContainsKey(file.FilePath))
                {
                    DockableTextEditor e = TextEditorInstances[file.FilePath];
                    e.Focus();
                    return e;
                }
                else
                {
                    editor = new DockableTextEditor();
                    TextEditorInstances.Add(file.FilePath, editor);
                }
            }
            else
            {
                editor = new DockableTextEditor();
            }
            
            editor.Show(dockPanel, document);
            editor.TargetFile = file;

            if (onSavedOverride != null)
                editor.SavedOverride += onSavedOverride;

            return editor;
        }
        private void TextBox_CustomAction(object sender, CustomActionEventArgs e)
        {
            switch (e.Action)
            {
                case FCTBAction.CustomAction1:
                    btnSave_Click(null, null);
                    break;
            }
        }
        private bool _isStreaming = false;
        private TextFile _targetFile;
        public TextFile TargetFile
        {
            get => _targetFile;
            set
            {
                string path = _targetFile is null || _targetFile.RootFile != _targetFile ? null : _targetFile.FilePath;
                if (!string.IsNullOrWhiteSpace(path))
                {
                    if (TextEditorInstances.ContainsKey(path))
                        TextEditorInstances.Remove(path);

                    TextBox.CloseBindingFile();
                }

                _targetFile = value;
                AppDomainHelper.Sponsor(_targetFile);

                path = _targetFile is null || _targetFile.RootFile != _targetFile ? string.Empty : _targetFile.FilePath ?? string.Empty;
                if (_isStreaming = !string.IsNullOrWhiteSpace(path))
                {
                    if (!TextEditorInstances.ContainsKey(path))
                        TextEditorInstances.Add(path, this);

                    Text = Path.GetFileName(path);
                    try
                    {
                        TextBox.OpenBindingFile(path, _targetFile.Encoding);
                    }
                    catch (Exception ex)
                    {
                        Engine.LogException(ex);
                        _isStreaming = false;
                        TextBox.Text = _targetFile.Text;
                    }
                }
                else
                {
                    Text = "Text Editor";
                    TextBox.Text = _targetFile.Text;
                }

                DetermineMode();

                TextBox.IsChanged = false;
                TextBox.ClearUndo();
            }
        }

        private void DetermineMode()
        {
            string path = _targetFile?.FilePath ?? string.Empty;
            if (_targetFile is CSharpScript || path.EndsWith("cs", StringComparison.InvariantCultureIgnoreCase))
            {
                Mode = ETextEditorMode.CSharp;
            }
            else if (_targetFile is PythonScript || path.EndsWith("py", StringComparison.InvariantCultureIgnoreCase))
            {
                Mode = ETextEditorMode.Python;
            }
            else if (_targetFile is LuaScript || path.EndsWith("lua", StringComparison.InvariantCultureIgnoreCase))
            {
                Mode = ETextEditorMode.Lua;
            }
            else if (_targetFile is GLSLScript || path.EndsWithAny(
                TFileObject.GetFile3rdPartyExtensions<GLSLScript>().Extensions, StringComparison.InvariantCultureIgnoreCase))
            {
                Mode = ETextEditorMode.GLSL;
            }
            else if (TextBox.Text.StartsWith("<?xml", StringComparison.OrdinalIgnoreCase))
            {
                Mode = ETextEditorMode.XML;
            }
            else
            {
                Mode = ETextEditorMode.Text;
            }
        }

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
                    case ETextEditorMode.Text:      UnInitText();   break;
                    case ETextEditorMode.Python:    UnInitPython(); break;
                    case ETextEditorMode.CSharp:    UnInitCSharp(); break;
                    case ETextEditorMode.Lua:       UnInitLua();    break;
                    case ETextEditorMode.GLSL:      UnInitGLSL();   break;
                    case ETextEditorMode.XML:       UnInitXML();    break;
                    case ETextEditorMode.JSON:      UnInitJSON();   break;
                }
                _mode = value;
                cboMode.SelectedIndex = (int)_mode;
                switch (_mode)
                {
                    case ETextEditorMode.Text:      InitText();     break;
                    case ETextEditorMode.Python:    InitPython();   break;
                    case ETextEditorMode.CSharp:    InitCSharp();   break;
                    case ETextEditorMode.Lua:       InitLua();      break;
                    case ETextEditorMode.GLSL:      InitGLSL();     break;
                    case ETextEditorMode.XML:       InitXML();      break;
                    case ETextEditorMode.JSON:      InitJSON();     break;
                }
                
                TextBox.OnTextChanged();
                TextBox.OnSyntaxHighlight(new TextChangedEventArgs(TextBox.Range));

                _updating = false;
            }
        }
        private void ReInit()
        {
            switch (_mode)
            {
                case ETextEditorMode.Text:      UnInitText();   InitText();     break;
                case ETextEditorMode.Python:    UnInitPython(); InitPython();   break;
                case ETextEditorMode.CSharp:    UnInitCSharp(); InitCSharp();   break;
                case ETextEditorMode.Lua:       UnInitLua();    InitLua();      break;
                case ETextEditorMode.GLSL:      UnInitGLSL();   InitGLSL();     break;
                case ETextEditorMode.XML:       UnInitXML();    InitXML();      break;
                case ETextEditorMode.JSON:      UnInitJSON();   InitJSON();     break;
            }
        }

        private static readonly string StringRegex = @"""""|@""""|''|@"".*?""|(?<!@)(?<range>"".*?[^\\]"")|'.*?[^\\]'";
        private static readonly string NumberRegex = @"\b\d+[\.]?\d*([eE]\-?\d+)?[lLdDfF]?\b|\b0x[a-fA-F\d]+\b";

        #region Text Init
        private void InitText()
        {
            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;
        }
        private void UnInitText()
        {
            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;
        }
        #endregion

        #region Python Init
        private void InitPython()
        {
            TextBox.CommentPrefix = "#";
            TextBox.AutoCompleteBrackets = true;
            TextBox.AutoIndent = true;
            //TextBox.DescriptionFile = Engine.Files.ScriptPath("PythonHighlighting.xml");
            TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_Python;
        }
        private void UnInitPython()
        {
            TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_Python;
        }
        public static string[] PythonKeywords =
        {
            "False",
            "class",
            "finally",
            "is",
            "return",
            "None",
            "continue",
            "for",
            "lambda",
            "try",
            "True",
            "def",
            "from",
            "nonlocal",
            "while",
            "and",
            "del",
            "global",
            "not",
            "with",
            "as",
            "elif",
            "if",
            "or",
            "yield",
            "assert",
            "else",
            "import",
            "pass",
            "break",
            "except",
            "in",
            "raise"
        };
        public static readonly string PythonKeywordsRegex = string.Join("|", PythonKeywords);
        private void TextBox_AutoIndentNeeded_Python(object sender, AutoIndentEventArgs e)
        {
            string line = e.LineText.Trim();
            if (line.EndsWith(":"))
            {
                e.ShiftNextLines = e.TabLength;
                return;
            }
        }
        #endregion

        #region GLSL Init
        private RenderShader _glslShader;
        private void InitGLSL()
        {
            if (_targetFile is GLSLScript script)
            {
                //_glslShader = Editor.DomainProxy.CreateInstance<RenderShader>(script);
                //AppDomainHelper.Sponsor(_glslShader);
                _glslShader = new RenderShader(script);
                _glslShader.Generate();
            }

            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;

            CurrentKeywords = GLSLKeywords;
            HighlightScriptSyntax = GLSLSyntaxHighlight;

            TextBox.LeftBracket = '(';
            TextBox.RightBracket = ')';
            TextBox.LeftBracket2 = '\x0';
            TextBox.RightBracket2 = '\x0';
            TextBox.AutoIndent = true;
            TextBox.AutoIndentChars = true;
            TextBox.AutoCompleteBrackets = true;
            TextBox.DelayedTextChangedInterval = 1000;
            TextBox.DelayedEventsInterval = 500;

            //TextBox.VisibleRangeChanged += TextBox_VisibleRangeChanged;
            //TextBox.TextChangedDelayed += TextBox_TextChangedDelayed;
            //TextBox.TextChanged += TextBox_TextChanged;
            TextBox.SelectionChangedDelayed += TextBox_SelectionChangedDelayed;
            TextBox.KeyDown += TextBox_KeyDown;
            TextBox.MouseMove += TextBox_MouseMove;
            TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_GLSL;
            TextBox.SelectionChanged += TextBox_SelectionChanged;

            TextBox.ChangedLineColor = ChangedLineColor;
            //if (btnHighlightCurrentLine.Checked)
            TextBox.CurrentLineColor = CurrentLineColor;
            TextBox.ShowFoldingLines = btnShowFoldingLines.Checked;

            //create autocomplete popup menu
            AutoCompleteMenu = new AutocompleteMenu(TextBox);
            //popupMenu.Items.ImageList = ilAutocomplete;
            AutoCompleteMenu.Opening += popupMenu_Opening;
            AutoCompleteMenu.SearchPattern = @"[\w\.:=!<>]";
            BuildAutocompleteMenu();
            //TextBox.Tag = new TextBoxInfo() { PopupMenu = AutoCompleteMenu };
        }
        private void UnInitGLSL()
        {
            _glslShader?.Dispose();
            _glslShader = null;

            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;

            TextBox.LeftBracket = '\x0';
            TextBox.RightBracket = '\x0';
            TextBox.LeftBracket2 = '\x0';
            TextBox.RightBracket2 = '\x0';

            //TextBox.TextChangedDelayed -= TextBox_TextChangedDelayed;
            //TextBox.TextChanged -= TextBox_TextChanged;
            TextBox.SelectionChangedDelayed -= TextBox_SelectionChangedDelayed;
            TextBox.KeyDown -= TextBox_KeyDown;
            TextBox.MouseMove -= TextBox_MouseMove;
            TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_GLSL;
            TextBox.SelectionChanged -= TextBox_SelectionChanged;
            AutoCompleteMenu.Opening -= popupMenu_Opening;
        }
        private void GLSLSyntaxHighlight(Range e)
        {
            //clear style of changed range
            e.ClearStyle(KeywordStyle, NumberStyle, ClassNameStyle, PreprocessorStyle, CommentStyle, StringStyle, MethodStyle);

            //method highlighting
            e.SetStyle(MethodStyle, @"\b(abs|acos|acosh|all|any|asin|asinh|atan|atanh|atomicAdd|atomicAnd|atomicCompSwap|atomicCounter|atomicCounterDecrement|atomicCounterIncrement|atomicExchange|atomicMax|atomicMin|atomicOr|atomicXor|barrier|bitCount|bitfieldExtract|bitfieldInsert|bitfieldReverse|ceil|clamp|cos|cosh|cross|degrees|determinant|dFdx|dFdxCoarse|dFdxFine|dFdy|dFdyCoarse|dFdyFine|distance|dot|EmitStreamVertex|EmitVertex|EndPrimitive|EndStreamPrimitive|equal|exp|exp2|faceforward|findLSB|findMSB|floatBitsToInt|floatBitsToUint|floor|fma|fract|frexp|fwidth|fwidthCoarse|fwidthFine|gl_ClipDistance|gl_CullDistance|gl_FragCoord|gl_FragDepth|gl_FrontFacing|gl_GlobalInvocationID|gl_HelperInvocation|gl_InstanceID|gl_InvocationID|gl_Layer|gl_LocalInvocationID|gl_LocalInvocationIndex|gl_NumSamples|gl_NumWorkGroups|gl_PatchVerticesIn|gl_PointCoord|gl_PointSize|gl_Position|gl_PrimitiveID|gl_PrimitiveIDIn|gl_SampleID|gl_SampleMask|gl_SampleMaskIn|gl_SamplePosition|gl_TessCoord|gl_TessLevelInner|gl_TessLevelOuter|gl_VertexID|gl_ViewportIndex|gl_WorkGroupID|gl_WorkGroupSize|greaterThan|greaterThanEqual|groupMemoryBarrier|imageAtomicAdd|imageAtomicAnd|imageAtomicCompSwap|imageAtomicExchange|imageAtomicMax|imageAtomicMin|imageAtomicOr|imageAtomicXor|imageLoad|imageSamples|imageSize|imageStore|imulExtended|intBitsToFloat|interpolateAtCentroid|interpolateAtOffset|interpolateAtSample|inverse|inversesqrt|isinf|isnan|ldexp|length|lessThan|lessThanEqual|log|log2|matrixCompMult|max|memoryBarrier|memoryBarrierAtomicCounter|memoryBarrierBuffer|memoryBarrierImage|memoryBarrierShared|min|mix|mod|modf|noise|noise1|noise2|noise3|noise4|normalize|not|notEqual|outerProduct|packDouble2x32|packHalf2x16|packSnorm2x16|packSnorm4x8|packUnorm|packUnorm2x16|packUnorm4x8|pow|radians|reflect|refract|round|roundEven|sign|sin|sinh|smoothstep|sqrt|step|tan|tanh|texelFetch|texelFetchOffset|texture|textureGather|textureGatherOffset|textureGatherOffsets|textureGrad|textureGradOffset|textureLod|textureLodOffset|textureOffset|textureProj|textureProjGrad|textureProjGradOffset|textureProjLod|textureProjLodOffset|textureProjOffset|textureQueryLevels|textureQueryLod|textureSamples|textureSize|transpose|trunc|uaddCarry|uintBitsToFloat|umulExtended|unpackDouble2x32|unpackHalf2x16|unpackSnorm2x16|unpackSnorm4x8|unpackUnorm|unpackUnorm2x16|unpackUnorm4x8|usubBorrow)\b");

            //string highlighting
            e.SetStyle(StringStyle, StringRegex);

            //keyword highlighting
            e.SetStyle(KeywordStyle, @"\b(attribute|const|uniform|varying|buffer|shared|coherent|volatile|restrict|readonly|writeonly|atomic_uint|layout|centroid|flat|smooth|noperspective|patch|sample|break|continue|do|for|while|switch|case|default|if|else|subroutine|in|out|inout|void|true|false|invariant|precise|discard|return|lowp|mediump|highp|precision|sampler1D|sampler2D|sampler3D|samplerCube|sampler1DShadow|sampler2DShadow|samplerCubeShadow|sampler1DArray|sampler2DArray|sampler1DArrayShadow|sampler2DArrayShadow|isampler1D|isampler2D|isampler3D|isamplerCube|isampler1DArray|isampler2DArray|usampler1D|usampler2D|usampler3D|usamplerCube|usampler1DArray|usampler2DArray|sampler2DRect|sampler2DRectShadow|isampler2DRect|usampler2DRect|samplerBuffer|isamplerBuffer|usamplerBuffer|sampler2DMS|isampler2DMS|usampler2DMS|sampler2DMSArray|isampler2DMSArray|usampler2DMSArray|samplerCubeArray|samplerCubeArrayShadow|isamplerCubeArray|usamplerCubeArray|image1D|iimage1D|uimage1D|image2D|iimage2D|uimage2D|image3D|iimage3D|uimage3D|image2DRect|iimage2DRect|uimage2DRect|imageCube|iimageCube|uimageCube|imageBuffer|iimageBuffer|uimageBuffer|image1DArray|iimage1DArray|uimage1DArray|image2DArray|iimage2DArray|uimage2DArray|imageCubeArray|iimageCubeArray|uimageCubeArray|image2DMS|iimage2DMS|uimage2DMS|image2DMSArray|iimage2DMSArray|uimage2DMSArray|struct)\b");

            //comment highlighting
            e.SetStyle(CommentStyle, @"//.*$", RegexOptions.Multiline);
            e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);

            //number highlighting
            e.SetStyle(NumberStyle, NumberRegex);

            //attribute highlighting
            //e.SetStyle(AttributeStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            e.SetStyle(PreprocessorStyle, @"^#(version|pragma|include)\b");

            //class name highlighting
            e.SetStyle(ClassNameStyle, @"\b(mat2|mat3|mat4|dmat2|dmat3|dmat4|mat2x2|mat2x3|mat2x4|dmat2x2|dmat2x3|dmat2x4|mat3x2|mat3x3|mat3x4|dmat3x2|dmat3x3|dmat3x4|mat4x2|mat4x3|mat4x4|dmat4x2|dmat4x3|dmat4x4|float|vec2|vec3|vec4|int|ivec2|ivec3|ivec4|bool|bvec2|bvec3|bvec4|double|dvec2|dvec3|dvec4|uint|uvec2|uvec3|uvec4)\b");

            //clear folding markers
            e.ClearFoldingMarkers();

            //set folding markers
            e.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            e.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
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
        public static readonly string[] GLSLKeywords =
        {
            "attribute",
            "const",
            "uniform",
            "varying",
            "buffer",
            "shared",
            "coherent",
            "volatile",
            "restrict",
            "readonly",
            "writeonly",
            "atomic_uint",
            "layout",
            "centroid",
            "flat",
            "smooth",
            "noperspective",
            "patch",
            "sample",
            "break",
            "continue",
            "do",
            "for",
            "while",
            "switch",
            "case",
            "default", "if", "else", "subroutine", "in", "out", "inout", "float", "double", "int", "void", "bool", "true", "false", "invariant", "precise", "discard", "return", "mat2", "mat3", "mat4", "dmat2", "dmat3", "dmat4", "mat2x2", "mat2x3", "mat2x4", "dmat2x2", "dmat2x3", "dmat2x4", "mat3x2", "mat3x3", "mat3x4", "dmat3x2", "dmat3x3", "dmat3x4", "mat4x2", "mat4x3", "mat4x4", "dmat4x2", "dmat4x3", "dmat4x4", "vec2", "vec3", "vec4", "ivec2", "ivec3", "ivec4", "bvec2", "bvec3", "bvec4", "dvec2", "dvec3", "dvec4", "uint", "uvec2", "uvec3", "uvec4", "lowp", "mediump", "highp", "precision", "sampler1D", "sampler2D", "sampler3D", "samplerCube", "sampler1DShadow", "sampler2DShadow", "samplerCubeShadow", "sampler1DArray", "sampler2DArray", "sampler1DArrayShadow", "sampler2DArrayShadow", "isampler1D", "isampler2D", "isampler3D", "isamplerCube", "isampler1DArray", "isampler2DArray", "usampler1D", "usampler2D", "usampler3D", "usamplerCube", "usampler1DArray", "usampler2DArray", "sampler2DRect", "sampler2DRectShadow", "isampler2DRect", "usampler2DRect", "samplerBuffer", "isamplerBuffer", "usamplerBuffer", "sampler2DMS", "isampler2DMS", "usampler2DMS", "sampler2DMSArray", "isampler2DMSArray", "usampler2DMSArray", "samplerCubeArray", "samplerCubeArrayShadow", "isamplerCubeArray", "usamplerCubeArray", "image1D", "iimage1D", "uimage1D", "image2D", "iimage2D", "uimage2D", "image3D", "iimage3D", "uimage3D", "image2DRect", "iimage2DRect", "uimage2DRect", "imageCube", "iimageCube", "uimageCube", "imageBuffer", "iimageBuffer", "uimageBuffer", "image1DArray", "iimage1DArray", "uimage1DArray", "image2DArray", "iimage2DArray", "uimage2DArray", "imageCubeArray", "iimageCubeArray", "uimageCubeArray", "image2DMS", "iimage2DMS", "uimage2DMS", "image2DMSArray", "iimage2DMSArray", "uimage2DMSArray", "struct" };

        #endregion

        #region Lua Init
        private void InitLua()
        {

        }
        private void UnInitLua()
        {

        }
        #endregion

        #region C# Init
        private void InitCSharp()
        {
            dgvObjectExplorer.Visible = true;
            lblSplitFileObjects.Visible = true;
            CurrentKeywords = CSharpKeywords;
            HighlightScriptSyntax = CSharpSyntaxHighlight;

            TextBox.AutoIndent = true;
            TextBox.AutoIndentChars = true;
            TextBox.AutoCompleteBrackets = true;
            TextBox.DelayedTextChangedInterval = 1000;
            TextBox.DelayedEventsInterval = 500;

            //TextBox.VisibleRangeChanged += TextBox_VisibleRangeChanged;
            //TextBox.TextChangedDelayed += TextBox_TextChangedDelayed;
            //TextBox.TextChanged += TextBox_TextChanged;
            TextBox.SelectionChangedDelayed += TextBox_SelectionChangedDelayed;
            TextBox.KeyDown += TextBox_KeyDown;
            TextBox.MouseMove += TextBox_MouseMove;
            TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_CSharp;
            TextBox.SelectionChanged += TextBox_SelectionChanged;

            TextBox.ChangedLineColor = ChangedLineColor;
            //if (btnHighlightCurrentLine.Checked)
                TextBox.CurrentLineColor = CurrentLineColor;
            TextBox.ShowFoldingLines = btnShowFoldingLines.Checked;

            //create autocomplete popup menu
            AutoCompleteMenu = new AutocompleteMenu(TextBox);
            //popupMenu.Items.ImageList = ilAutocomplete;
            AutoCompleteMenu.Opening += popupMenu_Opening;
            AutoCompleteMenu.SearchPattern = @"[\w\.:=!<>]";
            BuildAutocompleteMenu();
            //TextBox.Tag = new TextBoxInfo() { PopupMenu = AutoCompleteMenu };
        }
        private void UnInitCSharp()
        {
            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;

            //TextBox.TextChangedDelayed -= TextBox_TextChangedDelayed;
            TextBox.SelectionChangedDelayed -= TextBox_SelectionChangedDelayed;
            TextBox.KeyDown -= TextBox_KeyDown;
            TextBox.MouseMove -= TextBox_MouseMove;
            TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_CSharp;
            TextBox.SelectionChanged -= TextBox_SelectionChanged;
            AutoCompleteMenu.Opening -= popupMenu_Opening;
        }
        public static readonly string[] CSharpPreprocessDirectives =
        {
            "region",
            "endregion",
            "if",
            "elif",
            "else",
            "endif",
            "error",
            "warning",
            "pragma",
            "line",
        };
        public static readonly string CSharpPreprocessDirectivesRegex = string.Join("|", CSharpPreprocessDirectives);
        public static readonly string[] CSharpKeywords =
        {
            "abstract",
            "as",
            "base",
            "bool",
            "break",
            "byte",
            "case",
            "catch",
            "char",
            "checked",
            "class",
            "const",
            "continue",
            "decimal",
            "default",
            "delegate",
            "do",
            "double",
            "else",
            "enum",
            "event",
            "explicit",
            "extern",
            "false",
            "finally",
            "fixed",
            "float",
            "for",
            "foreach",
            "goto",
            "if",
            "implicit",
            "in",
            "int",
            "interface",
            "internal",
            "is",
            "lock",
            "long",
            "namespace",
            "new",
            "null",
            "object",
            "operator",
            "out",
            "override",
            "params",
            "private",
            "protected",
            "public",
            "readonly",
            "ref",
            "return",
            "sbyte",
            "sealed",
            "short",
            "sizeof",
            "stackalloc",
            "static",
            "string",
            "struct",
            "switch",
            "this",
            "throw",
            "true",
            "try",
            "typeof",
            "uint",
            "ulong",
            "unchecked",
            "unsafe",
            "ushort",
            "using",
            "virtual",
            "void",
            "volatile",
            "while",
            "add",
            "alias",
            "ascending",
            "descending",
            "dynamic",
            "from",
            "get",
            "global",
            "group",
            "into",
            "join",
            "let",
            "orderby",
            "partial",
            "remove",
            "select",
            "set",
            "value",
            "var",
            "where",
            "yield",
            "nameof"
        };
        public static readonly string CSharpKeywordsRegex = string.Join("|", CSharpKeywords);

        public string[] CurrentKeywords;

        public static readonly string[] CSharpObjectMethods =
        {
            "Equals()", "GetHashCode()", "GetType()", "ToString()"
        };
        public static readonly string[] CSharpSnippets =
        {
            "if (^)\n{\n\n}",
            "if (^)\n{\n\n}\nelse\n{\n\n}",
            "for (^;;)\n{\n\n}",
            "while (^)\n{\n\n}",
            "do\n{\n^;\n} ",
            "while (^);",
            "switch (^)\n{\n\n}",
            "case ^:",
            "case ^:\nbreak;",
            "case ^:\n{\n\n}\nbreak;",
        };
        public static readonly string[] CSharpDecSnippets =
        {
            "public class ^\n{\n\n}",
            "private class ^\n{\n\n}",
            "internal class ^\n{\n\n}",

            "public struct ^\n{\n\n}",
            "private struct ^\n{\n\n}",
            "internal struct ^\n{\n\n}",

            "public void ^()\n{\nthrow new NotImplementedException();\n}",
            "private void ^()\n{\nthrow new NotImplementedException();\n}",
            "internal void ^()\n{\nthrow new NotImplementedException();\n}",
            "internal protected void ^()\n{\nthrow new NotImplementedException();\n}",
            "protected void ^()\n{\nthrow new NotImplementedException();\n}",

            "public static void ^()\n{\nthrow new NotImplementedException();\n}",
            "private static  void ^()\n{\nthrow new NotImplementedException();\n}",
            "internal static void ^()\n{\nthrow new NotImplementedException();\n}",
            "internal protected static void ^()\n{\nthrow new NotImplementedException();\n}",
            "protected static void ^()\n{\nthrow new NotImplementedException();\n}",

            "public ^{ get; set; }",
            "private ^{ get; set; }",
            "internal ^{ get; set; }",
            "internal protected ^{ get; set; }",
            "protected ^{ get; set; }",
        };

        private void CSharpSyntaxHighlight(Range e)
        {
            TextBox.LeftBracket = '(';
            TextBox.RightBracket = ')';
            TextBox.LeftBracket2 = '\x0';
            TextBox.RightBracket2 = '\x0';

            //clear style of changed range
            e.ClearStyle(KeywordStyle, ClassNameStyle, PreprocessorStyle, NumberStyle, CommentStyle, StringStyle);

            //Range hoveredWord = e.GetIntersectionWith(HoveredWordRange);
            //if (hoveredWord.Length > 0)
            //    hoveredWord.SetStyle(HoveredWordStyle);

            //string highlighting
            e.SetStyle(StringStyle, StringRegex);

            //keyword highlighting
            e.SetStyle(KeywordStyle, @"\b(abstract|as|base|bool|break|byte|case|catch|char|checked|class|const|continue|decimal|default|delegate|do|double|else|enum|event|explicit|extern|false|finally|fixed|float|for|foreach|goto|if|implicit|in|int|interface|internal|is|lock|long|namespace|new|null|object|operator|out|override|params|private|protected|public|readonly|ref|return|sbyte|sealed|short|sizeof|stackalloc|static|string|struct|switch|this|throw|true|try|typeof|uint|ulong|unchecked|unsafe|ushort|using|virtual|void|volatile|while|add|alias|ascending|descending|dynamic|from|get|global|group|into|join|let|orderby|partial|remove|select|set|value|var|where|yield|nameof)\b");

            //comment highlighting
            e.SetStyle(CommentStyle, @"//.*$", RegexOptions.Multiline);
            e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);

            //number highlighting
            e.SetStyle(NumberStyle, NumberRegex);

            //attribute highlighting
            //e.SetStyle(AttributeStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            e.SetStyle(PreprocessorStyle, @"^#(region|endregion|if|elif|else|endif|error|warning|pragma|line)\b");

            //class name highlighting
            e.SetStyle(ClassNameStyle, @"\b(class|struct|enum|interface)\s+(?<range>\w+?)\b");

            //clear folding markers
            e.ClearFoldingMarkers();

            //set folding markers
            e.SetFoldingMarkers("{", "}"); //allow to collapse brackets block
            e.SetFoldingMarkers(@"#region\b", @"#endregion\b"); //allow to collapse #region blocks
            e.SetFoldingMarkers(@"/\*", @"\*/"); //allow to collapse comment block
        }
        private void TextBox_AutoIndentNeeded_CSharp(object sender, AutoIndentEventArgs args)
        {
            //block {}
            if (Regex.IsMatch(args.LineText, @"^[^""']*\{.*\}[^""']*$"))
                return;

            //start of block {}
            if (Regex.IsMatch(args.LineText, @"^[^""']*\{"))
            {
                args.ShiftNextLines = args.TabLength;
                return;
            }

            //end of block {}
            if (Regex.IsMatch(args.LineText, @"}[^""']*$"))
            {
                args.Shift = -args.TabLength;
                args.ShiftNextLines = -args.TabLength;
                return;
            }

            //label
            if (Regex.IsMatch(args.LineText, @"^\s*\w+\s*:\s*($|//)") &&
                !Regex.IsMatch(args.LineText, @"^\s*default\s*:"))
            {
                args.Shift = -args.TabLength;
                return;
            }

            //some statements: case, default
            if (Regex.IsMatch(args.LineText, @"^\s*(case|default)\b.*:\s*($|//)"))
            {
                args.Shift = -args.TabLength / 2;
                return;
            }

            //is unclosed operator in previous line ?
            if (Regex.IsMatch(args.PrevLineText, @"^\s*(if|for|foreach|while|[\}\s]*else)\b[^{]*$"))
                if (!Regex.IsMatch(args.PrevLineText, @"(;\s*$)|(;\s*//)")) //operator is unclosed
                {
                    args.Shift = args.TabLength;
                    return;
                }
        }
        #endregion

        #region XML Init
        private void InitXML()
        {
            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;

            CurrentKeywords = new string[0];
            HighlightScriptSyntax = XMLSyntaxHighlight;

            TextBox.LeftBracket = '<';
            TextBox.RightBracket = '>';
            TextBox.LeftBracket2 = '\x0';
            TextBox.RightBracket2 = '\x0';
            TextBox.AutoIndent = true;
            TextBox.AutoIndentChars = false;
            TextBox.AutoCompleteBrackets = true;
            TextBox.DelayedTextChangedInterval = 1000;
            TextBox.DelayedEventsInterval = 500;

            //TextBox.VisibleRangeChanged += TextBox_VisibleRangeChanged;
            //TextBox.TextChangedDelayed += TextBox_TextChangedDelayed;
            //TextBox.TextChanged += TextBox_TextChanged;
            TextBox.SelectionChangedDelayed += TextBox_SelectionChangedDelayed;
            //TextBox.KeyDown += TextBox_KeyDown;
            TextBox.MouseMove += TextBox_MouseMove;
            //TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded_XML;
            TextBox.SelectionChanged += TextBox_SelectionChanged;

            TextBox.ChangedLineColor = ChangedLineColor;
            //if (btnHighlightCurrentLine.Checked)
            TextBox.CurrentLineColor = CurrentLineColor;
            TextBox.ShowFoldingLines = btnShowFoldingLines.Checked;
        }
        private void UnInitXML()
        {
            dgvObjectExplorer.Visible = false;
            lblSplitFileObjects.Visible = false;

            TextBox.LeftBracket = '\x0';
            TextBox.RightBracket = '\x0';
            TextBox.LeftBracket2 = '\x0';
            TextBox.RightBracket2 = '\x0';

            //TextBox.TextChangedDelayed -= TextBox_TextChangedDelayed;
            //TextBox.TextChanged -= TextBox_TextChanged;
            TextBox.SelectionChangedDelayed -= TextBox_SelectionChangedDelayed;
            //TextBox.KeyDown -= TextBox_KeyDown;
            TextBox.MouseMove -= TextBox_MouseMove;
            //TextBox.AutoIndentNeeded -= TextBox_AutoIndentNeeded_GLSL;
            TextBox.SelectionChanged -= TextBox_SelectionChanged;
        }
        private void XMLSyntaxHighlight(Range e)
        {
            //clear style of changed range
            e.ClearStyle(NumberStyle, StringStyle, MethodStyle, ClassNameStyle);

            //if (HoveredWordRange != null)
            //{
            //    Range hoveredWord = e.GetIntersectionWith(HoveredWordRange);
            //    if (hoveredWord.Length > 0)
            //        hoveredWord.SetStyle(HoveredWordStyle);
            //}

            //attribute highlighting
            e.SetStyle(MethodStyle, "(?<=\"*\\s*)[a-zA-Z][a-zA-Z0-9.\\-_:]+(?=\\s*=)");

            //string highlighting
            e.SetStyle(StringStyle, StringRegex);

            //keyword highlighting
            //e.SetStyle(KeywordStyle, @"\b(attribute|const|uniform|varying|buffer|shared|coherent|volatile|restrict|readonly|writeonly|atomic_uint|layout|centroid|flat|smooth|noperspective|patch|sample|break|continue|do|for|while|switch|case|default|if|else|subroutine|in|out|inout|void|true|false|invariant|precise|discard|return|lowp|mediump|highp|precision|sampler1D|sampler2D|sampler3D|samplerCube|sampler1DShadow|sampler2DShadow|samplerCubeShadow|sampler1DArray|sampler2DArray|sampler1DArrayShadow|sampler2DArrayShadow|isampler1D|isampler2D|isampler3D|isamplerCube|isampler1DArray|isampler2DArray|usampler1D|usampler2D|usampler3D|usamplerCube|usampler1DArray|usampler2DArray|sampler2DRect|sampler2DRectShadow|isampler2DRect|usampler2DRect|samplerBuffer|isamplerBuffer|usamplerBuffer|sampler2DMS|isampler2DMS|usampler2DMS|sampler2DMSArray|isampler2DMSArray|usampler2DMSArray|samplerCubeArray|samplerCubeArrayShadow|isamplerCubeArray|usamplerCubeArray|image1D|iimage1D|uimage1D|image2D|iimage2D|uimage2D|image3D|iimage3D|uimage3D|image2DRect|iimage2DRect|uimage2DRect|imageCube|iimageCube|uimageCube|imageBuffer|iimageBuffer|uimageBuffer|image1DArray|iimage1DArray|uimage1DArray|image2DArray|iimage2DArray|uimage2DArray|imageCubeArray|iimageCubeArray|uimageCubeArray|image2DMS|iimage2DMS|uimage2DMS|image2DMSArray|iimage2DMSArray|uimage2DMSArray|struct)\b");

            //comment highlighting
            //e.SetStyle(CommentStyle, @"//.*$", RegexOptions.Multiline);
            //e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(/\*.*)", RegexOptions.Singleline);
            //e.SetStyle(CommentStyle, @"(/\*.*?\*/)|(.*\*/)", RegexOptions.Singleline | RegexOptions.RightToLeft);

            //number highlighting
            e.SetStyle(NumberStyle, NumberRegex);

            //attribute highlighting
            //e.SetStyle(AttributeStyle, @"^\s*(?<range>\[.+?\])\s*$", RegexOptions.Multiline);
            //e.SetStyle(PreprocessorStyle, @"^#(version|pragma|include)\b");

            //element name highlighting
            e.SetStyle(ClassNameStyle, @"(?<=<\s*\/?\s*)[a-zA-Z][a-zA-Z0-9.\-_:]+(?=\s*\/*>*)");

            //clear folding markers
            e.ClearFoldingMarkers();

            //set folding markers
            e.SetFoldingMarkers("<!--", "-->");
            //e.SetFoldingMarkers(
            //    "<\\s*[a-zA-Z][a-zA-Z0-9-.:_\\s\\\"=]*>",
            //    "<\\s*\\/\\s*[a-zA-Z][a-zA-Z0-9-.:_\\s\\\" =]*> ",
            //    RegexOptions.Multiline);
        }
        //private void TextBox_AutoIndentNeeded_XML(object sender, AutoIndentEventArgs e)
        //{
        //    string line = e.LineText.Trim();
        //    if (line.EndsWith("{"))
        //    {
        //        e.ShiftNextLines = e.TabLength;
        //        return;
        //    }
        //    else if (line.EndsWith("}"))
        //    {
        //        e.ShiftNextLines = -e.TabLength;
        //        return;
        //    }
        //}
        #endregion

        #region JSON Init
        private void InitJSON()
        {
            
        }
        private void UnInitJSON()
        {

        }
        #endregion

        void popupMenu_Opening(object sender, CancelEventArgs e)
        {
            //---block autocomplete menu for comments
            //get index of green style (used for comments)
            var iGreenStyle = TextBox.GetStyleIndex(CommentStyle);
            if (iGreenStyle >= 0 && TextBox.Selection.Start.iChar > 0)
            {
                //current char (before caret)
                var c = TextBox[TextBox.Selection.Start.iLine][TextBox.Selection.Start.iChar - 1];
                //green Style
                var greenStyleIndex = Range.ToStyleIndex(iGreenStyle);
                //if char contains green style then block popup menu
                if ((c.style & greenStyleIndex) != 0)
                {
                    e.Cancel = true;
                    return;
                }
            }
            //TODO: build menu when we can actually recognize the type in the selection
            //BuildAutocompleteMenu();
        }
        private void BuildAutocompleteMenu()
        {
            //TODO: use selection location to determine type to build relevant matches for
            List<AutocompleteItem> items = new List<AutocompleteItem>();

            foreach (var item in CSharpSnippets)
                items.Add(new SnippetAutocompleteItem(item) { ImageIndex = 1 });
            foreach (var item in CSharpDecSnippets)
                items.Add(new DeclarationSnippet(item) { ImageIndex = 0 });
            foreach (var item in CSharpObjectMethods)
                items.Add(new MethodAutocompleteItem(item) { ImageIndex = 2 });
            foreach (var item in CSharpKeywords)
                items.Add(new AutocompleteItem(item));

            items.Add(new InsertSpaceSnippet());
            items.Add(new InsertSpaceSnippet(@"^(\w+)([=<>!:]+)(\w+)$"));
            items.Add(new InsertEnterSnippet());

            //set as autocomplete source
            AutoCompleteMenu.Items.SetAutocompleteItems(items);
        }
        private void TextBox_MouseMove(object sender, MouseEventArgs e)
        {
            //var place = TextBox.PointToPlace(e.Location);
            //var range = new Range(TextBox, place, place);
            //Range r = HoveredWordRange;
            //HoveredWordRange = null;
            ////if (r != null && r.Length > 0)
            ////    HighlightScriptSyntax?.Invoke(r);
            ////else
            //    HighlightScriptSyntax?.Invoke(TextBox.VisibleRange);
            //HoveredWordRange = range.GetFragment("[a-zA-Z]");
            //if (HoveredWordRange != null && HoveredWordRange.Length > 0)
            //{
            //    HighlightScriptSyntax?.Invoke(HoveredWordRange);
            //    lblHoveredWord.Text = HoveredWordRange.Text;
            //}
            //else
            //{
            //    HighlightScriptSyntax?.Invoke(TextBox.VisibleRange);
            //    lblHoveredWord.Text = null;
            //}
        }
        //private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        //{
        //    TextBox.Selection = HoveredWordRange;
        //}
        private DateTime _lastClickedTime;
        private DateTime _lastDoubleClickedTime;
        private void TextBox_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left)
            //{
            //    int ms = SystemInformation.DoubleClickTime;
            //    DateTime now = DateTime.Now;
            //    if ((now - _lastDoubleClickedTime).TotalMilliseconds <= ms)
            //    {
            //        SelectHoveredLine();
            //        //Engine.PrintLine("Triple Clicked");
            //    }
            //    else if ((now - _lastClickedTime).TotalMilliseconds <= ms)
            //    {
            //        SelectHoveredWord();
            //        _lastDoubleClickedTime = now;
            //        //Engine.PrintLine("Double Clicked");
            //    }
            //    //else
            //    //    Engine.PrintLine("Clicked");
            //    _lastClickedTime = now;
            //}
        }
        //private void SelectHoveredWord()
        //{
        //    if (HoveredWordRange != null)
        //        TextBox.Selection = HoveredWordRange;
        //}
        //private void SelectHoveredLine()
        //{
        //    if (HoveredWordRange is null)
        //        return;
            
        //    Range line = HoveredWordRange.Clone();
        //    line.Expand();
        //    TextBox.Selection = line;
        //}

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Modifiers == Keys.Control && e.KeyCode == Keys.OemMinus)
            //{
            //    NavigateBackward();
            //    e.Handled = true;
            //}
            //if (e.Modifiers == (Keys.Control | Keys.Shift) && e.KeyCode == Keys.OemMinus)
            //{
            //    NavigateForward();
            //    e.Handled = true;
            //}
            if (e.KeyData == (Keys.K | Keys.Control))
            {
                //forced show (MinFragmentLength will be ignored)
                AutoCompleteMenu.Show(true);
                e.Handled = true;
            }
        }
        private void TextBox_SelectionChangedDelayed(object sender, EventArgs e)
        {
            TextBox.VisibleRange.ClearStyle(SameWordsStyle);
            if (!TextBox.Selection.IsEmpty)
                return;
            
            var fragment = TextBox.Selection.GetFragment(@"\w"); //Get selected word
            string text = fragment.Text;
            if (text.Length == 0 || 
                (CurrentKeywords != null && CurrentKeywords.Contains(text, StringComparison.InvariantCulture)) ||
                IsInString(fragment))
                return;
            
            Range[] ranges = TextBox.VisibleRange.GetRanges("\\b" + text + "\\b").ToArray();
            
            foreach (var range in ranges)
                range.SetStyle(SameWordsStyle);
        }

        private bool IsInString(Range fragment)
        {
            Range strRange = fragment.GetFragment(StringRegex);
            return strRange != null && strRange.GetUnionWith(fragment).TextLength == fragment.TextLength;
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            HighlightRange(e.ChangedRange);
        }
        private (bool Success, string Output) CompileGLSL()
        {
            if (_glslShader is null)
                return (false, null);

            string ext = string.IsNullOrEmpty(_targetFile.FilePath) ? string.Empty : Path.GetExtension(_targetFile.FilePath).Substring(1);
            EGLSLType mode = _glslShader.ShaderMode;
            switch (ext)
            {
                case "fs":
                case "frag":
                    mode = EGLSLType.Fragment;
                    break;
                case "vs":
                case "vert":
                    mode = EGLSLType.Vertex;
                    break;
                case "gs":
                case "geom":
                    mode = EGLSLType.Geometry;
                    break;
                case "tcs":
                case "tesc":
                    mode = EGLSLType.TessControl;
                    break;
                case "tes":
                case "tese":
                    mode = EGLSLType.TessEvaluation;
                    break;
                case "cs":
                case "comp":
                    mode = EGLSLType.Compute;
                    break;
            }
            _glslShader.SetSource(TextBox.Text, mode, false);
            bool success = _glslShader.Compile(out string info, false);
            return (success, info);
        }
        private void TextBox_TextChangedDelayed(object sender, TextChangedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem((o) => ReBuildObjectExplorer(TextBox.Text));

            if (Mode == ETextEditorMode.GLSL)
            {
                var (Success, Output) = CompileGLSL();
                if (!Success)
                {
                    Engine.PrintLine(Output);
                }
            }

            //if (Mode == ETextEditorMode.GLSL)
            //{
            //    var (Success, Output) = CompileGLSL();
            //    if (!Success)
            //    {
            //        Engine.PrintLine(Output);

            //        //int[] errorLines = output.FindAllOccurrences(0, ") : ");
            //        //foreach (int i in errorLines)
            //        //{
            //        //    int start = output.FindFirstReverse(i - 1, '(');
            //        //    int lineIndex = int.Parse(output.Substring(start + 1, i - start - 1)) - 1;
            //        //    if (lineIndex >= 0 && lineIndex < TextBox.LinesCount)
            //        //    {
            //        //        Line lineInfo = TextBox[lineIndex];
            //        //        _errorLines.Add(lineInfo);
            //        //        lineInfo.BackgroundBrush = _errorBrush;

            //        //        string line = lineInfo.Text;
            //        //        int errorStart = i + 4;//errors.FindFirst(i + 3, ':') + 2;
            //        //        int errorEnd = output.FindFirst(errorStart, "\n");
            //        //        string errorMsg = output.Substring(errorStart, errorEnd - errorStart);

            //        //        lineInfo.AddRange((" // " + errorMsg).Select(x => new FastColoredTextBoxNS.Char(x)));
            //        //    }
            //        //    //Match m = Regex.Match(errorMsg, "(?<= \").*(?=\")");
            //        //    //int tokenStart = line.IndexOf(m.Value);
            //        //    //if (tokenStart < 0)
            //        //    //    continue;
            //        //    //Place px;
            //        //    //for (int x = 0; x < m.Length; ++x)
            //        //    //{
            //        //    //    px = new Place(tokenStart + x, lineIndex);
            //        //    //    FastColoredTextBoxNS.Char cx = TextBox[px];
            //        //    //    cx.style = TextBox.GetStyleIndexMask(new Style[] { _errorStyle });
            //        //    //    TextBox[px] = cx;
            //        //    //}
            //        //    //0(line#) : error CXXXX: <message>
            //        //    //at token "<token>"
            //        //}
            //    }
            //}
        }
        public void HighlightRange(Range range)
        {
            HighlightInvisibleChars(range);
            HighlightScriptSyntax?.Invoke(range);
        }

        private delegate void DelHighlightScriptSyntax(Range range);
        private DelHighlightScriptSyntax HighlightScriptSyntax;

        private void HighlightInvisibleChars(Range range)
        {
            range.ClearStyle(InvisibleCharsStyle);
            if (btnShowInvisibleCharacters.Checked)
                range.SetStyle(InvisibleCharsStyle, @".$|.\r\n|\s");
        }

        #region Item Explorer
        private List<ExplorerItem> _explorerList = new List<ExplorerItem>();
        private void ReBuildObjectExplorer(string text)
        {
            try
            {
                List<ExplorerItem> list = new List<ExplorerItem>();
                int lastClassIndex = -1;
                //find classes, methods and properties
                Regex regex = new Regex(@"^(?<range>[\w\s]+\b(class|struct|enum|interface)\s+[\w<>,\s]+)|^\s*(public|private|internal|protected)[^\n]+(\n?\s*{|;)?", RegexOptions.Multiline);
                foreach (Match r in regex.Matches(text))
                {
                    try
                    {
                        string s = r.Value;
                        int i = s.IndexOfAny(new char[] { '=', '{', ';' });
                        if (i >= 0)
                            s = s.Substring(0, i);
                        s = s.Trim();

                        var item = new ExplorerItem()
                        {
                            Title = s,
                            Position = r.Index,
                            Length = r.Length,
                        };

                        if (Regex.IsMatch(item.Title, @"\b(class|struct|enum|interface)\b"))
                        {
                            item.Title = item.Title.Substring(item.Title.LastIndexOf(' ')).Trim();
                            item.Type = EExplorerItemType.Class;
                            list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());
                            lastClassIndex = list.Count;
                        }
                        else if (item.Title.Contains(" event "))
                        {
                            int ii = item.Title.LastIndexOf(' ');
                            item.Title = item.Title.Substring(ii).Trim();
                            item.Type = EExplorerItemType.Event;
                        }
                        else if (item.Title.Contains("("))
                        {
                            var parts = item.Title.Split('(');
                            item.Title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "(" + parts[1];
                            item.Type = EExplorerItemType.Method;
                        }
                        else if (item.Title.EndsWith("]"))
                        {
                            var parts = item.Title.Split('[');
                            if (parts.Length < 2) continue;
                            item.Title = parts[0].Substring(parts[0].LastIndexOf(' ')).Trim() + "[" + parts[1];
                            item.Type = EExplorerItemType.Method;
                        }
                        else
                        {
                            int ii = item.Title.LastIndexOf(' ');
                            item.Title = item.Title.Substring(ii).Trim();
                            item.Type = EExplorerItemType.Property;
                        }
                        list.Add(item);
                    }
                    catch { }
                }

                list.Sort(lastClassIndex + 1, list.Count - (lastClassIndex + 1), new ExplorerItemComparer());

                BeginInvoke(
                    new Action(() =>
                    {
                        _explorerList = list;
                        dgvObjectExplorer.RowCount = _explorerList.Count;
                        dgvObjectExplorer.Invalidate();
                    })
                );
            }
            catch { }
        }
        private enum EExplorerItemType
        {
            Class,
            Method,
            Property,
            Event
        }
        private class ExplorerItem
        {
            public EExplorerItemType Type { get; set; }
            public string Title { get; set; }
            public int Position { get; set; }
            public int Length { get; set; }
        }
        private class ExplorerItemComparer : IComparer<ExplorerItem>
        {
            public int Compare(ExplorerItem x, ExplorerItem y)
                => x.Title.CompareTo(y.Title);
        }
        private void dgvObjectExplorer_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (TextBox != null)
            {
                var item = _explorerList[e.RowIndex];
                TextBox.GoEnd();
                TextBox.SelectionStart = item.Position;
                TextBox.SelectionLength = item.Length;
                TextBox.DoSelectionVisible();
                TextBox.Focus();
            }
        }
        private void dgvObjectExplorer_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                ExplorerItem item = _explorerList[e.RowIndex];
                if (e.ColumnIndex == 1)
                    e.Value = item.Title;
                else
                    switch (item.Type)
                    {
                        case EExplorerItemType.Class:
                            e.Value = ClassImage;
                            return;
                        case EExplorerItemType.Method:
                            e.Value = MethodImage;
                            return;
                        case EExplorerItemType.Event:
                            e.Value = EventImage;
                            return;
                        case EExplorerItemType.Property:
                            e.Value = PropertyImage;
                            return;
                    }
            }
            catch { }
        }
        #endregion

        private void btnCut_Click(object sender, EventArgs e) => TextBox.Cut();
        private void btnCopy_Click(object sender, EventArgs e) => TextBox.Copy();
        private void btnPaste_Click(object sender, EventArgs e) => TextBox.Paste();
        private void btnSelectAll_Click(object sender, EventArgs e) => TextBox.Selection.SelectAll();
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TextBox.UndoEnabled)
                TextBox.Undo();
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TextBox.RedoEnabled)
                TextBox.Redo();
        }

        private void tmUpdateInterface_Tick(object sender, EventArgs e)
        {
            try
            {
                //if (TextBox != null && tsFiles.Items.Count > 0)
                //{
                //    var tb = TextBox;
                //    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = tb.UndoEnabled;
                //    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = tb.RedoEnabled;
                //    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = tb.IsChanged;
                //    saveAsToolStripMenuItem.Enabled = true;
                //    pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = true;
                //    cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                //    copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = !tb.Selection.IsEmpty;
                //    printToolStripButton.Enabled = true;
                //}
                //else
                //{
                //    saveToolStripButton.Enabled = saveToolStripMenuItem.Enabled = false;
                //    saveAsToolStripMenuItem.Enabled = false;
                //    cutToolStripButton.Enabled = cutToolStripMenuItem.Enabled =
                //    copyToolStripButton.Enabled = copyToolStripMenuItem.Enabled = false;
                //    pasteToolStripButton.Enabled = pasteToolStripMenuItem.Enabled = false;
                //    printToolStripButton.Enabled = false;
                //    undoStripButton.Enabled = undoToolStripMenuItem.Enabled = false;
                //    redoStripButton.Enabled = redoToolStripMenuItem.Enabled = false;
                //    dgvObjectExplorer.RowCount = 0;
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        
        private bool _tbFindChanged = false;
        private void tbFind_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' && TextBox != null)
            {
                Range range = _tbFindChanged ? TextBox.Range.Clone() : TextBox.Selection.Clone();
                _tbFindChanged = false;
                range.End = new Place(TextBox[TextBox.LinesCount - 1].Count, TextBox.LinesCount - 1);
                var pattern = Regex.Escape(tbFind.Text);
                foreach (var found in range.GetRanges(pattern))
                {
                    found.Inverse();
                    TextBox.Selection = found;
                    TextBox.DoSelectionVisible();
                    return;
                }
                MessageBox.Show("Not found.");
            }
            else
            {
                _tbFindChanged = true;

            }
        }
        private MarkerStyle SearchMatchStyle { get; } = new MarkerStyle(new SolidBrush(Color.FromArgb(120, Color.Orange)));
        private IEnumerable<Range> _searchMatches = null;
        private void tbFind_TextChanged(object sender, EventArgs e)
        {
            if (_searchMatches != null)
                foreach (var match in _searchMatches)
                    match.ClearStyle(SearchMatchStyle);

            if (string.IsNullOrEmpty(tbFind.Text))
            {
                _searchMatches = null;
                return;
            }

            var pattern = Regex.Escape(tbFind.Text);
            _searchMatches = TextBox.GetRanges(pattern);

            if (_searchMatches != null)
                foreach (var match in _searchMatches)
                    match.SetStyle(SearchMatchStyle);
        }

        private static Bitmap ClassImage;
        private static Bitmap MethodImage;
        private static Bitmap EventImage;
        private static Bitmap PropertyImage;
        private static Bitmap FieldImage;
        private static readonly int WidthHeight = 16;
        static DockableTextEditor()
        {
            LoadImages();
        }
        private static void LoadImages()
        {
            var classSVG = SvgDocument.Open(Engine.Files.TexturePath("Class.svg"));
            var methodSVG = SvgDocument.Open(Engine.Files.TexturePath("Method.svg"));
            var eventSVG = SvgDocument.Open(Engine.Files.TexturePath("Event.svg"));
            var propertySVG = SvgDocument.Open(Engine.Files.TexturePath("Property.svg"));
            var fieldSVG = SvgDocument.Open(Engine.Files.TexturePath("Field.svg"));

            ClassImage = classSVG.Draw(WidthHeight, WidthHeight);
            MethodImage = methodSVG.Draw(WidthHeight, WidthHeight);
            EventImage = eventSVG.Draw(WidthHeight, WidthHeight);
            PropertyImage = propertySVG.Draw(WidthHeight, WidthHeight);
            FieldImage = fieldSVG.Draw(WidthHeight, WidthHeight);
        }
        
        /// <summary>
        /// This item appears when any part of snippet text is typed
        /// </summary>
        private class DeclarationSnippet : SnippetAutocompleteItem
        {
            public DeclarationSnippet(string snippet) : base(snippet) { }

            public override CompareResult Compare(string fragmentText)
            {
                var pattern = Regex.Escape(fragmentText);
                if (Regex.IsMatch(Text, "\\b" + pattern, RegexOptions.IgnoreCase))
                    return CompareResult.Visible;
                return CompareResult.Hidden;
            }
        }

        /// <summary>
        /// Divides numbers and words: "123AND456" -> "123 AND 456"
        /// Or "i=2" -> "i = 2"
        /// </summary>
        private class InsertSpaceSnippet : AutocompleteItem
        {
            private string Pattern { get; set; }

            public InsertSpaceSnippet(string pattern)
                : base("") => Pattern = pattern;
            
            public InsertSpaceSnippet()
                : this(@"^(\d+)([a-zA-Z_]+)(\d*)$") { }

            public override CompareResult Compare(string fragmentText)
            {
                if (Regex.IsMatch(fragmentText, Pattern))
                {
                    Text = InsertSpaces(fragmentText);
                    if (Text != fragmentText)
                        return CompareResult.Visible;
                }
                return CompareResult.Hidden;
            }

            public string InsertSpaces(string fragment)
            {
                var m = Regex.Match(fragment, Pattern);
                if (m is null)
                    return fragment;
                if (m.Groups[1].Value == "" && m.Groups[3].Value == "")
                    return fragment;
                return (m.Groups[1].Value + " " + m.Groups[2].Value + " " + m.Groups[3].Value).Trim();
            }

            public override string ToolTipTitle => Text;
        }

        /// <summary>
        /// Inerts line break after '}'
        /// </summary>
        private class InsertEnterSnippet : AutocompleteItem
        {
            Place EnterPlace = Place.Empty;

            public InsertEnterSnippet()
                : base("[Line break]") { }

            public override CompareResult Compare(string fragmentText)
            {
                var r = Parent.Fragment.Clone();
                while (r.Start.iChar > 0)
                {
                    if (r.CharBeforeStart == '}')
                    {
                        EnterPlace = r.Start;
                        return CompareResult.Visible;
                    }

                    r.GoLeftThroughFolded();
                }

                return CompareResult.Hidden;
            }

            public override string GetTextForReplace()
            {
                //extend range
                Range r = Parent.Fragment;
                Place end = r.End;
                r.Start = EnterPlace;
                r.End = r.End;
                //insert line break
                return Environment.NewLine + r.Text;
            }

            public override void OnSelected(AutocompleteMenu popupMenu, SelectedEventArgs e)
            {
                base.OnSelected(popupMenu, e);
                if (Parent.Fragment.tb.AutoIndent)
                    Parent.Fragment.tb.DoAutoIndent();
            }

            public override string ToolTipTitle
            {
                get
                {
                    return "Insert line break after '}'";
                }
            }
        }

        private void btnAutoIndentSelectedLines_Click(object sender, EventArgs e) => TextBox.DoAutoIndent();
        private void btHighlightCurrentLine_Click(object sender, EventArgs e)
        {
            //btnHighlightCurrentLine.Checked = !btnHighlightCurrentLine.Checked;
            //if (btnHighlightCurrentLine.Checked)
            //    TextBox.CurrentLineColor = CurrentLineColor;
            //else
            //    TextBox.CurrentLineColor = Color.Transparent;
            //TextBox.Invalidate();
        }
        //private void cloneLinesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //expand selection
        //    TextBox.Selection.Expand();
        //    //get text of selected lines
        //    string text = Environment.NewLine + TextBox.Selection.Text;
        //    //move caret to end of selected lines
        //    TextBox.Selection.Start = TextBox.Selection.End;
        //    //insert text
        //    TextBox.InsertText(text);
        //}
        //private void cloneLinesAndCommentToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    //start autoUndo block
        //    TextBox.BeginAutoUndo();
        //    //expand selection
        //    TextBox.Selection.Expand();
        //    //get text of selected lines
        //    string text = Environment.NewLine + TextBox.Selection.Text;
        //    //comment lines
        //    TextBox.InsertLinePrefix(TextBox.CommentPrefix);
        //    //move caret to end of selected lines
        //    TextBox.Selection.Start = TextBox.Selection.End;
        //    //insert text
        //    TextBox.InsertText(text);
        //    //end of autoUndo block
        //    TextBox.EndAutoUndo();
        //}

        private void bookmarkPlusButton_Click(object sender, EventArgs e)
        {
            if (TextBox is null)
                return;
            TextBox.BookmarkLine(TextBox.Selection.Start.iLine);
        }
        private void bookmarkMinusButton_Click(object sender, EventArgs e)
        {
            if (TextBox is null)
                return;
            TextBox.UnbookmarkLine(TextBox.Selection.Start.iLine);
        }

        //private void gotoButton_DropDownOpening(object sender, EventArgs e)
        //{
        //    btnGoto.DropDownItems.Clear();
        //    FastColoredTextBox tb = TextBox;
        //    foreach (var bookmark in tb.Bookmarks)
        //    {
        //        var item = btnGoto.DropDownItems.Add(bookmark.Name + " [" + Path.GetFileNameWithoutExtension(tab.Tag as String) + "]");
        //        item.Tag = bookmark;
        //        item.Click += (o, a) =>
        //        {
        //            var b = (Bookmark)(o as ToolStripItem).Tag;
        //            try
        //            {
        //                TextBox = b.TB;
        //            }
        //            catch (Exception ex)
        //            {
        //                Engine.LogException(ex);
        //                return;
        //            }
        //            b.DoVisible();
        //        };
        //    }
        //}
        //private void btnZoom_Click(object sender, EventArgs e)
        //{
        //    TextBox.Zoom = int.Parse((sender as ToolStripItem).Tag.ToString());
        //}

        private void btnShowInvisibleCharacters_Click(object sender, EventArgs e)
        {
            btnShowInvisibleCharacters.Checked = !btnShowInvisibleCharacters.Checked;
            HighlightInvisibleChars(TextBox.Range);
            TextBox.Invalidate();
        }
        private void btnShowFoldingLines_Click(object sender, EventArgs e)
        {
            btnShowFoldingLines.Checked = !btnShowFoldingLines.Checked;
            TextBox.ShowFoldingLines = btnShowFoldingLines.Checked;
            TextBox.Invalidate();
        }

        public class SelectionStyle : Style
        {
            Pen Pen { get; set; }

            public SelectionStyle(Pen pen)
            {
                Pen = pen;
            }

            public override void Draw(Graphics graphics, Point position, Range range)
            {
                var tb = range.tb;
                graphics.DrawRectangle(Pen, new Rectangle(position, new Size(range.TextLength * tb.CharWidth, tb.CharHeight)));
            }
        }

        public class InvisibleCharsRenderer : Style
        {
            Pen Pen { get; set; }

            public InvisibleCharsRenderer(Pen pen)
            {
                Pen = pen;
            }

            public override void Draw(Graphics graphics, Point position, Range range)
            {
                var tb = range.tb;
                using (Brush brush = new SolidBrush(Pen.Color))
                {
                    foreach (var place in range)
                    {
                        switch (tb[place].c)
                        {
                            case ' ':
                                var point = tb.PlaceToPoint(place);
                                point.Offset(tb.CharWidth / 2, tb.CharHeight / 2);
                                graphics.DrawLine(Pen, point.X, point.Y, point.X + 1, point.Y);
                                break;
                        }

                        if (tb[place.iLine].Count - 1 == place.iChar)
                        {
                            var point = tb.PlaceToPoint(place);
                            point.Offset(tb.CharWidth, 0);
                            graphics.DrawString("¶", tb.Font, brush, point);
                        }
                    }
                }
            }
        }

        public string GetText() => TextBox.Text;

        public delegate void DelTextSavedHandler(DockableTextEditor editor, string targetPath);
        public event DelTextSavedHandler SavedOverride;

        private bool _updating = false;
        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
                Mode = (ETextEditorMode)cboMode.SelectedIndex;
        }

        #region Click
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
                fd.Apply += FontDialog_Apply;
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

        private void FontDialog_Apply(object sender, EventArgs e)
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
                Filter = TFileObject.GetFilter<TextFile>(true, true, false, false) + "|All files (*.*)|*.*",
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
        private void btnSave_Click(object sender, EventArgs e) => Save();
        private void btnSaveAs_Click(object sender, EventArgs e) => SaveAs();
        public void SaveAs()
        {
            if (TargetFile is null)
                return;

            string filter = TargetFile.GetFilter(true, true, false, false);
            using (SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = filter + "|All files (*.*)|*.*",
            })
            {
                if (sfd.ShowDialog(this) == DialogResult.OK)
                    SaveToPath(sfd.FileName);
            }
        }
        public void Save()
        {
            if (TargetFile is null)
                return;

            if (TextBox.IsChanged)
            {
                if (SavedOverride != null)
                {
                    SavedOverride(this, null);
                    TextBox.IsChanged = false;
                }
                else
                {
                    TargetFile.Text = GetText();
                    if (!string.IsNullOrWhiteSpace(TargetFile.FilePath))
                        SaveToPath(TargetFile.FilePath);
                    else
                        SaveAs();
                }
            }
        }
        public async void SaveToPath(string path)
        {
            TextBox.CloseBindingFile();

            string beginMessage = $"Text Editor: Now saving {path}";
            string finishMessage = $"Text Editor: Finished saving {path}";
            if (RemotingServices.IsTransparentProxy(TargetFile))
            {
                Editor.DomainProxy.SaveFileAs(
                    TargetFile, path,
                    beginMessage,
                    finishMessage);
            }
            else
            {
                await TargetFile.ExportAsync(path);
            }

            TextBox.OpenBindingFile(path, TargetFile.Encoding);

            TextBox.OnTextChanged();
            //TextBox.OnSyntaxHighlight(new TextChangedEventArgs(TextBox.Range));
            TextBox.IsChanged = false;
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
        private void btnCommentSelectedLines_Click(object sender, EventArgs e)
        {
            TextBox.InsertLinePrefix(TextBox.CommentPrefix);
        }
        private void btnUncommentSelectedLines_Click(object sender, EventArgs e)
        {
            TextBox.RemoveLinePrefix(TextBox.CommentPrefix);
        }
        private void btnCollapseAllFoldingBlocks_Click(object sender, EventArgs e)
        {
            TextBox.CollapseAllFoldingBlocks();
            TextBox.DoSelectionVisible();
        }
        private void btnExpandAllFoldingBlocks_Click(object sender, EventArgs e)
        {
            TextBox.ExpandAllFoldingBlocks();
            TextBox.DoSelectionVisible();
        }
        private void btnExpandAllRegionBlocks_Click(object sender, EventArgs e)
        {
            if (Mode != ETextEditorMode.CSharp)
                return;

            for (int iLine = 0; iLine < TextBox.LinesCount; iLine++)
                if (TextBox[iLine].FoldingStartMarker == @"#region\b")
                    TextBox.ExpandFoldedBlock(iLine);
        }
        private void btnCollapseAllRegionBlocks_Click(object sender, EventArgs e)
        {
            if (Mode != ETextEditorMode.CSharp)
                return;

            for (int iLine = 0; iLine < TextBox.LinesCount; iLine++)
                if (TextBox[iLine].FoldingStartMarker == @"#region\b")
                    TextBox.CollapseFoldingBlock(iLine);
        }
        private void btnRemoveEmptyLines_Click(object sender, EventArgs e)
        {
            var iLines = TextBox.FindLines(@"^\s*$", RegexOptions.None);
            TextBox.RemoveLines(iLines);
        }
        private void btnFind_Click(object sender, EventArgs e)
        {
            if (TextBox.Selection.TextLength > 0)
                TextBox.ShowFindDialog(TextBox.Selection.Text);
            else
                TextBox.ShowFindDialog();
        }
        private void btnReplace_Click(object sender, EventArgs e)
        {
            if (TextBox.Selection.TextLength > 0)
                TextBox.ShowReplaceDialog(TextBox.Selection.Text);
            else
                TextBox.ShowReplaceDialog();
        }
        private void btnGoto_Click(object sender, EventArgs e)
        {
            TextBox.ShowGoToDialog();
        }
        private void btnHotkeys_Click(object sender, EventArgs e)
        {
            var form = new HotkeysEditorForm(TextBox.HotkeysMapping);
            if (form.ShowDialog() == DialogResult.OK)
                TextBox.HotkeysMapping = form.GetHotkeys();
        }
        private void btnStartStopRecording_Click(object sender, EventArgs e)
        {
            TextBox.MacrosManager.IsRecording = !TextBox.MacrosManager.IsRecording;
            //btnStartStopRecording.Text = TextBox.MacrosManager.IsRecording ? "Stop Recording" : "Start Recording";
        }
        private void btnSaveRecordedMacros_Click(object sender, EventArgs e)
        {
            string macros = TextBox.MacrosManager.Macros;
            if (!string.IsNullOrWhiteSpace(macros))
            {
                TextFile file = macros;
                //SaveFileDialog sfd = new SaveFileDialog();
                //file.ExportAsync("");
            }
        }
        private void btnLoadMacros_Click(object sender, EventArgs e)
        {
            //OpenFileDialog ofd = new OpenFileDialog();
            //Add ToolStripButton with tag as macro text
        }
        private void btnClearMacros_Click(object sender, EventArgs e)
        {
            TextBox.MacrosManager.ClearMacros();
        }
        private void btnExecuteMacros_Click(object sender, EventArgs e)
        {
            ToolStripButton btn = sender as ToolStripButton;
            string macros = btn.Tag as string;
            TextBox.MacrosManager.Macros = macros;
            TextBox.MacrosManager.ExecuteMacros();
            TextBox.MacrosManager.Macros = null;
        }
        private void btnGoToDef_Click(object sender, EventArgs e)
        {

        }
        private void btnGoToInf_Click(object sender, EventArgs e)
        {

        }
        private void lblSplitFileObjects_Click(object sender, EventArgs e)
        {
            dgvObjectExplorer.Visible = !dgvObjectExplorer.Visible;
            lblSplitFileObjects.Text = dgvObjectExplorer.Visible ? "<" : ">";
        }
        private void btnFindAllRefs_Click(object sender, EventArgs e)
        {

        }
        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            if (TextBox.IsChanged)
            {
                DialogResult result = MessageBox.Show(this, 
                    "Do you want to save your changes before closing?", 
                    "Save changes?",
                    MessageBoxButtons.YesNoCancel, 
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button3);

                e.Cancel = result == DialogResult.Cancel;
                if (result == DialogResult.Yes)
                    Save();
            }
            base.OnClosing(e);
        }
        protected override void OnClosed(EventArgs e)
        {
            TextBox.CloseBindingFile();
            if (!string.IsNullOrWhiteSpace(TargetFile?.FilePath))
                TextEditorInstances.Remove(TargetFile.FilePath);
            base.OnClosed(e);
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool processed = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == Keys.Insert)
                TextBox_SelectionChanged(null, null);
            return processed;
        }

        //private Range _currentLine;
        //private SelectionStyle SelStyle = new SelectionStyle(new Pen(new SolidBrush(Color.Gray), 1.2f));
        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            bool insert = IsKeyLocked(Keys.Insert);
            Range newSelection = TextBox.Selection;
            lblStatusText.Text = string.Format("Ln {0} Col {1} {2}", 
                newSelection.Start.iLine + 1, newSelection.Start.iChar + 1, insert ? "OVR" : "INS");
            
            //_currentLine?.ClearStyle(SelStyle);
            //if (newSelection.Start.iLine == newSelection.End.iLine)
            //    newSelection.SetStyle(SelStyle);
            //_currentLine = newSelection;
        }
        
        private void lblSplitFileObjects_MouseEnter(object sender, EventArgs e)
        {
            lblSplitFileObjects.BackColor = Color.FromArgb(40, 40, 50);
        }
        private void lblSplitFileObjects_MouseLeave(object sender, EventArgs e)
        {
            lblSplitFileObjects.BackColor = Color.FromArgb(30, 30, 40);
        }
    }
}