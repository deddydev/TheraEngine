﻿using CustomEngine.Rendering.HUD;
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
    public abstract class Function : Function<IFuncValueInput, IFuncValueOutput>
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
        where TIn : HudComponent, IFuncValueInput where TOut : HudComponent, IFuncValueOutput
    {
        protected List<TIn> _inputs = new List<TIn>();
        protected List<TOut> _outputs = new List<TOut>();

        public Function()
        {
            AddInput(GetInputs());
            AddOutput(GetOutputs());
        }

        public List<TIn> InputArguments => _inputs;
        public List<TOut> OutputArguments => _outputs;

        public void SetOwner(HudComponent comp)
        {
            _parent?.Remove(this);
            comp.Add(this);
        }

        protected virtual List<TIn> GetInputs() => new List<TIn>();
        protected virtual List<TOut> GetOutputs() => new List<TOut>();

        protected void AddInput(List<TIn> input)
        {
            if (input != null)
                foreach (TIn v in input)
                {
                    v.Arrange(_inputs.Count);
                    _inputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddInput(TIn input)
        {
            input.Arrange(_inputs.Count);
            _inputs.Add(input);
            _children.Add(input);
            Resized();
        }
        protected void AddOutput(List<TOut> output)
        {
            if (output != null)
                foreach (TOut v in output)
                {
                    v.Arrange(_outputs.Count);
                    _outputs.Add(v);
                    _children.Add(v);
                }
            Resized();
        }
        protected void AddOutput(TOut output)
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
