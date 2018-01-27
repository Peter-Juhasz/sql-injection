using Remotion.Linq;
using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ResultOperators;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.Structure;
using System.Linq;

namespace PeterJuhasz.SqlInjection
{
    public abstract class InjectionQueryModelVisitor : QueryModelVisitorBase
    {
        public InjectionQueryModelVisitor(SqlWriter sqlWriter)
        {
            this.Writer = sqlWriter;
        }

        protected SqlWriter Writer { get; set; }
        protected IInjectionDbSet DbSet { get; set; }

        public string Render(IQueryable query)
        {
            DbSet = query as IInjectionDbSet;
            var model = new QueryParser(new ExpressionTreeParser(
                ExpressionTreeParser.CreateDefaultNodeTypeProvider(),
                ExpressionTreeParser.CreateDefaultProcessor(
                    ExpressionTransformerRegistry.CreateDefault(),
                    new NullEvaluatableExpressionFilter()
                )
            )).GetParsedQuery(DbSet.Expression);

            var skip = GetResultOperator<SkipResultOperator>(model);

            SubQueryFromClauseFlattenerWithoutChecking flattener = new SubQueryFromClauseFlattenerWithoutChecking();
            model.Accept(flattener);

            if (skip != null)
                model.ResultOperators.Add(skip);

            Writer.Clear();

            VisitQueryModel(model);

            return Writer.ToString();
        }


        public override string ToString()
        {
            return Writer.ToString();
        }


        private static TResultOperator GetResultOperator<TResultOperator>(QueryModel model) where TResultOperator : ResultOperatorBase
        {
            var @operator = model.ResultOperators.OfType<TResultOperator>().FirstOrDefault();
            if (@operator != null)
                return @operator;

            if (model.MainFromClause?.FromExpression is SubQueryExpression subQuery)
                return GetResultOperator<TResultOperator>(subQuery.QueryModel);

            return null;
        }
    }
}
