using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApplicationTemplate
{
	public class UnixTimestampJsonConverter : JsonConverter<DateTimeOffset>
	{
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var offset = reader.GetInt64();
			return FromUnixTimeSeconds(offset, TimeSpan.Zero);
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.ToUnixTimeSeconds());
		}

		// https://github.com/dotnet/coreclr/blob/50ef79d48df81635e58ca59386620f0151df6022/src/mscorlib/src/System/DateTime.cs#L71
		private const int DaysPerYear = 365;
		private const int DaysPer4Years = DaysPerYear * 4 + 1;
		private const int DaysPer100Years = DaysPer4Years * 25 - 1;
		private const int DaysPer400Years = DaysPer100Years * 4 + 1;
		private const int DaysTo1970 = DaysPer400Years * 4 + DaysPer100Years * 3 + DaysPer4Years * 17 + DaysPerYear;

		// https://github.com/dotnet/coreclr/blob/50ef79d48df81635e58ca59386620f0151df6022/src/mscorlib/src/System/DateTimeOffset.cs#L43
		private const long UnixEpochTicks = TimeSpan.TicksPerDay * DaysTo1970;
		private const long UnixEpochSeconds = UnixEpochTicks / TimeSpan.TicksPerSecond;
		private const long UnixEpochMilliseconds = UnixEpochTicks / TimeSpan.TicksPerMillisecond; // 62,135,596,800,000

		public static DateTimeOffset FromUnixTimeSeconds(long seconds, TimeSpan offset)
		{
			return new DateTimeOffset((seconds + UnixEpochSeconds + (long)offset.TotalSeconds) * TimeSpan.TicksPerSecond, offset);
		}

		public static DateTimeOffset FromUnixTimeMilliseconds(long milliseconds, TimeSpan offset)
		{
			return new DateTimeOffset((milliseconds + UnixEpochMilliseconds + (long)offset.TotalMilliseconds) * TimeSpan.TicksPerMillisecond, offset);
		}

		public static long ToUnixTimeMilliseconds(DateTime instance)
		{
			var milliseconds = instance.ToUniversalTime().Ticks / TimeSpan.TicksPerMillisecond;

			return milliseconds - UnixEpochMilliseconds;
		}
	}
}
