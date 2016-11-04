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
        public MaterialFuncInfo(string description, string keywords)
        {
            _keywords = keywords.Split(' ').ToList();
            _description = description;
        }

        public List<string> _keywords;
        public string _description;
    }
    public abstract class MaterialFunction : IGLVarOwner
    {
        private static Dictionary<Type, MaterialFuncInfo> _info = new Dictionary<Type, MaterialFuncInfo>();

        public List<Type> FindFunctions(string keywords)
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

        protected string _operation;
        
        protected List<GLVar> 
            _inputs = new List<GLVar>(), 
            _outputs = new List<GLVar>();

        public List<GLVar> InputArguments { get { return _inputs; } }
        public List<GLVar> OutputArguments { get { return _outputs; } }

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

        static MaterialFunction()
        {
            IEnumerable<Type> funcTypes = Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(MaterialFunction).IsAssignableFrom(t) && !t.IsAbstract);
            foreach (Type t in funcTypes)
            {
                MethodInfo method = t.GetMethod("GetInfo", BindingFlags.Static | BindingFlags.Public);
                if (method == null)
                    throw new Exception("public static List<string> GetInfo() not found in " + t.ToString());
                MaterialFuncInfo info = method.Invoke(null, new object[0]) as MaterialFuncInfo;
                if (info == null)
                    throw new Exception(t.ToString() + "'s GetInfo function did not return MaterialFuncInfo.");
                _info.Add(t, info);
            }
        }

        public MaterialFunction()
        {
            AddInput(GetInputArguments());
            AddOutput(GetOutputArguments());
            _operation = string.Format(GetOperation(), InputArguments, OutputArguments);
        }

        protected virtual List<GLVar> GetInputArguments() { return null; }
        protected virtual List<GLVar> GetOutputArguments() { return null; }
        
        /// <summary>
        /// Returns the base operation for string.Format.
        /// Input args first, then output args.
        /// </summary>
        protected abstract string GetOperation();

        protected void AddInput(List<GLVar> input)
        {
            if (input != null)
                foreach (GLVar v in input)
                    AddInput(v);
        }
        protected void AddInput(GLVar input)
        {
            input.Setup(false, this);
            _inputs.Add(input);
        }
        protected void AddOutput(List<GLVar> output)
        {
            if (output != null)
                foreach (GLVar v in output)
                    AddOutput(v);
        }
        protected void AddOutput(GLVar output)
        {
            output.Setup(true, this);
            _outputs.Add(output);
        }
        public override string ToString()
        {
            return _operation;
        }
    }
}
