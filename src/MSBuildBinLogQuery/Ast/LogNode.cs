using System;
using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class LogNode :
        IAstNode<Result.Build>,
        IAstNode<Project>,
        IAstNode<Target>,
        IAstNode<Task>
    {
        public LogNode()
        {
        }

        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<Result.Build> components);
        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<Project> components);
        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<Target> components);
        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<Task> components);
    }

    public abstract class LogNode<TThis> : LogNode, IAstNode<TThis, Component> where TThis : Log
    {
        public LogNodeType Type { get; }

        public LogNode(LogNodeType type) : base()
        {
            Type = type;
        }

        protected bool Equals(LogNode<TThis> other)
        {
            return other != null &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type);
        }

        public abstract IEnumerable<IQueryResult> Filter(IEnumerable<Component> components);
    }
}