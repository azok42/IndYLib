using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// Combination of <paramref name="TeacherId"/> and the amount of students who made an entry there today.
/// </summary>
/// <param name="TeacherId">The ID of the teacher.</param>
/// <param name="Hour">The hour of todays indy day.</param>
/// <param name="Count">The amount of students present.</param>
public record StudentCount (
         [property: JsonPropertyName("teacher_id")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("student_count")] int Count
      );
