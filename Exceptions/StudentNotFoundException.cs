namespace IndYLib.Exceptions;

/// <summary>
/// The exception which is thrown when a student was requested which does not exist. 
/// </summary>
public class StudentNotFoundException : Exception
{
   /// <summary>
   /// Initializes new instance of <see cref="StudentNotFoundException"/> with default message.
   /// </summary>
   public StudentNotFoundException () : base("Student not found") { }

   /// <summary>
   /// Initializes new instance of <see cref="StudentNotFoundException"/> with name of the requested student.
   /// </summary>
   /// <param name="name">The name of the requested student.</param>
   public StudentNotFoundException (string name) : base($"Student '{name}' not found") { }

   /// <summary>
   /// Initializes new instance of <see cref="StudentNotFoundException"/> with name of the requested student and a inner exception.
   /// </summary>
   /// <param name="name">The name of the requested student.</param>
   /// <param name="innerException">The inner excpetion and cause of this exception.</param>
   public StudentNotFoundException (string name, Exception innerException) : base($"Student '{name}' not found", innerException) { }
}
