using System.Linq.Expressions;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;

namespace PeterJuhasz.SqlInjection
{
    internal class NullEvaluatableExpressionFilter : EvaluatableExpressionFilterBase
    {
        public override bool IsEvaluatableMethodCall(MethodCallExpression node)
        {
            return node.Method.DeclaringType != typeof(MySqlFunctions);
        }
    }
}