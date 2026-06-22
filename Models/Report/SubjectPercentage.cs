using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

/// <summary>
/// A single subject with its usage count and percentage.
/// </summary>
/// <param name="Subject">The shortname of the subject.</param>
/// <param name="Count">The usage count of that <paramref name="Subject"/>.</param>
/// <param name="Percentage">The percentage of how often this <paramref name="Subject"/> was used in coparasion to the other.</param>
public record SubjectPercentage (
         [property: JsonPropertyName("Fach")] string Subject,
         [property: JsonPropertyName("Anzahl")] int Count,
         [property: JsonPropertyName("Anteil in Prozent")] string Percentage
      );
