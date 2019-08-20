using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Build.Logging.Query.Ast;
using Xunit;

namespace Microsoft.Build.Logging.Query.Tests.Ast
{
    public class ProjectNodeTests
    {
        private readonly TargetNode _testTarget;
        private readonly TaskNode _testTask;
        private readonly LogNode _testLog;
        private readonly List<ConstraintNode> _testConstraints;

        public ProjectNodeTests()
        {
            _testTarget = new TargetNode();
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
            var project = new ProjectNode();

            project.Next.Should().BeNull();
            project.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_NoNext_HasConstraints()
        {
            var project = new ProjectNode(_testConstraints);

            project.Next.Should().BeNull();
            project.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                project.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextTarget_NoConstraints()
        {
            var project = new ProjectNode(_testTarget);

            project.Next.Should().Be(_testTarget);
            project.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextTarget_HasConstraints()
        {
            var project = new ProjectNode(_testTarget, _testConstraints);

            project.Next.Should().Be(_testTarget);
            project.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                project.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextTask_NoConstraints()
        {
            var project = new ProjectNode(_testTask);

            project.Next.Should().BeOfType<TargetNode>();
            (project.Next as TargetNode).Next.Should().Be(_testTask);
            project.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextTask_HasConstraints()
        {
            var project = new ProjectNode(_testTask, _testConstraints);

            project.Next.Should().BeOfType<TargetNode>();
            (project.Next as TargetNode).Next.Should().Be(_testTask);
            project.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                project.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_HasNextLog_NoConstraints()
        {
            var project = new ProjectNode(_testLog);

            project.Next.Should().Be(_testLog);
            project.Constraints.Should().BeEmpty();
        }

        [Fact]
        public void TestConstructor_HasNextLog_HasConstraints()
        {
            var project = new ProjectNode(_testLog, _testConstraints);

            project.Next.Should().Be(_testLog);
            project.Constraints.Count.Should().Be(_testConstraints.Count);

            for (var i = 0; i < _testConstraints.Count; i++)
            {
                project.Constraints[i].Should().Be(_testConstraints[i]);
            }
        }

        [Fact]
        public void TestConstructor_AstNodeArgumentNull()
        {
            Action action = () => new ProjectNode((AstNode)null);
            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void TestConstructor_TaskNodeArgumentNull()
        {
            Action action = () => new ProjectNode((TaskNode)null);
            action.Should().Throw<ArgumentNullException>();
        }
    }
}