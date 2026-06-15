using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record TeacherAbsence (
         [property: JsonPropertyName("teacher")] string TeacherId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int hour
      );
