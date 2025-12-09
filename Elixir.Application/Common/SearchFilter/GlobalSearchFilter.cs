using System.Linq.Expressions;
using System.Reflection;

namespace Elixir.Application.Common.SearchFilter
{
    public static class GlobalSearchFilter
    {
        
        public static IQueryable<T> ApplySearchFilter<T>(IQueryable<T> source, string searchTerm)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            Expression? searchPredicate = null;

            bool isDate = DateTime.TryParse(searchTerm, out var searchDate);

            foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Expression? condition = null;

                // Search string properties (ignore case)
                if (!isDate && prop.PropertyType == typeof(string))
                {
                    var member = Expression.Property(parameter, prop);
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);
                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string), typeof(StringComparison) });
                    if (toLowerMethod != null && containsMethod != null)
                    {
                        // x.Prop != null && x.Prop.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)
                        var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                        var searchTermExpr = Expression.Constant(searchTerm, typeof(string));
                        var stringComparisonExpr = Expression.Constant(StringComparison.OrdinalIgnoreCase, typeof(StringComparison));
                        condition = Expression.AndAlso(
                            notNull,
                            Expression.Call(member, containsMethod, searchTermExpr, stringComparisonExpr)
                        );
                    }
                }
                // Search date properties
                else if (isDate && (prop.PropertyType == typeof(DateTime) || prop.PropertyType == typeof(DateTime?)))
                {
                    var member = Expression.Property(parameter, prop);
                    Expression? propertyDate = null;
                    Expression? searchDateExpr = null;

                    // Handle partial date input like "07/" (month only)
                    if (!string.IsNullOrWhiteSpace(searchTerm) && searchTerm.Trim().Length <= 3 && searchTerm.Trim().EndsWith("/"))
                    {
                        // Try to parse as month only (e.g., "07/")
                        if (int.TryParse(searchTerm.Trim().TrimEnd('/'), out int month) && month >= 1 && month <= 12)
                        {
                            var monthProperty = Expression.Property(member, "Month");
                            var monthValue = Expression.Constant(month, typeof(int));
                            var monthEquals = Expression.Equal(monthProperty, monthValue);

                            // Optionally, you can also check for year if needed
                            condition = monthEquals;
                        }
                    }
                    else
                    {
                        if (prop.PropertyType == typeof(DateTime))
                        {
                            propertyDate = Expression.Property(member, "Date");
                            var dateValue = Expression.Constant(searchDate, typeof(DateTime));
                            searchDateExpr = Expression.Property(dateValue, "Date");
                            condition = Expression.Equal(propertyDate, searchDateExpr);
                        }
                        else if (prop.PropertyType == typeof(DateTime?))
                        {
                            // member.HasValue && member.Value.Date == searchDate.Date
                            var hasValue = Expression.Property(member, "HasValue");
                            var value = Expression.Property(member, "Value");
                            propertyDate = Expression.Property(value, "Date");
                            var dateValue = Expression.Constant(searchDate, typeof(DateTime));
                            searchDateExpr = Expression.Property(dateValue, "Date");
                            var dateEquals = Expression.Equal(propertyDate, searchDateExpr);
                            condition = Expression.AndAlso(hasValue, dateEquals);
                        }
                    }
                }
                // Convert IsEnabled-like properties to "enabled"/"disabled" string comparisons
                else if (!isDate && prop.PropertyType == typeof(bool) &&
                        (prop.Name.ToLower().Contains("isenabled") || prop.Name.ToLower().Contains("status")))
                {
                    var member = Expression.Property(parameter, prop);
                    if (searchTerm.Equals("enabled", StringComparison.OrdinalIgnoreCase))
                        condition = Expression.Equal(member, Expression.Constant(true));
                    else if (searchTerm.Equals("disabled", StringComparison.OrdinalIgnoreCase))
                        condition = Expression.Equal(member, Expression.Constant(false));
                }

                if (condition != null)
                {
                    searchPredicate = searchPredicate == null
                        ? condition
                        : Expression.OrElse(searchPredicate, condition);
                }
            }

            if (searchPredicate == null) return source;

            var lambda = Expression.Lambda<Func<T, bool>>(searchPredicate, parameter);
            return source.Where(lambda);
        }
    }
}
