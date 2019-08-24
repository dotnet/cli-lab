using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public interface IAstNode
    {
    }

    public interface IAstNode<TBefore> : IAstNode, IFilterable<TBefore, IQueryResult>
        where TBefore : class, IQueryResult
    {
    }

    public interface IAstNode<TThis, TBefore> : IAstNode<TBefore>
        where TThis : class, IQueryResult
        where TBefore : class, IQueryResult
    {
    }
}