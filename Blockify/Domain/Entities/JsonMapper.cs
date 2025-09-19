using System.Text.Json;
using Blockify.Application.Exceptions;

namespace Blockify.Domain.Entities
{
    public static class JsonMapper<T>
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private static object MapType(Type type)
        {
            if (type.IsClass && type != typeof(string))
            {
                return type.GetProperties().ToDictionary(p => p.Name, p => MapType(p.PropertyType));
            }

            return type.Name;
        }

        public static string ToJson()
        {
            var structure = typeof(T).GetProperties()
                .ToDictionary(prop => prop.Name, prop => MapType(prop.PropertyType));

            return JsonSerializer.Serialize(structure, _options);
        }

        public static T FromJson(string json)
        {
            return JsonSerializer.Deserialize<T>(json, _options)
                ?? throw new FailedJsonSerializationException<T>(
                    $"Failed to deserialize json to {typeof(T).Name}", json);
        }
    }
}