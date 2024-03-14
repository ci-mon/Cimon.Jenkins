using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Cimon.Jenkins.Entities.Builds;

namespace Cimon.Jenkins;

using System.Buffers;

class DynamicItemConverter: JsonConverter<DynamicItem>
{
	public override DynamicItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var result = new DynamicItem();
		while (reader.Read() && reader.TokenType != JsonTokenType.EndObject) {
			if (reader.TokenType == JsonTokenType.PropertyName) {
				var propName = reader.GetString();
				if (string.IsNullOrWhiteSpace(propName)) continue;
				reader.Read();
				if (propName == "_class") {
					result.Class = reader.GetString() ?? string.Empty;
					continue;
				}
				object? value;
				if (reader.TokenType == JsonTokenType.StartArray) {
					var items = new List<DynamicItem>();
					while (reader.Read() && reader.TokenType != JsonTokenType.EndArray) {
						var item = JsonSerializer.Deserialize<DynamicItem>(ref reader, options);
						if (item != null)
							items.Add(item);
					}
					value = items;
				} else if (reader.TokenType == JsonTokenType.StartObject) {
					var item = JsonSerializer.Deserialize<DynamicItem>(ref reader, options);
					value = item;
				} else if (reader.TokenType == JsonTokenType.Number && reader.TryGetInt32(out var intVal)) {
					value = intVal;
				} else if (reader.TokenType == JsonTokenType.True) {
					value = true;
				} else if (reader.TokenType == JsonTokenType.False) {
					value = false;
				} else if (reader.TokenType == JsonTokenType.Null) {
					value = null;
				}  else if (reader.TokenType == JsonTokenType.Number) {
					value = reader.TryGetInt64(out var res) ? res : reader.GetDecimal();
				}  else if (reader.TokenType == JsonTokenType.String) {
					value = reader.GetString();
				} else {
					value = reader.HasValueSequence ? reader.ValueSequence.ToArray() : reader.ValueSpan.ToArray();
				}
				result.Props[propName] = value;
			}
		}
		if (string.IsNullOrEmpty(result.Class) && result.Props.Count == 0) {
			return null;
		}
		return result;
	}

	public override void Write(Utf8JsonWriter writer, DynamicItem value, JsonSerializerOptions options) {
		writer.WriteStartObject();
		writer.WritePropertyName("_class");
		writer.WriteStringValue(value.Class);
		foreach (var prop in value.Props) {
			writer.WritePropertyName(prop.Key);
			JsonSerializer.Serialize(writer, prop.Value);
		}
		writer.WriteEndObject();
	}
}
