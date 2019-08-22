using System;
using System.Text;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Scan
{
    public class Scanner
    {
        public Token.Token Token { get; private set; }
        public string Expression { get; }

        private int _index;
        private char _char;

        public Scanner(string expression)
        {
            Expression = expression ?? throw new ArgumentNullException();
            _index = 0;

            ReadNextCharacter();
            ReadNextToken();
        }

        public void ReadNextToken()
        {
            SkipWhiteSpace();

            switch (_char)
            {
                case '\0':
                    Token = EofToken.Instance;
                    break;
                case '/':
                    ReadNextCharacter();

                    if (_char == '/')
                    {
                        ReadNextCharacter();
                        Token = DoubleSlashToken.Instance;
                    }
                    else
                    {
                        Token = SingleSlashToken.Instance;
                    }

                    break;
                case '[':
                    ReadNextCharacter();
                    Token = LeftBracketToken.Instance;
                    break;
                case ']':
                    ReadNextCharacter();
                    Token = RightBracketToken.Instance;
                    break;
                case '=':
                    ReadNextCharacter();
                    Token = EqualToken.Instance;
                    break;
                case ',':
                    ReadNextCharacter();
                    Token = CommaToken.Instance;
                    break;
                case '\"':
                    ReadNextCharacter();
                    Token = new StringToken(ReadNextString());
                    break;
                case 'M':
                case 'm':
                    ReadNextKeyword("MESSAGE", MessageToken.Instance);
                    break;
                case 'W':
                case 'w':
                    ReadNextKeyword("WARNING", WarningToken.Instance);
                    break;
                case 'E':
                case 'e':
                    ReadNextKeyword("ERROR", ErrorToken.Instance);
                    break;
                case 'P':
                case 'p':
                    ReadNextCharacter();

                    if (char.ToUpper(_char) == 'R')
                    {
                        ReadNextKeyword("ROJECT", ProjectToken.Instance);
                    }
                    else if (char.ToUpper(_char) == 'A')
                    {
                        ReadNextKeyword("ATH", PathToken.Instance);
                    }
                    else
                    {
                        throw new ScanException(Expression);
                    }

                    break;
                case 'T':
                case 't':
                    ReadNextCharacter();

                    if (char.ToUpper(_char) != 'A')
                    {
                        throw new ScanException(Expression);
                    }

                    ReadNextCharacter();

                    if (char.ToUpper(_char) == 'R')
                    {
                        ReadNextKeyword("RGET", TargetToken.Instance);
                    }
                    else if (char.ToUpper(_char) == 'S')
                    {
                        ReadNextKeyword("SK", TaskToken.Instance);
                    }
                    else
                    {
                        throw new ScanException(Expression);
                    }

                    break;
                case 'I':
                case 'i':
                    ReadNextKeyword("ID", IdToken.Instance);
                    break;
                case 'N':
                case 'n':
                    ReadNextKeyword("NAME", NameToken.Instance);
                    break;
                case 'A':
                case 'a':
                    ReadNextKeyword("AFTER", AfterToken.Instance);
                    break;
                case 'B':
                case 'b':
                    ReadNextKeyword("BEFORE", BeforeToken.Instance);
                    break;
                case 'D':
                case 'd':
                    ReadNextKeyword("DEPENDON", DependOnToken.Instance);
                    break;
                default:
                    if (char.IsDigit(_char))
                    {
                        Token = new IntegerToken(ReadNextInteger());
                        break;
                    }

                    throw new ScanException(Expression);
            }
        }

        private string ReadNextString()
        {
            var stringBuilder = new StringBuilder();

            while (_char != '\"')
            {
                stringBuilder.Append(_char);
                ReadNextCharacter();
            }

            ReadNextCharacter();
            return stringBuilder.ToString();
        }

        private void ReadNextKeyword(string keyword, Token.Token token)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < keyword.Length; i++)
            {
                if (char.ToUpper(_char) != char.ToUpper(keyword[i]))
                {
                    throw new ScanException(Expression);
                }

                stringBuilder.Append(_char);

                ReadNextCharacter();
            }

            Token = token;
        }

        private int ReadNextInteger()
        {
            var stringBuilder = new StringBuilder();

            while (char.IsDigit(_char))
            {
                stringBuilder.Append(_char);
                ReadNextCharacter();
            }

            if (int.TryParse(stringBuilder.ToString(), out var value))
            {
                return value;
            }

            throw new ScanException(Expression);
        }

        private bool ReadNextCharacter()
        {
            if (_index < Expression.Length)
            {
                _char = Expression[_index++];
                return true;
            }

            _char = '\0';
            return false;
        }

        private void SkipWhiteSpace()
        {
            while (IsWhiteSpace(_char) && ReadNextCharacter()) { }
        }

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }
    }
}
