using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWrapper
{
    public static class CodeConverter
    {
        private static List<string> input;
        public static void Convert(string fileName, string inputDir, string outputDir)
        {
            string inPath = inputDir + "\\" + fileName + ".h";
            string outPathHeader = outputDir + "\\" + fileName + ".h";
            string outPathCode = outputDir + "\\" + fileName + ".cpp";
            string origFile = File.ReadAllText(inPath);

            input = origFile.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Clean();
            CNamespace namespaceTree = Interpret();

            string joined = string.Join(Environment.NewLine, input);
            File.WriteAllText(outPathHeader, joined);
        }
        private static CNamespace Interpret()
        {
            Stack<CNamespace> _namespaces = new Stack<CNamespace>();
            _namespaces.Push(new CNamespace());
            Func<CNamespace> CurrentNamespace = () =>
            {
                if (_namespaces.Count == 0)
                    return null;
                return _namespaces.Peek();
            };
            Func<bool> InClass = () =>
            {
                return CurrentNamespace() is CClass;
            };
            Action<INamespaceItem> AddItem = (INamespaceItem item) =>
            {
                CNamespace n = _namespaces.Pop();
                n.AddItem(item);
                _namespaces.Push(n);
            };
            bool ignore = false;
            for (int i = 0; i < input.Count; ++i)
            {
                string s = input[i];
                if (s.StartsWith("#"))
                {
                    if (!InClass() && s.Substring(1).StartsWith("include"))
                    {
                        //Utilize include path or ignore?
                        //s = s.Substring(s.FindFirstNot(' ', 8) + 1).Substring(0, s.Length - 1);
                        input.RemoveAt(i--);
                    }
                    continue;
                }
                if (s.StartsWith("typedef"))
                {
                    s = "//" + s;
                    continue;
                }
                if (s.Contains("}"))
                {
                    if (InClass())
                    {
                        //must pop before calling current namespace
                        CClass c = _namespaces.Pop() as CClass;
                        AddItem(c);
                        continue;
                    }
                    //should not happen, but ignore if it does
                    continue;
                }
                if (ignore)
                {
                    input.RemoveAt(i--);
                    continue;
                }
                if (s.StartsWith("enum"))
                {
                    AddItem(CEnum.ParseEnum(ref i, ref input));
                    continue;
                }
                bool isStruct = false;
                if (s.StartsWith("class") || (isStruct = s.StartsWith("struct")))
                {
                    //cut out 'class' or 'struct' from the string
                    s = s.Substring(s.IndexOfNotAfter(' ', isStruct ? 6 : 5)).Trim();

                    int spaceIndex = s.IndexOf(' ');
                    if (spaceIndex > 0)
                        s = s.Substring(s.IndexOfNotAfter(' ', spaceIndex));

                    //find a semicolon.
                    //if there is one, this is just a forward declaration.
                    //if not, this is a full class definition.
                    int semicolonIndex = s.IndexOf(';');
                    if (semicolonIndex > 0)
                        AddItem(new CRefClass() { _isStruct = isStruct, _name = s.Substring(0, semicolonIndex) });
                    else
                    {
                        //actual class
                        s = s.Substring(s.IndexOfNotAfter(' '));
                        _namespaces.Push(new CClass() { _name = s, _isStruct = isStruct });
                        if (input[i + 1].Contains("{"))
                            ++i; //Skip open bracket
                    }
                    continue;
                }
                if (InClass())
                {
                    if (s.StartsWith("protected:") || s.StartsWith("private:"))
                    {
                        ignore = true;
                        input.RemoveAt(i--);
                        continue;
                    }
                    else if (s.StartsWith("public:"))
                    {
                        ignore = false;
                        continue;
                    }
                    else if (s.Contains('(') && s.Contains(')'))
                    {
                        //this is a method
                        AddItem(CMethod.ParseLine(s));
                        SkipCode(ref i);
                        continue;
                    }
                    else
                    {
                        //this is a field
                        AddItem(CField.ParseLine(s));
                    }
                }
            }
            return _namespaces.Pop();
        }

        private static void SkipCode(ref int i)
        {
            int temp = 0;

            //find first open bracket
            while (!input[i + temp].Contains("{"))
            {
                ++temp;
                if (temp > 3)
                    throw new Exception("open bracket not found");
            }

            if (input[i + temp].Contains('}'))
                return;

            int openSections = 0;
            do
            {
                if (input[i].Contains("{"))
                    ++openSections;
                else if (input[i].Contains("}"))
                    --openSections;
                ++i;
            }
            while (openSections > 0);
            --i;
        }

        private static void Clean()
        {
            const bool removeComments = true;
            const bool removeWhitespace = true;

            bool comment = false;
            for (int i = 0; i < input.Count; ++i)
            {
                int index;
                string s = input[i];
                if (comment)
                {
                    index = s.IndexOf("*/");
                    if (index >= 0)
                    {
                        comment = false;
                        if (removeComments)
                        {
                            index += 2;
                            if (index >= s.Length - 1)
                            {
                                input.RemoveAt(i--);
                                continue;
                            }
                            s = s.Substring(index, s.Length - index);
                        }
                    }
                    else if (removeComments)
                    {
                        input.RemoveAt(i--);
                        continue;
                    }
                }
                Top:
                index = s.IndexOf("/*");
                if (index >= 0)
                {
                    comment = true;
                    if (removeComments)
                    {
                        int index2 = s.IndexOf("*/");
                        if (index2 > 0)
                        {
                            comment = false;
                            index2 += 2;
                            if (index2 >= s.Length - 1)
                            {
                                if (index == 0)
                                {
                                    input.RemoveAt(i--);
                                    continue;
                                }
                                else
                                    s = s.Substring(0, index);
                            }
                            else
                            {
                                if (index == 0)
                                    s = s.Substring(index2, s.Length - index2);
                                else
                                {
                                    s = s.Substring(0, index) + " " + s.Substring(index2, s.Length - index2);
                                    //There might be more /* */ comments on the same line
                                    goto Top;
                                }
                            }
                        }
                        else
                        {
                            if (index == 0)
                            {
                                input.RemoveAt(i--);
                                continue;
                            }
                            s = s.Substring(0, index);
                        }
                    }
                }
                index = s.IndexOf("//");
                if (index >= 0)
                {
                    if (removeComments)
                    {
                        if (index == 0)
                        {
                            input.RemoveAt(i--);
                            continue;
                        }
                        s = s.Substring(0, index);
                    }
                }

                s = s.Replace("\t", " ").Trim();

                if (removeWhitespace && String.IsNullOrWhiteSpace(s))
                    input.RemoveAt(i--);
                else
                    input[i] = s;
            }
        }
    }
}
