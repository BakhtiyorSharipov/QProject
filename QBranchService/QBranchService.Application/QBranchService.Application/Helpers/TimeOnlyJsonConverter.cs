using System.Text.Json;
using System.Text.Json.Serialization;

namespace QBranchService.Application.Helpers;

public class TimeOnlyJsonConverter : JsonConverter<TimeOnly>
{
    public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new JsonException("Time value cannot be null or empty");
        }

        
        if (value.Length >= 8)
        {
            value = value.Substring(0, 8); 
        }
        
        return TimeOnly.ParseExact(value, "HH:mm:ss");
    }

    public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("HH:mm:ss"));
    }
}