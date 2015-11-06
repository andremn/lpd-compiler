using LPD.Compiler.Lexical;
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
            if (token.Symbol != Symbols.SNumero && token.Symbol != Symbols.SIdentificador)
            {
                for (int i = _tokens.Count - 1; i >= 0; i--)
                {
                    var current = _tokens[i];
                    var priority1 = _priorities[current.Symbol];
                    var priority2 = _priorities[token.Symbol];

                    if (priority1 >= priority2)
                    {
                        _output.Add(current);
                        _tokens.RemoveAt(i);
                    }
                }

                _tokens.Add(token);
            }
            else
            {
                _output.Add(token);
            }
        }

        public ItemType AnalyzeDataTypes(VectorSymbolTable symbolsTable)
        {
            for (int i = _tokens.Count - 1; i >= 0; i--)
            {
                _output.Add(_tokens[i]);
            }

            Stack<ItemType> finalTypes = new Stack<ItemType>();
            Stack<ItemType> types = new Stack<ItemType>();
            string output = GetOutput();
            
            foreach (var token in _output)
            {
                if (token.Symbol == Symbols.SIdentificador || token.Symbol == Symbols.SFuncao)
                {
                    var identificator = symbolsTable.Search(token.Lexeme) as IdentificatorItem;

                    if (identificator != null)
                    {
                        types.Push(identificator.Type);
                        continue;
                    }

                    var func = symbolsTable.Search(token.Lexeme) as FunctionItem;

                    if (func != null)
                    {
                        types.Push(func.Type);
                    }
                }
                else if (token.Symbol == Symbols.SNumero)
                {
                    types.Push(ItemType.Integer);
                }
                else
                {
                    ItemType expectedType = ItemType.None;

                    if (token.Symbol == Symbols.SNao)
                    {
                        var item = types.Pop();

                        if (item != ItemType.Boolean)
                        {
                            throw new InvalidOperationException("Operador inválido");
                        }

                        finalTypes.Push(ItemType.Boolean);
                    }
                    else if (token.Symbol == Symbols.SMais || token.Symbol == Symbols.SMenos ||
                        token.Symbol == Symbols.SDiv || token.Symbol == Symbols.SMult)
                    {
                        var item1 = types.Pop();
                        var item2 = types.Pop();

                        if (item1 != ItemType.Integer || item2 != ItemType.Integer)
                        {
                            throw new InvalidOperationException("Operador inválido");
                        }

                        finalTypes.Push(ItemType.Integer);
                    }
                    else if (token.Symbol == Symbols.SMaior || token.Symbol == Symbols.SMenor ||
                        token.Symbol == Symbols.SMaiorIg || token.Symbol == Symbols.SMenorIg ||
                        token.Symbol == Symbols.SIg || token.Symbol == Symbols.SDif)
                    {
                        var item1 = types.Pop();
                        var item2 = types.Pop();

                        if (item1 != ItemType.Integer || item2 != ItemType.Integer)
                        {
                            throw new InvalidOperationException("Operador inválido");
                        }

                        finalTypes.Push(ItemType.Boolean);
                    }
                    else if (token.Symbol == Symbols.SE || token.Symbol == Symbols.SOu)
                    {
                        var item1 = types.Pop();
                        var item2 = types.Pop();

                        if (item1 != ItemType.Boolean || item2 != ItemType.Boolean)
                        {
                            throw new InvalidOperationException("Operador inválido");
                        }

                        finalTypes.Push(ItemType.Boolean);
                    }

                    finalTypes.Push(expectedType);
                }
            }

            if (finalTypes.All(item => item == ItemType.Integer))
            {
                return ItemType.Integer;
            }

            if (finalTypes.All(item => item == ItemType.Boolean))
            {
                return ItemType.Boolean;
            }

            throw new InvalidOperationException("Operador inválido");
        }

        public string GetOutput()
        {
            return string.Join(string.Empty, _output);
        }
    }
}
