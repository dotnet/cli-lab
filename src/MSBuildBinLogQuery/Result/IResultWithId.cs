namespace Microsoft.Build.Logging.Query.Result
{
    public interface IResultWithId : IQueryResult
    {
        int Id { get; }
    }
}