using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record SchoolEvent (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("description")] string Description,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour
      );
