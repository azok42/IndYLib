using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record SchoolEvent (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("description")] string Description,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour
      );

public record SchoolEventReturned (
      long StudentId, string Date, string Description, string TeacherId, int Hour,

      [property: JsonPropertyName("type")] string Type,
      [property: JsonPropertyName("signed")] int IsSigned

      ) : SchoolEvent(StudentId, Date, Description, TeacherId, Hour);
