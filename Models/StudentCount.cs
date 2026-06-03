using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record StudentCount (
         [property: JsonPropertyName("teacher_id")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("student_count")] int Count
      );
