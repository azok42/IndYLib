using System.Text.Json.Serialization;

namespace IndYLib.Models.Report;

/// <summary>
/// The entry type used in the report.
/// </summary>
/// <param name="StudentId">The ID of the student who made the entry.</param>
/// <param name="Date">The date of the indy day the entry happend.</param>
/// <param name="Hour">The hour in which the entry happend.</param>
/// <param name="TeacherId">The ID of the teacher where the entry is in.</param>
/// <param name="Subject">The subject specified in the entry. Is null when <paramref name="Type"/> is 'schoolevent'.</param>
/// <param name="Type">The type of the entry. ('normal', 'schoolevent')</param>
public record StatisticEntry (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("type")] string Type
      );
