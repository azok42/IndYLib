using System.Text.Json.Serialization;

namespace IndYLib.Models.Entry;

/// <summary>
/// The full version of the returned object. <b>Returned</b> by the server.
/// </summary>
/// <param name="Hour3">Single object list with some type of 'Returned' entry which was made in the 3rd hour.</param>
/// <param name="Hour4">Single object list with some type of 'Returned' entry which was made in the 4rd hour.</param>
public record FullRetured (
         [property: JsonPropertyName("3")] List<Returned> Hour3,
         [property: JsonPropertyName("4")] List<Returned> Hour4
      );


/// <summary>
/// Base class. Has basic info about the entry returned by the server, when requesting the made entries for a day.
/// </summary>
/// <param name="Hour">The hour in which the entry happend. (3 or 4)</param>
/// <param name="Date">The date on which the entry happend.</param>
/// <param name="StudentId">The ID of the student, who made the entry.</param>
/// <param name="TeacherId">The ID of the teacher, where this entry happend.</param>
/// <param name="IsSigned">Boolean (1 or 0). 1 if it is already signed by the teacher.</param>
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

/// <summary>
/// A absence entry returned by the server, when requesting the made entries for a day.
/// </summary>
/// <param name="Hour">The hour in which the entry happend. (3 or 4)</param>
/// <param name="Date">The date on which the entry happend.</param>
/// <param name="StudentId">The ID of the student, who made the entry.</param>
/// <param name="TeacherId">The ID of the teacher, where this entry happend.</param>
/// <param name="IsSigned">Boolean (1 or 0). 1 if it is already signed by the teacher.</param>
/// <param name="EntryPastPresent">Boolean (1 or 0). 1 if the entry was made before the <paramref name="Date"/> of the entry.</param>
public record AbsenceReturned (
         int Hour, DateOnly Date, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("entryPastPresent")] string EntryPastPresent,

         string TeacherId = ""

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

/// <summary>
/// A normal entry returned by the server, when requesting the made entries for a day.
/// </summary>
/// <param name="Hour">The hour in which the entry happend. (3 or 4)</param>
/// <param name="Date">The date on which the entry happend.</param>
/// <param name="StudentId">The ID of the student, who made the entry.</param>
/// <param name="TeacherId">The ID of the teacher, where this entry happend.</param>
/// <param name="IsSigned">Boolean (1 or 0). 1 if it is already signed by the teacher.</param>
/// <param name="Activity">The user's description of what the user did/does in the indy hour.</param>
/// <param name="Subject">The subject set in the entry.</param>
/// <param name="Room">The room in which the teacher is.</param>
public record NormalReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

/// <summary>
/// A schoolevent entry returned by the server, when requesting the made entries for a day.
/// </summary>
/// <param name="Hour">The hour in which the entry happend. (3 or 4)</param>
/// <param name="Date">The date on which the entry happend.</param>
/// <param name="StudentId">The ID of the student, who made the entry.</param>
/// <param name="TeacherId">The ID of the teacher, where this entry happend.</param>
/// <param name="IsSigned">Boolean (1 or 0). 1 if it is already signed by the teacher.</param>
/// <param name="Description">The description of what the event is about.</param>
public record SchoolEventReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("description")] string Description

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}

/// <summary>
/// A special entry returned by the server, when requesting the made entries for a day.
/// </summary>
/// <param name="Hour">The hour in which the entry happend. (3 or 4)</param>
/// <param name="Date">The date on which the entry happend.</param>
/// <param name="StudentId">The ID of the student, who made the entry.</param>
/// <param name="TeacherId">The ID of the teacher, where this entry happend.</param>
/// <param name="IsSigned">Boolean (1 or 0). 1 if it is already signed by the teacher.</param>
/// <param name="Day">The short name of a indy day. ('Mo', 'Mi', 'Fr').</param>
/// <param name="StartDate">The start of the range in which the special indy happens.</param>
/// <param name="EndDate">The end of the range in which the special indy happens.</param>
/// <param name="Activity">The activity of what the user is doing in the special indy.</param>
/// <param name="Subject">The subject of the activity of the user.</param>
/// <param name="Room">The room in which the special indy happens.</param>
public record SpecialReturned (
         int Hour, DateOnly Date, string TeacherId, long StudentId, string Type, int IsSigned,

         [property: JsonPropertyName("day")] string Day,
         [property: JsonPropertyName("endDate")] DateOnly EndDate,
         [property: JsonPropertyName("startDate")] DateOnly StartDate,
         [property: JsonPropertyName("activity")] string Activity,
         [property: JsonPropertyName("subject")] string Subject,
         [property: JsonPropertyName("room")] string Room

      ) : Returned(Hour, Date, StudentId, TeacherId, IsSigned) {}
