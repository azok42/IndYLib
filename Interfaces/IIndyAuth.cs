namespace IndYLib.Interfaces;

using IndYLib.Models;

public interface IIndyAuth
{
   Task<Token> GetToken(string username, string password);
}
