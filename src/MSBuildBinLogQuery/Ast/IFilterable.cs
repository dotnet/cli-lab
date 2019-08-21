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
}