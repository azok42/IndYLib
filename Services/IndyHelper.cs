using IndYLib.Exceptions;

namespace IndYLib.Services;

public static class IndyHelper
{
   public static async Task<T> TryRunAuthAsync<T>(this IndyClient client, Func<Task<T>> action)
   {
      try
      {
         return await action();
      }
      catch (InvalidTokenExcpetion)
      {
         Console.WriteLine("Token invalid: try refresh");
         await IndyAuth.RefreshTokenAsync(client);

         try
         {
            return await action();
         }
         catch (InvalidTokenExcpetion ex)
         {
            if (client.ReAuthAsync != null)
            {
               await client.ReAuthAsync(client);
               return await action();
            }
            throw new Exception("Still invalid after token refresh — refresh token may be expired.", ex);
         }
      }
   }
}
