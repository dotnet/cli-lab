using System;
using System.Text;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Scan
{
    public class Scanner
    {
        private readonly string _expression;
        private int _index;
        private char _char;

        public Scanner(string expression)
        {
            _expression = expression ?? throw new ArgumentNullException();
            _index = 0;
            _char = '\0';

            ReadNextCharacter();
        }

        public Token.Token ReadNextToken()
        {
            SkipWhiteSpace();

            switch (_char)
            {
                case '\0':
                    ReadNextCharacter();
                    return new EofToken();
                case '/':
                    ReadNextCharacter();
                    return new SlashToken();
                case '[':
                    ReadNextCharacter();
                    return new LeftBracketToken();
                case ']':
                    ReadNextCharacter();
                    return new RightBracketToken();
                case '=':
                    ReadNextCharacter();
                    return new EqualToken();
                case ',':
                    ReadNextCharacter();
                    return new CommaToken();
                case '\"':
                    ReadNextCharacter();
                    return new StringToken(ReadNextString());
                default:
                    if (IsAlpha(_char))
                    {
                        var identifier = ReadNextIdentifier();

                        if (identifier.ToUpper().Equals("MESSAGE"))
                        {
                            return new MessageToken();
                        }
                        else if (identifier.ToUpper().Equals("WARNING"))
                        {
                            return new WarningToken();
                        }
                        else if (identifier.ToUpper().Equals("ERROR"))
                        {
                            return new ErrorToken();
                        }
                        else if (identifier.ToUpper().Equals("PROJECT"))
                        {
                            return new ProjectToken();
                        }
                        else if (identifier.ToUpper().Equals("TARGET"))
                        {
                            return new TargetToken();
                        }
                        else if (identifier.ToUpper().Equals("TASK"))
                        {
                            return new TaskToken();
                        }
                    }

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

        private string ReadNextIdentifier()
        {
            var stringBuilder = new StringBuilder();

            while (IsAlpha(_char))
            {
                stringBuilder.Append(_char);
                ReadNextCharacter();
            }

            return stringBuilder.ToString();
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

        private static bool IsAlpha(char c)
        {
            return c == '_' || (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }
    }
}