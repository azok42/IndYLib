namespace IndYLib.Exceptions;

public class NotFoundException : IndyException
{
   public string ResourceType { get; }
   public string ResourceName { get; }

   public NotFoundException(string resource)
      : base($"Could not find requested resource '{resource}'")
   {
      ResourceType = resource;
      ResourceName = resource;
   }

   public NotFoundException(string resourceType, string resourceName)
      : base($"Could not find requested resource '{resourceName}' of type '{resourceType}'")
   {
      ResourceType = resourceType;
      ResourceName = resourceName;
   }

   public NotFoundException(string resourceType, string resourceName, Exception innerException)
      : base($"Could not find requested resource '{resourceName}' of type '{resourceType}'", innerException)
   {
      ResourceType = resourceType;
      ResourceName = resourceName;
   }
}
