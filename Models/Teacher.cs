using System.Text.Json.Serialization;

namespace IndYLib.Models;

public record Teacher (
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("firstname")] string Firstname,
         [property: JsonPropertyName("lastname")] string Lastname,
         [property: JsonPropertyName("username")] string Username,
         [property: JsonPropertyName("email")] string EMail,
         [property: JsonPropertyName("areaofexpertise")] string Expertises
      );
