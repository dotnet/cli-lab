namespace Microsoft.Build.Logging.Query.Graph
{
    public interface IShallowCopyableGraphNode<T> where T : IDirectedAcyclicGraphNode
    {
        T ShallowCopyAndClearEdges();
    }
}
