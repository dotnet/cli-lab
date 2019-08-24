namespace Microsoft.Build.Logging.Query.Result
{
    public interface IResultWithPath : IQueryResult
    {
        string Path { get; }
    }
}