using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record Student (
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("firstname")] string Firstname,
         [property: JsonPropertyName("lastname")] string Lastname,
         [property: JsonPropertyName("username")] string Username,
         [property: JsonPropertyName("email")] string EMail,
         [property: JsonPropertyName("class")] string Class
      );
