using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record Subject (
         [property: JsonPropertyName("subject")] string SubjectId,
         [property: JsonPropertyName("longname")] string SubjectLong,
         [property: JsonPropertyName("active")] int IsActive // basically always 1 but idk (fuck indy)
      );
