using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// Contains details to a student.
/// </summary>
/// <param name="StudentId">The ID of the user.</param>
/// <param name="Firstname">The firstname of the user.</param>
/// <param name="Lastname">The lastname of the user.</param>
/// <param name="Username">The username of the user. Format: <paramref name="Firstname"/>.<paramref name="Lastname"/></param>
/// <param name="EMail">The E-Mail of the user.</param>
/// <param name="Class">The class of the user.</param>
public record Student (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("firstname")] string Firstname,
         [property: JsonPropertyName("lastname")] string Lastname,
         [property: JsonPropertyName("username")] string Username,
         [property: JsonPropertyName("email")] string EMail,
         [property: JsonPropertyName("class")] string Class
      );
