using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// The status of a indy day.
/// </summary>
/// <param name="Date">The date of the indy day.</param>
/// <param name="DayName">The name of the indy day. ('Mo', 'Mi', 'Fr')</param>
/// <param name="Status">
/// The actual status.
///  <list type="bullet">
///   <item><description><c>0</c>: No entries have been made yet, can still be made.</description></item>
///   <item><description><c>1</c>: idk (i haven't found a 1 status yet).</description></item>
///   <item><description><c>2</c>: At least 1 entry has been made, but not signed yet.</description></item>
///   <item><description><c>3</c>: Both Entries have been made and signed.</description></item>
///   <item><description><c>4</c>: At least 1 entry has not been made and cannot be still made. Or teacher is absent.</description></item>
///   <item><description><c>5</c>: New special indy has been made for this teacher and thus the entry has been cancelled.</description></item>
///   <item><description><c>6</c>: Absence entries have been made.</description></item>
///  </list>
/// </param>
public record DayStatus (
         [property: JsonPropertyName("date")] DateOnly Date,
         [property: JsonPropertyName("day_name")] string DayName,
         [property: JsonPropertyName("status")] int Status
      );
