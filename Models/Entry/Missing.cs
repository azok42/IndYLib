using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record Missing (
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int Hour
      );
