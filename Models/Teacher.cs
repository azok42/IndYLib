using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// Represents a teacher.
/// </summary>
/// <param name="TeacherId">The Id of the teacher.</param>
/// <param name="Firstname">The firstname of the teacher.</param>
/// <param name="Lastname">The lastname of the teacher.</param>
/// <param name="Username">The username of the teacher. Format: <paramref name="Firstname"/>.<paramref name="Lastname"/></param>
/// <param name="EMail">The E-Mail of the teacher.</param>
/// <param name="Expertises">The expertises of the teacher. Format: no format</param>
public record Teacher (
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("firstname")] string Firstname,
         [property: JsonPropertyName("lastname")] string Lastname,
         [property: JsonPropertyName("username")] string Username,
         [property: JsonPropertyName("email")] string EMail,
         [property: JsonPropertyName("areaofexpertise")] string Expertises
      );
