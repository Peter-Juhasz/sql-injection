using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionVisitors;
using Remotion.Linq;
using Remotion.Linq.Clauses;
using System.Collections.ObjectModel;

namespace PeterJuhasz.SqlInjection
{
    internal class SubQueryFromClauseFlattenerWithoutChecking : QueryModelVisitorBase
    {
        public override void VisitMainFromClause(MainFromClause fromClause, QueryModel queryModel)
        {
            if (fromClause.FromExpression is SubQueryExpression subQueryExpression)
                FlattenSubQuery(subQueryExpression, fromClause, queryModel, 0);

            base.VisitMainFromClause(fromClause, queryModel);
        }

        public override void VisitAdditionalFromClause(AdditionalFromClause fromClause, QueryModel queryModel, int index)
        {
            if (fromClause.FromExpression is SubQueryExpression subQueryExpression)
                FlattenSubQuery(subQueryExpression, fromClause, queryModel, index + 1);

            base.VisitAdditionalFromClause(fromClause, queryModel, index);
        }

        protected virtual void FlattenSubQuery(SubQueryExpression subQueryExpression, IFromClause fromClause, QueryModel queryModel, int destinationIndex)
        {
            //CheckFlattenable(subQueryExpression.QueryModel);

            var innerMainFromClause = subQueryExpression.QueryModel.MainFromClause;
            fromClause.CopyFromSource(innerMainFromClause);

            var innerSelectorMapping = new QuerySourceMapping();
            innerSelectorMapping.AddMapping(fromClause, subQueryExpression.QueryModel.SelectClause.Selector);
            queryModel.TransformExpressions(ex => ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(ex, innerSelectorMapping, false));

            InsertBodyClauses(subQueryExpression.QueryModel.BodyClauses, queryModel, destinationIndex);

            var innerBodyClauseMapping = new QuerySourceMapping();
            innerBodyClauseMapping.AddMapping(innerMainFromClause, new QuerySourceReferenceExpression(fromClause));
            queryModel.TransformExpressions(ex => ReferenceReplacingExpressionVisitor.ReplaceClauseReferences(ex, innerBodyClauseMapping, false));
        }
        
        protected void InsertBodyClauses(ObservableCollection<IBodyClause> bodyClauses, QueryModel destinationQueryModel, int destinationIndex)
        {
            foreach (var bodyClause in bodyClauses)
            {
                destinationQueryModel.BodyClauses.Insert(destinationIndex, bodyClause);
                ++destinationIndex;
            }
        }
    }
}
