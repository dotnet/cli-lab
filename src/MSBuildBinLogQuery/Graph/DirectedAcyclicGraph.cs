using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class DirectedAcyclicGraph<T> where T : IQueryableGraphNode<T>
    {
        public Dictionary<T, DirectedAcyclicGraphNode<T>> Nodes { get; }

        public DirectedAcyclicGraph(IEnumerable<T> nodes, IEqualityComparer<T> equalityComparer)
        {
            Nodes = nodes.ToDictionary(
                node => node,
                node => new DirectedAcyclicGraphNode<T>(node),
                equalityComparer);
        }

        public bool TopologicalSort(out IEnumerable<DirectedAcyclicGraphNode<T>> topologicalOrdering)
        {
            foreach (var node in Nodes.Values)
            {
                foreach (var wrappedAdjacentNode in node.WrappedNode.AdjacentNodes)
                {
                    Nodes[wrappedAdjacentNode].InDegree++;
                }
            }

            var queue = new Queue<DirectedAcyclicGraphNode<T>>();

            foreach (var node in Nodes.Values)
            {
                if (node.InDegree == 0)
                {
                    queue.Append(node);
                }
            }

            topologicalOrdering = new List<DirectedAcyclicGraphNode<T>>();

            while (queue.TryDequeue(out var first))
            {
                topologicalOrdering.Append(first);

                foreach (var wrappedAdjacentNode in first.WrappedNode.AdjacentNodes)
                {
                    var adjacentNode = Nodes[wrappedAdjacentNode];
                    adjacentNode.InDegree--;

                    if (adjacentNode.InDegree == 0)
                    {
                        queue.Append(adjacentNode);
                    }
                }
            }
            
            return Nodes.Values.All(node => node.InDegree == 0);
        }
    }
}