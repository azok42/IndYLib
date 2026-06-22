using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// Represents a day on which indy happens.
/// </summary>
/// <param name="Date">The date of the indy day.</param>
/// <param name="DayName">The name of the day. ('Mo', 'Mi', 'Fr')</param>
public record IndyDay (
         [property: JsonPropertyName("date")] DateOnly Date,
         [property: JsonPropertyName("day_name")] string DayName // why you need that???
      );
