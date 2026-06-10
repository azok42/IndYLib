using IndYLib.Models;
using IndYLib.Services;

namespace IndYLib.Interfaces;

public interface IIndyAuth
{
   Task<IIndyClient> CreateClientAsync(Token token);
   Task<IIndyClient> CreateClientAsync(string username, string password);
   Task<Token> GetToken(string username, string password);
   Task<Access> RefreshTokenAsync(IndyClient client);
}
