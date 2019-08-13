using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Scan;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Parse
{
    public class Parser
    {
        private readonly Scanner _scanner;

        private Parser(Scanner scanner)
        {
            _scanner = scanner;
        }

        public static QueryNode Parse(Scanner scanner)
        {
            var parser = new Parser(scanner);
            return parser.ParseQueryNode();
        }

        public static QueryNode Parse(string expression)
        {
            var scanner = new Scanner(expression);
            return Parse(scanner);
        }

        private void Consume<TToken>() where TToken : Token.Token
        {
            if (_scanner.Token is TToken)
            {
                _scanner.ReadNextToken();
            }
            else
            {
                throw new ParseException(_scanner.Expression);
            }
        }

        private MessageNode ParseMessageNode()
        {
            Consume<MessageToken>();
            return new MessageNode(null);
        }

        private WarningNode ParseWarningNode()
        {
            Consume<WarningToken>();
            return new WarningNode(null);
        }

        private ErrorNode ParseErrorNode()
        {
            Consume<ErrorToken>();
            return new ErrorNode(null);
        }

        private QueryNode ParseLogNode()
        {
            if (_scanner.Token is MessageToken)
            {
                return ParseMessageNode();
            }
            else if (_scanner.Token is WarningToken)
            {
                return ParseWarningNode();
            }
            else if (_scanner.Token is ErrorToken)
            {
                return ParseErrorNode();
            }
            
            throw new ParseException(_scanner.Expression);
        }

        private QueryNode ParseTaskNode()
        {
            Consume<TaskToken>();

            if (_scanner.Token is SlashToken)
            {
                Consume<SlashToken>();

                var next = ParseMessageNode();
                return new TaskNode(next);
            }
            else if (_scanner.Token is EofToken)
            {
                return new TaskNode(null);
            }
            else
            {
                throw new ParseException(_scanner.Expression);
            }
        }

        private QueryNode ParseTargetNode()
        {
            Consume<TargetToken>();

            if (_scanner.Token is SlashToken)
            {
                Consume<SlashToken>();

                if (_scanner.Token is TaskToken)
                {
                    var next = ParseTaskNode();
                    return new TargetNode(next);
                }
                else
                {
                    var next = ParseLogNode();
                    return new TargetNode(next);
                }
            }
            else if (_scanner.Token is EofToken)
            {
                return new TargetNode(null);
            }
            else
            {
                throw new ParseException(_scanner.Expression);
            }
        }

        private QueryNode ParseProjectNode()
        {
            Consume<ProjectToken>();

            if (_scanner.Token is SlashToken)
            {
                Consume<SlashToken>();

                if (_scanner.Token is TargetToken)
                {
                    var next = ParseTargetNode();
                    return new ProjectNode(next);
                }
                else if (_scanner.Token is TaskToken)
                {
                    var next = ParseTaskNode();
                    return new ProjectNode(next);
                }
                else
                {
                    var next = ParseLogNode();
                    return new ProjectNode(next);
                }
            }
            else if (_scanner.Token is EofToken)
            {
                return new ProjectNode(null);
            }
            else
            {
                throw new ParseException(_scanner.Expression);
            }
        }

        private QueryNode ParseQueryNode()
        {
            Consume<SlashToken>();

            if (_scanner.Token is ProjectToken)
            {
                return ParseProjectNode();
            }
            else if (_scanner.Token is TargetToken)
            {
                return ParseTargetNode();
            }
            else if (_scanner.Token is TaskToken)
            {
                return ParseTaskNode();
            }
            else
            {
                return ParseLogNode();
            }
        }
    }
}