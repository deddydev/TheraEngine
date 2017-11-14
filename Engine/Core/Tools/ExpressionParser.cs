using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TheraEngine.Tools
{
    public static class ExpressionParser
    {
        /// <summary>
        /// Parses an expression and returns the resulting primitive type.
        /// </summary>
        /// <param name="expression">The expression to evaluate, as a string.</param>
        /// <param name="provider">The object to provide variable values.</param>
        public static T Evaluate<T>(string expression, object provider)
            where T : struct, IComparable, IConvertible, IComparable<T>, IEquatable<T>
        {
            Queue<String> queue = new Queue<String>();
            Stack<String> stack = new Stack<String>();
            ConvertToPostFix(expression, queue, stack);
            string result = GetAnswer(provider, queue, stack);
            T value = (T)GetValue(result, provider);
            return value;
        }
        private static string[] SplitInFix(string inFix)
        {
            inFix = inFix.ReplaceWhitespace("").Replace(")", " ) ").Replace("(", " ( ");
            var ops = _precedence.SelectMany(x => x).ToList();
            foreach (var o in ops)
                inFix = inFix.Replace(o, " " + o + " ");
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

            if (token == "null")
                return null;

            string origToken = token;

            int invertCount = 0;
            while (token.StartsWith("!"))
            {
                ++invertCount;
                token = token.Substring(1);
            }

            bool shouldInvert = (invertCount & 1) != 0;

            if (token.StartsWith("\""))
            {
                if (token.EndsWith("\""))
                {
                    string str = token.Substring(1, token.Length - 2);
                    for (int i = 0; i < str.Length; ++i)
                    {
                        char c = str[i];
                        if (c == '\\')
                            str = str.Remove(i, 1);
                    }
                    return str;
                }
                else
                    throw new Exception("Invalid string: " + token);
            }

            int paramStart = token.IndexOf("(");
            if (paramStart > 0)
            {
                int paramEnd = token.LastIndexOf(")");
                if (paramEnd > paramStart)
                {
                    string methodName = token.Substring(0, paramStart);
                    MethodInfo[] methods = provider == null ? new MethodInfo[0] : provider.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(x => string.Equals(x.Name, methodName)).ToArray();
                    var parameters = token.Substring(paramStart + 1, paramEnd - (paramStart + 1)).Split(',');
                    
                    //TODO: invoke matching method, return value
                }
                else
                    throw new Exception("Invalid method: " + token);
            }

            FieldInfo[] fields = provider == null ? new FieldInfo[0] : provider.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            PropertyInfo[] properties = provider == null ? new PropertyInfo[0] : provider.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            FieldInfo field = fields.FirstOrDefault(x => x.Name.Equals(token));
            if (field != null)
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

            PropertyInfo property = properties.FirstOrDefault(x => x.Name.Equals(token));
            if (property != null)
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

            token = token.ToLowerInvariant();

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
            Engine.LogWarning("Token not recognized: " + origToken);
            return null;
        }
        private static readonly Dictionary<string, string[]> _implicitConversions = new Dictionary<string, string[]>()
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

                    //Parse each operand so they can be evaluated
                    object value1 = GetValue(stackToken1, provider);
                    object value2 = GetValue(stackToken2, provider);

                    if (value1 == null)
                    {
                        bool isNull = value2 == null;
                        if (token == "==")
                            stack.Push(isNull ? "true" : "false");
                        else if (token == "!=")
                            stack.Push(isNull ? "false" : "true");
                        else
                            throw new Exception();
                        continue;
                    }
                    else if (value2 == null)
                    {
                        if (token == "==")
                            stack.Push("false");
                        else if (token == "!=")
                            stack.Push("true");
                        else
                            throw new Exception();
                        continue;
                    }

                    Type t1 = value1.GetType();
                    Type t2 = value2.GetType();
                    if (t1.BaseType == typeof(Enum))
                        t1 = t1.BaseType;
                    if (t2.BaseType == typeof(Enum))
                        t2 = t2.BaseType;

                    string result = "";
                    switch (token)
                    {
                        case "&&":
                            if (t1.BaseType != typeof(bool) || t2.BaseType != typeof(bool))
                                throw new Exception("Cannot logical-and non-boolean types.");
                            result = ((bool)value1 && (bool)value2).ToString();
                            break;
                        case "||":
                            if (t1.BaseType != typeof(bool) || t2.BaseType != typeof(bool))
                                throw new Exception("Cannot logical-or non-boolean types.");
                            result = ((bool)value1 || (bool)value2).ToString();
                            break;
                        case "*":
                        case "/":
                        case "+":
                        case "-":
                        case "<<":
                        case ">>":
                        case "<":
                        case ">":
                        case "<=":
                        case ">=":
                        case "&":
                        case "^":
                        case "|":
                        case "==":
                        case "!=":
                            result = EvaluateOperation(t1, t2, value1, value2, token);
                            break;
                        default:
                            throw new Exception(string.Format("Token \"{0}\" not supported.", token));
                    }
                    //Push the result back onto the stack
                    stack.Push(result);
                }
                else if (!token.Equals("("))
                {
                    //Push numbers directly to the stack; ignore '('
                    //object value = GetValue(token, provider);
                    //stack.Push(value == null ? "null" : value.ToString());
                    stack.Push(token);
                }
            }
            //The result of the expression is now the only number in the stack
            return stack.Pop();
        }
        private static string EvaluateOperation(
            Type t1, Type t2,
            object value1, object value2,
            string token)
        {
            if (t1.BaseType != typeof(ValueType) || t2.BaseType != typeof(ValueType))
                throw new Exception(string.Format("Cannot evaluate {0} {1] {2]", t1.Name, token, t2.Name));

            //types are the same?
            if (string.Equals(t1.Name, t2.Name))
                return EvalToken(value1, value2, token, t1.Name);

            //t1 can be converted to t2's type?
            else if (_implicitConversions.ContainsKey(t1.Name) &&
                _implicitConversions[t1.Name].FirstOrDefault(x => string.Equals(x, t2.Name)) != null)
                return EvalToken(value1, value2, token, t2.Name);

            //t2 can be converted to t1's type?
            else if (_implicitConversions.ContainsKey(t2.Name) &&
                _implicitConversions[t2.Name].FirstOrDefault(x => string.Equals(x, t1.Name)) != null)
                return EvalToken(value1, value2, token, t1.Name);

            //both can be converted to a next largest common type?
            else if (_implicitConversions.ContainsKey(t1.Name) &&
                _implicitConversions.ContainsKey(t2.Name))
            {
                string[] commonImplicitTypes = _implicitConversions[t1.Name].Intersect(_implicitConversions[t2.Name]).ToArray();
                if (commonImplicitTypes.Length > 0)
                    return EvalToken(value1, value2, token, commonImplicitTypes[0]);
                else
                    throw new Exception(string.Format("Cannot evaluate {0} {1] {2]", t1.Name, token, t2.Name));
            }
            else
                throw new Exception(string.Format("Cannot evaluate {0} {1] {2]", t1.Name, token, t2.Name)); 
        }
        private static bool IsOperator(String s)
            => _precedence.Any(x => x.Contains(s));
        private static readonly string[][] _precedence = new string[][]
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
        private static string EvalToken(object value1, object value2, string token, string commonType)
        {
            switch (token)
            {
                case "*":
                    return EvalMult(value1, value2, commonType);
                case "/":
                    return EvalDiv(value1, value2, commonType);
                case "+":
                    return EvalAdd(value1, value2, commonType);
                case "-":
                    return EvalSub(value1, value2, commonType);
                case "<<":
                    return EvalLeftShift(value1, value2, commonType);
                case ">>":
                    return EvalRightShift(value1, value2, commonType);
                case "<":
                    return EvalLess(value1, value2, commonType);
                case ">":
                    return EvalGreater(value1, value2, commonType);
                case "<=":
                    return EvalLessEqual(value1, value2, commonType);
                case ">=":
                    return EvalGreaterEqual(value1, value2, commonType);
                case "&":
                    return EvalAnd(value1, value2, commonType);
                case "^":
                    return EvalXor(value1, value2, commonType);
                case "|":
                    return EvalOr(value1, value2, commonType);
                case "==":
                    return EvalEquals(value1, value2, commonType);
                case "!=":
                    return EvalNotEquals(value1, value2, commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalMult(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 * (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 * (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 * (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 * (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 * (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 * (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 * (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 * (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 * (float)value2).ToString();
                case "Double":
                    return ((double)value1 * (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 * (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalDiv(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 / (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 / (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 / (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 / (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 / (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 / (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 / (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 / (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 / (float)value2).ToString();
                case "Double":
                    return ((double)value1 / (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 / (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalAdd(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 + (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 + (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 + (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 + (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 + (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 + (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 + (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 + (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 + (float)value2).ToString();
                case "Double":
                    return ((double)value1 + (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 + (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalSub(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 - (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 - (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 - (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 - (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 - (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 - (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 - (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 - (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 - (float)value2).ToString();
                case "Double":
                    return ((double)value1 - (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 - (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalLeftShift(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 << (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 << (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 << (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 << (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 << (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 << (int)value2).ToString();
                case "Int64":
                    return ((long)value1 << (int)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 << (int)value2).ToString();
                case "Single":
                case "Double":
                case "Decimal":
                    throw new Exception("Cannot left shift " + commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalRightShift(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 >> (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 >> (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 >> (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 >> (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 >> (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 >> (int)value2).ToString();
                case "Int64":
                    return ((long)value1 >> (int)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 >> (int)value2).ToString();
                case "Single":
                case "Double":
                case "Decimal":
                    throw new Exception("Cannot right shift " + commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalAnd(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 & (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 & (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 & (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 & (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 & (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 & (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 & (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 & (ulong)value2).ToString();
                case "Single":
                case "Double":
                case "Decimal":
                    throw new Exception("Cannot bitwise-and " + commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalXor(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 ^ (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 ^ (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 ^ (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 ^ (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 ^ (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 ^ (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 ^ (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 ^ (ulong)value2).ToString();
                case "Single":
                case "Double":
                case "Decimal":
                    throw new Exception("Cannot bitwise-xor " + commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalOr(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 | (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 | (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 | (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 | (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 | (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 | (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 | (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 | (ulong)value2).ToString();
                case "Single":
                case "Double":
                case "Decimal":
                    throw new Exception("Cannot bitwise-or " + commonType);
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalLess(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 < (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 < (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 < (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 < (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 < (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 < (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 < (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 < (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 < (float)value2).ToString();
                case "Double":
                    return ((double)value1 < (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 < (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalGreater(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 > (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 > (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 > (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 > (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 > (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 > (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 > (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 > (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 > (float)value2).ToString();
                case "Double":
                    return ((double)value1 > (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 > (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalLessEqual(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 <= (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 <= (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 <= (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 <= (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 <= (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 <= (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 <= (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 <= (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 <= (float)value2).ToString();
                case "Double":
                    return ((double)value1 <= (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 <= (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalGreaterEqual(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "SByte":
                    return ((sbyte)value1 >= (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 >= (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 >= (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 >= (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 >= (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 >= (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 >= (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 >= (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 >= (float)value2).ToString();
                case "Double":
                    return ((double)value1 >= (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 >= (decimal)value2).ToString();
            }
            throw new Exception("Not a numeric primitive type");
        }
        private static string EvalEquals(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "Boolean":
                    return ((bool)value1 == (bool)value2).ToString();
                case "SByte":
                    return ((sbyte)value1 == (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 == (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 == (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 == (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 == (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 == (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 == (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 == (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 == (float)value2).ToString();
                case "Double":
                    return ((double)value1 == (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 == (decimal)value2).ToString();
            }
            throw new Exception("Not a primitive type");
        }
        private static string EvalNotEquals(object value1, object value2, string commonType)
        {
            switch (commonType)
            {
                case "Boolean":
                    return ((bool)value1 != (bool)value2).ToString();
                case "SByte":
                    return ((sbyte)value1 != (sbyte)value2).ToString();
                case "Byte":
                    return ((byte)value1 != (byte)value2).ToString();
                case "Int16":
                    return ((short)value1 != (short)value2).ToString();
                case "UInt16":
                    return ((ushort)value1 != (ushort)value2).ToString();
                case "Int32":
                    return ((int)value1 != (int)value2).ToString();
                case "UInt32":
                    return ((uint)value1 != (uint)value2).ToString();
                case "Int64":
                    return ((long)value1 != (long)value2).ToString();
                case "UInt64":
                    return ((ulong)value1 != (ulong)value2).ToString();
                case "Single":
                    return ((float)value1 != (float)value2).ToString();
                case "Double":
                    return ((double)value1 != (double)value2).ToString();
                case "Decimal":
                    return ((decimal)value1 != (decimal)value2).ToString();
            }
            throw new Exception("Not a primitive type");
        }
    }
}
