using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

/// <summary>
/// A normal entry. Returned by the server, when creating a new normal entry.
/// </summary>
/// <param name="TeacherId">The ID of the teacher, where the entry has been made.</param>
/// <param name="Hour">The hour of the entry.</param>
/// <param name="StudentId">The ID if the student wanting to create the entry.</param>
/// <param name="TeacherAbsence">Boolean (1 or 0). 1 if the teacher is absent</param>
/// <param name="Date">The date of the indy day.</param>
/// <param name="Activity">The description of what the user wants to do.</param>
/// <param name="Subject">The subject used in the entry.</param>
public record Normal (
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("lehrerAbsenz")] int TeacherAbsence,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject
      );
