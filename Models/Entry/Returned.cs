using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

public record FullRetured (
         [property: JsonPropertyName("3")] List<Returned> Hour3,
         [property: JsonPropertyName("4")] List<Returned> Hour4
      );


[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(AbsenceReturned), typeDiscriminator: "entryabsence")]
[JsonDerivedType(typeof(NormalReturned), typeDiscriminator: "entrynormal")]
[JsonDerivedType(typeof(SpecialReturned), typeDiscriminator: "entryspecial")]
[JsonDerivedType(typeof(SchoolEventReturned), typeDiscriminator: "entryschoolevent")]
public record Returned (
         [property: JsonPropertyName("hour")] int Hour,
         [property: JsonPropertyName("indy_date")] DateOnly Date,
         [property: JsonPropertyName("sid")] long StudentId,
         [property: JsonPropertyName("tid")] string TeacherId,
         [property: JsonPropertyName("signed")] int IsSigned
      );

public record AbsenceReturned (
         int Hour, DateOnly Date, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("entryPastPresent")] string EntryPastPresent,

         string TeacherId = ""

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

public record NormalReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

public record SchoolEventReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("description")] string Description

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

public record SpecialReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("endDate")] DateOnly EndDate,
         [property: JsonPropertyName("startDate")] DateOnly StartDate,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}
