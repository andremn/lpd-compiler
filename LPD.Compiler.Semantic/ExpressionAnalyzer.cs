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
        private List<ExpressionItem> _output;
        private List<ExpressionItem> _items;
        private VectorSymbolTable _symbolTable;

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

        private readonly Dictionary<Symbols, ItemType> _operatorsType = new Dictionary<Symbols, ItemType>
        {
            [Symbols.SMaisUnario] = ItemType.Integer,
            [Symbols.SMenosUnario] = ItemType.Integer,
            [Symbols.SMult] = ItemType.Integer,
            [Symbols.SDiv] = ItemType.Integer,
            [Symbols.SMais] = ItemType.Integer,
            [Symbols.SMenos] = ItemType.Integer,
            [Symbols.SNao] = ItemType.Boolean,
            [Symbols.SMaior] = ItemType.Boolean,
            [Symbols.SMaiorIg] = ItemType.Boolean,
            [Symbols.SMenor] = ItemType.Boolean,
            [Symbols.SMenorIg] = ItemType.Boolean,
            [Symbols.SDif] = ItemType.Boolean,
            [Symbols.SIg] = ItemType.Boolean,
            [Symbols.SE] = ItemType.Boolean,
            [Symbols.SOu] = ItemType.Boolean,
            [Symbols.SNumero] = ItemType.Integer,
            [Symbols.SVerdadeiro] = ItemType.Boolean,
            [Symbols.SFalso] = ItemType.Boolean
        };

        public ExpressionAnalyzer(VectorSymbolTable symbolTable)
        {
            _items = new List<ExpressionItem>();
            _output = new List<ExpressionItem>();
            _symbolTable = symbolTable;
        }

        public void Add(Token token)
        {
            var item = new ExpressionItem
            {
                Lexeme = token.Lexeme,
                Symbol = token.Symbol
            };

            if (token.Symbol != Symbols.SNumero && token.Symbol != Symbols.SIdentificador && 
                token.Symbol != Symbols.SVerdadeiro && token.Symbol != Symbols.SFalso && 
                token.Symbol != Symbols.SAbreParenteses)
            {
                if (token.Symbol == Symbols.SFechaParenteses)
                {
                    for (int i = _items.Count - 1; i >= 0; i--)
                    {
                        var current = _items[i];

                        if (current.Symbol != Symbols.SAbreParenteses)
                        {
                            _output.Add(current);
                        }
                        else
                        {
                            _items.RemoveAt(i);
                            break;
                        }

                        _items.RemoveAt(i);
                    }

                    return;                 
                }

                for (int i = _items.Count - 1; i >= 0; i--)
                {
                    var current = _items[i];

                    if (current.Symbol == Symbols.SAbreParenteses)
                    {
                        break;
                    }

                    var priority1 = _priorities[current.Symbol];
                    var priority2 = _priorities[token.Symbol]; 
                    
                    if (priority1 >= priority2)
                    {
                        _output.Add(current);
                        _items.RemoveAt(i);
                    }
                }

                if (token.Symbol != Symbols.SAbreParenteses)
                {
                    item.Type = _operatorsType[token.Symbol];
                    _items.Add(item);
                }
            }
            else
            {
                if (token.Symbol == Symbols.SAbreParenteses)
                {
                    item.Type = ItemType.None;
                    _items.Add(item);
                }
                else
                {
                    if (token.Symbol != Symbols.SFechaParenteses)
                    {
                        if (token.Symbol == Symbols.SIdentificador)
                        {
                            item.Type = GetIdentificatorType(token.Lexeme);
                        }
                        else
                        {
                            item.Type = _operatorsType[token.Symbol];
                        }

                        _output.Add(item);
                    }
                }
            }
        }

        public ItemType Analyze()
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                _output.Add(_items[i]);
            }

            ItemType result = ItemType.None;
            Stack<ItemType> types = new Stack<ItemType>();

            foreach (var item in _output)
            {
                var symbol = item.Symbol;

                if (symbol == Symbols.SNao)
                {
                    var firstOperand = types.Pop();

                    if (firstOperand != ItemType.Boolean)
                    {
                        throw new InvalidOperationException(InvalidOperandNotOperator);
                    }
                }
                else if (symbol != Symbols.SIdentificador && item.Symbol != Symbols.SNumero &&
                         symbol != Symbols.SVerdadeiro && symbol != Symbols.SFalso)
                {
                    var firstType = types.Pop();
                    var type = item.Type;

                    //Unary operator expects only one operand and it always results in a integer expression.
                    if (symbol == Symbols.SMaisUnario || symbol == Symbols.SMenosUnario)
                    {
                        if (firstType != ItemType.Integer)
                        {
                            throw new InvalidOperationException(string.Format(InvalidOperatorInExpression, item.Lexeme, firstType));
                        }
                    }
                    else
                    {
                        type = GetExpressionType(item, firstType, types.Pop());
                    }
                }

                types.Push(item.Type);
                result = item.Type;
            }

            return result;
        }

        public string GetOutput()
        {
            return string.Join(string.Empty, _output.Select(token => token.Lexeme));
        }

        public void Reset()
        {
            _items.Clear();
            _output.Clear();
        }

        private ItemType GetExpressionType(ExpressionItem item, ItemType firstType, ItemType secondType)
        {
            if (item.Symbol == Symbols.SMais || item.Symbol == Symbols.SMenos ||
                    item.Symbol == Symbols.SDiv || item.Symbol == Symbols.SMult)
            {
                if (firstType == ItemType.Integer && secondType == ItemType.Integer)
                {
                    return ItemType.Integer;
                }
                else
                {
                    ThrowInvalidOperandError(item.Lexeme, firstType, secondType);
                }
            }
            else if (item.Symbol == Symbols.SMaior || item.Symbol == Symbols.SMenor ||
                       item.Symbol == Symbols.SMaiorIg || item.Symbol == Symbols.SMenorIg)
            {
                if (firstType == ItemType.Integer && secondType == ItemType.Integer)
                {
                    return ItemType.Boolean;
                }
                else
                {
                    ThrowInvalidOperandError(item.Lexeme, firstType, secondType);
                }
            }
            else if (item.Symbol == Symbols.SIg || item.Symbol == Symbols.SDif)
            {
                if ((firstType == ItemType.Integer && secondType == ItemType.Integer) || 
                    (firstType == ItemType.Boolean && secondType == ItemType.Boolean))
                {
                    return ItemType.Boolean;
                }
                else
                {
                    ThrowInvalidOperandError(item.Lexeme, firstType, secondType);
                }
            }
            else if (item.Symbol == Symbols.SE || item.Symbol == Symbols.SOu)
            {
                if (firstType == ItemType.Boolean && secondType == ItemType.Boolean)
                {
                    return ItemType.Boolean;
                }
                else
                {
                    ThrowInvalidOperandError(item.Lexeme, firstType, secondType);
                }
            }

            return ItemType.None;
        }

        private ItemType GetIdentificatorType(string lexeme)
        {
            var item = _symbolTable.Search(lexeme);

            if (item != null)
            {
                var identificator = item as IdentificatorItem;

                if (identificator != null)
                {
                    return identificator.Type;
                }

                var func = item as FunctionItem;

                if (func != null)
                {
                    return func.Type;
                }
            }

            throw new ExpressionException(lexeme);
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
