using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// A teacher absence.
/// </summary>
/// <param name="TeacherId">The ID of the teacher.</param>
/// <param name="Date">The date of the indy day.</param>
/// <param name="Hour">The hour on which the teacher is missing.</param>
public record TeacherAbsence (
         [property: JsonPropertyName("teacher")] string TeacherId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int Hour
      );
