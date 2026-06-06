using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record IndyHour (
         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("room")] string Room,
         [property: JsonPropertyName("teacher")] string TeacherId,
         [property: JsonPropertyName("consultation")] int Consultation,
         [property: JsonPropertyName("slimit")] int StudentLimit,
         [property: JsonPropertyName("fullname")] string TeacherName,
         [property: JsonPropertyName("area_of_expertise")] string AreaOfExpertise
      );
