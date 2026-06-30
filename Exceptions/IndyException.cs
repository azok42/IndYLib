namespace IndYLib.Exceptions;

public class IndyException : Exception
{
    public IndyException(string message) : base(message) { }

    public IndyException(string message, Exception innerException) : base(message, innerException) { }
}
