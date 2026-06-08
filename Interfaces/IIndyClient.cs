using IndYLib.Models;
using IndYLib.Models.Entry;

namespace IndYLib.Interfaces;

public interface IIndyClient
{
   Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity);
   Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date);
   Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description);
   Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);
   Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour);
   Task<SchoolEvent> MakeSchoolEventEntryAsync(DateOnly date, int hour, string tid, string description);
   Task<List<DayStatus>> GetDayStatusesAsync(DateOnly startDate, DateOnly endDate);
   Task<List<Student>> GetStudentAsync();
   Task<List<Teacher>> GetTeachers();
}
