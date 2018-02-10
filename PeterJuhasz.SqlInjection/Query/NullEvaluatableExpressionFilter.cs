using System.Linq.Expressions;
using System.Net.Http;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace PeterJuhasz.SqlInjection
{
    internal class NullEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
    {
        public override bool IsEvaluatableMethodCall(MethodCallExpression node) => node.Method.DeclaringType != typeof(MySqlFunctions);
    }
}