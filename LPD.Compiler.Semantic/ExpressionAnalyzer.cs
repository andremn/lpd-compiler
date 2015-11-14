using LPD.Compiler.Lexical;
using static LPD.Compiler.Semantic.Properties.Resource;
using LPD.Compiler.SymbolsTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LPD.Compiler.Semantic
{
    public class ExpressionAnalyzer
    {
        private List<Token> _output;
        private List<Token> _tokens;

        private readonly Dictionary<Symbols, ushort> _priorities = new Dictionary<Symbols, ushort>
        {
            [Symbols.SNao] = 5,
            [Symbols.SMaisUnario] = 5,
            [Symbols.SMenosUnario] = 5,
            [Symbols.SMult] = 4,
            [Symbols.SDiv] = 4,
            [Symbols.SMais] = 3,
            [Symbols.SMenos] = 3,
            [Symbols.SMaior] = 2,
            [Symbols.SMaiorIg] = 2,
            [Symbols.SMenor] = 2,
            [Symbols.SMenorIg] = 2,
            [Symbols.SDif] = 2,
            [Symbols.SIg] = 2,
            [Symbols.SE] = 1,
            [Symbols.SOu] = 0,
        };

        public ExpressionAnalyzer()
        {
            _tokens = new List<Token>();
            _output = new List<Token>();
        }

        public void Add(Token token)
        {
            if (token.Symbol != Symbols.SNumero && token.Symbol != Symbols.SIdentificador && token.Symbol != Symbols.SAbreParenteses)
            {
                if (token.Symbol == Symbols.SFechaParenteses)
                {
                    for (int i = _tokens.Count - 1; i >= 0; i--)
                    {
                        var current = _tokens[i];

                        if (current.Symbol != Symbols.SAbreParenteses)
                        {
                            _output.Add(current);
                        }
                        else
                        {
                            _tokens.RemoveAt(i);
                            break;
                        }

                        _tokens.RemoveAt(i);
                    }

                    return;                 
                }

                for (int i = _tokens.Count - 1; i >= 0; i--)
                {
                    var current = _tokens[i];

                    if (current.Symbol == Symbols.SAbreParenteses)
                    {
                        break;
                    }

                    var priority1 = _priorities[current.Symbol];
                    var priority2 = _priorities[token.Symbol]; 
                    
                    if (priority1 >= priority2)
                    {
                        _output.Add(current);
                        _tokens.RemoveAt(i);
                    }
                }

                if (token.Symbol != Symbols.SAbreParenteses)
                {
                    _tokens.Add(token);
                }
            }
            else
            {
                if (token.Symbol == Symbols.SAbreParenteses)
                {
                    _tokens.Add(token);
                }
                else
                {
                    if (token.Symbol != Symbols.SFechaParenteses)
                    {
                        _output.Add(token);
                    }
                }
            }
        }

        public ItemType Analyze(VectorSymbolTable symbolTable)
        {
            for (int i = _tokens.Count - 1; i >= 0; i--)
            {
                _output.Add(_tokens[i]);
            }

            ItemType result = ItemType.None;
            Stack<ItemType> types = new Stack<ItemType>();

            foreach (var token in _output)
            {
                var symbol = token.Symbol;

                if (symbol == Symbols.SNao)
                {
                    var firstOperand = types.Pop();

                    if (firstOperand == ItemType.Boolean)
                    {
                        types.Push(ItemType.Boolean);
                        result = ItemType.Boolean;
                    }
                    else
                    {
                        throw new InvalidOperationException(InvalidOperandNotOperator);
                    }
                }
                else if (symbol == Symbols.SIdentificador)
                {
                    var identificator = symbolTable.Search(token.Lexeme) as IdentificatorItem;

                    if (identificator != null)
                    {
                        types.Push(identificator.Type);
                        result = identificator.Type;
                        continue;
                    }

                    var func = symbolTable.Search(token.Lexeme) as FunctionItem;

                    if (func != null)
                    {
                        types.Push(func.Type);
                        result = identificator.Type;
                    }
                }
                else if (token.Symbol == Symbols.SNumero)
                {
                    types.Push(ItemType.Integer);
                }
                else if (token.Symbol == Symbols.SVerdadeiro || token.Symbol == Symbols.SFalso)
                {
                    types.Push(ItemType.Boolean);
                }
                else
                {
                    var firstType = types.Pop();
                    var type = ItemType.None;

                    //Unary operator expects only one operand and it always results in a integer expression.
                    if (token.Symbol == Symbols.SMaisUnario || token.Symbol == Symbols.SMenosUnario)
                    {
                        if (firstType != ItemType.Integer)
                        {
                            throw new InvalidOperationException(string.Format(InvalidOperatorInExpression, token.Lexeme, firstType));
                        }

                        type = ItemType.Integer;
                    }
                    else
                    {
                        type = GetExpressionType(token, firstType, types.Pop());
                    }

                    if (type == ItemType.None)
                    {
                        throw new InvalidOperationException("Token inválido.");
                    }

                    types.Push(type);
                    result = type;
                }
            }

            return result;
        }

        public string GetOutput()
        {
            return string.Join(string.Empty, _output.Select(token => token.Lexeme));
        }

        public void Reset()
        {
            _tokens.Clear();
            _output.Clear();
        }

        private ItemType GetExpressionType(Token op, ItemType firstType, ItemType secondType)
        {
            if (op.Symbol == Symbols.SMais || op.Symbol == Symbols.SMenos ||
                    op.Symbol == Symbols.SDiv || op.Symbol == Symbols.SMult)
            {
                if (firstType == ItemType.Integer && secondType == ItemType.Integer)
                {
                    return ItemType.Integer;
                }
                else
                {
                    ThrowInvalidOperandError(op.Lexeme, firstType, secondType);
                }
            }
            else if (op.Symbol == Symbols.SMaior || op.Symbol == Symbols.SMenor ||
                       op.Symbol == Symbols.SMaiorIg || op.Symbol == Symbols.SMenorIg ||
                       op.Symbol == Symbols.SIg || op.Symbol == Symbols.SDif)
            {
                if (firstType == ItemType.Integer && secondType == ItemType.Integer)
                {
                    return ItemType.Boolean;
                }
                else
                {
                    ThrowInvalidOperandError(op.Lexeme, firstType, secondType);
                }
            }
            else if (op.Symbol == Symbols.SE || op.Symbol == Symbols.SOu)
            {
                if (firstType == ItemType.Boolean && secondType == ItemType.Boolean)
                {
                    return ItemType.Boolean;
                }
                else
                {
                    ThrowInvalidOperandError(op.Lexeme, firstType, secondType);
                }
            }

            return ItemType.None;
        }

        private void ThrowInvalidOperandError(string operatorLexeme, ItemType firstType, ItemType secondType)
        {
            string message = string.Empty;

            if (firstType == secondType)
            {
                message = string.Format(InvalidOperatorInExpression, operatorLexeme, firstType.GetFriendlyName());
            }
            else
            {
                message = string.Format(InvalidOperandsInExpression, operatorLexeme, 
                    firstType.GetFriendlyName(), secondType.GetFriendlyName());
            }
            
            throw new InvalidOperationException(message);
        }
    }
}
