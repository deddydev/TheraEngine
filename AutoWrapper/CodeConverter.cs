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
        public static void Convert(string fileName, string inputDir, string outputDir)
        {
            string inPath = inputDir + "\\" + fileName + ".h";
            string outPathHeader = outputDir + "\\" + fileName + ".h";
            string outPathCode = outputDir + "\\" + fileName + ".cpp";
            string origFile = File.ReadAllText(inPath);

            List<string> code = origFile.Split(new string[] { Environment.NewLine, ";" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            Clean(ref code, true, true);

            File.WriteAllText(outPathHeader, string.Join(Environment.NewLine, code));
        }
        private static void Clean(ref List<string> input, bool removeComments, bool removeWhitespace)
        {
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
                index = s.IndexOf("/*");
                if (index >= 0)
                {
                    comment = true;
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
                if (s.Contains("private:") || s.Contains("protected:"))
                {

                }

                if (removeWhitespace && String.IsNullOrWhiteSpace(s))
                    input.RemoveAt(i--);
                else
                    input[i] = s;
            }
        }
    }

    public class CMethod
    {
        public enum Type
        {
            _virtual,
            _abstract,
            _override,
            _inline,
        }

        public Type _type;
        public string _returnType;
    }

    public class CClass
    {
        public string _name;
        public List<CMethod> _publicMethods;
    }
}
