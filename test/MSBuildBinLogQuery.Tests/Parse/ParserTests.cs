using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Parse;
using Microsoft.Build.Logging.Query.Result;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Parse
{
    public class ParserTests
    {
        public static IEnumerable<object[]> GenerateDataForTestParsedAst()
        {
            yield return new object[]
            {
                "/message",
                new MessageNode(LogNodeType.Direct)
            };

            yield return new object[]
            {
                "/warning",
                new WarningNode(LogNodeType.Direct)
            };

            yield return new object[]
            {
                "/error",
                new ErrorNode(LogNodeType.Direct)
            };

            yield return new object[]
            {
                "/task",
                new ProjectNode(new TargetNode(new TaskNode()))
            };

            yield return new object[]
            {
                "/target",
                new ProjectNode(new TargetNode())
            };

            yield return new object[]
            {
                "/project",
                new ProjectNode()
            };

            yield return new object[]
            {
                "/task/message",
                new ProjectNode(new TargetNode(new TaskNode(new MessageNode(LogNodeType.Direct))))
            };

            yield return new object[]
            {
                "/target/warning",
                new ProjectNode(new TargetNode(new WarningNode(LogNodeType.Direct)))
            };

            yield return new object[]
            {
                "/project/error",
                new ProjectNode(new ErrorNode(LogNodeType.Direct))
            };

            yield return new object[]
            {
                "/target/task/message",
                new ProjectNode(new TargetNode(new TaskNode(new MessageNode(LogNodeType.Direct))))
            };

            yield return new object[]
            {
                "/project/task/warning",
                new ProjectNode(new TargetNode(new TaskNode(new WarningNode(LogNodeType.Direct))))
            };

            yield return new object[]
            {
                "/project/target/task/error",
                new ProjectNode(new TargetNode(new TaskNode(new ErrorNode(LogNodeType.Direct))))
            };

            yield return new object[]
            {
                "//message",
                new MessageNode(LogNodeType.All)
            };

            yield return new object[]
            {
                "/project//warning",
                new ProjectNode(new WarningNode(LogNodeType.All))
            };

            yield return new object[]
            {
                "/target/task//error",
                new ProjectNode(new TargetNode(new TaskNode(new ErrorNode(LogNodeType.All))))
            };

            yield return new object[]
            {
                "/project/task//message",
                new ProjectNode(new TargetNode(new TaskNode(new MessageNode(LogNodeType.All))))
            };

            yield return new object[]
            {
                "/Task[]",
                new ProjectNode(new TargetNode(new TaskNode()))
            };

            yield return new object[]
            {
                "/Task[ID=341]/Message",
                new ProjectNode(new TargetNode(new TaskNode(
                    new MessageNode(LogNodeType.Direct),
                    new List<ConstraintNode<Task>> { new IdNode<Task>(341) })))
            };

            yield return new object[]
            {
                "/Project/Task[id=1, Id=2, ID=3]//Warning",
                new ProjectNode(new TargetNode(new TaskNode(
                    new WarningNode(LogNodeType.All),
                    new List<ConstraintNode<Task>>
                    {
                        new IdNode<Task>(1),
                        new IdNode<Task>(2),
                        new IdNode<Task>(3)
                    })))
            };

            yield return new object[]
            {
                "/Target[]",
                new ProjectNode(new TargetNode())
            };

            yield return new object[]
            {
                "/Target[Id=153]",
                new ProjectNode(new TargetNode(new List<ConstraintNode<Target>> { new IdNode<Target>(153) }))
            };

            yield return new object[]
            {
                "/Project/Target[Id=980321]//Error",
                new ProjectNode(
                    new TargetNode(
                        new ErrorNode(LogNodeType.All),
                        new List<ConstraintNode<Target>> { new IdNode<Target>(980321) }))
            };

            yield return new object[]
            {
                "/Target[Id=9]/Task[ID=81]/Warning",
                new ProjectNode(new TargetNode(
                    new TaskNode(
                        new WarningNode(LogNodeType.Direct),
                        new List<ConstraintNode<Task>> { new IdNode<Task>(81) }),
                    new List<ConstraintNode<Target>> { new IdNode<Target>(9) }))
            };

            yield return new object[]
            {
                "/Project[]",
                new ProjectNode()
            };

            yield return new object[]
            {
                "/Project[Id=536]",
                new ProjectNode(new List<ConstraintNode<Project>> { new IdNode<Project>(536) })
            };

            yield return new object[]
            {
                "/Project[ID=448]/Error",
                new ProjectNode(
                    new ErrorNode(LogNodeType.Direct),
                    new List<ConstraintNode<Project>> { new IdNode<Project>(448) })
            };

            yield return new object[]
            {
                "/Project[Id=121]/Task[Id=421]",
                new ProjectNode(
                    new TargetNode(
                        new TaskNode(new List<ConstraintNode<Task>> { new IdNode<Task>(421) })),
                    new List<ConstraintNode<Project>> { new IdNode<Project>(121) })
            };

            yield return new object[]
            {
                "/Project[Id=1]/Target[Id=2]/Task[Id=3]//Message",
                new ProjectNode(
                    new TargetNode(
                        new TaskNode(
                            new MessageNode(LogNodeType.All),
                            new List<ConstraintNode<Task>> { new IdNode<Task>(3) }),
                        new List<ConstraintNode<Target>> { new IdNode<Target>(2) }),
                    new List<ConstraintNode<Project>> { new IdNode<Project>(1) })
            };

            yield return new object[]
            {
                "/Target[Name=\"Compile\"]",
                new ProjectNode(
                    new TargetNode(
                        new List<ConstraintNode<Target>> { new NameNode<Target>("Compile") }))
            };

            yield return new object[]
            {
                "/Task[Name=\"Message\"]",
                new ProjectNode(
                    new TargetNode(
                        new TaskNode(
                            new List<ConstraintNode<Task>> { new NameNode<Task>("Message") })))
            };

            yield return new object[]
            {
                "/Target[Name=\"Compile\"]//Message",
                new ProjectNode(
                    new TargetNode(
                        new MessageNode(LogNodeType.All),
                        new List<ConstraintNode<Target>> { new NameNode<Target>("Compile") }))
            };

            yield return new object[]
            {
                "/Project[Id=1]/Target[Name=\"Compile\"]/Warning",
                new ProjectNode(
                    new TargetNode(
                        new WarningNode(LogNodeType.Direct),
                        new List<ConstraintNode<Target>> { new NameNode<Target>("Compile") }),
                    new List<ConstraintNode<Project>> { new IdNode<Project>(1) }
                )
            };

            yield return new object[]
            {
                "/Project[Id=1]/Task[Name=\"Message\"]//Warning",
                new ProjectNode(
                    new TargetNode(new TaskNode(
                        new WarningNode(LogNodeType.All),
                        new List<ConstraintNode<Task>> { new NameNode<Task>("Message") })),
                    new List<ConstraintNode<Project>> { new IdNode<Project>(1) }
                )
            };

            yield return new object[]
            {
                "/Project[Name=\"MSBuildBinLogQuery\"]",
                new ProjectNode(
                    new List<ConstraintNode<Project>> { new NameNode<Project>("MSBuildBinLogQuery") })
            };

            yield return new object[]
            {
                "/Project[Name=\"MSBuildBinLogQuery\"]//Message",
                new ProjectNode(
                    new MessageNode(LogNodeType.All),
                    new List<ConstraintNode<Project>> { new NameNode<Project>("MSBuildBinLogQuery") })
            };

            yield return new object[]
            {
                "/Project[Path=\"./Proj1.csproj\"]",
                new ProjectNode(
                    new List<ConstraintNode<Project>> { new PathNode<Project>("./Proj1.csproj") })
            };

            yield return new object[]
            {
                "/Project[Path=\"./test/Proj1.Tests/Proj1.Tests.csproj\"]/Task[Name=\"ResolveSdk\"]/Warning",
                new ProjectNode(
                    new TargetNode(new TaskNode(
                        new WarningNode(LogNodeType.Direct),
                        new List<ConstraintNode<Task>> { new NameNode<Task>("ResolveSdk") })),
                    new List<ConstraintNode<Project>> { new PathNode<Project>("./test/Proj1.Tests/Proj1.Tests.csproj") })
            };
        }

        [Theory]
        [MemberData(nameof(GenerateDataForTestParsedAst))]
        public void TestParsedAst(string expression, IAstNode expectedAst)
        {
            var actualAst = Parser.Parse(expression);
            actualAst.Should().Be(expectedAst);
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("//")]
        [InlineData("message")]
        [InlineData("project/message")]
        [InlineData("/warning/")]
        [InlineData("/message/project")]
        [InlineData("/task/task")]
        [InlineData("/project/target/target/task/error")]
        [InlineData("/project//target")]
        [InlineData("//message/target")]
        [InlineData("/project/target/task//warning/task")]
        [InlineData("/message/message")]
        [InlineData("/warning//error")]
        [InlineData("//error/message")]
        [InlineData("//warning//message")]
        [InlineData("/Task[ID=\"123\"]")]
        [InlineData("/Task[ID==123]")]
        [InlineData("/Task[ID]")]
        [InlineData("/Task[ID=123")]
        [InlineData("/Target[Id=\"999\"]/Task")]
        [InlineData("/Target[Id,Id=123]")]
        [InlineData("/Project[[Id=1]]")]
        public void TestParsedAstException(string expression)
        {
            Action action = () =>
            {
                var _ = Parser.Parse(expression);
            };

            action.Should().Throw<ParseException>(expression);
        }
    }
}