using IndYLib.Models;
using IndYLib.Models.Entry;
using IndYLib.Models.Report;

namespace IndYLib.Interfaces;

/// <summary>
/// API wrapper class. Includes all known endpoints.
/// </summary>
public interface IIndyClient
{
   /// <summary>
   /// Make 2 new normal entries for both hours.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="tid">The ID of the teacher in which to make the entries.</param>
   /// <param name="subject">The subject to set in the entry.</param>
   /// <param name="activity">The activity of the user in this entry.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>A list of the 3 returned normal entries.</returns>
   Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity);

   /// <summary>
   /// Make a new normal entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry for. Must be either 3 or 4.</param>
   /// <param name="tid">The ID of the teacher in which to make the entry in.</param>
   /// <param name="subject">The subject to set in the entry.</param>
   /// <param name="activity">The activity of the user in this entry.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned normal entry.</returns>
   Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);

   /// <summary>
   /// Make 2 new absence entries for both hours.A list of the 2 returned absence entries.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>A list of the 2 returned absence entries.</returns>
   Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date);

   /// <summary>
   /// Make a new absence entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry for. Must be either 4 or 4.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned absence entry.</returns>
   Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour);

   /// <summary>
   /// Make 2 new schoolevent entries for both hours instead of just 1.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="tid">The ID of the teacher to make the entry in.</param>
   /// <param name="description">The description of what the user is doing.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>A list of the 2 returned schoolevent entries.</returns>
   Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description);

   /// <summary>
   /// Make a new schoolevent entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry. Must be either 3 or 4.</param>
   /// <param name="tid">The ID of the teacher to make the entry in.</param>
   /// <param name="description">The description of what the user is doing.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned schoolevent entry.</returns>
   Task<SchoolEvent> MakeSchoolEventEntryAsync(DateOnly date, int hour, string tid, string description);

   /// <summary>
   /// Get all user detailes for the logged in user
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>List of a single student object (**WHY IS IT A LIST??**)</returns>
   Task<List<Student>> GetStudentAsync();

   /// <summary>
   /// Get all teachers.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of teachers</returns>
   Task<List<Teacher>> GetTeachersAsync();

   /// <summary>
   /// Get the status of all indy days in range.
   /// </summary>
   /// <param name="startDate">The start of the range.</param>
   /// <param name="endDate">The end of the range. Must not be after startdate.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of statuses.</returns>
   Task<List<DayStatus>> GetDayStatusesAsync(DateOnly startDate, DateOnly endDate);

   /// <summary>
   /// Get all teacher absences.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of the absences.</returns>
   Task<List<TeacherAbsence>> GetTeacherAbsencesAsync();

   /// <summary>
   /// Get all made entries for a specific date.
   /// </summary>
   /// <param name="date">The date of the fetched entries.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned object.</returns>
   Task<FullRetured> GetEntriesAsync(DateOnly date);

   /// <summary>
   /// Get all made normal entries for the user which is logged in.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of normal entries.</returns>
   Task<List<Normal>> GetAllNormalEntriesAsync();

   /// <summary>
   /// Get all normal entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of normal entries.</returns>
   Task<List<Normal>> GetAllNormalEntriesAsync(long studentId);

   /// <summary>
   /// Get all absence entries made for the user logged in
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of absence entries.</returns>
   Task<List<Absence>> GetAllAbsenceEntriesAsync();

   /// <summary>
   /// Get all absence entries made.
   /// </summary>
   /// <param name="studentId">THe ID of the user to fetch the entries for.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of absence entries.</returns>
   Task<List<Absence>> GetAllAbsenceEntriesAsync(long studentId);

   /// <summary>
   /// Get all schoolevent entries made for the user which is logged in.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of schoolevent entries.</returns>
   Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync();

   /// <summary>
   /// Get all schoolevent entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of schoolevent entries</returns>
   Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync(long studentId);

   /// <summary>
   /// Get all freeroom entries made for the user which is logged in
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>A List of Object (freeroom record is in progress (i don't have a sample :( ))</returns>
   Task<List<Object>> GetAllFreeroomEntriesAsync();

   /// <summary>
   /// Get all freeroom entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>A list of Object (freeroom record is in profress (i don't have a sample :( ))</returns>
   Task<List<Object>> GetAllFreeroomEntriesAsync(long studentId);

   /// <summary>
   /// Get all missing entris (not?) made for the user which is logged in.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned list of missing entries.</returns>
   Task<List<Missing>> GetAllMissingEntriesAsync();

   /// <summary>
   /// Get all missing entries (not?) made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the missing entries for.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned List of missing entries.</returns>
   Task<List<Missing>> GetAllMissingEntriesAsync(long studentId);

   /// <summary>
   /// Get the rank corresponding to the user which is logged in.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned absence rank object.</returns>
   Task<AbsenceRank> GetAbsenceRankAsync();

   /// <summary>
   /// Get the rank corresponding to the name of a student.
   /// </summary>
   /// <param name="name">The name of the student.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <exception cref="StudentNotFoundException">Throw when the requested user <paramref name="name"/> does not exist.</exception>
   /// <returns>The returned absence rank object.</returns>
   Task<AbsenceRank> GetAbsenceRankAsync(string name);

   /// <summary>
   /// Get the report of the user corresponding to the token.
   /// </summary>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned report object.</returns>
   Task<Report> GetReportAsync();

   /// <summary>
   /// Gets the report of the user.
   /// </summary>
   /// <param name="studentId">The ID of the user.</param>
   /// <exception cref="InvalidTokenExcpetion">Throw when the token is invalid or has expired, refresh failed, and reauth function has not been set.</exception>
   /// <returns>The returned report object.</returns>
   Task<Report> GetReportAsync(long studentId);
}
