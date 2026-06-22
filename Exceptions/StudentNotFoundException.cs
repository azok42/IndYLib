namespace IndYLib.Exceptions;

public class StudentNotFoundException : Exception
{
   public StudentNotFoundException () : base("Student not found") { }

   public StudentNotFoundException (string name) : base("Student '" + name + "' not found") { }

   public StudentNotFoundException (string name, Exception innerExcpetion) : base("Student '" + name + "' not found", innerExcpetion) { }
}
