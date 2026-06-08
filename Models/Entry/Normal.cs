using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record Normal (
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("lehrerAbsenz")] int TeacherAbsence,
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject
      );
