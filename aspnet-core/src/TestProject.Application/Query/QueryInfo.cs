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






        public IQueryable<TEntity> GetQuery<TEntity>(QueryInfo queryInfo, List<TEntity> list)
        {
            var sortInfo = queryInfo.Sorters;
            var filterInfo = queryInfo.Filter;
            var rules = filterInfo.Rules;

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "x");
            
            IQueryable<TEntity> myList = list.AsQueryable();

            var filteredList = GetFilteredList<TEntity>(parameter, rules, filterInfo.Condition);
            myList = myList.Where(GetFilterExpression<TEntity>(parameter, filteredList));

            bool sorted = false;

            foreach (var sort in sortInfo)
            {
                switch (sort.SortDirection)
                {
                    case 'a':
                        if (sorted == false)
                        {
                            myList = myList.OrderBy(GetOrderExpression<TEntity>(parameter, sort));
                            sorted = true;
                            break;
                        }

                        var sortedList1 = myList as IOrderedQueryable<TEntity>;
                        myList = sortedList1.ThenBy(GetOrderExpression<TEntity>(parameter, sort));
                        break;

                    case 'd':
                        if (sorted == false)
                        {
                            myList = myList.OrderByDescending(GetOrderExpression<TEntity>(parameter, sort));
                            sorted = true;
                            break;
                        }

                        var sortedList2 = myList as IOrderedQueryable<TEntity>;
                        myList = sortedList2.ThenBy(GetOrderExpression<TEntity>(parameter, sort));
                        break;
                }
            }

            myList = myList.Skip(queryInfo.Skip).Take(queryInfo.Take);





            return myList;



        }


        public Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(ParameterExpression parameter,
            Expression expression)
        {
            return Expression.Lambda<Func<TEntity, bool>>(expression, parameter);
        }






        public Expression GetFilteredList<TEntity>(ParameterExpression parameter, List<RuleInfo> rules,
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
                            GetFilteredList<TEntity>(parameter, rule.Rules, rule.Condition));
                        break;

                    case "or":
                        result = Expression.OrElse(result,
                            GetFilteredList<TEntity>(parameter, rule.Rules, rule.Condition));
                        break;

                    default:
                        throw new UserFriendlyException($"Unexpected condition {condition}");
                }

            }


            return result;
        }




        public Expression<Func<TEntity, object>> GetOrderExpression<TEntity>(ParameterExpression parameter, SortInfo sortInfo)
        {
            var propExpression = Expression.Property(parameter, sortInfo.Property);

            var convertExp = Expression.Convert(propExpression, typeof(object));

            return Expression.Lambda<Func<TEntity, object>>(convertExp, parameter);
        }






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




        public BinaryExpression GetBinaryExpressionForString(string operand, Expression propExpression, ConstantExpression constant)
        {
            var trueExpression = Expression.Constant(true, typeof(bool));

            BinaryExpression bin;

            //MethodInfo compareToMethod = typeof(string).GetMethod("CompareTo", new[] {typeof(string)});

            //var compareCall = Expression.Call(propExpression, compareToMethod, constant);

            //var zero = Expression.Constant(0);

            //var comparison = Expression.Equal(compareCall, zero);

            switch (operand)
            {
                case "eq":
                    return Expression.Equal(propExpression, constant);

                case "ct":
                    MethodInfo containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });
                    var ignoreCase = Expression.Constant(StringComparison.OrdinalIgnoreCase);
                    var contains = Expression.Call(propExpression, containsMethod, constant, ignoreCase);
                    bin = Expression.Equal(contains, trueExpression);
                    break;

                case "sw":
                    MethodInfo startsMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                    var startsWith = Expression.Call(propExpression, startsMethod, constant);
                    bin = Expression.Equal(startsWith, trueExpression);
                    break;

                case "ew":
                    MethodInfo endsMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                    var endsWith = Expression.Call(propExpression, endsMethod, constant);
                    bin = Expression.Equal(endsWith, trueExpression);
                    break;

                default:
                    throw new InvalidOperationException($"Neocekivani operator {operand}");
            }

            return bin;
        }





    }
}
