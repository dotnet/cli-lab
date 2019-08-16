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
                    Token = new EofToken();
                    break;
                case '/':
                    ReadNextCharacter();

                    if (_char == '/')
                    {
                        ReadNextCharacter();
                        Token = new DoubleSlashToken();
                    }
                    else
                    {
                        Token = new SingleSlashToken();
                    }

                    break;
                case '[':
                    ReadNextCharacter();
                    Token = new LeftBracketToken();
                    break;
                case ']':
                    ReadNextCharacter();
                    Token = new RightBracketToken();
                    break;
                case '=':
                    ReadNextCharacter();
                    Token = new EqualToken();
                    break;
                case ',':
                    ReadNextCharacter();
                    Token = new CommaToken();
                    break;
                case '\"':
                    ReadNextCharacter();
                    Token = new StringToken(ReadNextString());
                    break;
                case 'M':
                case 'm':
                    ReadNextKeyword("MESSAGE", () => new MessageToken());
                    break;
                case 'W':
                case 'w':
                    ReadNextKeyword("WARNING", () => new WarningToken());
                    break;
                case 'E':
                case 'e':
                    ReadNextKeyword("ERROR", () => new ErrorToken());
                    break;
                case 'P':
                case 'p':
                    ReadNextCharacter();

                    if (char.ToUpper(_char) == 'R')
                    {
                        ReadNextKeyword("ROJECT", () => new ProjectToken());
                    }
                    else if (char.ToUpper(_char) == 'A')
                    {
                        ReadNextKeyword("ATH", () => new PathToken());
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
                        ReadNextKeyword("RGET", () => new TargetToken());
                    }
                    else if (char.ToUpper(_char) == 'S')
                    {
                        ReadNextKeyword("SK", () => new TaskToken());
                    }
                    else
                    {
                        throw new ScanException(Expression);
                    }

                    break;
                case 'I':
                case 'i':
                    ReadNextKeyword("ID", () => new IdToken());
                    break;
                case 'N':
                case 'n':
                    ReadNextKeyword("NAME", () => new NameToken());
                    break;
                default:
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

        private void ReadNextKeyword(string keyword, Func<Token.Token> thunk)
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

            Token = thunk.Invoke();
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
            while (IsWhiteSpace(_char) && ReadNextCharacter());
        }

        private static bool IsWhiteSpace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }
    }
}