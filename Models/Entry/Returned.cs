using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record Returned (
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("indy_date")] string Date,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("type")] string Type,
         [property: JsonPropertyName("signed")] int IsSigned
      );

public record AbsenceReturned (
         int Hour, string Date, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("entryPastPresent")] string EntryPastPresent,

         string TeacherId = ""

      ) : Returned(Hour, Date, StudentId, TeacherId, Type, IsSigned);

public record NormalReturned (
         int Hour, string Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, Type, IsSigned);

public record SchoolEventReturned (
         int Hour, string Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("description")] string Description

      ) : Returned(Hour, Date, StudentId, TeacherId, Type, IsSigned);

public record SpecialReturned (
         int Hour, string Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("endDate")] string EndDate,
         [property: JsonPropertyName("startDate")] string StartDate,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, Type, IsSigned);
