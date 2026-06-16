using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

public record SubjectPercentage (
         [property: JsonPropertyName("Fach")] string Subject,
         [property: JsonPropertyName("Anzahl")] int Count,
         [property: JsonPropertyName("Anteil in Prozent")] string Percentage
      );
