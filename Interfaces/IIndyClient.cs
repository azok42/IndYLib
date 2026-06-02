using IndYLib.Models;

namespace IndYLib.Interfaces;

public interface IIndyClient
{
   Task<Entry> MakeEntryAsync(DateOnly date, int hour, string tid, string subject, string activity);
}
