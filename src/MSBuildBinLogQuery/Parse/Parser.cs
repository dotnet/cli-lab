using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Result;
using Microsoft.Build.Logging.Query.Scan;
using Microsoft.Build.Logging.Query.Token;

namespace Microsoft.Build.Logging.Query.Parse
{
    public class Parser
    {
        private readonly Scanner _scanner;

        private delegate bool TryConstraintParser<TParent>(out ConstraintNode<TParent> constraint)
            where TParent : class, IQueryResult, IResultWithId;

        private Parser(Scanner scanner)
        {
            _scanner = scanner;
        }

        public static IAstNode<Result.Build> Parse(Scanner scanner)
        {
            var parser = new Parser(scanner);
            var queryNode = parser.ParseQuery();

            if (parser._scanner.Token is EofToken)
            {
                return queryNode;
            }

            throw new ParseException(parser._scanner.Expression);
        }

        public static IAstNode<Result.Build> Parse(string expression)
        {
            var scanner = new Scanner(expression);
            return Parse(scanner);
        }

        private TToken Consume<TToken>() where TToken : Token.Token
        {
            if (_scanner.Token is TToken)
            {
                var result = _scanner.Token as TToken;
                _scanner.ReadNextToken();

                return result;
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

        private IdNode<TParent> ParseIdConstraint<TParent>()
            where TParent : class, IQueryResult, IResultWithId
        {
            Consume<IdToken>();
            Consume<EqualToken>();

            var value = Consume<IntegerToken>().Value;
            return new IdNode<TParent>(value);
        }

        private NameNode<TParent> ParseNameConstraint<TParent>()
            where TParent : class, IQueryResult, IResultWithName
        {
            Consume<NameToken>();
            Consume<EqualToken>();

            var value = Consume<StringToken>().Value;
            return new NameNode<TParent>(value);
        }

        private PathNode<TParent> ParsePathConstraint<TParent>()
            where TParent : class, IQueryResult, IResultWithPath
        {
            Consume<PathToken>();
            Consume<EqualToken>();

            var value = Consume<StringToken>().Value;
            return new PathNode<TParent>(value);
        }

        private bool TryParseTaskConstraint(out ConstraintNode<Task> constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint<Task>();
                    return true;
                case NameToken _:
                    constraint = ParseNameConstraint<Task>();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private bool TryParseTargetConstraint(out ConstraintNode<Target> constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint<Target>();
                    return true;
                case NameToken _:
                    constraint = ParseNameConstraint<Target>();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private bool TryParseProjectConstraint(out ConstraintNode<Project> constraint)
        {
            switch (_scanner.Token)
            {
                case IdToken _:
                    constraint = ParseIdConstraint<Project>();
                    return true;
                case NameToken _:
                    constraint = ParseNameConstraint<Project>();
                    return true;
                case PathToken _:
                    constraint = ParsePathConstraint<Project>();
                    return true;
                default:
                    constraint = null;
                    return false;
            };
        }

        private List<ConstraintNode<TParent>> ParseConstraints<TParent>(TryConstraintParser<TParent> tryConstraintParser)
            where TParent : class, IQueryResult, IResultWithId
        {
            var constraints = new List<ConstraintNode<TParent>>();

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

            var constraints = ParseConstraints<Task>(TryParseTaskConstraint);
            var next = ParseNullableLogNode();
            var task = next == null ?
                new TaskNode(constraints) :
                new TaskNode(next, constraints);

            return task;
        }

        private TargetNode ParseTargetNode()
        {
            Consume<TargetToken>();

            var constraints = ParseConstraints<Target>(TryParseTargetConstraint);
            var next = ParseNullableNodeUnderTarget();
            var target = next == null ?
                new TargetNode(constraints) :
                new TargetNode(next, constraints);

            return target;
        }

        private ProjectNode ParseProjectNode()
        {
            Consume<ProjectToken>();

            var constraints = ParseConstraints<Project>(TryParseProjectConstraint);
            var next = ParseNullableNodeUnderProject();
            var project = next == null ?
                new ProjectNode(constraints) :
                new ProjectNode(next, constraints);

            return project;
        }

        private IAstNode<Target> ParseSingleSlashNodeUnderTarget()
        {
            return _scanner.Token switch
            {
                TaskToken _ => ParseTaskNode() as IAstNode<Target>,
                _ => ParseLogNodeWithType(LogNodeType.Direct),
            };
        }

        private IAstNode<Target> ParseNullableNodeUnderTarget()
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

        private IAstNode<Project> ParseSingleSlashNodeUnderProject()
        {
            return _scanner.Token switch
            {
                TargetToken _ => ParseTargetNode() as IAstNode<Project>,
                TaskToken _ => new TargetNode(ParseTaskNode()),
                _ => ParseLogNodeWithType(LogNodeType.Direct),
            };
        }

        private IAstNode<Project> ParseNullableNodeUnderProject()
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

        private IAstNode<Result.Build> ParseSingleSlashQueryNode()
        {
            return _scanner.Token switch
            {
                ProjectToken _ => ParseProjectNode() as IAstNode<Result.Build>,
                TargetToken _ => new ProjectNode(ParseTargetNode()),
                TaskToken _ => new ProjectNode(new TargetNode(ParseTaskNode())),
                _ => ParseLogNodeWithType(LogNodeType.Direct),
            };
        }

        private IAstNode<Result.Build> ParseQuery()
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