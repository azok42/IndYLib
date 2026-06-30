namespace IndYLib.Exceptions;

public class InvalidIndyDayException : IndyException
{
   public DateOnly Date { get; }

   public InvalidIndyDayException(DateOnly date)
      : base($"No valid IndyDay on {date.ToString()}")
   {
      Date = date;
   }

   public InvalidIndyDayException(DateOnly date, Exception innerExcpetion)
      : base($"No valid IndyDay on {date.ToString()}", innerExcpetion)
   {
      Date = date;
   }
}
