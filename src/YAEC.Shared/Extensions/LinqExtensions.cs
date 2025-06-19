using System.Linq.Expressions;

namespace YAEC.Shared.Extensions;

public static class LinqExtensions
{
    public static Expression<Func<T, bool>> True<T>() => x => true;

    public static Expression<Func<T, bool>> False<T>() => x => false;

    public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => MergeExpression(Expression.AndAlso, left, right);

    public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        => MergeExpression(Expression.OrElse, left, right);

    public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        => Expression.Lambda<Func<T, bool>>(Expression.Not(expression.Body), expression.Parameters);

    private static Expression<Func<T, bool>> MergeExpression<T>(
        Func<Expression, Expression, Expression> operation,
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right
    )
    {
        var map = CreateMap(left, right);
        var visitor = new ParameterVisitor(map);
        var body = operation(left.Body, visitor.Visit(right.Body));
        return Expression.Lambda<Func<T, bool>>(body, left.Parameters);
    }

    private static Dictionary<ParameterExpression, ParameterExpression> CreateMap<T>(
        Expression<Func<T, bool>> left,
        Expression<Func<T, bool>> right
    )
    {
        return left.Parameters
            .Select((parameter, idx) => new
            {
                RightParameter = right.Parameters[idx],
                LeftParameter = parameter
            })
            .ToDictionary(
                x => x.RightParameter,
                x => x.LeftParameter
            );
    }
    
    public static IQueryable<TSource> Paging<TSource>(this IQueryable<TSource> source, int pageIndex, int pageSize)
    {
        return source
            .Skip(pageSize * (pageIndex - 1))
            .Take(pageSize);
    }

    public static IOrderedQueryable<TSource> OrderByAscOrDesc<TSource, TKey>(
        this IQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        bool asc = true
    )
    {
        return asc ? source.OrderBy(keySelector) : source.OrderByDescending(keySelector);
    }

    public static IOrderedQueryable<TSource> ThenByAscOrDesc<TSource, TKey>(
        this IOrderedQueryable<TSource> source,
        Expression<Func<TSource, TKey>> keySelector,
        bool asc = true
    )
    {
        return asc ? source.ThenBy(keySelector) : source.ThenByDescending(keySelector);
    }
}

internal sealed class ParameterVisitor(Dictionary<ParameterExpression, ParameterExpression> map) : ExpressionVisitor
{
    protected override Expression VisitParameter(ParameterExpression node) => map.GetValueOrDefault(node, node);
}