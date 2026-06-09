using System.Net.Http.Json;
using IndYLib.Interfaces;
using IndYLib.Models;

namespace IndYLib.Services;

public class IndyAuth : IIndyAuth
{
   private readonly HttpClient _httpClient;

   public IndyAuth(HttpClient httpClient)
   {
      _httpClient = httpClient;
   }

   public async Task<IIndyClient> CreateClientAsync(Token token)
   {
      return new IndyClient(token);
   }

   public async Task<IIndyClient> CreateClientAsync(string username, string password)
   {
      Token token = await GetToken(username, password);

      return new IndyClient(token);
   }

   public async Task<Token> GetToken(string username, string password)
   {
      var userDetails = new Dictionary<string, string>
      {
         {"username", username},
         {"password", password},
      };

      using var content = new FormUrlEncodedContent(userDetails);

      var response = await _httpClient.PostAsync("token", content);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Validation Failed: {errorJson}");
      }

      var result = await response.Content.ReadFromJsonAsync<Token>();

      return result ?? throw new Exception("Login failed: Server returned an empty token.");
   }
}
