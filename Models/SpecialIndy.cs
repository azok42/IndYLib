using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// A special indy hour with all infos to it.
/// </summary>
/// <param name="TeacherId">The ID of the teacher offering the special indy.</param>
/// <param name="Day">The day name of the indy day. ('Mo', 'Mi', 'Fr')</param>
/// <param name="Hour">The hour of the special indy. (3 or 4)</param>
/// <param name="AreaOfExpertise">The activity of the special indy. Aka why it is special.</param>
/// <param name="StartDate">The start of the range.</param>
/// <param name="EndDate">The end of the range.</param>
/// <param name="StudentLimit">The amount of students which can attend this special indy.</param>
/// <param name="Room">The room in which the special indy happens.</param>
/// <param name="TeacherFullname">The full name of the teacher offering the special indy.</param>
public record SpecialIndy(
         [property: JsonPropertyName("teacher")] string TeacherId,
         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("area_of_expertise")] string AreaOfExpertise,
         [property: JsonPropertyName("start_date")] DateOnly StartDate,
         [property: JsonPropertyName("end_date")] DateOnly EndDate,
         [property: JsonPropertyName("slimit")] int StudentLimit,
         [property: JsonPropertyName("room")] string Room,
         [property: JsonPropertyName("fullname")] string TeacherFullname
      );
