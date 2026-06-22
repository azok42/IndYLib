using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// A school subject.
/// </summary>
/// <param name="SubjectId">The ID or shortname of the subject.</param>
/// <param name="SubjectLong">The fullname of the subject.</param>
/// <param name="IsActive">Boolean (0 or 1). Basically always 1 but idk (fuck indy) </param>
public record Subject (
         [property: JsonPropertyName("subject")] string SubjectId,
         [property: JsonPropertyName("longname")] string SubjectLong,
         [property: JsonPropertyName("active")] int IsActive
      );
