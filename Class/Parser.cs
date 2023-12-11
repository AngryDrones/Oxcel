using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace Oxcel.Class
{
    class Parser
    {
        const string oneAndZeroMsg = "1 and 0 allowed only";

        public static int ParseExpression(string input, ObservableCollection<ObservableCollection<CellItem>> cells)
        {
            // Перевід операндів і операцій в токени
            string[] tokens = input.Split(' ');

            // Відповідні для них стеки
            Stack<int> operandStack = new Stack<int>();
            Stack<string> operatorStack = new Stack<string>();

            foreach (string token in tokens)
            {
                if (int.TryParse(token, out int number))
                {
                    // Токен операнд
                    operandStack.Push(number);
                }
                else if (IsOperator(token))
                {
                    // Токен оператор
                    while (operatorStack.Count > 0 && IsHigherPrecedence(operatorStack.Peek(), token))
                    {
                        PerformOperation(operandStack, operatorStack);
                    }
                    operatorStack.Push(token);
                }
                // Посилання на клітинку
                else if (IsCellReference(token))
                {
                    var cellValue = GetCellValue(cells, token);
                    operandStack.Push(cellValue);
                }

                else
                {
                    throw new ArgumentException("Invalid token");
                }
            }

            // Залишок операндів
            while (operatorStack.Count > 0)
            {
                PerformOperation(operandStack, operatorStack);
            }

            if (operandStack.Count == 1 && operatorStack.Count == 0)
            {
                return operandStack.Peek();
            }
            else
            {
                throw new ArgumentException("Invalid input");
            }
        }

        static void PerformOperation(Stack<int> operandStack, Stack<string> operatorStack)
        {
            string op = operatorStack.Pop();

            if (op == "AND")
            {
                int b = operandStack.Pop();
                int a = operandStack.Pop();
                if ((a != 0 && a != 1) || (b != 0 && b != 1))
                {
                    throw new ArgumentException(oneAndZeroMsg);
                }

                operandStack.Push(a & b); // AND
            }
            else if (op == "OR")
            {
                int b = operandStack.Pop();
                int a = operandStack.Pop();
                if ((a != 0 && a != 1) || (b != 0 && b != 1))
                {
                    throw new ArgumentException(oneAndZeroMsg);
                }
                operandStack.Push(a | b); // OR
            }
            else if (op == "NOT")
            {
                int a = operandStack.Pop();
                if (a != 0 && a != 1)
                {
                    throw new ArgumentException(oneAndZeroMsg);
                }
                operandStack.Push(a == 0 ? 1 : 0); // NOT
            }
            else if (op == "<")
            {
                int b = operandStack.Pop();
                int a = operandStack.Pop();
                operandStack.Push(a < b ? 1 : 0); // Менше ніж
            }
            else if (op == ">")
            {
                int b = operandStack.Pop();
                int a = operandStack.Pop();
                operandStack.Push(a > b ? 1 : 0); // Більше ніж
            }
            else if (op == "=")
            {
                int b = operandStack.Pop();
                int a = operandStack.Pop();
                operandStack.Push(a == b ? 1 : 0); // Equals to
            }
            else if (op == "+")
            {
                int a = operandStack.Pop();
                operandStack.Push(a); // Унарний плюс
            }
            else if (op == "-")
            {
                int a = operandStack.Pop();
                operandStack.Push(-a); // Унарний мінус
            }
        }

        static bool IsOperator(string token)
        {
            return token == "AND" || token == "OR" || token == "NOT" || token == "<" || token == ">" || token == "=" || token == "+" || token == "-";
        }

        static bool IsHigherPrecedence(string op1, string op2)
        {
            if ((op1 == "AND" || op1 == "OR") && (op2 == "<" || op2 == ">" || op2 == "=" || op2 == "NOT" || op2 == "+" || op2 == "-"))
            {
                return false;
            }
            return true;
        }

        private static bool IsCellReference(string token)
        {
            return Regex.IsMatch(token, @"^[A-Z]\d+$");
        }

        private static int GetCellValue(ObservableCollection<ObservableCollection<CellItem>> cells, string cellReference)
        {
            // Отримати рядок та колону
            var column = cellReference[0] - 'A';
            var row = int.Parse(cellReference.Substring(1)) - 1;

            return cells[row][column].CellValue;
        }
    }
}