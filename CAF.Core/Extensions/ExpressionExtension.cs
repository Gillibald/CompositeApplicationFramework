#region Usings

using System;
using System.Linq.Expressions;
using System.Reflection;
 using CompositeApplicationFramework.Properties;

#endregion

namespace CompositeApplicationFramework.Extensions
{
    public static class ExpressionExtension
    {
        public static PropertyInfo GetProperty<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> source)
        {
            var member = source.Body as MemberExpression;

            if (member == null)
            {
                throw new ArgumentException($"Expression '{source}' refers to a method, not a property.");
            }

            var propertyInfo = member.Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException($"Expression '{source}' refers to a field, not a property.");
            }

            return propertyInfo;
        }

        public static string GetPropertyName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> source)
        {
            return source.GetProperty().Name;
        }

        public static string GetPropertyName<T>(this Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var lMemberExpression = propertyExpression.Body as MemberExpression;
            if (lMemberExpression == null)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_expression_is_not_a_member_access_expression,
                    nameof(propertyExpression));
            }

            var lProperty = lMemberExpression.Member as PropertyInfo;
            if (lProperty == null)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_member_access_expression_does_not_access_a_property,
                    nameof(propertyExpression));
            }

            var lGetMethod = lProperty.GetGetMethod(true);
            if (lGetMethod == null)
            {
                // this shouldn't happen - the expression would reject the property before reaching this far
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_referenced_property_does_not_have_a_get_method,
                    nameof(propertyExpression));
            }

            if (lGetMethod.IsStatic)
            {
                throw new ArgumentException(
                    Resources.ViewModelBase_ExtractPropertyName_The_referenced_property_is_a_static_property,
                    nameof(propertyExpression));
            }

            return lMemberExpression.Member.Name;
        }
    }
}