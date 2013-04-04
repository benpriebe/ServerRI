using System;
using AutoMapper;

namespace Core.Extensions
{
    public static class MappingExpressionExtensions
    {
        public static IMappingExpression<TSource, TDest> IgnoreAllUnmapped<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Ignore());
            return expression;
        }

        public static IMappingExpression<TSource, TDest> IgnoreAllSourceDefaultValues<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Condition(ctx => ctx.SourceValue == null || !ctx.SourceValue.Equals(GetDefault(ctx.SourceType))));
            return expression;
        }

        public static IMappingExpression<TSource, TDest> IgnorePropertiesThatHaveNotBeenSet<TSource, TDest>(this IMappingExpression<TSource, TDest> expression)
        {
            expression.ForAllMembers(opt => opt.Condition(ctx =>
            {
                var changeTracker  = ctx.Parent.SourceValue as IPropertyChangeTracker;
                return changeTracker == null || changeTracker.IsModified(ctx.MemberName);
            }));
            return expression;
        }

        public static object GetDefault(Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}