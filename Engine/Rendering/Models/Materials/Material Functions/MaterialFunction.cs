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
        public MaterialFuncInfo(string category, string description, string keywords)
        {
            _keywords = keywords.Split(' ').ToList();
            _description = description;
        }

        public List<string> _keywords;
        public string _description, _category;
    }
    public abstract class MaterialFunction : HudPanel, IGLVarOwner
    {
        private static Dictionary<Type, MaterialFuncInfo> _info = new Dictionary<Type, MaterialFuncInfo>();

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

        protected string _operation;
        protected List<BaseGLArgument> _inputs = new List<BaseGLArgument>();

        public List<BaseGLArgument> InputArguments { get { return _inputs; } }
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

        public MaterialFunction(HudComponent owner) : base(owner)
        {
            AddInput(GetArguments());
            _operation = string.Format(GetOperation(), InputArguments);
        }

        protected virtual List<BaseGLArgument> GetArguments() { return new List<BaseGLArgument>(); }
        
        /// <summary>
        /// Returns the base operation for string.Format.
        /// Input args first, then output args.
        /// </summary>
        protected abstract string GetOperation();

        protected void AddInput(List<BaseGLArgument> input)
        {
            if (input != null)
                foreach (BaseGLArgument v in input)
                    AddInput(v);
        }
        protected void AddInput(BaseGLArgument input)
        {
            _inputs.Add(input);
        }
        public override string ToString()
        {
            return _operation;
        }
    }
}
