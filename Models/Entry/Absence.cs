using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

/// <summary>
/// A absence entry. Returned from the server when making a new one.
/// </summary>
/// <param name="Hour">The hour of the absence entry.</param>
/// <param name="StudentId">The ID of the student the entry has been made for.</param>
/// <param name="Date">The date of the indy day the entry has been made for.</param>
/// <param name="EntryPastPresent">Boolean (0 or 1). 1 if the entry has been made before the <paramref name="Date"/> of the entry.</param>
public record Absence (
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy-date")] DateOnly Date,
         [property: JsonPropertyName("entryPastPresent")] string EntryPastPresent
      );

/// <summary>
/// The returned value when requesting a users absence rank.
/// </summary>
/// <param name="Rank">The rank of the requested user.</param>
/// <param name="AbsenceCount">The number of absences the requested user has.</param>
public record AbsenceRank (
         [property: JsonPropertyName("rank")] int Rank,
         [property: JsonPropertyName("absence_hours")] int AbsenceCount
      );
