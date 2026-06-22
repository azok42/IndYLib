using IndYLib.Models;

namespace IndYLib.Interfaces;

public interface IIndyAuth
{
   /// <summary>
   /// Create a new IndyClient class using an existing token.
   /// </summary>
   /// <param name="token">The preexisting token used to authenticate.</param>
   /// <returns>A IIndyClient instance where the token is set.</returns>
   Task<IIndyClient> CreateClientAsync(Token token);

   /// <summary>
   /// Create a new IndyClient class using the user credentials to get a new token.
   /// </summary>
   /// <param name="username">The username of the user wanting to log in.</param>
   /// <param name="password">The password of the user wanting to log in.</param>
   /// <returns>A IIndyClient instance where the token is set.</returns>
   Task<IIndyClient> CreateClientAsync(string username, string password);

   /// <summary>
   /// Get a token using the user credentials.
   /// </summary>
   /// <param name="username">The username of the user wanting to log in.</param>
   /// <param name="password">The password of the user wanting to log in.</param>
   /// <returns>A new token object returned by the server.</returns>
   Task<Token> GetToken(string username, string password);
}
