using Microsoft.Build.Logging.Query.Graph;

namespace Microsoft.Build.Logging.Query.Result
{
    public interface IResultWithBeforeThis<TNode> : IQueryResult where TNode : IDirectedAcyclicGraphNode<TNode>
    {
        TNode Node_BeforeThis { get; }
    }
}