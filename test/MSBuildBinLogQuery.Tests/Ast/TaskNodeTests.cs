using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Ast;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Ast
{
    public class TaskNodeTests
    {
        private readonly LogNode _testLog;
        private readonly List<ConstraintNode> _testConstraints;

        public TaskNodeTests()
        {
            _testLog = new MessageNode(LogNodeType.All);
            _testConstraints = new List<ConstraintNode>
            {
                new IdNode(123),
                new IdNode(456)
            };
        }

        [Fact]
        public void TestConstructor_NoNext_NoConstraints()
        {
            var task = new TaskNode();

            task.Next.Should().BeNull();
            task.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_NoNext_HasConstraints()
        {
            var task = new TaskNode(_testConstraints);

            task.Next.Should().BeNull();
            task.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                task.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextLog_NoConstraints()
        {
            var task = new TaskNode(_testLog);

            task.Next.Should().Be(_testLog);
            task.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextLog_HasConstraints()
        {
            var task = new TaskNode(_testLog, _testConstraints);

            task.Next.Should().Be(_testLog);
            task.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                task.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_ArgumentNull()
        {
            Action action = () => new TaskNode((AstNode)null);
            action.Should().Throw<ArgumentNullException>();
        }
    }
}