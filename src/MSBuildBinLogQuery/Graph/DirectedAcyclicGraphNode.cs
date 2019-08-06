using System.Collections.Generic;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class DirectedAcyclicGraphNode<T> where T : IQueryableGraphNode<T>
    {
        public T WrappedNode { get; }
        public int InDegree { get; set; }
        public HashSet<DirectedAcyclicGraphNode<T>> ReachableFromThis { get; internal set; }

        public DirectedAcyclicGraphNode(T wrappedNode)
        {
            WrappedNode = wrappedNode;
            InDegree = default;
            ReachableFromThis = new HashSet<DirectedAcyclicGraphNode<T>>();
        }
    }
}