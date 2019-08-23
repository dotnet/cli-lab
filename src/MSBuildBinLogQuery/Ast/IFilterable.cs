using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Ast
{
    public interface IFilterable<TIn, TOut>
        where TIn : IQueryResult
        where TOut : IQueryResult
    {
        IEnumerable<TOut> Filter(IEnumerable<TIn> components);
    }

    public interface IFilterable<TIn, TOut, TBefore>
        where TIn : IQueryResult
        where TOut : IQueryResult
        where TBefore : IQueryResult
    {
        IEnumerable<TOut> Filter(IEnumerable<TIn> components, IEnumerable<TBefore> previousComponents);
    }
}