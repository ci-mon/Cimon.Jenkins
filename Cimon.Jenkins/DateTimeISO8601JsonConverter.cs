﻿using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cimon.Jenkins;

class DateTimeIso8601JsonConverter : JsonConverter<DateTime>
{
	private const string Format = "yyyy-MM-dd HH:mm:ss zzz";
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
		return DateTime.ParseExact(reader.GetString() ?? string.Empty, Format, CultureInfo.InvariantCulture);
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) {
		writer.WriteStringValue(value.ToString(Format));
	}
}
