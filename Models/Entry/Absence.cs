using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record Absence (
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("indy-date")] string Date,
         [property: JsonPropertyName("entryPastPresent")] string EntryPastPresent
      );

public record AbsenceReturned (
      int Hour, long StudentId, string Date, string EntryPastPresent,
      
      [property: JsonPropertyName("type")] string Type,
      [property: JsonPropertyName("signed")] int IsSigned

      ) : Absence(Hour, StudentId, Date, EntryPastPresent);
