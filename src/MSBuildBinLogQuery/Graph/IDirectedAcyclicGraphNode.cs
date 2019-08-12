using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public interface IDirectedAcyclicGraphNode
    {
    }

    public interface IDirectedAcyclicGraphNode<T> : IDirectedAcyclicGraphNode where T : IDirectedAcyclicGraphNode
    {
        ISet<T> AdjacentNodes { get; }
    }
}
