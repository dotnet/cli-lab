using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Component
{
    public interface IComponentWithId : IQueryResult
    {
        int Id { get; }
    }
}