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

        public static AstNode Parse(Scanner scanner)
        {
            var parser = new Parser(scanner);
            var queryNode = parser.ParseQuery();

            if (parser._scanner.Token is EofToken)
            {
                return queryNode;
            }

            throw new ParseException(parser._scanner.Expression);
        }

        public static AstNode Parse(string expression)
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

        private LogNodeType ParseSlash()
        {
            if (_scanner.Token is SingleSlashToken)
            {
                Consume<SingleSlashToken>();
                return LogNodeType.Direct;
            }
            else if (_scanner.Token is DoubleSlashToken)
            {
                Consume<DoubleSlashToken>();
                return LogNodeType.All;
            }

            throw new ParseException(_scanner.Expression);
        }

        private LogNode ParseLogNodeWithType(LogNodeType type)
        {
            if (_scanner.Token is MessageToken)
            {
                Consume<MessageToken>();
                return new MessageNode(type);
            }
            else if (_scanner.Token is WarningToken)
            {
                Consume<WarningToken>();
                return new WarningNode(type);
            }
            else if (_scanner.Token is ErrorToken)
            {
                Consume<ErrorToken>();
                return new ErrorNode(type);
            }

            throw new ParseException(_scanner.Expression);
        }

        private LogNode ParseLogNode()
        {
            var type = ParseSlash();
            return ParseLogNodeWithType(type);
        }

        private LogNode ParseLogNodeOrNull()
        {
            if (_scanner.Token is SingleSlashToken || _scanner.Token is DoubleSlashToken)
            {
                return ParseLogNode();
            }

            return null;
        }

        private AstNode ParseTaskNodeOrSingleSlashLogNode()
        {
            if (_scanner.Token is TaskToken)
            {
                Consume<TaskToken>();

                var next = ParseLogNodeOrNull();
                return new TaskNode(next);
            }

            return ParseLogNodeWithType(LogNodeType.Direct);
        }

        private AstNode ParseTaskNodeOrLogNodeOrNull()
        {
            if (_scanner.Token is SingleSlashToken)
            {
                Consume<SingleSlashToken>();
                return ParseTaskNodeOrSingleSlashLogNode();
            }
            else if (_scanner.Token is DoubleSlashToken)
            {
                Consume<DoubleSlashToken>();
                return ParseLogNodeWithType(LogNodeType.All);
            }

            return null;
        }

        private AstNode ParseTargetNodeOrTaskNodeOrSingleSlashLogNode()
        {
            if (_scanner.Token is TargetToken)
            {
                Consume<TargetToken>();

                var next = ParseTaskNodeOrLogNodeOrNull();
                return new TargetNode(next);
            }
            else if (_scanner.Token is TaskToken)
            {
                Consume<TaskToken>();

                var next = ParseLogNodeOrNull();
                return new TaskNode(next);
            }

            return ParseLogNodeWithType(LogNodeType.Direct);
        }

        private AstNode ParseTargetNodeOrTaskNodeOrLogNodeOrNull()
        {
            if (_scanner.Token is SingleSlashToken)
            {
                Consume<SingleSlashToken>();
                return ParseTargetNodeOrTaskNodeOrSingleSlashLogNode();
            }
            else if (_scanner.Token is DoubleSlashToken)
            {
                Consume<DoubleSlashToken>();
                return ParseLogNodeWithType(LogNodeType.All);
            }

            return null;
        }

        private AstNode ParseProjectNodeOrTargetNodeOrTaskNodeOrSingleSlashLogNode()
        {
            if (_scanner.Token is ProjectToken)
            {
                Consume<ProjectToken>();

                var next = ParseTargetNodeOrTaskNodeOrLogNodeOrNull();
                return new ProjectNode(next is TaskNode ? next as TaskNode : next);
            }
            else if (_scanner.Token is TargetToken)
            {
                Consume<TargetToken>();

                var next = ParseTaskNodeOrLogNodeOrNull();
                return new TargetNode(next);
            }
            else if (_scanner.Token is TaskToken)
            {
                Consume<TaskToken>();

                var next = ParseLogNodeOrNull();
                return new TaskNode(next);
            }

            return ParseLogNodeWithType(LogNodeType.Direct);
        }

        private AstNode ParseQuery()
        {
            if (_scanner.Token is SingleSlashToken)
            {
                Consume<SingleSlashToken>();
                return ParseProjectNodeOrTargetNodeOrTaskNodeOrSingleSlashLogNode();
            }
            else if (_scanner.Token is DoubleSlashToken)
            {
                Consume<DoubleSlashToken>();
                return ParseLogNodeWithType(LogNodeType.All);
            }

            throw new ParseException(_scanner.Expression);
        }
    }
}