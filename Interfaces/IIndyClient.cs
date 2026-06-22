using IndYLib.Models;
using IndYLib.Models.Entry;

namespace IndYLib.Interfaces;

public interface IIndyClient
{
   Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity);

   Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);

   Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date);

   Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour);

   Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description);

   Task<SchoolEvent> MakeSchoolEventEntryAsync(DateOnly date, int hour, string tid, string description);

   Task<List<Student>> GetStudentAsync();

   Task<List<Teacher>> GetTeachersAsync();

   Task<List<DayStatus>> GetDayStatusesAsync(DateOnly startDate, DateOnly endDate);

   Task<List<TeacherAbsence>> GetTeacherAbsencesAsync();

   Task<FullRetured> GetEntriesAsync(DateOnly date);

   Task<List<Normal>> GetAllNormalEntriesAsync();

   Task<List<Normal>> GetAllNormalEntriesAsync(long studentId);

   Task<List<Absence>> GetAllAbsenceEntriesAsync();

   Task<List<Absence>> GetAllAbsenceEntriesAsync(long studentId);

   Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync();

   Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync(long studentId);

   Task<List<Object>> GetAllFreeroomEntriesAsync();

   Task<List<Object>> GetAllFreeroomEntriesAsync(long studentId);

   Task<List<Missing>> GetAllMissingEntriesAsync();

   Task<List<Missing>> GetAllMissingEntriesAsync(long studentId);

   Task<AbsenceRank> GetAbsenceRankAsync();

   Task<AbsenceRank> GetAbsenceRankAsync(string name);

   Task<Report> GetReportAsync();

   Task<Report> GetReportAsync(long studentId);
}
