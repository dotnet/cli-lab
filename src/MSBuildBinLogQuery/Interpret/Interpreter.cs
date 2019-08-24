using System.Collections.Generic;
using Microsoft.Build.Logging.Query.Ast;
using Microsoft.Build.Logging.Query.Parse;
using Microsoft.Build.Logging.Query.Result;

namespace Microsoft.Build.Logging.Query.Interpret
{
    public class Interpreter
    {
        private readonly IAstNode<Result.Build> _ast;

        public Interpreter(IAstNode<Result.Build> ast)
        {
            _ast = ast;
        }

        public Interpreter(string expression)
        {
            _ast = Parser.Parse(expression);
        }
        
        public IEnumerable<IQueryResult> Filter(Result.Build build)
        {
            return _ast.Filter(new[] { build });
        }
    }
}