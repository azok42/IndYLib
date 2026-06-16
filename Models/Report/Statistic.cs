using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

public record Statistic (
         [property: JsonPropertyName("eingetragene Stunden")] int EntriesMade,
         [property: JsonPropertyName("mögliche Stunden (gesamt)")] int PossibleEntries,
         [property: JsonPropertyName("Fehlstunden")] int MissingHours,
         [property: JsonPropertyName("Fächerstatistik")] List<SubjectPercentage> Statistics
      );
