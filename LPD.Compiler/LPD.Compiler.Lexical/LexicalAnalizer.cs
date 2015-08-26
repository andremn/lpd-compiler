using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private const char DotCommaChar = ';';
        private const char CommaChar = ',';
        private const char OpenParenthesesCommaChar = '(';
        private const char CloseParenthesesCommaChar = ')';
        private const char DotChar = '.';
        private const char PlusChar = '+';
        private const char MinusChar = '-';
        private const char MultChar = '*';

        private static readonly char[] ArithmeticOperators = { '+', '-', '*' };
        private static readonly char[] RelationalOperators = { '<', '>', '=' };
        private static readonly char[] PontuationOperators = { ';', ',', '(', ')', '.' };

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

        private StreamReader _reader;
        private string _filePath;

        public LexicalAnalizer(string filePath)
        {
            _filePath = filePath;
        }

        public IList<Token> DoAnalisys()
        {
            List<Token> tokens = new List<Token>();

            using (FileStream fileStream = File.OpenRead(_filePath))
            {
                using (_reader = new StreamReader(fileStream))
                {
                    int read;

                    while ((read = _reader.Read()) >= 0)
                    {
                        char character = (char)read;

                        if (character == '\r' || character == '\n')
                        {
                            continue;
                        }

                        if (character == CommentStart)
                        {
                            while ((read = _reader.Read()) >= 0 && (char)read != CommentEnd)
                            {
                                read = _reader.Read();
                            }

                            continue;
                        }
                        else if (character == SpaceChar)
                        {
                            continue;
                        }

                        tokens.Add(GetToken(character));
                    }
                }
            }

            return tokens;
        }

        private Token GetToken(char character)
        {
            Token token;

            if (char.IsDigit(character))
            {
                token = HandleDigit(character);
            }
            else if (char.IsLetter(character))
            {
                token = HandleKeyWord(character);
            }
            else if (character == TwoPointsChar)
            {
                token = HandleAtribution(character);
            }
            else if (ArithmeticOperators.Contains(character))
            {
                token = HandleArithmeticOperator(character);
            }
            else if (RelationalOperators.Contains(character))
            {
                token = HandleRelationalOperator(character);
            }
            else if (PontuationOperators.Contains(character))
            {
                token = HandlePontuation(character);
            }
            else
            {
                throw new InvalidOperationException();
            }

            return token;
        }

        private Token HandlePontuation(char character)
        {
            Token token = new Token();

            switch (character)
            {
                case DotCommaChar:
                    token.Symbol = Symbols.SPontoVirgula;
                    break;
                case CommaChar:
                    token.Symbol = Symbols.SVirgula;
                    break;
                case DotChar:
                    token.Symbol = Symbols.SPonto;
                    break;
                case OpenParenthesesCommaChar:
                    token.Symbol = Symbols.SAbreParenteses;
                    break;
                case CloseParenthesesCommaChar:
                    token.Symbol = Symbols.SFechaParenteses;
                    break;
            }

            token.Lexema = character.ToString();
            return token;
        }

        private Token HandleRelationalOperator(char character)
        {
            Token token = new Token();
            string id = string.Empty;

            id += character;

            if (character == EqualChar)
            {
                token.Symbol = Symbols.SIg;
            }
            else
            {
                if (character == '>')
                {
                    token.Symbol = Symbols.SMaior;
                }
                else if (character == '<')
                {
                    token.Symbol = Symbols.SMenor;
                }
                else
                {
                    character = (char)_reader.Read();

                    if (id[0] == '>' && character == EqualChar)
                    {
                        token.Symbol = Symbols.SMaiorig;
                    }
                    else if (id[0] == '<' && character == EqualChar)
                    {
                        token.Symbol = Symbols.SMenorig;
                    }
                }
            }

            token.Lexema = id;
            return token;
        }

        private Token HandleArithmeticOperator(char character)
        {
            Token token = new Token();

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

            token.Lexema = character.ToString();
            return token;
        }

        private Token HandleAtribution(char character)
        {
            Token token = new Token();
            string lex = string.Empty;

            lex += character;
            character = (char)_reader.Peek();

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
            token.Lexema = lex;
            return token;
        }

        private Token HandleKeyWord(char character)
        {
            Token token = new Token();
            Symbols symbol;
            StringBuilder stringBuilder = new StringBuilder();
            string id;

            while (true)
            {
                stringBuilder.Append(character);
                character = (char)_reader.Peek();

                if (!char.IsLetterOrDigit(character) && character != UnderscoreChar)
                {
                    break;
                }

                character = (char)_reader.Read();
            }

            id = stringBuilder.ToString();
            token.Lexema = id;
            
            if (!Tokens.TryGetValue(id, out symbol))
            {
                symbol = Symbols.SIdentificador;
            }

            token.Symbol = symbol;
            return token;
        }

        private Token HandleDigit(char character)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(character);
            character = (char)_reader.Peek();

            while (true)
            {
                if (!char.IsDigit(character))
                {
                    break;
                }

                stringBuilder.Append(character);
                character = (char)_reader.Read();
            }

            return new Token() { Lexema = stringBuilder.ToString(), Symbol = Symbols.SNumero };
        } 
    }
}
