using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using TheraEngine.Core.Files.Serialization;

namespace TheraEngine.Rendering.Text
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class TagNameAttribute : Attribute
    {
        public TagNameAttribute(string tagName) => TagName = tagName;
        /// <summary>
        /// The name put between the opening and closing tag.
        /// </summary>
        public string TagName { get; set; }
        /// <summary>
        /// If the string needs to be redrawn with each tick. Otherwise static.
        /// </summary>
        public bool NeedsTickUpdate { get; set; } = false;
    }
    /// <summary>
    /// Used to retrieve various text information from the engine using <example> tags within strings drawn using the TextDrawer class.
    /// In order to draw < or > normally, put a \ before the bracket, like this: \<example\>
    /// Override and add your own methods to execute when the method's name is put between brackets.
    /// </summary>
    public partial class VariableStringTable
    {
        [TSerialize]
        private Dictionary<string, (bool needsTickUpdate, MethodInfo method)> _table;
        
        public VariableStringTable()
        {
            _table = new Dictionary<string, (bool needsTickUpdate, MethodInfo method)>();
            
            Type type = typeof(VariableStringTable);
            MethodInfo[] methods = type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (MethodInfo method in methods)
            {
                var attrib = method.GetCustomAttribute<TagNameAttribute>();
                string tag = attrib?.TagName;
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    if (method.ReturnType != typeof(string))
                        continue;

                    var parameters = method.GetParameters();
                    if (parameters.Length < 2)
                    {
                        if (parameters.Length == 1)
                        {
                            Type paramType = parameters[0].ParameterType;
                            bool validParam = SerializationCommon.IsSerializableAsString(paramType);
                            if (!validParam)
                            {
                                Engine.LogWarning($"Format parameter {paramType.GetFriendlyName()} cannot be parsed from a string.");
                                continue;
                            }
                        }

                        if (!_table.ContainsKey(tag))
                            _table.Add(attrib.TagName, (attrib.NeedsTickUpdate, method));
                        else
                            Engine.LogWarning($"{nameof(VariableStringTable)} already contains method for {tag}.");
                    }
                }
            }
        }
        public string SolveAnalyzedString(string str, (int index, int length)[] variableLocations)
        {
            string newStr = "";
            string format, name, varCode, result;
            int lastIndex = 0;

            foreach (var (index, length) in variableLocations)
            {
                if (index > lastIndex)
                    newStr += str.Substring(lastIndex, index - lastIndex);

                varCode = str.Substring(index, length);
                format = null;
                name = null;

                int formatSplitter = varCode.IndexOf(':');
                if (formatSplitter > 0)
                {
                    format = varCode.Substring(formatSplitter + 1);
                    name = varCode.Substring(0, formatSplitter);

                    result = ResolveVariable(name, format);
                }
                else
                {
                    name = varCode;

                    result = ResolveVariable(name);
                }
                
                newStr += result;
                lastIndex = index + length;
            }

            if (str.Length > lastIndex)
                newStr += str.Substring(lastIndex, str.Length - lastIndex);

            return newStr;
        }
        public string AnalyzeString(string str, out (int index, int length, bool redraw)[] variableLocations)
        {
            string newStr = "";
            List<(int index, int length, bool redraw)> varLocs = new List<(int index, int length, bool redraw)>();

            int lastIndex = 0;
            char[] codeChars = { '<', '>', '\\' };
            int[] codeCharIndices = str.FindAllOccurrences(0, str.Length - 1, true, codeChars);
            List<int> escapeIndices = new List<int>();
            char codeChar, nextCodeChar;
            int endBracketIndex;
            for (int i = 0, codeCharIndex; i < codeCharIndices.Length; ++i)
            {
                codeCharIndex = codeCharIndices[i];

                if (codeCharIndex > lastIndex)
                    newStr += str.Substring(lastIndex, codeCharIndex - lastIndex);

                codeChar = str[codeCharIndex];

                bool escaped = escapeIndices.Contains(codeCharIndex - 1);
                if (escaped)
                {
                    newStr += codeChar;
                    continue;
                }

                switch (codeChar)
                {
                    case '<':

                        if (i + 1 >= codeCharIndices.Length)
                            throw new InvalidOperationException("Variable name must be closed with a closing bracket.");
                        
                        endBracketIndex = codeCharIndices[i + 1];
                        nextCodeChar = str[endBracketIndex];

                        switch (nextCodeChar)
                        {
                            case '<': throw new InvalidOperationException("Variable name cannot contain nested open brackets.");
                            case '\\': throw new InvalidOperationException("Variable name cannot contain \\ escape character.");
                            case '>':
                                int index = codeCharIndex + 1;
                                int length = endBracketIndex - index;

                                string varCode = str.Substring(index, length);
                                //string format = null;
                                string name = null;

                                int formatSplitter = varCode.IndexOf(':');
                                if (formatSplitter > 0)
                                {
                                    //format = varCode.Substring(formatSplitter + 1);
                                    name = varCode.Substring(0, formatSplitter);
                                }
                                else
                                {
                                    name = varCode;
                                }

                                if (_table.ContainsKey(name))
                                {
                                    bool tickUpdate = VariableNeedsTickUpdate(name);
                                    varLocs.Add((newStr.Length, length, tickUpdate));
                                }
                                else
                                {
                                    Engine.LogWarning("String variable <" + varCode + "> not recognized.");
                                }

                                newStr += varCode;
                                lastIndex = index + length + 1;

                                break;
                        }
                        break;
                    case '>':
                        break;
                    case '\\':
                        bool escaping = codeCharIndex + 1 < str.Length && codeChars.Contains(str[codeCharIndex + 1]);
                        if (escaping)
                            escapeIndices.Add(codeCharIndex);
                        else
                            newStr += codeChar;
                        break;
                }
            }

            if (str.Length > lastIndex)
                newStr += str.Substring(lastIndex, str.Length - lastIndex);

            variableLocations = varLocs.ToArray();
            return newStr;
        }
        /// <summary>
        /// Returns the evaluated variable value for the given string id.
        /// </summary>
        public string ResolveVariable(string id)
        {
            if (_table.ContainsKey(id))
                return (string)_table[id].method.Invoke(this, null);
            
            Engine.LogWarning("Invalid variable string id: " + id);

            //Replace the tag with itself so it's clear it is invalid
            return id;
        }
        /// <summary>
        /// Returns the evaluated variable value for the given string id.
        /// </summary>
        public string ResolveVariable(string id, string format)
        {
            if (_table.ContainsKey(id))
            {
                MethodInfo info = _table[id].method;
                ParameterInfo[] parameters = info.GetParameters();
                ParameterInfo parameter = parameters[0];
                Type type = parameter.ParameterType;
                object obj = SerializationCommon.ParseString(format, type);
                return (string)info.Invoke(this, new object[] { obj });
            }

            Engine.LogWarning("Invalid variable string id: " + id);

            //Replace the tag with itself so it's clear it is invalid
            return id;
        }
        public bool VariableNeedsTickUpdate(string id)
        {
            if (_table.ContainsKey(id))
                return _table[id].needsTickUpdate;
            
            Engine.LogWarning("Invalid variable string id: " + id);
            
            return false;
        }
    }
}
