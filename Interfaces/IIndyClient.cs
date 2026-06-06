using IndYLib.Models;
using IndYLib.Models.Entry;

namespace IndYLib.Interfaces;

public interface IIndyClient
{
   Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);
   Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour);
   Task<SchoolEvent> MakeSchoolEventEntryAsync(DateOnly date, int hour, string tid, string description);
   Task<List<ValidDay>> GetValidDaysAsync(DateOnly startDate, DateOnly endDate);
   Task<List<Student>> GetStudentAsync();
   Task<List<Teacher>> GetTeachers();
}
