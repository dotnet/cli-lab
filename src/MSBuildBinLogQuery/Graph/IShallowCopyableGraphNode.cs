namespace Microsoft.Build.Logging.Query.Graph
{
    public interface IShallowCopyableGraphNode<T> where T : IQueryableGraphNode
    {
        T ShallowCopyAndClearEdges();
    }
}
