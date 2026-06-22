using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

/// <summary>
/// A schoolevent entry. Returned by the server, when creating a new schoolevent entry.
/// </summary>
/// <param name="StudentId">The ID of the student who created the entry.</param>
/// <param name="Date">The date of the indy day the entry was created for.</param>
/// <param name="Description">The description of what the schoolevent is about.</param>
/// <param name="TeacherId">The ID if the teacher where the schoolevent happens.</param>
/// <param name="Hour">The hour of the entry. (3 or 4)</param>
public record SchoolEvent (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("description")] string Description,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("hour")] int Hour
      );
