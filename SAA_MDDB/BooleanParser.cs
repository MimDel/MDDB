using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAA_MDDB
{
    internal class BooleanParser
    {
        public static bool EvaluateBooleanExpression(string expression)
        {
            Stack<object> stack = new Stack<object>();
            MyList<char> operators = new MyList<char> { '&', '|' };

            foreach (char c in expression)
            {
                if (c == 'T' || c == 'F')
                {
                    stack.Push(c == 'T');
                }
                else if (operators.Contains(c))
                {
                    stack.Push(c);
                }
                else if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    List<object> subExpression = new List<object>();
                    while (stack.Peek() is not char || (char)stack.Peek() != '(')
                    {
                        subExpression.Add(stack.Pop());
                    }
                    stack.Pop();
                    subExpression.Reverse();

                    object operand1 = subExpression[0];
                    for (int i = 1; i < subExpression.Count; i += 2)
                    {
                        char op = (char)subExpression[i];
                        object operand2 = subExpression[i + 1];
                        operand1 = EvaluateOperator(op, operand1, operand2);
                    }
                    stack.Push(operand1);
                }
            }

            while (stack.Count > 1)
            {
                object operand1 = stack.Pop();
                char op = (char)stack.Pop();
                object operand2 = stack.Pop();
                object result = EvaluateOperator(op, operand1, operand2);
                stack.Push(result);
            }

            return (bool)stack.Pop();
        }

        private static object EvaluateOperator(char op, object operand1, object operand2)
        {
            bool boolOperand1 = (bool)operand1;
            bool boolOperand2 = (bool)operand2;

            switch (op)
            {
                case '&':
                    return boolOperand1 && boolOperand2;
                case '|':
                    return boolOperand1 || boolOperand2;
                default:
                    throw new ArgumentException($"Invalid operator: {op}");
            }
        }

        public static string ExpresionToTrueOrFalse(MyList<Cell> row, string expression)
        {
            if (expression[0] != '(')
            {
                expression = "( " + expression + " )";
            }
            var exp = StringHelper.SplitAttributesIncludeIgnore(expression, '"', ' ');
            string c = "";
            string output = "";

            for (int i = 0; i < exp.Length; i++)
            {
                if (exp[i] == "and" || exp[i] == "or" || exp[i] == ")")
                {
                    if (c == "")
                    {
                        output += ")";
                        break;
                    }

                    var comp = Compare(row, StringHelper.Trim(c));
                    if (!comp.Success)
                    {
                        Console.WriteLine(comp.ErrorMessage);
                        return "";
                    }

                    char opr = ')';
                    if (exp[i] == "and")
                        opr = '&';
                    else if (exp[i] == "or")
                        opr = '|';

                    if (comp.Result)
                        output += "T" + " " + opr + " ";
                    else
                        output += "F" + " " + opr + " ";

                    c = "";
                }
                else if (exp[i] == "(")
                {
                    output += "(" + " ";
                }
                else if (exp[i] == ")")
                {
                    output += ")" + " ";
                }
                else
                {
                    c += exp[i] + " ";
                }
            }

            return output;
        }

        private static ExpressionResult Compare(MyList<Cell> row, string expression)
        {
            var exp = StringHelper.SplitAttributesIncludeIgnore(expression, '"', ' ');
            Cell? cell = null;
            string left;
            string opr;
            string right;
            bool notInExp = false;
            bool endResult = false;
            if (exp[0] == "not")
            {
                notInExp = true;
                left = exp[1];
                opr = exp[2];
                right = exp[3];
            }
            else
            {
                left = exp[0];
                opr = exp[1];
                right = exp[2];
            }

            if (right[0] == '"')
            {
                right = StringHelper.Substring(right, 1, right.Length - 1);
            }

            for (int i = 0; i < row.Count; i++)
            {
                if (left == row[i].ColName)
                {
                    cell = row[i];
                    break;
                }
            }

            if (cell == null)
                return new ExpressionResult(false, false, "Not valid colnm name.");

            int result = -2;
            switch (cell.Type)
            {
                case MDDBType.Int:
                    result = int.Parse(cell.Value).CompareTo(int.Parse(right));
                    break;
                case MDDBType.String:
                    result = StringHelper.CompareStrings(cell.Value, right);
                    break;
                case MDDBType.Date:
                    result = DateTime.Compare(DateTime.Parse(cell.Value), DateTime.Parse(right));
                    break;
            }

            if (result == -2)
                return new ExpressionResult(false, false, "Invalid type.");

            switch (opr)
            {
                case "<>": endResult = (result != 0); break;
                case "=": endResult = (result == 0); break;
                case ">": endResult = (result == 1); break;
                case "<": endResult = (result == -1); break;
                case "<=": endResult = (result == -1 || result == 0); break;
                case ">=": endResult = (result == 1 || result == 0); break;
                default: return new ExpressionResult(false, false, "Invalid operator.");
            }

            if (notInExp)
                return new ExpressionResult(!endResult);

            return new ExpressionResult(endResult);
        }
    }
}
