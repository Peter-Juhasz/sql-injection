using System.Linq.Expressions;

namespace PeterJuhasz.SqlInjection
{
    public interface IErrorExpressionProvider
    {
        Expression GetErrorExpression();
    }
}
