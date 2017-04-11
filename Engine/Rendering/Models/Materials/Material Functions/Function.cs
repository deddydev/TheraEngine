using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Materials
{
    public class FunctionDefinition : Attribute
    {
        public FunctionDefinition(string category, string name, string description, string keywords)
        {
            _keywords = keywords.Split(' ').ToList();
            _description = description;
            _name = name;
            _category = category;
        }

        public List<string> _keywords;
        public string _name, _description, _category;
    }
    public interface IFunction
    {

    }
    public abstract class Function : Function<FuncValueInput, FuncValueOutput>
    {
        public static List<Type> FindFunctions<T>(string keywords) where T : IFunction
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
                foreach (string keyword in def._keywords)
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
            int maxVal = CustomMath.Max(types.Keys.ToArray());
            return types[maxVal];
        }
    }
    /// <summary>
    /// Provides a hud component that can connect to other functions with parameters and execution flow.
    /// </summary>
    /// <typeparam name="TIn">The input class to use.</typeparam>
    /// <typeparam name="TOut">The output class to use.</typeparam>
    public abstract class Function<TIn, TOut> : HudComponent, IGLVarOwner, IFunction
        where TIn : FuncValueInput where TOut : FuncValueOutput
    {
        protected List<FuncValueInput> _inputs = new List<FuncValueInput>();
        protected List<FuncValueOutput> _outputs = new List<FuncValueOutput>();

        public Function()
        {
            AddInput(GetInputs());
            AddOutput(GetOutputs());
        }

        public List<FuncValueInput> InputArguments => _inputs;
        public List<FuncValueOutput> OutputArguments => _outputs;

        public void SetOwner(HudComponent comp)
        {
            _parent?.Remove(this);
            comp.Add(this);
        }

        protected virtual List<FuncValueInput> GetInputs() => new List<FuncValueInput>();
        protected virtual List<FuncValueOutput> GetOutputs() => new List<FuncValueOutput>();

        protected void AddInput(List<FuncValueInput> input)
        {
            if (input != null)
                foreach (FuncValueInput v in input)
                {
                    v.Arrange(_inputs.Count);
                    _inputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddInput(FuncValueInput input)
        {
            input.Arrange(_inputs.Count);
            _inputs.Add(input);
            _children.Add(input);
            Resized();
        }
        protected void AddOutput(List<FuncValueOutput> output)
        {
            if (output != null)
                foreach (FuncValueOutput v in output)
                {
                    v.Arrange(_outputs.Count);
                    _outputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddOutput(FuncValueOutput output)
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
        internal const float Padding = 3.0f;
        internal const float MaxArgTextWidth = 20.0f;
        public void Resized()
        {
            Height = _name.Length * TextCharWidth + Math.Max(_inputs.Count, _outputs.Count) * (BaseFuncArg.ConnectionBoxDims + BaseFuncArg.PaddingBetweenBoxes);
        }

        public FunctionDefinition Definition => GetType().GetCustomAttribute<FunctionDefinition>();
        public ReadOnlyCollection<string> Keywords => Definition?._keywords.AsReadOnly();
        public string FunctionName => Definition?._name;
        public string Description => Definition?._description;
        public string Category => Definition?._category;
    }
}
