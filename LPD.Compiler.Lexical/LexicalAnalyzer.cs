using LPD.Compiler.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LPD.Compiler.Lexical
{
    /// <summary>
    /// The lexical analyzer. It is responsible for reading the file and get the tokens.
    /// </summary>
    public class LexicalAnalyzer : IDisposable
    {
        #region Error messages

        private const string UnknownTokenErrorMessageFormat = "O símbolo '{0}' não é um símbolo válido.";
        private const string ExpectedTokenErrorMessageFormat = "Esperado o símbolo '{0}' {1} do símbolo '{2}'.";

        #endregion

        #region Chars

        private const char CommentStart = '{';
        private const char CommentEnd = '}';
        private const char SpaceChar = ' ';
        private const char TwoPointsChar = ':';
        private const char UnderscoreChar = '_';
        private const char ExclamationChar = '!';
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
        private const char LineFeedChar = '\r';
        private const char CarriageReturnChar = '\n';
        private const char TabChar = '\t';

        #endregion

        private static readonly char[] ArithmeticOperators = { PlusChar, MinusChar, MultChar };
        private static readonly char[] RelationalOperators = { LessChar, GreaterChar, EqualChar, ExclamationChar };
        private static readonly char[] PontuationOperators = { SemiColonChar, CommaChar, OpenBracketChar, CloseBracketChar, DotChar };
        
        private static readonly Dictionary<string, Symbols> Keywords = new Dictionary<string, Symbols>()
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
        private CodePosition _currentPosition;
        private TokenCollection _tokenCollection;

        /// <summary>
        /// Gets the position of the current token.
        /// </summary>
        public CodePosition Position
        {
            get { return _currentPosition; }
        }

        /// <summary>
        /// Gets the tokens read.
        /// </summary>
        public TokenCollection ReadTokens
        {
            get { return _tokenCollection; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LexicalAnalyzer"/> class with the specified file.
        /// </summary>
        /// <param name="filePath">The absolute path of the file to be analyzed.</param>
        public LexicalAnalyzer(string filePath)
        {
            _filePath = filePath;
            _currentPosition.Column = 1;
            _currentPosition.Line = 1;
            _currentPosition.Index = -1;
            _reader = new CharReader(filePath, Encoding.UTF8);
            _reader.CharRead += OnCharRead;
            _tokenCollection = new TokenCollection();
        }

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <param name="item">The <see cref="LexicalItem"/> that will contain the next token, if any.</param>
        /// <returns>true if a next token was found; false otherwise.</returns>
        public bool GetToken(out LexicalItem item)
        {
            LexicalItem nextItem = NextItem();

            if (nextItem == null)
            {
                item = null;
                return false;
            }

            item = nextItem;
            _tokenCollection.Append(item.Token);
            return true;
        }

        /// <summary>
        /// Releases all the resources used by this object.
        /// </summary>
        public void Dispose()
        {
            _reader.Dispose();
        }

        private LexicalItem NextItem()
        {
            LexicalItem item = null;
            char? character;
            
            while ((character = _reader.Read()).HasValue)
            {
                if (character == CarriageReturnChar)
                {
                    _currentPosition.Line++;
                    _currentPosition.Column = 1;
                    continue;
                }

                if (character == LineFeedChar)
                {
                    character = _reader.Read().Value;

                    if (character == CarriageReturnChar)
                    {
                        _currentPosition.Line++;
                        _currentPosition.Column = 1;
                    }

                    continue;
                }

                if (character == CommentStart)
                {
                    ConsumeComment();
                    continue;
                }

                if (character == SpaceChar || character == TabChar)
                {
                    ConsumeCharacter(character.Value);
                    continue;
                }

                item = BuildItem();
                break;
            }
            
            return item;
        }

        private LexicalItem BuildItem()
        {
            LexicalItem item;
            char character = _reader.Current.Value;

            if (char.IsDigit(character))
            {
                item = HandleDigit();
            }
            else if (char.IsLetter(character))
            {
                item = HandleKeyWord();
            }
            else if (character == TwoPointsChar)
            {
                item = HandleAtribution();
            }
            else if (ArithmeticOperators.Contains(character))
            {
                item = HandleArithmeticOperator();
            }
            else if (RelationalOperators.Contains(character))
            {
                item = HandleRelationalOperator();
            }
            else if (PontuationOperators.Contains(character))
            {
                item = HandlePontuation();
            }
            else
            {
                return BuildLexicalItem(Token.Empty, new CompilationError(_currentPosition, string.Format(UnknownTokenErrorMessageFormat, character)));
            }

            return item;
        }

        private LexicalItem HandlePontuation()
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
            return BuildLexicalItem(token, null);
        }

        private LexicalItem HandleRelationalOperator()
        {
            Token token = new Token();
            string id = string.Empty;
            char character = _reader.Current.Value;

            id += character;

            if (character == EqualChar)
            {
                token.Symbol = Symbols.SIg;
            }
            else if (character == ExclamationChar)
            {
                character = _reader.Read().Value;

                if (character != EqualChar)
                {
                    return new LexicalItem()
                    {
                        Error = new CompilationError(_currentPosition, string.Format(ExpectedTokenErrorMessageFormat, EqualChar, ExclamationChar))
                    };
                }

                id += character;
                token.Symbol = Symbols.SDif;
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
            return BuildLexicalItem(token, null);
        }

        private LexicalItem HandleArithmeticOperator()
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
            return BuildLexicalItem(token, null);
        }

        private LexicalItem HandleAtribution()
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
                _reader.Read();
            }
            else
            {
                token.Symbol = Symbols.SDoisPontos;
            }

            token.Lexeme = lex;
            return BuildLexicalItem(token, null);
        }

        private LexicalItem HandleKeyWord()
        {
            Token token = new Token();
            Symbols symbol;
            StringBuilder stringBuilder = new StringBuilder();
            char? character = _reader.Current.Value;
            string id;

            stringBuilder.Append(character);

            while ((character = _reader.Peek()).HasValue)
            {
                if (!char.IsLetterOrDigit(character.Value) && character != UnderscoreChar)
                {
                    break;
                }

                stringBuilder.Append(character);

                if (character.Value.HasDiacritic())
                {
                    _reader.Read();

                    return new LexicalItem()
                    {
                        Error = new CompilationError(_currentPosition, string.Format(UnknownTokenErrorMessageFormat, character))
                    };
                }

                _reader.Read();
            }

            id = stringBuilder.ToString();
            token.Lexeme = id;
            
            if (!Keywords.TryGetValue(id, out symbol))
            {
                symbol = Symbols.SIdentificador;
            }

            token.Symbol = symbol;
            return BuildLexicalItem(token, null);
        }

        private LexicalItem HandleDigit()
        {
            StringBuilder stringBuilder = new StringBuilder();
            char? character = _reader.Current;

            stringBuilder.Append(character);

            while ((character = _reader.Peek()).HasValue && char.IsDigit(character.Value))
            {
                stringBuilder.Append(character);
                _reader.Read();
            }
            
            return BuildLexicalItem(new Token() { Lexeme = stringBuilder.ToString(), Symbol = Symbols.SNumero }, null);
        }

        private void ConsumeComment()
        {
            char? character;

            while ((character = _reader.Read()).HasValue)
            {
                if (character == CommentEnd)
                {
                    break;
                }
            }
        }

        private void ConsumeCharacter(char character)
        {
            char? c;

            while ((c = _reader.Peek()).HasValue)
            {
                if (c != character)
                {
                    break;
                }

                _reader.Read();
            }
        }

        private LexicalItem BuildLexicalItem(Token token, CompilationError error)
        {
            if (error == null)
            {
                return new LexicalItem() { Token = token, Error = null };
            }

            return new LexicalItem() { Error = error };
        }

        private void OnCharRead(object sender, EventArgs e)
        {
            _currentPosition.Index++;
            _currentPosition.Column++;
        }
    }
}
