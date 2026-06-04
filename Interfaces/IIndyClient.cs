using IndYLib.Models;
using IndYLib.Models.Entry;

namespace IndYLib.Interfaces;

public interface IIndyClient
{
   Task<Normal> MakeEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);
   Task<List<Student>> GetStudentAsync();
   Task<List<Teacher>> GetTeachers();
}
