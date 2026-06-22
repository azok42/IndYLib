using System.Net.Http.Json;
using IndYLib.Interfaces;
using IndYLib.Models;

namespace IndYLib.Services;

public class IndyAuth : IIndyAuth
{
   private static HttpClient HttpClient
   {
      get
      {
         if (field != null)
            return field;

         throw new NullReferenceException("HttpClient is null (have you created a instance?)");
      }

      set
      {
         if (field == null) 
            field = value;
      }
   }

   /// <summary>
   /// Init a new IndyAuth object.
   /// </summary>
   /// <param name="httpClient">The http client used to make calls.</param>
   public IndyAuth(HttpClient httpClient)
   {
      HttpClient = httpClient;
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

      var response = await HttpClient.PostAsync("token", content);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Validation Failed: {errorJson}");
      }

      var result = await response.Content.ReadFromJsonAsync<Token>();

      return result ?? throw new Exception("Login failed: Server returned an empty token.");
   }

   /// <summary>
   /// <i>Try</i> to refresh a token using its refresh token and update the token from the provided client. As of now doesn't really work (server fault).
   /// </summary>
   /// <param name="client">The client to update.</param>
   /// <returns>The new access token.</returns>
   public static async Task<Access> RefreshTokenAsync(IndyClient client)
   {
      var payload = new Dictionary<string, string?>()
      {
         {"refresh_token", client.Token.RefreshToken}
      };

      var response = await HttpClient.PostAsJsonAsync("refresh", payload);
      if (!response.IsSuccessStatusCode)
      {
         var error = await response.Content.ReadAsStringAsync();
         throw new Exception($"Token refresh failed ({response.StatusCode}): {error}");
      }

      var result = await response.Content.ReadFromJsonAsync<Access>();

      if (result == null)
         throw new NullReferenceException("Result is null");

      client.Token = new Token(result.AccessToken, client.Token.RefreshToken, client.Token.Type);

      return result;
   }
}
