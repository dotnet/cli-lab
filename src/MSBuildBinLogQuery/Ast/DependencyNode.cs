using Microsoft.Build.Logging.Query.Graph;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class DependencyNode<TParent, TGraphNode> : ConstraintNode<TParent, IAstNode<Result.Build>>
        where TParent : class, IQueryResult
        where TGraphNode : IDirectedAcyclicGraphNode<TGraphNode>
    {
        public DependencyNodeType Type { get; }

        public DependencyNode(IAstNode<Result.Build> value, DependencyNodeType type) : base(value)
        {
            Type = type;
        }

        protected bool Equals(DependencyNode<TParent, TGraphNode> other)
        {
            return base.Equals(other) &&
                   Type == other.Type;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}