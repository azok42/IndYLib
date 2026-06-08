using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

// no normal Special because its a normal entry

public record SpecialReturned (
         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("endDate")] string EndDate,
         [property: JsonPropertyName("startDate")] string StartDate,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("type")] string Type,
         [property: JsonPropertyName("room")] string Room,
         [property: JsonPropertyName("signed")] int IsSigned
      );
