namespace Microsoft.Build.Logging.Query.Ast
{
    public abstract class QueryNode
    {
        public QueryNode Next { get; }

        public QueryNode(QueryNode next)
        {
            Next = next;
        }
    }
}