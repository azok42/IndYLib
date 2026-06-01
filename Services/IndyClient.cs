using IndYLib.Interfaces;
using IndYLib.Models;

namespace IndYLib.Services;

public class IndyClient : IIndyClient
{
   private readonly HttpClient _httpClient;
   private readonly Token _token;

   private readonly static HttpClient _staticHttpClient = new()
   {
      BaseAddress = new Uri("https://indy.sz-ybbs.ac.at:8443/")
   };

   public IndyClient(HttpClient httpClient, Token token)
   {
      _httpClient = httpClient;
      _token = token;
   }
}
