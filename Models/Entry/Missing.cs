using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

/// <summary>
/// Used to get all hours where no entries have been made.
/// </summary>
/// <param name="Date">The date of the indy day, where the entry should have been made.</param>
/// <param name="Hour">The hour of the missing entry.</param>
public record Missing (
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("hour")] int Hour
      );
