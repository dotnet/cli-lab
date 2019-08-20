using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Scan;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Parse
{
    public class Parser
    {
        private readonly Scanner _scanner;

        private delegate bool TryConstraintParser(out ConstraintNode constraint);

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

        private IdNode ParseIdConstraint()
        {
            Consume<IdToken>();
            Consume<EqualToken>();

            var integerToken = _scanner.Token;
            Consume<IntegerToken>();
            var value = (integerToken as IntegerToken).Value;

            return new IdNode(value);
        }

        private bool TryParseTaskConstraint(out ConstraintNode constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private bool TryParseTargetConstraint(out ConstraintNode constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private bool TryParseProjectConstraint(out ConstraintNode constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private List<ConstraintNode> ParseConstraints(TryConstraintParser tryConstraintParser)
        {
            var constraints = new List<ConstraintNode>();

            if (!(_scanner.Token is LeftBracketToken))
            {
                return constraints;
            }

            Consume<LeftBracketToken>();

            if (!tryConstraintParser.Invoke(out var constraint))
            {
                Consume<RightBracketToken>();
                return constraints;
            }

            constraints.Add(constraint);

            while (_scanner.Token is CommaToken)
            {
                Consume<CommaToken>();

                if (!tryConstraintParser.Invoke(out var anotherConstraint))
                {
                    throw new ParseException(_scanner.Expression);
                }

                constraints.Add(anotherConstraint);
            }

            Consume<RightBracketToken>();

            return constraints;
        }

        private TaskNode ParseTaskNode()
        {
            Consume<TaskToken>();

            var constraints = ParseConstraints(TryParseTaskConstraint);
            var next = ParseNullableLogNode();
            var task = next == null ?
                new TaskNode(constraints) :
                new TaskNode(next, constraints);

            return task;
        }

        private TargetNode ParseTargetNode()
        {
            Consume<TargetToken>();

            var constraints = ParseConstraints(TryParseTargetConstraint);
            var next = ParseNullableNodeUnderTarget();
            var target = next == null ?
                new TargetNode(constraints) :
                new TargetNode(next, constraints);

            return target;
        }

        private ProjectNode ParseProjectNode()
        {
            Consume<ProjectToken>();

            var constraints = ParseConstraints(TryParseProjectConstraint);
            var next = ParseNullableNodeUnderProject();
            var project = next == null ?
                new ProjectNode(constraints) :
                new ProjectNode(next, constraints);

            return project;
        }

        private AstNode ParseSingleSlashNodeUnderTarget()
        {
            return _scanner.Token switch
            {
                TaskToken _ => ParseTaskNode() as AstNode,
                _ => ParseLogNodeWithType(LogNodeType.Direct) as AstNode,
            };
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
            return _scanner.Token switch
            {
                TargetToken _ => ParseTargetNode() as AstNode,
                TaskToken _ => ParseTaskNode() as AstNode,
                _ => ParseLogNodeWithType(LogNodeType.Direct) as AstNode,
            };
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
            return _scanner.Token switch
            {
                ProjectToken _ => ParseProjectNode() as AstNode,
                TargetToken _ => ParseTargetNode() as AstNode,
                TaskToken _ => ParseTaskNode() as AstNode,
                _ => ParseLogNodeWithType(LogNodeType.Direct) as AstNode,
            };
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