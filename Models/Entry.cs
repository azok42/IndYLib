using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record Entry(
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long SchuelerId,
         [property: JsonPropertyName("lehrerAbsenz")] int TeacherAbsence,
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject
      );
