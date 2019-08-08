using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Build.Logging.Query.Graph
{
    public class DirectedAcyclicGraph<T>
        where T : IQueryableGraphNode<T>, IShallowCopyableGraphNode<T>
    {
        public IEnumerable<T> Nodes { get; }

        public DirectedAcyclicGraph(IEnumerable<T> nodes)
        {
            Nodes = nodes;
        }

        public bool TopologicalSort(out List<T> topologicalOrdering)
        {
            var inDegrees = new Dictionary<T, int>();

            foreach (var node in Nodes)
            {
                inDegrees[node] = 0;
            }

            foreach (var node in Nodes)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    inDegrees[adjacentNode]++;
                }
            }

            var queue = new Queue<T>();

            foreach (var node in Nodes)
            {
                if (inDegrees[node] == 0)
                {
                    queue.Enqueue(node);
                }
            }

            topologicalOrdering = new List<T>();

            while (queue.TryDequeue(out var first))
            {
                topologicalOrdering.Add(first);

                foreach (var adjacentNode in first.AdjacentNodes)
                {
                    inDegrees[adjacentNode]--;

                    if (inDegrees[adjacentNode] == 0)
                    {
                        queue.Enqueue(adjacentNode);
                    }
                }
            }
            
            return Nodes.All(node => inDegrees[node] == 0);
        }

        public bool GetReachableNodes(out Dictionary<T, HashSet<T>> reachables)
        {
            var reversedGraph = Reverse();

            reachables = new Dictionary<T, HashSet<T>>();

            if (!reversedGraph.TopologicalSort(out var topologicalOrdering))
            {
                return false;
            }

            foreach (var node in reversedGraph.Nodes)
            {
                reachables[node] = new HashSet<T>();
            }

            foreach (var node in topologicalOrdering)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    reachables[adjacentNode].UnionWith(reachables[node]);
                    reachables[adjacentNode].Add(node);
                }
            }

            return true;
        }

        public DirectedAcyclicGraph<T> Reverse()
        {
            var reversedNodes = Nodes.ToDictionary(
                node => node,
                node => node.ShallowCopyAndClearEdges());

            foreach (var node in Nodes)
            {
                foreach (var adjacentNode in node.AdjacentNodes)
                {
                    reversedNodes[adjacentNode].AdjacentNodes.Add(reversedNodes[node]);
                }
            }

            return new DirectedAcyclicGraph<T>(reversedNodes.Values);
        }
    }
}