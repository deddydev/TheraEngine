using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWrapper
{
    public enum MethodType
    {
        Regular,
        Constructor,
        Destructor,
        Operator,
    }
    public class CMethod : IClassItem
    {
        public Modifiers _modifiers;
        [Flags]
        public enum Modifiers
        {
            _static = 1,
            _virtual = 2,
            _inline = 4,
            _const = 8,
        }

        public string _name;
        public CType _returnType;
        public MethodType _type;
        public List<CParam> _parameters = new List<CParam>();

        public static CMethod ParseLine(ref int i, ref List<string> input)
        {
            int bracketIndex = CodeConverter.FindOpenBracket(i);
            int closingP;
            for (int x = bracketIndex; x >= i; --x)
            {
                int pIndex = input[x].IndexOf('(');
                if (pIndex >= 0)
                {
                    int colonIndex = input[x].IndexOf(':');
                    if (colonIndex >= 0)
                    {
                        if (pIndex < colonIndex)
                        {
                            closingP = pIndex;
                            break;
                        }
                    }
                }
            }

            string s = input[i];

            //Ignore everything after the colon
            //this is for setting parameters or calling another method, which we don't need
            //int colonIndex = s.IndexOf(": ");
            //if (colonIndex > 0)
            //    s = s.Substring(0, colonIndex);

            CMethod method = new CMethod();
            
            int paramEnding = s.LastIndexOf(')');
            int paramOpening = s.IndexOfBefore('(', paramEnding);

            MethodType mType = MethodType.Regular;
            int nameIndex = 0;
            if (s.StartsWith("~"))
            {
                mType = MethodType.Destructor;
                nameIndex = 1;
            }
            else
            {
                int operatorIndex = s.IndexOf("operator");
                if (operatorIndex >= 0)
                {
                    mType = MethodType.Operator;
                    nameIndex = operatorIndex + 8;
                }
                else
                {
                    int lastSpace = s.IndexOfBefore(' ', paramOpening);
                    if (lastSpace < 0)
                    {
                        nameIndex = 0;
                        mType = MethodType.Constructor;
                    }
                    else
                        nameIndex = lastSpace + 1;
                }
            }
            method._name = s.Substring(nameIndex, paramOpening - nameIndex).Trim();
            method._type = mType;

            int startIndex = mType == MethodType.Destructor ? 1 : 0;
            List<string> modifiers = s.Substring(startIndex, nameIndex - startIndex).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            List<string> parameters = s.Substring(paramOpening + 1, paramEnding - (paramOpening + 1)).Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (mType == MethodType.Operator)
            {
                int oIndex = modifiers.IndexOf("operator");
                if (oIndex >= 0)
                    modifiers.RemoveAt(oIndex);
            }

            if (modifiers.Contains("template"))
            {
                s = "//" + s;
            }
            else
            {
                foreach (string x in Enum.GetNames(typeof(Modifiers)))
                {
                    int xIndex = modifiers.IndexOf(x.Substring(1));
                    if (xIndex >= 0)
                    {
                        modifiers.RemoveAt(xIndex);
                        method._modifiers |= (Modifiers)Enum.Parse(typeof(Modifiers), x);
                    }
                }
                if (modifiers.Count > 1)
                {
                    s = "//" + s;
                    parameters.Clear();
                    //throw new Exception();
                }
                else if (modifiers.Count == 1)
                {
                    if (mType == MethodType.Constructor ||
                        mType == MethodType.Destructor)
                        throw new Exception();
                    method._returnType = new CType(modifiers[0]);
                }
                foreach (string p in parameters)
                    method._parameters.Add(CParam.ParseSegment(p));
            }
            return method;
        }
    }
    public class CType
    {
        public int _pointerCount;
        public bool _byReference;
        public string _type;

        public CType(string fullType)
        {
            List<int> pointerIndices = fullType.OccurrencesOf('*');

            _pointerCount = pointerIndices.Count;

            pointerIndices.Sort();
            for (int i = 0; i < pointerIndices.Count; ++i)
                fullType = fullType.Remove(pointerIndices[i] - i, 1);

            int refIndex = fullType.IndexOf('&');
            if (_byReference = refIndex >= 0)
                fullType = fullType.Remove(refIndex, 1);

            _type = fullType;
        }
    }
    public class CParam
    {
        public bool _const;
        public CType _type;
        public string _name;
        public string _value;

        public static CParam ParseSegment(string p)
        {
            CParam param = new CParam();

            p = p.Trim();

            if (param._const = p.StartsWith("const"))
                p = p.Substring(5).Trim();

            int equalIndex = p.IndexOf('=');
            if (equalIndex >= 0)
            {
                param._value = p.Substring(equalIndex + 1).Trim();
                p = p.Substring(0, equalIndex).Trim();
            }

            if (!p.Contains(' '))
                param._name = p;
            else
            {
                param._name = p.Substring(p.LastIndexOf(' ')).Trim();
                param._type = new CType(p.Substring(0, p.IndexOf(' ')).Trim());
            }

            return param;
        }
    }
    public class CField : CParam, IClassItem
    {
        public Modifiers _modifiers;
        [Flags]
        public enum Modifiers
        {
            _static = 1,
            _volatile = 2,
            _restrict = 4,
            _const = 8,
        }

        public static CField ParseLine(string s)
        {
            CField field = new CField();
            int startIndex, endIndex;
            if (s.Contains("="))
            {
                int equalIndex = s.IndexOf('=');
                int valueIndex = s.IndexOfNotAfter(' ', equalIndex);
                int semicolonIndex = s.IndexOfAfter(';', equalIndex);

                field._value = s.Substring(valueIndex, semicolonIndex - valueIndex);

                endIndex = s.IndexOfNotBefore(' ', equalIndex) + 1;
                startIndex = s.IndexOfBefore(' ', endIndex);
                field._name = s.Substring(startIndex, endIndex - startIndex).Trim();

                s = s.Substring(0, s.IndexOfNotBefore(' ', equalIndex) + 1);
            }
            else
            {
                startIndex = s.LastIndexOf(' ') + 1;
                endIndex = s.IndexOf(";");
                field._name = s.Substring(startIndex, endIndex - startIndex).Trim();
            }

            List<string> modifiers = s.Substring(0, s.LastIndexOf(' ')).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (string x in Enum.GetNames(typeof(Modifiers)))
            {
                int xIndex = modifiers.IndexOf(x.Substring(1));
                if (xIndex >= 0)
                {
                    modifiers.RemoveAt(xIndex);
                    field._modifiers |= (Modifiers)Enum.Parse(typeof(Modifiers), x);
                }
            }
            if (modifiers.Count == 1)
            {
                field._type = new CType(modifiers[0]);
            }
            else
            {
                //s = "//" + s;
                throw new Exception();
            }
            return field;
        }
    }
    public class CEnum : IClassItem
    {
        public string _name;
        public List<KeyValuePair<string, string>> _values = new List<KeyValuePair<string, string>>();

        public static void ParseSegment(string s, out string key, out string value)
        {
            if (s.Contains('='))
            {
                key = s.Substring(0, s.IndexOf('=')).Trim();
                value = s.Substring(s.IndexOf('=') + 1).Trim();
                if (value.Contains(","))
                    value = value.Substring(0, value.Length - 1);
            }
            else
            {
                key = s.Trim();
                if (key.Contains(","))
                    key = key.Substring(0, key.Length - 1);
                value = null;
            }
        }
        public static CEnum ParseEnum(ref int i, ref List<string> input)
        {
            string s = input[i];
            CEnum newEnum = new CEnum() { _name = s.Substring(s.IndexOfNotAfter(' ', 4)) };

            if (s.Contains("}"))
            {
                int open = s.IndexOf('{');
                s = s.Substring(open, s.IndexOf('}') - open);
                string[] enums = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string e in enums)
                {
                    string key, value;
                    ParseSegment(e.Trim(), out key, out value);
                    newEnum._values.Add(new KeyValuePair<string, string>(key, value));
                }
            }
            else
            {
                if (input[++i].Contains("{"))
                    ++i; //Skip open bracket

                while (true)
                {
                    s = input[i];
                    if (s.Contains("}"))
                        break;
                    string key, value;
                    ParseSegment(s, out key, out value);
                    newEnum._values.Add(new KeyValuePair<string, string>(key, value));
                    ++i;
                }
            }
            return newEnum;
        }
    }
    public class CClass : CNamespace, INamespaceItem
    {
        public bool _isStruct;
        public override void AddItem(INamespaceItem item)
        {
            if (item is IClassItem)
                base.AddItem(item);
        }
    }
    public class CNamespace
    {
        public string _name;
        private List<INamespaceItem> _items = new List<INamespaceItem>();
        public virtual void AddItem(INamespaceItem item) { _items.Add(item); }
    }
    public class CRefClass : INamespaceItem
    {
        public string _name;
        public bool _isStruct;
    }
    public interface INamespaceItem
    {

    }
    public interface IClassItem : INamespaceItem
    {

    }
}
