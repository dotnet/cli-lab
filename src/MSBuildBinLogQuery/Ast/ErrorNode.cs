namespace Microsoft.Build.Logging.Query.Ast
{
    public class ErrorNode : QueryNode
    {
        public ErrorNode(QueryNode next) : base(next)
        {
        }
    }
}