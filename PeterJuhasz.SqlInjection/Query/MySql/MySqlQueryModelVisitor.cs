using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.ResultOperators;
using System.Linq;
using System.Linq.Expressions;

namespace PeterJuhasz.SqlInjection
{
    internal class MySqlQueryModelVisitor : InjectionQueryModelVisitor
    {
        public MySqlQueryModelVisitor(SqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        public override void VisitQueryModel(QueryModel queryModel)
        {
            VisitSelectClause(queryModel.SelectClause, queryModel);
            VisitMainFromClause(queryModel.MainFromClause, queryModel);

            bool first = true;
            foreach (WhereClause w in queryModel.BodyClauses.OfType<WhereClause>())
            {
                if (!first)
                {
                    Writer.WriteWhiteSpace();
                    Writer.WriteKeyword("AND");
                }

                VisitWhereClause(w, queryModel, 0);

                first = false;
            }

            foreach (OrderByClause clause in queryModel.BodyClauses.OfType<OrderByClause>())
                VisitOrderByClause(clause, queryModel, 0);

            foreach (var op in queryModel.ResultOperators)
                VisitResultOperator(op, queryModel, 0);
        }

        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            Writer.WriteWhiteSpace();
            Writer.WriteKeyword("FROM");
            Writer.WriteWhiteSpace();
            
            Writer.WriteIdentifier((DbSet ?? (fromClause.FromExpression as ConstantExpression).Value as IInjectionDbSet).Table);
        }

        public override void VisitSelectClause(SelectClause selectClause, QueryModel queryModel)
        {
            Writer.WriteKeyword("SELECT");
            Writer.WriteWhiteSpace();

            if (queryModel.ResultOperators.OfType<DistinctResultOperator>().Any())
            {
                Writer.WriteKeyword("DISTINCT");
                Writer.WriteWhiteSpace();
            }

            if (queryModel.ResultOperators.OfType<CountResultOperator>().Any())
            {
                Writer.WriteIdentifier("COUNT");
                Writer.Write("(");
                Writer.Write("*");
                Writer.Write(")");
            }

            new MySqlExpressionVisitor(this, Writer).Visit(selectClause.Selector);

            base.VisitSelectClause(selectClause, queryModel);
        }

        public override void VisitWhereClause(WhereClause whereClause, QueryModel queryModel, int index)
        {
            Writer.WriteWhiteSpace();
            Writer.WriteKeyword("WHERE");
            Writer.WriteWhiteSpace();

            new MySqlExpressionVisitor(this, Writer).Visit(whereClause.Predicate);

            base.VisitWhereClause(whereClause, queryModel, index);
        }

        public override void VisitOrderByClause(OrderByClause orderByClause, QueryModel queryModel, int index)
        {
            Writer.WriteWhiteSpace();
            Writer.WriteKeyword("ORDER");
            Writer.WriteWhiteSpace();
            Writer.WriteKeyword("BY");
            Writer.WriteWhiteSpace();

            foreach (var ordering in orderByClause.Orderings)
            {
                new MySqlExpressionVisitor(this, Writer).Visit(ordering.Expression);
                
                if (ordering.OrderingDirection == OrderingDirection.Desc)
                {
                    Writer.WriteWhiteSpace();
                    Writer.WriteKeyword("DESC");
                }
            }

            base.VisitOrderByClause(orderByClause, queryModel, index);
        }

        public override void VisitResultOperator(ResultOperatorBase resultOperator, QueryModel queryModel, int index)
        {
            switch (resultOperator)
            {
                case TakeResultOperator take:
                    Writer.WriteWhiteSpace();
                    Writer.WriteKeyword("LIMIT");
                    Writer.WriteWhiteSpace();
                    Writer.Write(take.Count);
                    break;

                case SkipResultOperator skip:
                    Writer.WriteWhiteSpace();
                    Writer.WriteKeyword("OFFSET");
                    Writer.WriteWhiteSpace();
                    Writer.Write(skip.Count);
                    break;
            }

            base.VisitResultOperator(resultOperator, queryModel, index);
        }
    }
}
