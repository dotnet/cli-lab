using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Parse;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Scan
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
                new TaskNode(null)
            };

            yield return new object[]
            {
                "/target",
                new TargetNode(null)
            };

            yield return new object[]
            {
                "/project",
                new ProjectNode(null)
            };

            yield return new object[]
            {
                "/task/message",
                new TaskNode(new MessageNode(LogNodeType.Direct))
            };

            yield return new object[]
            {
                "/target/warning",
                new TargetNode(new WarningNode(LogNodeType.Direct))
            };

            yield return new object[]
            {
                "/project/error",
                new ProjectNode(new ErrorNode(LogNodeType.Direct))
            };

            yield return new object[]
            {
                "/target/task/message",
                new TargetNode(new TaskNode(new MessageNode(LogNodeType.Direct)))
            };

            yield return new object[]
            {
                "/project/task/warning",
                new ProjectNode(new TaskNode(new WarningNode(LogNodeType.Direct)))
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
        }

        [Theory]
        [MemberData(nameof(GenerateDataForTestParsedAst))]
        public void TestParsedAst(string expression, AstNode expectedAst)
        {
            var actualAst = Parser.Parse(expression);
            actualAst.Should().Be(expectedAst);
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("message")]
        [InlineData("project/message")]
        [InlineData("/warning/")]
        [InlineData("/message/project")]
        [InlineData("/task/task")]
        [InlineData("/project/target/target/task/error")]
        [InlineData("/project//target")]
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