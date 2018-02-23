using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using TheraEngine.Rendering.Models.Materials;

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
    /// </summary>
    /// <typeparam name="TVIn">The input value argument class to use.</typeparam>
    /// <typeparam name="TVOut">The output value class to use.</typeparam>
    /// <typeparam name="TEIn">The input execution argument class to use.</typeparam>
    /// <typeparam name="TEOut">The output execution class to use.</typeparam>
    public abstract class Function<TVIn, TVOut, TEIn, TEOut>
        : UIMaterialRectangleComponent, IShaderVarOwner, IFunction
        where TVIn : UIComponent, IFuncValueInput
        where TVOut : UIComponent, IFuncValueOutput
        where TEIn : UIComponent, IFuncExecInput
        where TEOut : UIComponent, IFuncExecOutput
    {
        protected List<TVIn> _inputs = new List<TVIn>();
        protected List<TVOut> _outputs = new List<TVOut>();
        protected TEIn _mainExecIn;
        protected TEOut _mainExecOut;

        public Function() : base(MakeFunctionMaterial())
        {
            AddInput(GetValueInputs());
            AddOutput(GetValueOutputs());
            RenderInfo.LayerIndex = 2;
            Size = new Vec2(100.0f, 50.0f);
            DockStyle = HudDockStyle.None;
        }

        private static TMaterial MakeFunctionMaterial()
        {
            return TMaterial.CreateUnlitColorMaterialForward(Color.Green);
        }

        public List<TVIn> InputArguments => _inputs;
        public List<TVOut> OutputArguments => _outputs;

        protected virtual List<TVIn> GetValueInputs() => new List<TVIn>();
        protected virtual List<TVOut> GetValueOutputs() => new List<TVOut>();
        protected virtual List<TEIn> GetExecutionInputs(out TEIn mainInput)
        {
            mainInput = Activator.CreateInstance<TEIn>();
            return new List<TEIn>();
        }
        protected virtual List<TEOut> GetExecutionInputs(out TEOut mainOutput)
        {
            mainOutput = Activator.CreateInstance<TEOut>();
            return new List<TEOut>();
        }

        protected void AddInput(List<TVIn> input)
        {
            if (input != null)
                foreach (TVIn v in input)
                {
                    v.Arrange(_inputs.Count);
                    _inputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddInput(TVIn input)
        {
            input.Arrange(_inputs.Count);
            _inputs.Add(input);
            _children.Add(input);
            Resized();
        }
        protected void AddOutput(List<TVOut> output)
        {
            if (output != null)
                foreach (TVOut v in output)
                {
                    v.Arrange(_outputs.Count);
                    _outputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddOutput(TVOut output)
        {
            output.Arrange(_outputs.Count);
            _outputs.Add(output);
            _children.Add(output);
            Resized();
        }
        public override string ToString()
            => FunctionName;

        internal const float TitleHeight = 10.0f;
        internal const float TextCharWidth = 5.0f;
        internal const float TextCharHeight = 5.0f;
        internal const float MaxArgTextWidth = 20.0f;
        public void Resized()
        {
            float titleWidth = FunctionName.Length * TextCharWidth;
            float titleHeight = TextCharHeight;
            float connectionBoxBounds = BaseFuncValue.ConnectionBoxDims + BaseFuncValue.ConnectionBoxMargin;
            int maxRows = Math.Max(_inputs.Count, _outputs.Count);

            Height = titleHeight + maxRows * connectionBoxBounds;

            float maxWidth = 0.0f;
            float currentRowWidth;
            for (int i = 0; i < maxRows; ++i)
            {
                currentRowWidth = 0.0f;
                if (i < _inputs.Count)
                {
                    TVIn t = _inputs[i];
                }
                if (i < _outputs.Count)
                {
                    TVOut t = _outputs[i];
                }
            }

            Width = titleWidth;
        }
        
        public FunctionDefinition Definition => GetType().GetCustomAttribute<FunctionDefinition>();
        public ReadOnlyCollection<string> Keywords => Definition?.Keywords.AsReadOnly();
        public string FunctionName => Definition?.Name;
        public string Description => Definition?.Description;
        public string Category => Definition?.Category;

        protected TEIn MainExecIn { get => _mainExecIn; set => _mainExecIn = value; }
        protected TEOut MainExecOut { get => _mainExecOut; set => _mainExecOut = value; }
    }
}
