namespace Microsoft.Build.Logging.Query.Result
{
    public interface IResultWithName : IQueryResult
    {
        string Name { get; }
    }
}