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
    public class MaterialFuncInfo
    {
        public MaterialFuncInfo(string category, string name, string description, string keywords)
        {
            _keywords = keywords.Split(' ').ToList();
            _description = description;
        }

        public List<string> _keywords;
        public string _name, _description, _category;
    }
    public abstract class MaterialFunction : HudComponent, IGLVarOwner
    {
        private static Dictionary<Type, MaterialFuncInfo> _info = new Dictionary<Type, MaterialFuncInfo>();

        /// <summary>
        /// Determines if this function can be contained within one line.
        /// Otherwise written as a method located outside of main().
        /// Ex: type thing = FUNC_OPERATION;
        /// otherwise:
        /// type FUNC_NAME(in/out type args) { FUNC_OPERATION }
        /// </summary>
        protected bool _inline = false;

        public static MaterialFunction Instantiate(Type t, HudComponent owner)
        {
            if (!t.IsAssignableFrom(typeof(MaterialFunction)))
                return null;

            return Activator.CreateInstance(t, owner) as MaterialFunction;
        }
        public static List<Type> FindFunctions(string keywords)
        {
            string[] keyArray = keywords.Split(' ');
            Dictionary<int, List<Type>> types = new Dictionary<int, List<Type>>();
            foreach (var keyval in _info)
            {
                int count = 0;
                foreach (string keyword in keyval.Value._keywords)
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
                        types[count].Add(keyval.Key);
                    else
                        types.Add(count, new List<Type>() { keyval.Key });
                }
            }
            int maxVal = CustomMath.Max(types.Keys.ToArray());
            return types[maxVal];
        }
        
        protected List<GLInput> _inputs = new List<GLInput>();
        protected List<GLOutput> _outputs = new List<GLOutput>();
        
        public List<GLInput> InputArguments { get { return _inputs; } }
        public List<GLOutput> OutputArguments { get { return _outputs; } }

        public ReadOnlyCollection<string> Keywords
        {
            get
            {
                Type t = GetType();
                if (_info.ContainsKey(t))
                    return _info[t]._keywords.AsReadOnly();
                return null;
            }
        }
        public string FunctionName
        {
            get
            {
                Type t = GetType();
                if (_info.ContainsKey(t))
                    return _info[t]._name;
                return null;
            }
        }
        public string Description
        {
            get
            {
                Type t = GetType();
                if (_info.ContainsKey(t))
                    return _info[t]._description;
                return null;
            }
        }
        public string Category
        {
            get
            {
                Type t = GetType();
                if (_info.ContainsKey(t))
                    return _info[t]._category;
                return null;
            }
        }

        static MaterialFunction()
        {
            IEnumerable<Type> funcTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(MaterialFunction).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (Type t in funcTypes)
            {
                MethodInfo method = t.GetMethod("GetInfo", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                    throw new Exception("public static MaterialFuncInfo GetInfo() not found in " + t.ToString());
                MaterialFuncInfo info = method.Invoke(null, new object[0]) as MaterialFuncInfo;
                if (info == null)
                    throw new Exception(t.ToString() + "'s GetInfo function did not return MaterialFuncInfo.");
                _info.Add(t, info);
            }
        }

        public void SetOwner(HudComponent comp)
        {
            _parent?.Remove(this);
            comp.Add(this);
        }

        public MaterialFunction() : base(null)
        {
            AddInput(GetInputs());
            AddOutput(GetOutputs());
        }

        protected virtual List<GLInput> GetInputs() { return new List<GLInput>(); }
        protected virtual List<GLOutput> GetOutputs() { return new List<GLOutput>(); }

        /// <summary>
        /// Returns the base operation for string.Format.
        /// </summary>
        protected abstract string GetOperation();

        protected void AddInput(List<GLInput> input)
        {
            if (input != null)
                foreach (GLInput v in input)
                    AddInput(v);
        }
        protected void AddInput(GLInput input)
        {
            _inputs.Add(input);
        }
        protected void AddOutput(List<GLOutput> output)
        {
            if (output != null)
                foreach (GLOutput v in output)
                    AddOutput(v);
        }
        protected void AddOutput(GLOutput output)
        {
            _outputs.Add(output);
        }
        public override string ToString() { return FunctionName; }
        public string GetLineOperation(
            string[] inputNames,
            string[] outputNames,
            bool declareOutputs = false)
        {
            if (inputNames.Length != _inputs.Count ||
                outputNames.Length != _outputs.Count)
                throw new InvalidOperationException();

            if (_inline)
                return string.Format(GetOperation(), inputNames);

            string s = "\n";
            if (declareOutputs)
                for (int i = 0; i < _outputs.Count; ++i)
                {
                    string name = outputNames[i];
                    GLTypeName type = _outputs[i].CurrentArgumentType;
                    s += type + " " + name + ";\n";
                }

            s += Name + "(";
            for (int i = 0; i < _inputs.Count; ++i)
            {
                string name = inputNames[i];

            }

            return s;
        }
        public string GetGlobalMethodDeclaration()
        {
            string s = "void " + FunctionName + "(";
            bool first = true;
            foreach (GLInput arg in InputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "in " + arg.CurrentArgumentType.ToString().Substring(1) + " " + arg.Name;
            }
            foreach (GLOutput arg in OutputArguments)
            {
                if (first)
                    first = false;
                else
                    s += ", ";
                s += "out " + arg.CurrentArgumentType.ToString().Substring(1) + " " + arg.Name;
            }
            s += ")\n{\n" + GetOperation() + "\n}\n";
            return s;
        }
    }
}
