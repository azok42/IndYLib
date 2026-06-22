using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

/// <summary>
/// The full report object. The actuall returned object returned by the server, when requesting the report.
/// </summary>
/// <param name="Entries">A list of all normal/special and schoolevent entries made.</param>
/// <param name="Statistic">The corresponding stats to the user.</param>
public record Report (
         [property: JsonPropertyName("entries")] List<StatisticEntry> Entries,
         [property: JsonPropertyName("statistik")] Statistic Statistic
      );
