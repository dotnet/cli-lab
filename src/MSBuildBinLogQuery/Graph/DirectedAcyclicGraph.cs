using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class DirectedAcyclicGraph<T> where T : IQueryableGraphNode<T>, IShallowCopyableGraphNode<T>
    {
        public Dictionary<T, DirectedAcyclicGraphNode<T>> Nodes { get; }

        private readonly IEqualityComparer<T> _equalityComparer;

        public DirectedAcyclicGraph(IEnumerable<T> nodes, IEqualityComparer<T> equalityComparer)
        {
            Nodes = nodes.ToDictionary(
                node => node,
                node => new DirectedAcyclicGraphNode<T>(node),
                equalityComparer);
            _equalityComparer = equalityComparer;
        }

        public bool TopologicalSort(out List<DirectedAcyclicGraphNode<T>> topologicalOrdering)
        {
            foreach (var node in Nodes.Values)
            {
                node.InDegree = 0;
            }

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
                    queue.Enqueue(node);
                }
            }

            topologicalOrdering = new List<DirectedAcyclicGraphNode<T>>();

            while (queue.TryDequeue(out var first))
            {
                topologicalOrdering.Add(first);

                foreach (var wrappedAdjacentNode in first.WrappedNode.AdjacentNodes)
                {
                    var adjacentNode = Nodes[wrappedAdjacentNode];
                    adjacentNode.InDegree--;

                    if (adjacentNode.InDegree == 0)
                    {
                        queue.Enqueue(adjacentNode);
                    }
                }
            }
            
            return Nodes.Values.All(node => node.InDegree == 0);
        }

        public bool CalculateReachableNodes()
        {
            var reversedGraph = Reverse();

            if (!reversedGraph.TopologicalSort(out var topologicalOrdering))
            {
                return false;
            }

            foreach (var node in topologicalOrdering)
            {
                foreach (var wrappedAdjacentNode in node.WrappedNode.AdjacentNodes)
                {
                    var adjacentNode = reversedGraph.Nodes[wrappedAdjacentNode];

                    node.ReachableFromThis.UnionWith(adjacentNode.ReachableFromThis);
                    node.ReachableFromThis.Add(adjacentNode);
                }
            }

            return true;
        }

        public DirectedAcyclicGraph<T> Reverse()
        {
            var reservedNodes = Nodes.Keys.ToDictionary(
                node => node,
                node => node.ShallowCopyAndClearEdges(),
                _equalityComparer);

            foreach (var node in Nodes.Keys)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    reservedNodes[adjacentNode].AdjacentNodes.Add(node);
                }
            }

            return new DirectedAcyclicGraph<T>(reservedNodes.Values, _equalityComparer);
        }
    }
}