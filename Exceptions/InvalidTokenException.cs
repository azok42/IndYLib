namespace IndYLib.Exceptions;

public class InvalidTokenExcpetion : Exception
{
   public InvalidTokenExcpetion() : base("Token is invalid or has expired") { }

   public InvalidTokenExcpetion(string message) : base(message) { }

   public InvalidTokenExcpetion(string message, Exception innerException) : base(message, innerException) { }
}
