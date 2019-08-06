using Microsoft.Build.Logging.Query.Utility;

namespace Microsoft.Build.Logging.Query.Graph
{
    public interface IQueryableGraphNode
    {
    }

    public interface IQueryableGraphNode<T> : IQueryableGraphNode where T : IQueryableGraphNode
    {
        ConcurrentHashSet<T> AdjacentNodes { get; }
    }
}
