using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cimon.Jenkins;

class DateTimeIso8601JsonConverter : JsonConverter<DateTime>
{
	private const string Format = "yyyy-MM-ddTHH:mm:ss.ffffffZ";
	private const string OffsetFormat = "yyyy-MM-dd HH:mm:ss zzz";//2024-03-25 18:08:47 +0200

	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		var value = reader.GetString();
		if (string.IsNullOrWhiteSpace(value))
			return default;
		if (DateTimeOffset.TryParseExact(value, OffsetFormat, CultureInfo.InvariantCulture,
				DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out var dateOffset)) {
			return dateOffset.UtcDateTime;
		}
		return DateTime.TryParseExact(value, Format, CultureInfo.InvariantCulture,
			DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out DateTime date)
			? date.ToUniversalTime()
			: default;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
		writer.WriteStringValue(value.ToString(Format));
	}
}
