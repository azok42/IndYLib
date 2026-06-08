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

public record NormalReturned (
      string TeacherId, int Hour, long StudentId, int TeacherAbsence, string Date, string Activity, string Subject,

      [property: JsonPropertyName("type")] string Type,
      [property: JsonPropertyName("signed")] int IsSigned

      ) : Normal(TeacherId, Hour, StudentId, TeacherAbsence, Date, Activity, Subject);
