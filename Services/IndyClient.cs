using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using IndYLib.Interfaces;
using IndYLib.Exceptions;
using IndYLib.Models;
using IndYLib.Models.Entry;

namespace IndYLib.Services;

public class IndyClient : IIndyClient
{
   public Token Token { get; set; }

   public Func<IndyClient, Task>? ReAuthAsync { get; set; }

   private readonly static HttpClient _httpClient = new()
   {
      BaseAddress = new Uri("https://indy.sz-ybbs.ac.at:8443/")
   };

   public static async Task<List<IndyDay>> GetIndyDaysAsync(DateOnly startDate, DateOnly endDate)
   {
      try
      {
         var dates = new Dictionary<string, string?>()
         {
            {"start_date", startDate.ToString("yyyy-MM-dd")},
            {"end_date", endDate.ToString("yyyy-MM-dd")}
         };

         var uri = QueryHelpers.AddQueryString("validdays/", dates);

         var response = await _httpClient.GetFromJsonAsync<List<IndyDay>>(uri);

         if (response == null)
            throw new HttpRequestException("Getting Days failed: response is null");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new HttpRequestException($"Getting Subjects failed: status {e.StatusCode}");
      }
      catch (JsonException e)
      {
         throw new JsonException($"Getting Subjects failed: failed to parse ({e})");
      }
   }

   public static async Task<List<Subject>> GetActiveSubjectsAsync()
   {
      try
      {
         var response = await _httpClient.GetFromJsonAsync<List<Subject>>("subject/active");

         if (response == null)
            throw new NullReferenceException("Getting Subjects failed: response is null");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new HttpRequestException($"Getting Subjects failed: status {e.StatusCode}");
      }
      catch (JsonException e)
      {
         throw new JsonException($"Getting Subjects failed: failed to parse ({e})");
      }
   }

   public static async Task<List<IndyHour>> GetIndyHoursAsync()
   {
      try
      {
         var response = await _httpClient.GetFromJsonAsync<List<IndyHour>>("hour/");

         if (response == null)
            throw new NullReferenceException("Getting Indyhours failed: response is null");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new HttpRequestException("Getting Indyhours failed: status " + e.StatusCode);
      }
      catch (JsonException e)
      {
         throw new JsonException("Getting indyhours failed: failed to parse (" + e + ")");
      }
   }

   public static async Task<List<SpecialIndy>> GetSpecialIndyAsync()
   {
      try
      {
         var response = await _httpClient.GetFromJsonAsync<List<SpecialIndy>>("specialindy/");

         if (response == null)
            throw new NullReferenceException("Getting Specialindy failed: response is null");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new HttpRequestException("Getting Specialindy failed: status " + e.StatusCode); 
      }
      catch (JsonException e)
      {
         throw new JsonException("Getting Specialindy failed: failed to parse (" + e + ")"); 
      }
   }

   public static async Task<List<StudentCount>> GetStudentCountAsync(DateOnly date)
   {
      try
      {
         var response = await _httpClient.GetFromJsonAsync<List<StudentCount>>("studentcount/?indy_date=" + date.ToString("yyyy-MM-dd"));

         if (response == null)
            throw new NullReferenceException("Getting Studentcount failed: response is null");

         return response;
      }
      catch (HttpRequestException e)
      {
         throw new HttpRequestException("Getting Studentcount failed: status " + e.StatusCode); 
      }
      catch (JsonException e)
      {
         throw new JsonException("Getting Studentcount failed: failed to parse (" + e + ")"); 
      }
   }

   public IndyClient(Token token)
   {
      Token = token;
   }

   public async Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity)
   {
      var results = new List<Normal>();

      results.Add(await MakeNormalEntryAsync(date, 3, tid, subject, activity));
      results.Add(await MakeNormalEntryAsync(date, 4, tid, subject, activity));

      return results;
   }

   public async Task<Normal> MakeNormalEntryAsync(DateOnly date, int hour, string tid, string subject, string activity)
   {
      return await this.TryRunAuthAsync<Normal>(async () =>
      {
         if (hour != 3 && hour != 4)
            throw new ArgumentOutOfRangeException("Parameter 'hour' may only be 3 or 4: was " + hour);

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
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Entry creation failed: {errorJson}");
         }

         Normal? result;

         try
         {
            result = await response.Content.ReadFromJsonAsync<Normal>();

            return result ?? throw new Exception("Entry creation failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entry parsing failed: {e} \n{errorJson}");
         }
      });
   }

   public async Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date)
   {
      var results = new List<Absence>();

      results.Add(await MakeAbsenceEntryAsync(date, 3));
      results.Add(await MakeAbsenceEntryAsync(date, 4));

      return results;
   }

   public async Task<Absence> MakeAbsenceEntryAsync(DateOnly date, int hour)
   {
      return await this.TryRunAuthAsync<Absence>(async () =>
      {
         if (hour != 3 && hour != 4)
            throw new ArgumentOutOfRangeException("Parameter 'hour' may only be 3 or 4: was " + hour);

         var parameters = new Dictionary<string, string?>()
         {
            {"indy_date", date.ToString("yyyy-MM-dd")},
            {"hour", hour.ToString()}
         };

         var uri = QueryHelpers.AddQueryString("entry/absence/", parameters);
         var request = new HttpRequestMessage(HttpMethod.Post, uri);
         request.Headers.Authorization = new  AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Absence creation failed: {errorJson}");
         }

         Absence? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<Absence>();

            return result ?? throw new Exception("Absence creation failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Absence parsing failed: {errorJson} {e}");
         }
      });
   }

   public async Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description)
   {
      var results = new List<SchoolEvent>();

      results.Add(await MakeSchoolEventEntryAsync(date, 3, tid, description));
      results.Add(await MakeSchoolEventEntryAsync(date, 4, tid, description));

      return results;
   }

   public async Task<SchoolEvent> MakeSchoolEventEntryAsync(DateOnly date, int hour, string tid, string description)
   {
      return await this.TryRunAuthAsync<SchoolEvent>(async () =>
      {
         if (hour != 3 && hour != 4)
            throw new ArgumentOutOfRangeException("Parameter 'hour' may only be 3 or 4: was " + hour);

         var parameters = new Dictionary<string, string?>()
         {
            {"indy_date", date.ToString("yyyy-MM-dd")},
            {"hour", hour.ToString()},
            {"tid", tid},
            {"description", description}
         };

         var uri = QueryHelpers.AddQueryString("entry/schoolevent/", parameters);
         var request = new HttpRequestMessage(HttpMethod.Post, uri);
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"SchoolEvent creation failed: {errorJson}");
         }

         SchoolEvent? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<SchoolEvent>();

            return result ?? throw new Exception("SchoolEvent creation failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"SchoolEvent parsing failed: {errorJson} {e}");
         }
      });
   }

   public async Task<List<Student>> GetStudentAsync()
   {
      return await this.TryRunAuthAsync<List<Student>>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "student/");

         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting student failes: {errorJson}");
         }

         List<Student>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<Student>>();

            return result ?? throw new Exception("Getting student failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Student parsing failed: {errorJson} {e.Message}");
         }
      });
   }

   public async Task<List<Teacher>> GetTeachers()
   {
      return await this.TryRunAuthAsync<List<Teacher>>(async () => 
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "teacher/");

         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting teachers failes: {errorJson}");
         }

         List<Teacher>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<Teacher>>();

            return result ?? throw new Exception("Getting teachers failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Teachers parsing failed: {errorJson} {e.Message}");
         }
      });
   }

   public async Task<List<DayStatus>> GetDayStatusesAsync(DateOnly startDate, DateOnly endDate)
   {
      return await this.TryRunAuthAsync<List<DayStatus>>(async () => 
      {
         var parameters = new Dictionary<string, string?>()
         {
            {"start_date", startDate.ToString("yyyy-MM-dd")},
            {"end_date", endDate.ToString("yyyy-MM-dd")}
         };

         var uri = QueryHelpers.AddQueryString("validdays/status/", parameters);
         var request = new HttpRequestMessage(HttpMethod.Get, uri);
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting ValidDays failed: {errorJson}");
         }

         List<DayStatus>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<DayStatus>>();

            return result ?? throw new Exception("Getting ValidDays failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"ValidDays parsing failed {errorJson} {e}");
         }
      });
   }

   public async Task<List<TeacherAbsence>> GetTeacherAbsencesAsync()
   {
      return await this.TryRunAuthAsync<List<TeacherAbsence>>(async () => 
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "teacher/absences/");
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Teacher absences failed: {errorJson}");
         }

         List<TeacherAbsence>? result;
         try
         {
             result = await response.Content.ReadFromJsonAsync<List<TeacherAbsence>>();

             return result ?? throw new Exception("Getting Teacher absences failed");
         }
         catch (JsonException e)
         {
             var errorJson = await response.Content.ReadAsStringAsync();
             throw new JsonException($"Teacher absences parsing failed: {errorJson} {e}");
         }
      });
   }

   public async Task<FullRetured> GetEntriesAsync(DateOnly date)
   {
      return await this.TryRunAuthAsync<FullRetured>(async () => 
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/date/?indy_date=" + date.ToString("yyyy-MM-dd"));
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Entries failed: {response.StatusCode} {errorJson}");
         }

         FullRetured? result;
         try
         {
            var opt = new JsonSerializerOptions { AllowOutOfOrderMetadataProperties = true };

            result = await response.Content.ReadFromJsonAsync<FullRetured>(opt);

            return result ?? throw new Exception("Getting Entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });
   }

   public async Task<List<Normal>> GetAllNormalEntriesAsync(long studentId)
   {
      return await this.TryRunAuthAsync<List<Normal>>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/normal/" + studentId.ToString());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Entries failed: {response.StatusCode} {errorJson}");
         }

         List<Normal>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<Normal>>();

            return result ?? throw new Exception("Getting Entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });
   }
}
