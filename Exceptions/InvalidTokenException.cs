namespace IndYLib.Exceptions;

/// <summary>
/// The exception which is thrown when the token is invalid or expired.
/// </summary>
public class InvalidTokenExcpetion : IndyException
{
   /// <summary>
   /// Initializes new instance of <see cref="InvalidTokenExcpetion"/> with default message.
   /// </summary>
   public InvalidTokenExcpetion() : base("Token is invalid or has expired") { }

   /// <summary>
   /// Initializes new instance of <see cref="InvalidTokenExcpetion"/> with custom message.
   /// </summary>
   /// <param name="message">The custom message.</param>
   public InvalidTokenExcpetion(string message) : base(message) { }

   /// <summary>
   /// Initializes new instance of <see cref="InvalidTokenExcpetion"/> with custom message and a inner exception.
   /// </summary>
   /// <param name="message">The custom message.</param>
   /// <param name="innerException">The inner excpetion and cause of this exception.</param>
   public InvalidTokenExcpetion(string message, Exception innerException) : base(message, innerException) { }
}
