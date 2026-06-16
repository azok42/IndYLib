using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

public record StatisticEntry (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("type")] string Type
      );
