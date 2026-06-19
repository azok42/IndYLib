using System.Text.Json.Serialization;

namespace IndYLib.Models;

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
