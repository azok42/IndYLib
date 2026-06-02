using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using IndYLib.Interfaces;
using IndYLib.Models;

namespace IndYLib.Services;

public class IndyClient : IIndyClient
{
   private readonly HttpClient _httpClient;

   public Token _token { get; }

   private readonly static HttpClient _staticHttpClient = new()
   {
      BaseAddress = new Uri("https://indy.sz-ybbs.ac.at:8443/")
   };

   public static async Task<List<SpecialIndy>> GetSpecialIndyAsync()
   {
      try
      {
         var response = await _staticHttpClient.GetFromJsonAsync<List<SpecialIndy>>("specialindy/");

         if (response == null)
            throw new InvalidOperationException("Getting Specialindy failed: response is empty");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new InvalidOperationException("Getting Specialindy failed: StatusCode " + e.StatusCode); 
      }
      catch (JsonException e)
      {
         throw new InvalidOperationException("Getting Specialindy failed: Could not parse response (" + e + ")"); 
      }
   }

   public IndyClient(HttpClient httpClient, Token token)
   {
      _httpClient = httpClient;
      _token = token;
   }

   public async Task<Entry> MakeEntryAsync(DateOnly date, int hour, string tid, string subject, string activity)
   {
      var parameters = new Dictionary<string, string?>
      {
         {"indy_date", date.ToString("yyyy-MM-dd")},
         {"hour", hour.ToString()},
         {"tid", tid},
         {"subject", subject},
         {"activity", activity}
      };

      var uri = QueryHelpers.AddQueryString("entry/normal/", parameters);
      var request = new HttpRequestMessage(HttpMethod.Post, uri);
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

      var response = await _httpClient.SendAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Entry creation failed: {errorJson}");
      }

      Entry? result;

      try
      {
         result = await response.Content.ReadFromJsonAsync<Entry>();
      }
      catch (Exception e)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Entry parsing failed: {e} \n{errorJson}");
      }

      return result ?? throw new Exception("Entry creation failed");
   }
}
