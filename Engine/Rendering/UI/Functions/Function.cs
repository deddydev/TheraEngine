using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Models.Materials.Functions;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Functions
{
    public enum EGLSLVersion
    {
        Invalid = -1,

        /// <summary>
        /// OpenGL 2.0 April 2004
        /// </summary>
        Ver_110,
        /// <summary>
        /// OpenGL 2.1 September 2006
        /// </summary>
        Ver_120,
        /// <summary>
        /// OpenGL 3.0 August 2008
        /// </summary>
        Ver_130,
        /// <summary>
        /// OpenGL 3.1 March 2009
        /// </summary>
        Ver_140,
        /// <summary>
        /// OpenGL 3.2 August 2009
        /// </summary>
        Ver_150,
        /// <summary>
        /// OpenGL 3.3 February 2010
        /// </summary>
        Ver_330,
        /// <summary>
        /// OpenGL 4.0 March 2010
        /// </summary>
        Ver_400,
        /// <summary>
        /// OpenGL 4.1 July 2010
        /// </summary>
        Ver_410,
        /// <summary>
        /// OpenGL 4.2 August 2011
        /// </summary>
        Ver_420,
        /// <summary>
        /// OpenGL 4.3 August 2012
        /// </summary>
        Ver_430,
        /// <summary>
        /// OpenGL 4.4 July 2013
        /// </summary>
        Ver_440,
        /// <summary>
        /// OpenGL 4.5 August 2014
        /// </summary>
        Ver_450,
        /// <summary>
        /// OpenGL 4.6 July 2017
        /// </summary>
        Ver_460,
    }
    public class FunctionDefinition : Attribute
    {
        public FunctionDefinition(
            string category,
            string name,
            string description,
            string keywords,
            int operationCount = 1)
        {
            Keywords = keywords.Split(' ').ToList();
            Description = description;
            Name = name;
            Category = category;
            OperationCount = operationCount;
        }

        public List<string> Keywords { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
        public int OperationCount { get; }
    }
    public interface IFunction : IUIComponent
    {

    }
    public abstract class BaseFunction : UIMaterialRectangleComponent, IShaderVarOwner, IFunction
    {
        public static Vec4 RegularColor { get; set; } = new Vec4(0.1f, 0.1f, 0.1f, 1.0f);
        public static Vec4 SelectedColor { get; set; } = new Vec4(0.1f, 0.2f, 0.25f, 1.0f);
        public static Vec4 HighlightedColor { get; set; } = new Vec4(0.1f, 0.3f, 0.4f, 1.0f);
        
        [Browsable(false)]
        public FunctionDefinition Definition => GetType().GetCustomAttribute<FunctionDefinition>();
        [Browsable(false)]
        public ReadOnlyCollection<string> Keywords => Definition?.Keywords.AsReadOnly();
        [Browsable(false)]
        public string FunctionName => Definition?.Name;
        [Browsable(false)]
        public string Description => Definition?.Description;
        [Browsable(false)]
        public string Category => Definition?.Category;

        protected UITextComponent _headerText;
        protected UIString2D _headerString;
        protected List<UITextComponent> _inputParamTexts = new List<UITextComponent>();
        protected List<UITextComponent> _outputParamTexts = new List<UITextComponent>();

        protected const int HeaderPadding = 2;
        protected Font _paramFont = new Font("Segoe UI", 9.0f, FontStyle.Regular);
        protected Font _headerFont = new Font("Segoe UI", 11.0f, FontStyle.Bold);

        public BaseFunction() : base(MakeFunctionMaterial())
        {
            _headerText = new UITextComponent
            {
                Name = FunctionName + " [Header Text]",
                DockStyle = UIDockStyle.Top,
                Height = TextRenderer.MeasureText(FunctionName, _headerFont).Height + HeaderPadding * 2,
            };
            _headerText.TextDrawer.Add(_headerString = new UIString2D()
            {
                Text = FunctionName,
                Font = _headerFont,
                Format = new StringFormat(
                    //StringFormatFlags.FitBlackBox | 
                    StringFormatFlags.NoClip |
                    StringFormatFlags.NoWrap)
            });
            ChildComponents.Add(_headerText);
        }

        private static TMaterial MakeFunctionMaterial()
        {
            return TMaterial.CreateUnlitColorMaterialForward(new ColorF4(0.1f, 1.0f));
        }

        protected void AddParam(BaseFuncArg arg)
        {
            ChildComponents.Add(arg);

            UITextComponent text = new UITextComponent
            {
                Name = arg.Name + " Text",
            };
            text.TextDrawer.Add(new UIString2D()
            {
                Text = arg.Name,
                Font = _paramFont,
                Format = new StringFormat(
                    //StringFormatFlags.FitBlackBox | 
                    StringFormatFlags.NoClip |
                    StringFormatFlags.NoWrap)
            });
            ChildComponents.Add(text);

            if (arg is IFuncExecInput || arg is IFuncValueInput)
            {
                _inputParamTexts.Add(text);
            }
            else if (arg is IFuncExecOutput || arg is IFuncValueOutput)
            {
                _outputParamTexts.Add(text);
            }
        }

        public override string ToString()
            => FunctionName;

        public static List<Type> Find<T>(string keywords) where T : IFunction
        {
            string[] keyArray = keywords.Split(' ');
            Dictionary<int, List<Type>> types = new Dictionary<int, List<Type>>();
            IEnumerable<Type> functions = Assembly.GetExecutingAssembly().GetTypes().
                Where(x => x is T && x.IsDefined(typeof(FunctionDefinition)));
            foreach (Type func in functions)
            {
                if (func.IsAbstract)
                    continue;

                FunctionDefinition def = func.GetCustomAttribute<FunctionDefinition>();

                int count = 0;
                foreach (string keyword in def.Keywords)
                {
                    foreach (string typedKeyword in keyArray)
                        if (keyword.Contains(typedKeyword))
                        {
                            ++count;
                            break;
                        }
                }
                if (count > 0)
                {
                    if (types.ContainsKey(count))
                        types[count].Add(func);
                    else
                        types.Add(count, new List<Type>() { func });
                }
            }
            int maxVal = TMath.Max(types.Keys.ToArray());
            return types[maxVal];
        }
    }
    /// <summary>
    /// Provides a hud component that can connect to other functions with parameters and execution flow.
    /// Use FunctionDefinition attribute to specify information to display on the UI. 
    /// Nothing else is necessary for custom functions to show to users.
    /// </summary>
    /// <typeparam name="TVIn">The input value argument class to use.</typeparam>
    /// <typeparam name="TVOut">The output value class to use.</typeparam>
    /// <typeparam name="TEIn">The input execution argument class to use.</typeparam>
    /// <typeparam name="TEOut">The output execution class to use.</typeparam>
    public abstract class Function<TVIn, TVOut, TEIn, TEOut> : BaseFunction
        where TVIn : BaseFuncValue, IFuncValueInput
        where TVOut : BaseFuncValue, IFuncValueOutput
        where TEIn : BaseFuncExec, IFuncExecInput
        where TEOut : BaseFuncExec, IFuncExecOutput
    {
        public Function(bool deferControlArrangement = false) : base()
        {
            if (!deferControlArrangement)
                AddArguments();
        }

        protected virtual void AddArguments()
        {
            AddExecInput(GetExecInputs());
            AddExecOutput(GetExecOutputs());
            AddValueInput(GetValueInputs());
            AddValueOutput(GetValueOutputs());
            ArrangeControls();
        }

        #region Input/Output Exec
        protected List<TEIn> _execInputs = new List<TEIn>();
        protected List<TEOut> _execOutputs = new List<TEOut>();
        [Browsable(false)]
        public List<TEIn> InputExec => _execInputs;
        [Browsable(false)]
        public List<TEOut> OutputExec => _execOutputs;
        protected virtual TEIn[] GetExecInputs() => null;
        protected virtual TEOut[] GetExecOutputs() => null;
        protected void AddExecInput(TEIn[] input) => input.ForEach(AddExecInput);
        protected void AddExecOutput(TEOut[] output) => output.ForEach(AddExecOutput);
        protected void AddExecInput(TEIn input)
        {
            _execInputs.Add(input);
            AddParam(input);
        }
        protected void AddExecOutput(TEOut output)
        {
            _execOutputs.Add(output);
            AddParam(output);
        }
        #endregion

        #region Input/Output Values
        protected List<TVIn> _valueInputs = new List<TVIn>();
        protected List<TVOut> _valueOutputs = new List<TVOut>();
        [Browsable(false)]
        public List<TVIn> InputArguments => _valueInputs;
        [Browsable(false)]
        public List<TVOut> OutputArguments => _valueOutputs;
        protected virtual TVIn[] GetValueInputs() => null;
        protected virtual TVOut[] GetValueOutputs() => null;
        protected void AddValueInput(TVIn[] input) => input.ForEach(AddValueInput);
        protected void AddValueOutput(TVOut[] output) => output.ForEach(AddValueOutput);
        protected void AddValueInput(TVIn input)
        {
            _valueInputs.Add(input);
            AddParam(input);
        }
        protected void AddValueOutput(TVOut output)
        {
            _valueOutputs.Add(output);
            AddParam(output);
        }
        #endregion

        #region Control Arrangement
        public void ArrangeControls()
        {
            Vec2 headerSize = _headerString.Bounds.Extents;
            int totalHeaderPadding = HeaderPadding * 2;
            headerSize.Y += totalHeaderPadding;
            headerSize.X += totalHeaderPadding;
            
            int connectionBoxBounds = BaseFuncArg.ConnectionBoxDims + BaseFuncArg.ConnectionBoxMargin;
            int rows = Math.Max(
                _valueInputs.Count + _execInputs.Count,
                _valueOutputs.Count + _execOutputs.Count);

            Size[] inputTextSizes = new Size[rows];
            Size[] outputTextSizes = new Size[rows];
            int[] maxHeights = new int[rows];
            int maxRows = Math.Max(inputTextSizes.Length, outputTextSizes.Length);

            int middleMargin = 2;

            int maxRowWidth = 0;
            int maxRowHeight = 0;
            int currentRowWidth;
            _size.Y = headerSize.Y + BaseFuncArg.ConnectionBoxMargin * 2.0f;
            for (int i = 0; i < maxRows; ++i)
            {
                currentRowWidth = middleMargin;

                if (i < _execInputs.Count)
                    Arrange1(_execInputs[i], i, inputTextSizes, ref currentRowWidth);
                else if (i - _execInputs.Count < _valueInputs.Count)
                    Arrange1(_valueInputs[i - _execInputs.Count], i, inputTextSizes, ref currentRowWidth);

                if (i < _execOutputs.Count)
                    Arrange1(_execOutputs[i], i, outputTextSizes, ref currentRowWidth);
                else if (i - _execOutputs.Count < _valueOutputs.Count)
                    Arrange1(_valueOutputs[i - _execOutputs.Count], i, outputTextSizes, ref currentRowWidth);

                maxRowWidth = Math.Max(maxRowWidth, currentRowWidth);
                maxRowHeight = TMath.Max(connectionBoxBounds,
                    i < inputTextSizes.Length ? inputTextSizes[i].Height : 0,
                    i < outputTextSizes.Length ? outputTextSizes[i].Height : 0);
                maxHeights[i] = maxRowHeight;
                _size.Y += maxRowHeight;
            }
            SizeableHeight.CurrentValue = _size.Y;

            _size.X = SizeableWidth.CurrentValue = Math.Max(maxRowWidth, headerSize.X);

            float yTrans = _size.Y - headerSize.Y - BaseFuncArg.ConnectionBoxMargin;
            for (int i = 0; i < maxRows; ++i)
            {
                int height = TMath.Max(connectionBoxBounds,
                    i < inputTextSizes.Length ? inputTextSizes[i].Height : 0,
                    i < outputTextSizes.Length ? outputTextSizes[i].Height : 0);

                yTrans -= height;

                if (i < _execInputs.Count)
                    Arrange2(_execInputs[i], _inputParamTexts[i], inputTextSizes[i], true, headerSize.Y, yTrans, maxHeights[i]);
                else if (i - _execInputs.Count < _valueInputs.Count)
                    Arrange2(_valueInputs[i - _execInputs.Count], _inputParamTexts[i], inputTextSizes[i], true, headerSize.Y, yTrans, maxHeights[i]);

                if (i < _execOutputs.Count)
                    Arrange2(_execOutputs[i], _outputParamTexts[i], outputTextSizes[i], false, headerSize.Y, yTrans, maxHeights[i]);
                else if (i - _execOutputs.Count < _valueOutputs.Count)
                    Arrange2(_valueOutputs[i - _execOutputs.Count], _outputParamTexts[i], outputTextSizes[i], false, headerSize.Y, yTrans, maxHeights[i]);
            }

            //_headerText.LocalTranslation = new Vec2(0.0f, _size.Y);

            PerformResize();
        }
        private void Arrange2(BaseFuncArg arg, UITextComponent text, Size size, bool input, float headerHeight, float yTrans, int maxRowHeight)
        {
            text.Size = size;
            int t = BaseFuncArg.ConnectionBoxDims + BaseFuncArg.ConnectionBoxMargin;

            float xTrans;
            if (input)
            {
                xTrans = BaseFuncArg.ConnectionBoxMargin;
                arg.LocalOriginPercentage = new Vec2(0.0f, 0.6f);

                text.LocalOriginPercentage = new Vec2(0.0f, 0.0f);
            }
            else
            {
                xTrans = _size.X - BaseFuncArg.ConnectionBoxMargin;
                arg.LocalOriginPercentage = new Vec2(1.0f, 0.6f);

                t = -t;
                text.LocalOriginPercentage = new Vec2(1.0f, 0.0f);
            }
            arg.LocalTranslation = new Vec2(xTrans, yTrans);
            text.LocalTranslation = new Vec2(xTrans + t, yTrans);
            arg.LocalTranslationY += maxRowHeight * 0.5f;
        }
        private void Arrange1(BaseFuncArg arg, int i, Size[] sizes, ref int currentRowWidth)
        {
            Size argTextSize = TextRenderer.MeasureText(arg.Name, _paramFont);
            sizes[i] = argTextSize;
            currentRowWidth += BaseFuncArg.ConnectionBoxDims + BaseFuncArg.ConnectionBoxMargin * 2 + argTextSize.Width;
        }
        #endregion

        protected override void HandleSingleChildAdded(SceneComponent item)
        {
            base.HandleSingleChildAdded(item);
            if (OwningActor != null)
            {
                foreach (var e in _execInputs)
                {
                    OwningActor.RootComponent.ChildComponents.Add(e);
                }
                foreach (var v in _valueInputs)
                {
                    OwningActor.RootComponent.ChildComponents.Add(v);
                }
            }
        }
    }
}
