using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record Missing (
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("hour")] string Hour
      );
