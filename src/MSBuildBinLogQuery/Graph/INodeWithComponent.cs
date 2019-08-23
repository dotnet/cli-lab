using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Graph
{
    public interface INodeWithComponent<TComponent> where TComponent : Component
    {
        TComponent Component { get; }
    }
}