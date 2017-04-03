using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Tools
{
    public static class BooleanExpressionParser
    {
        /// <summary>
        /// Parses a boolean expression and returns the result as a boolean primitive.
        /// </summary>
        /// <param name="expression">The expression to evaluate, as a string.</param>
        /// <param name="provider">The object to provide variable values.</param>
        /// <returns></returns>
        public static bool Evaluate(string expression, object provider)
        {
            Queue<String> queue = new Queue<String>();
            Stack<String> stack = new Stack<String>();
            ConvertToPostFix(expression, queue, stack);
            string result = GetAnswer(provider, queue, stack);
            return Boolean.Parse(result);
        }
        private static string[] SplitInFix(string inFix)
        {
            inFix = inFix.ReplaceWhitespace("").Replace(")", ") ").Replace("(", "( ");
            var ops = _precedence.SelectMany(x => x).ToList();
            for (int i = 0; i < inFix.Length; ++i)
            {
                char c = inFix[i];
                string cs = c.ToString();
                if (ops.Contains(cs))
                {
                    inFix = inFix.Insert(i++, " ");
                    if (inFix[i + 1] == c && ops.Contains(cs + cs))
                        ++i;
                    inFix = inFix.Insert(++i, " ");
                }
            }
            return inFix.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }
        private static void ConvertToPostFix(String inFix, Queue<String> queue, Stack<String> stack)
        {
            //Split up the expression by the spaces between each token
            String[] tokens = SplitInFix(inFix);
            //Loop through each token
            for (int i = 0; i < tokens.Length; ++i)
            {
                //Get current token from the array
                String token = tokens[i];
                //Ignore blank tokens
                if (token == null || token.Length == 0)
                    continue;
                //Push open parentheses to the stack
                if (token.Equals("("))
                    stack.Push(token);
                else if (token.Equals(")"))
                    //Enqueue stack until the next open parentheses in the stack
                    while (!stack.Peek().Equals("("))
                        queue.Enqueue(stack.Pop());
                else if (IsOperator(token))
                {
                    //enqueue stack as long as it is an operator
                    //and the token operator does not take precedence over it
                    while (stack.Count > 0 &&
                        !stack.Peek().Equals("(") &&
                        !ComparePrecedence(token, stack.Peek()))
                        queue.Enqueue(stack.Pop());

                    stack.Push(token);
                }
                else //is a number
                    queue.Enqueue(token);
            }
            //Queue everything left in the stack
            while (stack.Count > 0)
                queue.Enqueue(stack.Pop());
        }
        private static bool DigitsOnly(string str)
        {
            foreach (char c in str)
                if (c < '0' || c > '9')
                    return false;
            return true;
        }
        private static bool HexDigitsOnly(string str)
        {
            foreach (char c in str)
                if (c < '0' || c > 'F' || c == '@')
                    return false;
            return true;
        }
        private static object GetValue(string token, object provider)
        {
            token = token.Trim();

            FieldInfo[] fields = provider.GetType().GetFields();
            PropertyInfo[] properties = provider.GetType().GetProperties();

            int invertCount = 0;
            while (token.StartsWith("!"))
            {
                ++invertCount;
                token = token.Substring(1);
            }

            bool shouldInvert = (invertCount & 1) != 0;

            foreach (FieldInfo field in fields)
                if (field.Name.Equals(token))
                {
                    object value = field.GetValue(provider);
                    if (field.FieldType == typeof(bool))
                    {
                        if (shouldInvert)
                            return !(bool)value;
                    }
                    else if (invertCount > 0)
                        throw new Exception("Cannot invert non-bool");
                    return value;
                }

            foreach (PropertyInfo property in properties)
                if (property.Name.Equals(token))
                {
                    object value = property.GetValue(provider);
                    if (property.PropertyType == typeof(bool))
                    {
                        if (shouldInvert)
                            return !(bool)value;
                    }
                    else if (invertCount > 0)
                        throw new Exception("Cannot invert non-bool");
                    return value;
                }

            token = token.ToLower();

            //Handle boolean literals
            if (token.Equals("true") || token.Equals("false"))
            {
                bool value = bool.Parse(token);
                if (shouldInvert)
                    return !value;
                return value;
            }

            if (invertCount > 0)
                throw new Exception("Cannot invert non-bool");

            //Handle numeric literals
            bool isIntegral = !token.Contains(".");
            if (isIntegral)
            {
                bool isHex = token.StartsWith("0x");
                if (isHex)
                    token = token.Substring(2);
                bool forceLong = false, forceUnsigned = false;
                if (token.EndsWith("l"))
                {
                    forceLong = true;
                    token = token.Substring(0, token.Length - 1);
                    if (token.EndsWith("u"))
                    {
                        forceUnsigned = true;
                        token = token.Substring(0, token.Length - 1);
                    }
                }
                else if (token.EndsWith("u"))
                {
                    forceUnsigned = true;
                    token = token.Substring(0, token.Length - 1);
                    if (token.EndsWith("l"))
                    {
                        forceLong = true;
                        token = token.Substring(0, token.Length - 1);
                    }
                }
                if (isHex)
                {
                    if (HexDigitsOnly(token))
                    {
                        if (forceUnsigned)
                        {
                            if (forceLong)
                                return ulong.Parse(token);
                            else
                                return uint.Parse(token);
                        }
                        else
                        {
                            if (forceLong)
                                return long.Parse(token);
                            else
                                return int.Parse(token);
                        }
                    }
                }
                else
                {
                    if (DigitsOnly(token))
                    {
                        if (forceUnsigned)
                        {
                            if (forceLong)
                                return ulong.Parse(token);
                            else
                                return uint.Parse(token);
                        }
                        else
                        {
                            if (forceLong)
                                return long.Parse(token);
                            else
                                return int.Parse(token);
                        }
                    }
                }
            }
            else
            {
                bool
                    forceSingle = token.EndsWith("f"),
                    forceDouble = token.EndsWith("d"),
                    forceDecimal = token.EndsWith("m");
                if (forceSingle || forceDouble || forceDecimal)
                    token = token.Substring(0, token.Length - 1);
                string[] parts = token.Split('.');
                if (DigitsOnly(parts[0]) && DigitsOnly(parts[1]))
                {
                    if (forceSingle)
                        return float.Parse(token);
                    if (forceDecimal)
                        return decimal.Parse(token);
                    //if (forceDouble)
                        return double.Parse(token);
                }
            }
            throw new Exception();
        }
        public static readonly Dictionary<string, string[]> _implicitConversions = new Dictionary<string, string[]>()
        {
            { "SByte",  new string[] { "Int16", "Int32", "Int64", "Single", "Double", "Decimal" } },
            { "Byte",   new string[] { "Int16", "UInt16", "Int32", "UInt32", "Int64", "UInt64", "Single", "Double", "Decimal" } },
            { "Int16",  new string[] { "Int32", "Int64", "Single", "Double", "Decimal" } },
            { "UInt16", new string[] { "Int32", "UInt32", "Int64", "UInt64", "Single", "Double", "Decimal" } },
            { "Int32",  new string[] { "Int64", "Single", "Double", "Decimal" } },
            { "UInt32", new string[] { "Int64", "UInt64", "Single", "Double", "Decimal" } },
            { "Int64",  new string[] { "Single", "Double", "Decimal" } },
            { "UInt64", new string[] { "Single", "Double", "Decimal" } },
            { "Char",   new string[] { "UInt16", "Int32", "UInt32", "Int64", "UInt64", "Single", "Double", "Decimal" } },
            { "Single", new string[] { "Single", "Double" } },
        };
        private static String GetAnswer(object provider, Queue<String> queue, Stack<String> stack)
        {
            //Dequeue sorted operators and numbers from the queue until the queue is empty
            while (queue.Count > 0)
            {
                String token = queue.Dequeue();
                if (IsOperator(token))
                {
                    //Get the second operand first
                    String stackToken2 = stack.Pop();
                    //First operand comes second
                    String stackToken1 = stack.Pop();

                    //Parse each operand as a double so they can be evaluated
                    object value1 = GetValue(stackToken1, provider);
                    object value2 = GetValue(stackToken2, provider);

                    Type t1 = value1.GetType();
                    Type t2 = value2.GetType();
                    if (t1.BaseType == typeof(Enum))
                        t1 = t1.BaseType;
                    if (t2.BaseType == typeof(Enum))
                        t2 = t2.BaseType;

                    string result = "";
                    switch (token)
                    {
                        case "==":
                            result = (value1 == value2).ToString();
                            break;
                        case "!=":
                            result = (value1 != value2).ToString();
                            break;
                        case "&&":
                            if (t1.BaseType != typeof(bool) ||
                                t2.BaseType != typeof(bool))
                                throw new Exception("Cannot logical-and non-boolean types.");
                            result = ((bool)value1 && (bool)value2).ToString();
                            break;
                        case "||":
                            if (t1.BaseType != typeof(bool) ||
                                t2.BaseType != typeof(bool))
                                throw new Exception("Cannot logical-or non-boolean types.");
                            result = ((bool)value1 || (bool)value2).ToString();
                            break;
                        case "*":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot multiply non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "/":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot divide non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "+":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot add non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "-":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot subtract non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "<<":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot left-shift non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case ">>":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot right-shift non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "<":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot compare non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case ">":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot compare non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "<=":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot compare non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case ">=":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot compare non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            break;
                        case "&":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot bitwise-and non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            //result = ((long)value1 & (long)value2).ToString();
                            break;
                        case "^":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot bitwise-xor non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            //result = ((long)value1 ^ (long)value2).ToString();
                            break;
                        case "|":
                            if (t1.BaseType != typeof(ValueType) ||
                                t2.BaseType != typeof(ValueType))
                                throw new Exception("Cannot bitwise-or non-value types.");
                            //TODO
                            throw new NotImplementedException();
                            //result = ((long)value1 | (long)value2).ToString();
                            break;
                    }
                    //Push the result back onto the stack
                    stack.Push(result);
                }
                else if (!token.Equals("("))
                {
                    //Push numbers directly to the stack; ignore '('
                    stack.Push(GetValue(token, provider).ToString());
                }
            }
            //The result of the expression is now the only number in the stack
            return stack.Pop();
        }

        private static bool IsOperator(String s)
            => _precedence.Any(x => x.Contains(s));
        
        public static readonly string[][] _precedence = new string[][]
        {
            new string[] { "*", "/" },
            new string[] { "+", "-" },
            new string[] { "<<", ">>" },
            new string[] { "<", ">", "<=", ">=" },
            new string[] { "==", "!=" },
            new string[] { "&" },
            new string[] { "^" },
            new string[] { "|" },
            new string[] { "&&" },
            new string[] { "||" },
        };
        
        // True if op1 >= op2 in precedence.
        private static bool ComparePrecedence(String op1, String op2)
        {
            int op1p = -1, op2p = -1;
            for (int i = 0; i < _precedence.Length; ++i)
            {
                string[] ops = _precedence[i];
                foreach (string s in ops)
                {
                    if (op1.Equals(s))
                        op1p = i;
                    if (op2.Equals(s))
                        op2p = i;
                    if (op1p >= 0 && op2p >= 0)
                        return op1p >= op2p;
                }
            }
            return false;
        }
    }
}
