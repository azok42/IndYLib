using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

/// <summary>
/// The stats object used in the report. There's a server related error somewhere in <paramref name="EntriesMade"/>, <paramref name="PossibleEntries"/> or <paramref name="MissingHours"/>.
/// </summary>
/// <param name="EntriesMade">The amount of entries made in total this year.</param>
/// <param name="PossibleEntries">The amount of indy hours, where entries were possible.</param>
/// <param name="MissingHours">The amount of missed hours.</param>
/// <param name="Statistics">The list of subject stats.</param>
public record Statistic (
         [property: JsonPropertyName("eingetragene Stunden")] int EntriesMade,
         [property: JsonPropertyName("mögliche Stunden (gesamt)")] int PossibleEntries,
         [property: JsonPropertyName("Fehlstunden")] int MissingHours,
         [property: JsonPropertyName("Fächerstatistik")] List<SubjectPercentage> Statistics
      );
