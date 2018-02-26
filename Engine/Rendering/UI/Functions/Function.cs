using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
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
    public static class Function
    {
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
                //if (def == null)
                //    continue;

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
    public abstract class Function<TVIn, TVOut, TEIn, TEOut>
        : UIMaterialRectangleComponent, IShaderVarOwner, IFunction
        where TVIn : BaseFuncValue, IFuncValueInput
        where TVOut : BaseFuncValue, IFuncValueOutput
        where TEIn : BaseFuncExec, IFuncExecInput
        where TEOut : BaseFuncExec, IFuncExecOutput
    {
        public Function() : base(MakeFunctionMaterial())
        {
            DockStyle = HudDockStyle.None;

            _headerText = new TextHudComponent
            {
                Name = FunctionName + "_Text",
                DockStyle = HudDockStyle.Top,
                Height = TextRenderer.MeasureText(FunctionName, _textFont).Height + HeaderPadding * 2,
            };
            _headerText.TextDrawer.Add(new TextData(FunctionName, _textFont, Color.White, new Vec2(), new Vec2(), 0.0f, Vec2.One, 0.0f));
            ChildComponents.Add(_headerText);

            AddExecInput(GetExecInputs());
            AddExecOutput(GetExecOutputs());
            AddValueInput(GetValueInputs());
            AddValueOutput(GetValueOutputs());
        }

        private static TMaterial MakeFunctionMaterial()
        {
            return TMaterial.CreateUnlitColorMaterialForward(Color.Green);
        }

        private TextHudComponent _headerText;
        private List<TextHudComponent> _inputParamTexts = new List<TextHudComponent>();
        private List<TextHudComponent> _outputParamTexts = new List<TextHudComponent>();
        
        private void AddParam(BaseFuncArg arg)
        {
            ChildComponents.Add(arg);

            TextHudComponent text = new TextHudComponent
            {
                Name = arg.Name + "_Text",
                DockStyle = HudDockStyle.None,
            };
            text.TextDrawer.Add(new TextData(arg.Name, _textFont, Color.Black, new Vec2(), new Vec2(), 0.0f, Vec2.One, 0.0f));
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
            OnResized();
        }
        protected void AddExecInput(TEIn input)
        {
            HandleExecInputAdded(input);
            OnResized();
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
            OnResized();
        }
        protected void AddExecOutput(TEOut output)
        {
            HandleExecOutputAdded(output);
            OnResized();
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
            OnResized();
        }
        protected void AddValueInput(TVIn input)
        {
            HandleValueInputAdded(input);
            OnResized();
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
            OnResized();
        }
        protected void AddValueOutput(TVOut output)
        {
            HandleValueOutputAdded(output);
            OnResized();
        }
        private void HandleValueOutputAdded(TVOut output)
        {
            _valueOutputs.Add(output);
            AddParam(output);
        }
        #endregion

        public override string ToString()
            => FunctionName;

        internal const int HeaderPadding = 2;
        private Font _textFont = new Font("Arial", 24.0f, FontStyle.Regular);

        public override BoundingRectangle Resize(BoundingRectangle parentRegion)
        {
            Size headerSize = TextRenderer.MeasureText(FunctionName, _textFont);
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

            _size.X = Math.Max(maxRowWidth, headerSize.Width);

            float yTrans = _size.Y - headerSize.Height - BaseFuncArg.ConnectionBoxMargin;
            for (int i = 0; i < maxRows; ++i)
            {
                int height = TMath.Max(connectionBoxBounds,
                    i < inputTextSizes.Length ? inputTextSizes[i].Height : 0,
                    i < outputTextSizes.Length ? outputTextSizes[i].Height : 0);
                yTrans -= height / 2.0f;

                if (i < _execInputs.Count)
                    Arrange2(_execInputs[i], _inputParamTexts[i], inputTextSizes, true, i, headerSize.Height, yTrans);
                else if (i - _execInputs.Count < _valueInputs.Count)
                    Arrange2(_valueInputs[i - _execInputs.Count], _inputParamTexts[i], inputTextSizes, true, i, headerSize.Height, yTrans);

                if (i < _execOutputs.Count)
                    Arrange2(_execOutputs[i], _outputParamTexts[i], outputTextSizes, false, i, headerSize.Height, yTrans);
                else if (i - _execOutputs.Count < _valueOutputs.Count)
                    Arrange2(_valueOutputs[i - _execOutputs.Count], _outputParamTexts[i], outputTextSizes, false, i, headerSize.Height, yTrans);

                yTrans -= height / 2.0f;
            }

            _headerText.LocalTranslation = new Vec2(0.0f, _size.Y);

            return base.Resize(parentRegion);
        }

        private void Arrange2(BaseFuncArg arg, TextHudComponent text, Size[] sizes, bool input, int i, float headerHeight, float yTrans)
        {
            float xTrans;
            if (input)
                xTrans = BaseFuncArg.ConnectionBoxMargin;
            else
                xTrans = _size.X - BaseFuncArg.ConnectionBoxMargin;
            arg.LocalTranslation = new Vec2(xTrans, yTrans);
            
        }
        private void Arrange1(BaseFuncArg arg, int i, Size[] sizes, ref int currentRowWidth)
        {
            Size argTextSize = TextRenderer.MeasureText(arg.Name, _textFont);
            sizes[i] = argTextSize;
            currentRowWidth += BaseFuncArg.ConnectionBoxDims + BaseFuncArg.ConnectionBoxMargin * 2 + argTextSize.Width;
        }

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
    }
}
