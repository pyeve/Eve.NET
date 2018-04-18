using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Eve
{
    public class WhereTranslator : ExpressionVisitor
    {
        static Dictionary<ExpressionType, string> nodeTypes = new Dictionary<ExpressionType, string>
        {
            { ExpressionType.Equal, null },
            { ExpressionType.NotEqual, "$ne" },
            { ExpressionType.Or, "$or" },
            { ExpressionType.AndAlso, ", " },
        };

        public static string Translate(Expression node)
        {
            string filter = null;
            switch (node.NodeType)
            {
                case ExpressionType.And:
                    filter = TranslateAnd((BinaryExpression)node);
                    break;
                case ExpressionType.AndAlso:
                    filter = TranslateAndAlso((BinaryExpression)node);
                    break;
                case ExpressionType.Equal:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.NotEqual:
                    filter = TranslateComparison((BinaryExpression)node);
                    break;
                case ExpressionType.Or:
                    filter = TranslateOr((BinaryExpression)node);
                    break;
                case ExpressionType.OrElse:
                    filter = TranslateOrElse((BinaryExpression)node);
                    break;
            }
            if (filter == null)
            {
                throw new NotSupportedException($"Unknown or unsupported filter: {node.ToString()}");
            }
            return filter;
        }

        private static string TranslateAnd(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.Type == typeof(bool) && binaryExpression.Right.Type == typeof(bool))
            {
                return TranslateAndAlso(binaryExpression);
            }
            return null;
        }
        private static string TranslateAndAlso(BinaryExpression binaryExpression)
        {
            return $"{Translate(binaryExpression.Left)}, {Translate(binaryExpression.Right)}";

        }
        private static string TranslateOr(BinaryExpression binaryExpression)
        {
            return $"$or: [{{{Translate(binaryExpression.Left)}}}, {{{Translate(binaryExpression.Right)}}}]";
        }
        private static string TranslateOrElse(BinaryExpression binaryExpression)
        {
            if (binaryExpression.Left.Type == typeof(bool) && binaryExpression.Right.Type == typeof(bool))
            {
                return TranslateOr(binaryExpression);
            }
            return null;
        }
        private static string TranslateComparison(BinaryExpression binaryExpression)
        {
            var variableExpression = binaryExpression.Left as MemberExpression;
            var constantExpression = binaryExpression.Right as ConstantExpression;
            var operatorType = binaryExpression.NodeType;

            if (variableExpression == null)
                // TODO: support properties on the right-hand too.
                throw new NotSupportedException("A property/variable expression should be on the left-hand of the comparison");

            if (constantExpression == null)
                // TODO: support constant expressions on the left-hand too.
                throw new NotSupportedException("A costant expression should be on the right-hand of the comparison");


            var member = (PropertyInfo)variableExpression.Member;
            var jsonAttr = member.GetCustomAttribute<JsonPropertyAttribute>();
            var field = new MySnakeCaseNamingStrategy()
                .ResolvePropertyName((jsonAttr == null) ? member.Name : jsonAttr.PropertyName);

            var value = constantExpression.Value;
            if (nodeTypes[operatorType] != null)
            {
                value = $"{{\"{nodeTypes[operatorType]}\":\"{value}\"}}";
            }
            else
            {
                value = $"\"{value}\"";
            }
            return $"\"{field}\": {value}";
        }
    }
}