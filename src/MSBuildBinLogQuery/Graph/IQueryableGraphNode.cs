using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public interface IQueryableGraphNode
    {
    }

    public interface IQueryableGraphNode<T> : IQueryableGraphNode where T : IQueryableGraphNode
    {
        ISet<T> AdjacentNodes { get; }
    }
}
