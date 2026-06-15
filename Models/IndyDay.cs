using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record IndyDay (
         [property: JsonPropertyName("date")] DateOnly Date,
         [property: JsonPropertyName("day_name")] string DayName // why you need that???
      );
