using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Abp.UI;

namespace TestProject.Query
{
    public class QueryInfo
    {
        public int Skip { get; set; }
        
        public int Take { get; set; }
        
        public string SearchText { get; set; }

        public List<string> SearchProperties { get; set; }
        
        public List<SortInfo> Sorters = new List<SortInfo>();
        
        public FilterInfo Filter { get; set; }

        
        public IQueryable<TEntity> GetQuery<TEntity>(QueryInfo queryInfo, IQueryable<TEntity> list)
        {
            var sortInfo = queryInfo.Sorters;
            var filterInfo = queryInfo.Filter;
            var rules = filterInfo.Rules;

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            
            var filteredList = GetFilterExpression<TEntity>(parameter, rules, filterInfo.Condition);
            list = list.Where(GetLambdaExpression<TEntity>(parameter, filteredList, queryInfo));

            bool sorted = false;

            foreach (var sort in sortInfo)
            {
                switch (sort.SortDirection)
                {
                    case 'a':
                        if (sorted == false)
                        {
                            list = list.OrderBy(GetOrderExpression<TEntity>(parameter, sort));
                            sorted = true;
                            break;
                        }

                        var sortedList1 = list as IOrderedQueryable<TEntity>;
                        list = sortedList1.ThenBy(GetOrderExpression<TEntity>(parameter, sort));
                        break;

                    case 'd':
                        if (sorted == false)
                        {
                            list = list.OrderByDescending(GetOrderExpression<TEntity>(parameter, sort));
                            sorted = true;
                            break;
                        }

                        var sortedList2 = list as IOrderedQueryable<TEntity>;
                        list = sortedList2.ThenBy(GetOrderExpression<TEntity>(parameter, sort));
                        break;
                }
            }

            list = list.Skip(queryInfo.Skip).Take(queryInfo.Take);
            
            return list;
        }

        //------------------- ORDER LAMBDA EXPRESSION FILTER -------------------//
        public Expression<Func<TEntity, bool>> GetLambdaExpression<TEntity>(ParameterExpression parameter,
            Expression expression, QueryInfo queryInfo)
        {
            // ReSharper disable once RedundantAssignment
            Expression searchResult = Expression.Constant(true, typeof(bool));

            Expression result = Expression.Constant(false, typeof(bool));
            
            ParameterExpression paramExpression = parameter;

            foreach (var property in queryInfo.SearchProperties)
            {
                var propExpression = Expression.Property(paramExpression, property);

                var type = propExpression.Type;

                var convertedValue = Convert.ChangeType(queryInfo.SearchText, type);

                var constant = Expression.Constant(convertedValue);

                switch (convertedValue)
                {
                    case string _:
                        searchResult = GetBinaryExpressionForString("ct", propExpression, constant);
                        break;

                    case int _:
                        searchResult = GetBinaryExpressionForInt("ct", propExpression, constant);
                        break;

                    default:
                        throw new UserFriendlyException("Search again with different parameters");
                }

                result = Expression.OrElse(result, searchResult);
            }

            result = Expression.AndAlso(result, expression);

            return Expression.Lambda<Func<TEntity, bool>>(result, parameter);
        }

        //------------------- ORDER EXPRESSION FILTER -------------------//
        public Expression GetFilterExpression<TEntity>(ParameterExpression parameter, List<RuleInfo> rules,
            string condition)
        {
            //true & false expr
            var trueExpression = Expression.Constant(true, typeof(bool));
            var falseExpression = Expression.Constant(false, typeof(bool));

            //parameter
            Expression parameterExpression = parameter;

            BinaryExpression binary;

            Expression result;

            result = condition == "and" ? trueExpression : falseExpression;

            foreach (var rule in rules)
            {
                //property
                Expression propertyExpression = Expression.Property(parameterExpression, rule.Property);

                //type of property
                Type type = propertyExpression.Type;

                //value converted
                var convertedValue = Convert.ChangeType(rule.Value, type);

                //value as constant
                var constant = Expression.Constant(convertedValue);

                switch (convertedValue)
                {
                    case string _:
                        binary = GetBinaryExpressionForString(rule.Operator, propertyExpression, constant);
                        break;

                    case int _:
                        binary = GetBinaryExpressionForInt(rule.Operator, propertyExpression, constant);
                        break;

                    default:
                        throw new UserFriendlyException($"Unexpected value type {rule.Property}");
                }

                switch (condition)
                {
                    case "and":
                        result = Expression.AndAlso(result, binary);
                        break;

                    case "or":
                        result = Expression.OrElse(result, binary);
                        break;

                    default:
                        throw new UserFriendlyException($"Unexpected condition {condition}");
                }

                if (rule.Condition == null)
                {
                    continue;
                }

                switch (condition)
                {
                    case "and":
                        result = Expression.AndAlso(result,
                            GetFilterExpression<TEntity>(parameter, rule.Rules, rule.Condition));
                        break;

                    case "or":
                        result = Expression.OrElse(result,
                            GetFilterExpression<TEntity>(parameter, rule.Rules, rule.Condition));
                        break;

                    default:
                        throw new UserFriendlyException($"Unexpected condition {condition}");
                }
            }
            
            return result;
        }

        //------------------- ORDER EXPRESSION -------------------//
        public Expression<Func<TEntity, object>> GetOrderExpression<TEntity>(ParameterExpression parameter, SortInfo sortInfo)
        {
            var propExpression = Expression.Property(parameter, sortInfo.Property);

            var convertExp = Expression.Convert(propExpression, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(convertExp, parameter);
        }

        //------------------- ORDER BINARY EXPRESSION FOR INT -------------------//
        public BinaryExpression GetBinaryExpressionForInt(string operand, Expression propExpression, ConstantExpression constant)
        {
            switch (operand)
            {
                case "gt":
                    return Expression.GreaterThan(propExpression, constant);
                case "lt":
                    return Expression.LessThan(propExpression, constant);
                case "gte":
                    return Expression.GreaterThanOrEqual(propExpression, constant);
                case "lte":
                    return Expression.LessThanOrEqual(propExpression, constant);
                case "eq":
                    return Expression.Equal(propExpression, constant);

                default:
                    throw new InvalidOperationException($"Neocekivani operator {operand}");
            }
        }

        //------------------- ORDER BINARY EXPRESSION FOR STRING -------------------//
        public BinaryExpression GetBinaryExpressionForString(string operand, Expression propExpression, ConstantExpression constant)
        {
            var trueExpression = Expression.Constant(true, typeof(bool));

            BinaryExpression bin;

            var ignoreCase = Expression.Constant(StringComparison.OrdinalIgnoreCase);

            switch (operand)
            {
                case "eq":
                    return Expression.Equal(propExpression, constant);

                case "ct":
                    MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });
                    var contains = Expression.Call(propExpression, containsMethod, constant, ignoreCase);
                    bin = Expression.Equal(contains, trueExpression);
                    break;

                case "sw":
                    MethodInfo startsMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string), typeof(StringComparison) });
                    var startsWith = Expression.Call(propExpression, startsMethod, constant, ignoreCase);
                    bin = Expression.Equal(startsWith, trueExpression);
                    break;

                case "ew":
                    MethodInfo endsMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string), typeof(StringComparison) });
                    var endsWith = Expression.Call(propExpression, endsMethod, constant, ignoreCase);
                    bin = Expression.Equal(endsWith, trueExpression);
                    break;

                default:
                    throw new InvalidOperationException($"Neocekivani operator {operand}");
            }

            return bin;
        }
    }
}
