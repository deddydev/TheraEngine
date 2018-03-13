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
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Rendering.Text;

namespace TheraEngine.Rendering.UI.Functions
{
    public class FunctionDefinition : Attribute
    {
        public FunctionDefinition(string category, string name, string description, string keywords)
        {
            Keywords = keywords.Split(' ').ToList();
            Description = description;
            Name = name;
            Category = category;
        }

        public List<string> Keywords { get; }
        public string Name { get; }
        public string Description { get; }
        public string Category { get; }
    }
    public interface IFunction
    {

    }
    public abstract class BaseFunction : UIMaterialRectangleComponent, IShaderVarOwner, IFunction
    {
        public static Vec4 RegularColor { get; set; } = new Vec4(0.1f, 0.1f, 0.1f, 1.0f);
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
            _headerText.TextDrawer.Add(new UIString2D()
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
        public Function() : base()
        {
            AddExecInput(GetExecInputs());
            AddExecOutput(GetExecOutputs());
            AddValueInput(GetValueInputs());
            AddValueOutput(GetValueOutputs());
            ArrangeControls();
            PerformResize();
        }

        #region Input/Output Exec
        protected List<TEIn> _execInputs = new List<TEIn>();
        protected List<TEOut> _execOutputs = new List<TEOut>();
        [Browsable(false)]
        public List<TEIn> InputExec => _execInputs;
        [Browsable(false)]
        public List<TEOut> OutputExec => _execOutputs;
        protected virtual List<TEIn> GetExecInputs() => new List<TEIn>();
        protected virtual List<TEOut> GetExecOutputs() => new List<TEOut>();
        protected void AddExecInput(List<TEIn> input)
        {
            if (input != null)
                foreach (TEIn v in input)
                    HandleExecInputAdded(v);
            PerformResize();
        }
        protected void AddExecInput(TEIn input)
        {
            HandleExecInputAdded(input);
            PerformResize();
        }
        private void HandleExecInputAdded(TEIn input)
        {
            _execInputs.Add(input);
            AddParam(input);
        }
        protected void AddExecOutput(List<TEOut> output)
        {
            if (output != null)
                foreach (TEOut v in output)
                    HandleExecOutputAdded(v);
            PerformResize();
        }
        protected void AddExecOutput(TEOut output)
        {
            HandleExecOutputAdded(output);
            PerformResize();
        }
        private void HandleExecOutputAdded(TEOut output)
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
        protected virtual List<TVIn> GetValueInputs() => new List<TVIn>();
        protected virtual List<TVOut> GetValueOutputs() => new List<TVOut>();
        protected void AddValueInput(List<TVIn> input)
        {
            if (input != null)
                foreach (TVIn v in input)
                    HandleValueInputAdded(v);
            PerformResize();
        }
        protected void AddValueInput(TVIn input)
        {
            HandleValueInputAdded(input);
            PerformResize();
        }
        private void HandleValueInputAdded(TVIn input)
        {
            _valueInputs.Add(input);
            AddParam(input);
        }
        protected void AddValueOutput(List<TVOut> output)
        {
            if (output != null)
                foreach (TVOut v in output)
                    HandleValueOutputAdded(v);
            PerformResize();
        }
        protected void AddValueOutput(TVOut output)
        {
            HandleValueOutputAdded(output);
            PerformResize();
        }
        private void HandleValueOutputAdded(TVOut output)
        {
            _valueOutputs.Add(output);
            AddParam(output);
        }
        #endregion

        #region Control Arrangement
        private void ArrangeControls()
        {
            Size headerSize = TextRenderer.MeasureText(FunctionName, _headerFont);
            int totalHeaderPadding = HeaderPadding * 2;
            headerSize.Height += totalHeaderPadding;
            headerSize.Width += totalHeaderPadding;

            int connectionBoxBounds = BaseFuncArg.ConnectionBoxDims + BaseFuncArg.ConnectionBoxMargin;
            Size[] inputTextSizes = new Size[_valueInputs.Count + _execInputs.Count];
            Size[] outputTextSizes = new Size[_valueOutputs.Count + _execOutputs.Count];
            int maxRows = Math.Max(inputTextSizes.Length, outputTextSizes.Length);

            int middleMargin = 2;

            int maxRowWidth = 0;
            int currentRowWidth;
            _size.Y = headerSize.Height + BaseFuncArg.ConnectionBoxMargin * 2.0f;
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
                _size.Y += TMath.Max(connectionBoxBounds,
                    i < inputTextSizes.Length ? inputTextSizes[i].Height : 0,
                    i < outputTextSizes.Length ? outputTextSizes[i].Height : 0);
            }
            SizeableHeight.CurrentValue = _size.Y;

            _size.X = SizeableWidth.CurrentValue = Math.Max(maxRowWidth, headerSize.Width);

            float yTrans = _size.Y - headerSize.Height - BaseFuncArg.ConnectionBoxMargin;
            for (int i = 0; i < maxRows; ++i)
            {
                int height = TMath.Max(connectionBoxBounds,
                    i < inputTextSizes.Length ? inputTextSizes[i].Height : 0,
                    i < outputTextSizes.Length ? outputTextSizes[i].Height : 0);

                yTrans -= height;

                if (i < _execInputs.Count)
                    Arrange2(_execInputs[i], _inputParamTexts[i], inputTextSizes, true, i, headerSize.Height, yTrans);
                else if (i - _execInputs.Count < _valueInputs.Count)
                    Arrange2(_valueInputs[i - _execInputs.Count], _inputParamTexts[i], inputTextSizes, true, i, headerSize.Height, yTrans);

                if (i < _execOutputs.Count)
                    Arrange2(_execOutputs[i], _outputParamTexts[i], outputTextSizes, false, i, headerSize.Height, yTrans);
                else if (i - _execOutputs.Count < _valueOutputs.Count)
                    Arrange2(_valueOutputs[i - _execOutputs.Count], _outputParamTexts[i], outputTextSizes, false, i, headerSize.Height, yTrans);
            }

            //_headerText.LocalTranslation = new Vec2(0.0f, _size.Y);
        }
        private void Arrange2(BaseFuncArg arg, UITextComponent text, Size[] sizes, bool input, int i, float headerHeight, float yTrans)
        {
            text.Size = sizes[i];
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
            arg.LocalTranslationY += text.Height * 0.5f;
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
