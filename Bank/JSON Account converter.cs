using System.Text.Json;
using System.Text.Json.Serialization;
using BankApp;
namespace BankApp
{
    public class AccountConverter : JsonConverter<Account>
    {
        public override Account Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            JsonElement root = doc.RootElement;

            // Create new options without this converter to avoid infinite recursion
            var newOptions = new JsonSerializerOptions(options);
            newOptions.Converters.Clear();
            foreach (var converter in options.Converters)
            {
                if (converter != this)
                    newOptions.Converters.Add(converter);
            }

            // Check for type discriminator first
            if (root.TryGetProperty("$type", out JsonElement typeElement))
            {
                string? accountType = typeElement.GetString();
                if (accountType != null)
                {
                    return accountType switch
                    {
                        "saving" or "SavingAccount" => JsonSerializer.Deserialize<SavingAccount>(root.GetRawText(), newOptions)!,
                        "checking" or "CheckingAccount" => JsonSerializer.Deserialize<CheckingAccount>(root.GetRawText(), newOptions)!,
                        _ => throw new JsonException($"Unknown account type: {accountType}")
                    };
                }
            }

            // Fallback: detect type by unique properties
            if (root.TryGetProperty("minimumBalance", out _))
            {
                return JsonSerializer.Deserialize<SavingAccount>(root.GetRawText(), newOptions)!;
            }
            if (root.TryGetProperty("overdraftLimit", out _))
            {
                return JsonSerializer.Deserialize<CheckingAccount>(root.GetRawText(), newOptions)!;
            }

            // Default fallback - assume SavingAccount
            return JsonSerializer.Deserialize<SavingAccount>(root.GetRawText(), newOptions)!;
        }

        public override void Write(Utf8JsonWriter writer, Account value, JsonSerializerOptions options)
        {
            // Create new options without this converter
            var newOptions = new JsonSerializerOptions(options);
            newOptions.Converters.Clear();
            foreach (var converter in options.Converters)
            {
                if (converter != this)
                    newOptions.Converters.Add(converter);
            }

            writer.WriteStartObject();

            // Write type discriminator
            string typeName = value switch
            {
                SavingAccount => "saving",
                CheckingAccount => "checking",
                _ => value.GetType().Name
            };
            writer.WriteString("$type", typeName);

            // Serialize the actual object and copy its properties
            string json = JsonSerializer.Serialize(value, value.GetType(), newOptions);
            using JsonDocument doc = JsonDocument.Parse(json);

            foreach (JsonProperty property in doc.RootElement.EnumerateObject())
            {
                if (property.Name != "$type")
                {
                    property.WriteTo(writer);
                }
            }

            writer.WriteEndObject();
        }
    }
}