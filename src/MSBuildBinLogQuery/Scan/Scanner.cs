using System;
using System.Text;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Scan
{
    public class Scanner
    {
        public Token.Token Token { get; private set; }

        private readonly string _expression;
        private int _index;
        private char _char;

        public Scanner(string expression)
        {
            _expression = expression ?? throw new ArgumentNullException();
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
                    ReadNextKeyword("PROJECT", () => new ProjectToken());
                    break;
                case 'T':
                case 't':
                    ReadNextCharacter();

                    if (char.ToUpper(_char) != 'A')
                    {
                        throw new ScanException(_expression);
                    }

                    ReadNextCharacter();

                    if (char.ToUpper(_char) == 'R')
                    {
                        ReadNextKeyword("RGET", () => new TargetToken());
                        break;
                    }
                    else if (char.ToUpper(_char) == 'S')
                    {
                        ReadNextKeyword("SK", () => new TaskToken());
                        break;
                    }
                    else
                    {
                        throw new ScanException(_expression);
                    }
                default:
                    throw new ScanException(_expression);
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
                    throw new ScanException(_expression);
                }

                stringBuilder.Append(_char);

                ReadNextCharacter();
            }

            Token = thunk.Invoke();
        }

        private bool ReadNextCharacter()
        {
            if (_index < _expression.Length)
            {
                _char = _expression[_index++];
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