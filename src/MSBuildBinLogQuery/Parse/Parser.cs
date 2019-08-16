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
            switch (_scanner.Token)
            {
                case SingleSlashToken _:
                    Consume<SingleSlashToken>();
                    return LogNodeType.Direct;
                case DoubleSlashToken _:
                    Consume<DoubleSlashToken>();
                    return LogNodeType.All;
                default:
                    throw new ParseException(_scanner.Expression);
            }
        }

        private LogNode ParseLogNodeWithType(LogNodeType type)
        {
            switch (_scanner.Token)
            {
                case MessageToken _:
                    Consume<MessageToken>();
                    return new MessageNode(type);
                case WarningToken _:
                    Consume<WarningToken>();
                    return new WarningNode(type);
                case ErrorToken _:
                    Consume<ErrorToken>();
                    return new ErrorNode(type);
                default:
                    throw new ParseException(_scanner.Expression);
            }
        }

        private LogNode ParseLogNode()
        {
            var type = ParseSlash();
            return ParseLogNodeWithType(type);
        }

        private LogNode ParseNullableLogNode()
        {
            switch (_scanner.Token)
            {
                case SingleSlashToken _:
                case DoubleSlashToken _:
                    return ParseLogNode();
                default:
                    return null;
            }
        }

        private AstNode ParseSingleSlashNodeUnderTarget()
        {
            switch (_scanner.Token)
            {
                case TaskToken _:
                    Consume<TaskToken>();

                    var next = ParseNullableLogNode();
                    return new TaskNode(next);
                default:
                    return ParseLogNodeWithType(LogNodeType.Direct);
            }
        }

        private AstNode ParseNullableNodeUnderTarget()
        {
            switch (_scanner.Token)
            {
                case SingleSlashToken _:
                    Consume<SingleSlashToken>();
                    return ParseSingleSlashNodeUnderTarget();
                case DoubleSlashToken _:
                    Consume<DoubleSlashToken>();
                    return ParseLogNodeWithType(LogNodeType.All);
                default:
                    return null;
            }
        }

        private AstNode ParseSingleSlashNodeUnderProject()
        {
            switch (_scanner.Token)
            {
                case TargetToken _:
                    Consume<TargetToken>();

                    var next = ParseNullableNodeUnderTarget();
                    return new TargetNode(next);
                case TaskToken _:
                    Consume<TaskToken>();

                    next = ParseNullableLogNode();
                    return new TaskNode(next);
                default:
                    return ParseLogNodeWithType(LogNodeType.Direct);
            }
        }

        private AstNode ParseNullableNodeUnderProject()
        {
            switch (_scanner.Token)
            {
                case SingleSlashToken _:
                    Consume<SingleSlashToken>();
                    return ParseSingleSlashNodeUnderProject();
                case DoubleSlashToken _:
                    Consume<DoubleSlashToken>();
                    return ParseLogNodeWithType(LogNodeType.All);
                default:
                    return null;
            }
        }

        private AstNode ParseSingleSlashQueryNode()
        {
            switch (_scanner.Token)
            {
                case ProjectToken _:
                    Consume<ProjectToken>();

                    var next = ParseNullableNodeUnderProject();
                    return new ProjectNode(next is TaskNode ? next as TaskNode : next);
                case TargetToken _:
                    Consume<TargetToken>();

                    next = ParseNullableNodeUnderTarget();
                    return new TargetNode(next);
                case TaskToken _:
                    Consume<TaskToken>();

                    next = ParseNullableLogNode();
                    return new TaskNode(next);
                default:
                    return ParseLogNodeWithType(LogNodeType.Direct);
            }
        }

        private AstNode ParseQuery()
        {
            switch (_scanner.Token)
            {
                case SingleSlashToken _:
                    Consume<SingleSlashToken>();
                    return ParseSingleSlashQueryNode();
                case DoubleSlashToken _:
                    Consume<DoubleSlashToken>();
                    return ParseLogNodeWithType(LogNodeType.All);
                default:
                    throw new ParseException(_scanner.Expression);

            }
        }
    }
}