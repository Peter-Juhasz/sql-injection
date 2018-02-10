using System;
using System.Linq.Expressions;

namespace PeterJuhasz.SqlInjection
{
    using static MySqlFunctions;

    internal class ExponentFunctionErrorExpressionProvider : IErrorExpressionProvider
    {
        public Expression GetErrorExpression() => Expression.Call(
            typeof(MySqlFunctions).GetMethod(nameof(Exp), new Type[] { typeof(int) }),
            Expression.Constant(710)
        );
    }
}
