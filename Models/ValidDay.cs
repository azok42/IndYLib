using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record ValidDay (
         [property: JsonPropertyName("date")] string Date,
         [property: JsonPropertyName("day_name")] string DayName,
         [property: JsonPropertyName("status")] int Status
      );
