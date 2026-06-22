using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// Represents a indy hour in which can a entry can be made.
/// </summary>
/// <param name="DayName">The shortname of the indy day. ('Mo', 'Mi', 'Fr')</param>
/// <param name="Hour">The hour where the indy hour happens.</param>
/// <param name="Room">The room in which the hour happens.</param>
/// <param name="TeacherId">The ID if the teacher whose indy hour this is.</param>
/// <param name="Consultation">Boolean (0 or 1). Whether it is a consultation hour or not.</param>
/// <param name="StudentLimit">The amount of students which can make an entry for this hour.</param>
/// <param name="TeacherName">The name of the teacher holding this indy hour.</param>
/// <param name="AreaOfExpertise">The expertises of the teacher.</param>
public record IndyHour (
         [property: JsonPropertyName("day")] string DayName,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("room")] string Room,
         [property: JsonPropertyName("teacher")] string TeacherId,
         [property: JsonPropertyName("consultation")] int Consultation,
         [property: JsonPropertyName("slimit")] int StudentLimit,
         [property: JsonPropertyName("fullname")] string TeacherName,
         [property: JsonPropertyName("area_of_expertise")] string AreaOfExpertise
      );
