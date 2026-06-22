using IndYLib.Exceptions;

namespace IndYLib.Services;

/// <summary>
/// A helper class of the indy client.
/// </summary>
public static class IndyHelper
{
   /// <summary>
   /// A wrapper function for all token-using functions. Automatically tries to refresh and after a fail reauths. After success reruns origin function.
   /// </summary>
   /// <param name="client">The client instance used for refreshing.</param>
   /// <param name="action">The wrapped function excecuted.</param>
   /// <typeparam name="T">The returned type of the wrapped <paramref name="action"/>.</typeparam>
   /// <returns>The value of the wrapped <paramref name="action"/> function.</returns>
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
