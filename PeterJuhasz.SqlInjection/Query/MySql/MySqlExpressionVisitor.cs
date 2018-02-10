using Remotion.Linq.Clauses;
using Remotion.Linq.Clauses.Expressions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace PeterJuhasz.SqlInjection
{
    internal class MySqlExpressionVisitor : InjectionExpressionVisitor
    {
        public MySqlExpressionVisitor(
            MySqlQueryModelVisitor queryModelVisitor,
            SqlWriter writer
        )
        {
            QueryModelVisitor = queryModelVisitor;
            Writer = writer;
        }

        protected MySqlQueryModelVisitor QueryModelVisitor { get; }
        protected SqlWriter Writer { get; }

        public override Expression Visit(Expression node)
        {
            switch (node)
            {
                case MemberExpression member:
                    VisitMember(member);
                    break;

                case MethodCallExpression methodCall:
                    VisitMethodCall(methodCall);
                    break;

                case ConstantExpression constant:
                    VisitConstant(constant);
                    break;

                case BinaryExpression binary:
                    VisitBinary(binary);
                    break;

                case UnaryExpression unary:
                    VisitUnary(unary);
                    break;

                case ConditionalExpression conditional:
                    VisitConditional(conditional);
                    break;

                case QuerySourceReferenceExpression reference:
                    if ((reference.ReferencedQuerySource as IFromClause)?.FromExpression is SubQueryExpression subQuery)
                    {
                        Visit(subQuery.QueryModel.SelectClause.Selector);
                    }
                    break;

                case SubQueryExpression query:
                    Writer.Write("(");
                    var visitor = new MySqlQueryModelVisitor(Writer);
                    visitor.Render(query.QueryModel);
                    Writer.Write(")");
                    break;
                    
                default:
                    throw new NotSupportedException($"Can't translate expression of type '{node.NodeType}' ({node.Type})");
            }

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Writer.WriteIdentifier(
                node.Member.GetCustomAttribute<ColumnAttribute>()?.Name ??
                node.Member.Name
            );
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(DynamicEntity) && (node.Method.Name == "get_Item" || node.Method.Name == "Value"))
            {
                var arg = node.Arguments.Single() as ConstantExpression;
                var name = arg.Value as string;
                Writer.WriteIdentifier(name);
            }
            else if (node.Method.DeclaringType == typeof(MySqlFunctions) && node.Method.Name == nameof(MySqlFunctions.Between))
            {
                Visit(node.Arguments[0]);
                Writer.WriteWhiteSpace();
                Writer.WriteKeyword("BETWEEN");
                Writer.WriteWhiteSpace();
                Visit(node.Arguments[1]);
                Writer.WriteWhiteSpace();
                Writer.WriteKeyword("AND");
                Writer.WriteWhiteSpace();
                Visit(node.Arguments[2]);
            }
            else if (node.Method.DeclaringType == typeof(MySqlFunctions) && node.Method.Name == nameof(MySqlFunctions.Like))
            {
                Visit(node.Arguments[0]);
                Writer.WriteWhiteSpace();
                Writer.WriteKeyword("LIKE");
                Writer.WriteWhiteSpace();
                Visit(node.Arguments[1]);
            }
            else
            {
                Writer.Write(node.Method.Name);
                Writer.Write("(");

                bool first = true;
                foreach (var arg in node.Arguments)
                {
                    if (!first)
                    {
                        Writer.Write(",");
                        Writer.WriteOptionalWhiteSpace();
                    }

                    Visit(arg);
                    first = false;
                }

                Writer.Write(")");
            }

            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Writer.Write("If");
            Writer.Write("(");
            Visit(node.Test);
            Writer.Write(",");
            Writer.WriteOptionalWhiteSpace();
            Visit(node.IfTrue);
            Writer.Write(",");
            Writer.WriteOptionalWhiteSpace();
            Visit(node.IfFalse);
            Writer.Write(")");

            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            switch (node.Value)
            {
                case null:
                    Writer.WriteKeyword("NULL");
                    break;

                case string str:
                    Writer.WriteString(str);
                    break;

                case int str:
                    Writer.WriteNumber(str);
                    break;

                case long str:
                    Writer.WriteNumber(str);
                    break;

                case double str:
                    Writer.WriteNumber(str);
                    break;

                case bool b:
                    Writer.WriteBoolean(b);
                    break;
            }

            return node;
        }

        protected override Expression VisitExtension(Expression node)
        {
            Visit(node);
            return node;
        }
        
        protected override Expression VisitBinary(BinaryExpression node)
        {
            Visit(node.Left);

            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("=");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.NotEqual:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("<>");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.LessThan:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("<");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.LessThanOrEqual:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("<=");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.GreaterThan:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write(">");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.GreaterThanOrEqual:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write(">=");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.AndAlso:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("AND");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.OrElse:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("OR");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.Add:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("+");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.Subtract:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("-");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.Multiply:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("*");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.Divide:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("/");
                    Writer.WriteOptionalWhiteSpace();
                    break;

                case ExpressionType.Modulo:
                    Writer.WriteOptionalWhiteSpace();
                    Writer.Write("%");
                    Writer.WriteOptionalWhiteSpace();
                    break;
            }

            Visit(node.Right);

            return node;
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Convert:
                    Visit(node.Operand);
                    break;

                default:
                    throw new NotSupportedException($"Can't translate expression of type '{node.NodeType}' ({node.Type})");
            }

            return node;
        }
    }
}
