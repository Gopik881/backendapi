using Elixir.Application.Common.Constants;

namespace Elixir.Application.Features.Company.Queries.GetCompany5TabHistory.Company5TabHistoryMapEntityToDictionary;

public class Company5TabHistoryMapEntitytoDictionary
{
    public Dictionary<string, string> MapEntityToDictionary<T>(T entity)
    {
        var keyValueMap = new Dictionary<string, string>();

        if (entity == null)
            return keyValueMap;

        var properties = typeof(T).GetProperties()
            .Where(p =>
                !Attribute.IsDefined(p, typeof(System.ComponentModel.DataAnnotations.KeyAttribute)) && // skip PKs
                !p.Name.Equals(AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_VERSION, StringComparison.OrdinalIgnoreCase) &&
                !p.Name.EndsWith(AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_ID, StringComparison.OrdinalIgnoreCase) &&
                !p.Name.EndsWith(AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_BY, StringComparison.OrdinalIgnoreCase) &&
                !p.Name.EndsWith(AppConstants.COMPANY5TABHISTORYBYVERSIONNUMBER_AT, StringComparison.OrdinalIgnoreCase) &&
                !(p.PropertyType == typeof(DateTime) || p.PropertyType == typeof(DateTime?))
            );

        foreach (var property in properties)
        {
            var value = property.GetValue(entity)?.ToString() ?? string.Empty;
            keyValueMap[property.Name] = value;
        }

        return keyValueMap;
    }
}
