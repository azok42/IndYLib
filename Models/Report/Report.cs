using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

public record Report (
         [property: JsonPropertyName("entries")] List<StatisticEntry> Entries,
         [property: JsonPropertyName("statistik")] Statistic Statistic
      );
