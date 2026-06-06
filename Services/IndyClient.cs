using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using IndYLib.Interfaces;
using IndYLib.Models;
using IndYLib.Models.Entry;

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
         throw new InvalidOperationException("Getting Specialindy failed: Status " + e.StatusCode); 
      }
      catch (JsonException e)
      {
         throw new InvalidOperationException("Getting Specialindy failed: Could not parse response (" + e + ")"); 
      }
   }

   public static async Task<List<StudentCount>> GetStudentCountAsync(DateOnly date)
   {
      try
      {
         var response = await _staticHttpClient.GetFromJsonAsync<List<StudentCount>>("studentcount/?indy_date=" + date.ToString("yyyy-MM-dd"));

         if (response == null)
            throw new InvalidOperationException("Getting Studentcount failed: response is empty");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new InvalidOperationException("Getting Studentcount failed: Status " + e.StatusCode); 
      }
      catch (JsonException e)
      {
         throw new InvalidOperationException("Getting Studentcount failed: Could not parse response (" + e + ")"); 
      }
   }

   public IndyClient(HttpClient httpClient, Token token)
   {
      _httpClient = httpClient;
      _token = token;
   }

   public async Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity)
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

      Normal? result;

      try
      {
         result = await response.Content.ReadFromJsonAsync<Normal>();
      }
      catch (Exception e)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Entry parsing failed: {e} \n{errorJson}");
      }

      return result ?? throw new Exception("Entry creation failed");
   }

   public async Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour)
   {
      var parameters = new Dictionary<string, string?>()
      {
         {"indy_date", date.ToString("yyyy-MM-dd")},
         {"hour", hour.ToString()}
      };

      var uri = QueryHelpers.AddQueryString("entry/absence/", parameters);
      var request = new HttpRequestMessage(HttpMethod.Post, uri);
      request.Headers.Authorization = new  AuthenticationHeaderValue("Bearer", _token.AccessToken);

      var response = await _httpClient.SendAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"Absence creation failed: {errorJson}");
      }

      Absence? result;
      try
      {
         result = await response.Content.ReadFromJsonAsync<Absence>();
      }
      catch (Exception e)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"Absence parsing failed: {errorJson} {e}");
      }

      return result ?? throw new Exception("Absence creation failed");
   }

   public async Task<List<Student>> GetStudentAsync()
   {
      var request = new HttpRequestMessage(HttpMethod.Get, "student/");

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

      var response = await _httpClient.SendAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Getting student failes: {errorJson}");
      }

      List<Student>? result;
      try
      {
          result = await response.Content.ReadFromJsonAsync<List<Student>>();
      }
      catch (Exception e)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"Student parsing failed: {errorJson} {e.Message}");
      }

      return result ?? throw new Exception("Getting student failed");
   }

   public async Task<List<Teacher>> GetTeachers()
   {
      var request = new HttpRequestMessage(HttpMethod.Get, "teacher/");

      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

      var response = await _httpClient.SendAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
         var errorJson = await response.Content.ReadAsStringAsync();
         throw new Exception($"Getting teachers failes: {errorJson}");
      }

      List<Teacher>? result;
      try
      {
          result = await response.Content.ReadFromJsonAsync<List<Teacher>>();
      }
      catch (Exception e)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"Teachers parsing failed: {errorJson} {e.Message}");
      }

      return result ?? throw new Exception("Getting teachers failed");
   }

   public async Task<List<ValidDay>> GetValidDaysAsync(DateOnly startDate, DateOnly endDate)
   {
      var parameters = new Dictionary<string, string?>()
      {
         {"start_date", startDate.ToString("yyyy-MM-dd")},
         {"end_date", endDate.ToString("yyyy-MM-dd")}
      };

      var uri = QueryHelpers.AddQueryString("validdays/status/", parameters);
      var request = new HttpRequestMessage(HttpMethod.Get, uri);
      request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token.AccessToken);

      var response = await _httpClient.SendAsync(request);
      if (response.StatusCode != System.Net.HttpStatusCode.OK)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"Getting ValidDays failed: {errorJson}");
      }

      List<ValidDay>? result;
      try
      {
          result = await response.Content.ReadFromJsonAsync<List<ValidDay>>();
      }
      catch (Exception e)
      {
          var errorJson = await response.Content.ReadAsStringAsync();
          throw new Exception($"ValidDays parsing failed {errorJson} {e}");
      }

      return result ?? throw new Exception("Getting ValidDays failed");
   }
}
