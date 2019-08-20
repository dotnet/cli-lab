using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Ast;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Ast
{
    public class TargetNodeTests
    {
        private readonly TaskNode _testTask;
        private readonly LogNode _testLog;
        private readonly List<ConstraintNode> _testConstraints;

        public TargetNodeTests()
        {
            _testTask = new TaskNode();
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
            var target = new TargetNode();

            target.Next.Should().BeNull();
            target.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_NoNext_HasConstraints()
        {
            var target = new TargetNode(_testConstraints);

            target.Next.Should().BeNull();
            target.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                target.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextTask_NoConstraints()
        {
            var target = new TargetNode(_testTask);

            target.Next.Should().Be(_testTask);
            target.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextTask_HasConstraints()
        {
            var target = new TargetNode(_testTask, _testConstraints);

            target.Next.Should().Be(_testTask);
            target.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                target.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextLog_NoConstraints()
        {
            var target = new TargetNode(_testLog);

            target.Next.Should().Be(_testLog);
            target.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextLog_HasConstraints()
        {
            var target = new TargetNode(_testLog, _testConstraints);

            target.Next.Should().Be(_testLog);
            target.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                target.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_ArgumentNull()
        {
            Action action = () => new TargetNode((AstNode)null);
            action.Should().Throw<ArgumentNullException>();
        }
    }
}