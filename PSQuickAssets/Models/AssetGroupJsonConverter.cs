using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PSQuickAssets.Models
{
    internal class AssetGroupJsonConverter : JsonConverter<AssetGroup>
    {
        public override AssetGroup? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Failed to find start of the object.");

            AssetGroup? group = null;

            List<Asset> assets = new List<Asset>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                    return group;

                if (reader.TokenType != JsonTokenType.PropertyName)
                    throw new JsonException("Failed to find propertyName.");

                string propertyName = reader.GetString() ?? throw new JsonException();

                if (propertyName.Equals(nameof(AssetGroup.Name)))
                {
                    reader.Read();
                    group = new AssetGroup(reader.GetString() ?? throw new JsonException());
                }
                else if (propertyName.Equals(nameof(AssetGroup.Assets)))
                {
                    reader.Read();

                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        reader.Read();

                        while (reader.TokenType != JsonTokenType.EndArray)
                        {
                            var asset = JsonSerializer.Deserialize<Asset>(ref reader, options) ?? throw new JsonException("Failed to deserialize Asset.");
                            assets.Add(asset);

                            reader.Read();
                        }
                    }
                    else
                        throw new JsonException("Start of the assets is nowhere to be found.");
                }

                if (group is not null)
                    group.AddMultipleAssets(assets, DuplicateHandling.Allow);
            }

            return group;
        }

        public override void Write(Utf8JsonWriter writer, AssetGroup value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            //string propName = nameof(AssetGroup.Name);
            //writer.WritePropertyName(options.PropertyNamingPolicy?.ConvertName(propName) ?? propName);
            writer.WriteString(nameof(AssetGroup.Name), value.Name);
            writer.WritePropertyName(nameof(AssetGroup.Assets));

            writer.WriteStartArray();

            foreach (var asset in value.Assets)
            {
                JsonSerializer.Serialize(writer, asset, options);
            }

            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
