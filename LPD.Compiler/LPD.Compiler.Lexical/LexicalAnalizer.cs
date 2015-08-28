using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LPD.Compiler.Lexical
{
    public class LexicalAnalizer
    {
        private const char CommentStart = '{';
        private const char CommentEnd = '}';
        private const char SpaceChar = ' ';
        private const char TwoPointsChar = ':';
        private const char UnderscoreChar = '_';
        private const char EqualChar = '=';
        private const char SemiColonChar = ';';
        private const char CommaChar = ',';
        private const char OpenBracketChar = '(';
        private const char CloseBracketChar = ')';
        private const char DotChar = '.';
        private const char PlusChar = '+';
        private const char MinusChar = '-';
        private const char MultChar = '*';
        private const char GreaterChar = '>';
        private const char LessChar = '<';

        private static readonly char[] ArithmeticOperators = { PlusChar, MinusChar, MultChar };
        private static readonly char[] RelationalOperators = { LessChar, GreaterChar, EqualChar };
        private static readonly char[] PontuationOperators = { SemiColonChar, CommaChar, OpenBracketChar, CloseBracketChar, DotChar };

        private static readonly Dictionary<string, Symbols> Tokens = new Dictionary<string, Symbols>()
        {
            ["programa"] = Symbols.SPrograma,
            ["se"] = Symbols.SSe,
            ["entao"] = Symbols.SEntao,
            ["senao"] = Symbols.SSenao,
            ["enquanto"] = Symbols.SEnquanto,
            ["faca"] = Symbols.SFaca,
            ["inicio"] = Symbols.SInicio,
            ["fim"] = Symbols.SFim,
            ["escreva"] = Symbols.SEscreva,
            ["leia"] = Symbols.SLeia,
            ["var"] = Symbols.SVar,
            ["inteiro"] = Symbols.SInteiro,
            ["booleano"] = Symbols.SBooleano,
            ["verdadeiro"] = Symbols.SVerdadeiro,
            ["falso"] = Symbols.SFalso,
            ["procedimento"] = Symbols.SProcedimento,
            ["funcao"] = Symbols.SFuncao,
            ["div"] = Symbols.SDiv,
            ["e"] = Symbols.SE,
            ["ou"] = Symbols.SOu,
            ["nao"] = Symbols.SNao
        };

        private CharReader _reader;
        private string _filePath;

        public LexicalAnalizer(string filePath)
        {
            _filePath = filePath;
        }

        public TokenCollection GetTokens()
        {
            TokenCollection tokens = new TokenCollection();

            using (FileStream fileStream = File.OpenRead(_filePath))
            {
                using (_reader = new CharReader(fileStream, Encoding.UTF8))
                {
                    char? character;

                    while ((character = _reader.Read()).HasValue)
                    {
                        if (character == '\r' || character == '\n')
                        {
                            continue;
                        }

                        if (character == CommentStart)
                        {
                            while (character != CommentEnd)
                            {
                                character = _reader.Read();
                            }

                            continue;
                        }
                        else if (character == SpaceChar)
                        {
                            continue;
                        }

                        tokens.Append(GetToken());
                    }
                }
            }

            return tokens;
        }

        private Token GetToken()
        {
            Token token;
            char character = _reader.Current.Value;

            if (char.IsDigit(character))
            {
                token = HandleDigit();
            }
            else if (char.IsLetter(character))
            {
                token = HandleKeyWord();
            }
            else if (character == TwoPointsChar)
            {
                token = HandleAtribution();
            }
            else if (ArithmeticOperators.Contains(character))
            {
                token = HandleArithmeticOperator();
            }
            else if (RelationalOperators.Contains(character))
            {
                token = HandleRelationalOperator();
            }
            else if (PontuationOperators.Contains(character))
            {
                token = HandlePontuation();
            }
            else
            {
                throw new InvalidOperationException();
            }

            return token;
        }

        private Token HandlePontuation()
        {
            Token token = new Token();
            char character = _reader.Current.Value;

            switch (character)
            {
                case SemiColonChar:
                    token.Symbol = Symbols.SPontoVirgula;
                    break;
                case CommaChar:
                    token.Symbol = Symbols.SVirgula;
                    break;
                case DotChar:
                    token.Symbol = Symbols.SPonto;
                    break;
                case OpenBracketChar:
                    token.Symbol = Symbols.SAbreParenteses;
                    break;
                case CloseBracketChar:
                    token.Symbol = Symbols.SFechaParenteses;
                    break;
            }

            token.Lexeme = character.ToString();
            return token;
        }

        private Token HandleRelationalOperator()
        {
            Token token = new Token();
            string id = string.Empty;
            char character = _reader.Current.Value;

            id += character;

            if (character == EqualChar)
            {
                token.Symbol = Symbols.SIg;
            }
            else if (character == GreaterChar)
            {
                character = _reader.Peek().Value;

                if (character == EqualChar)
                {
                    id += _reader.Read();
                    token.Symbol = Symbols.SMaiorIg;
                }
                else
                {
                    token.Symbol = Symbols.SMaior;
                }
            }
            else if (character == LessChar)
            {
                character = _reader.Peek().Value;

                if (character == EqualChar)
                {
                    id += _reader.Read();
                    token.Symbol = Symbols.SMenorIg;
                }
                else
                {
                    token.Symbol = Symbols.SMenor;
                }
            }

            token.Lexeme = id;
            return token;
        }

        private Token HandleArithmeticOperator()
        {
            Token token = new Token();
            char character = _reader.Current.Value;

            switch (character)
            {
                case PlusChar:
                    token.Symbol = Symbols.SMais;
                    break;
                case MinusChar:
                    token.Symbol = Symbols.SMenos;
                    break;
                case MultChar:
                    token.Symbol = Symbols.SMult;
                    break;
            }

            token.Lexeme = character.ToString();
            return token;
        }

        private Token HandleAtribution()
        {
            Token token = new Token();
            string lex = string.Empty;
            char character = _reader.Current.Value;

            lex += character;
            character = _reader.Peek().Value;

            if (character == EqualChar)
            {
                lex += character;
                token.Symbol = Symbols.SAtribuicao;
            }
            else
            {
                token.Symbol = Symbols.SDoisPontos;
            }

            _reader.Read();
            token.Lexeme = lex;
            return token;
        }

        private Token HandleKeyWord()
        {
            Token token = new Token();
            Symbols symbol;
            StringBuilder stringBuilder = new StringBuilder();
            char character = _reader.Current.Value;
            string id;

            while (true)
            {
                stringBuilder.Append(character);
                character = _reader.Peek().Value;

                if (!char.IsLetterOrDigit(character) && character != UnderscoreChar)
                {
                    break;
                }

                character = _reader.Read().Value;
            }

            id = stringBuilder.ToString();
            token.Lexeme = id;
            
            if (!Tokens.TryGetValue(id, out symbol))
            {
                symbol = Symbols.SIdentificador;
            }

            token.Symbol = symbol;
            return token;
        }

        private Token HandleDigit()
        {
            StringBuilder stringBuilder = new StringBuilder();
            char character = _reader.Current.Value;

            stringBuilder.Append(character);
            character = _reader.Peek().Value;

            while (true)
            {
                if (!char.IsDigit(character))
                {
                    break;
                }

                stringBuilder.Append(character);
                character = _reader.Read().Value;
            }

            return new Token() { Lexeme = stringBuilder.ToString(), Symbol = Symbols.SNumero };
        } 
    }
}
