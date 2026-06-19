using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using IndYLib.Interfaces;
using IndYLib.Exceptions;
using IndYLib.Models;
using IndYLib.Models.Entry;
using IndYLib.Models.Report;

namespace IndYLib.Services;

public class IndyClient : IIndyClient
{
   public Token Token { get; set; }

   public Func<IndyClient, Task>? ReAuthAsync { get; set; }

   private readonly static HttpClient _httpClient = new()
   {
      BaseAddress = new Uri("https://indy.sz-ybbs.ac.at:8443/")
   };

   /// <summary>
   /// Get all indy days in range.
   /// </summary>
   /// <param name="startDate">The start of the range.</param>
   /// <param name="endDate">The end of the range.</param>
   /// <returns>The returned list of indy days.</returns>
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

   /// <summary>
   /// Get all valid subjects.
   /// </summary>
   /// <returns>The returned list of available subjects.</returns>
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

   /// <summary>
   /// Get all possible entries the user can make.
   /// </summary>
   /// <returns>The returned list of all possible indy hours.</returns>
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

   /// <summary>
   /// Get all special indy offers.
   /// </summary>
   /// <returns>The returned list of special indy offers.</returns>
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

   /// <summary>
   /// Get the student count for each possible entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <returns>The returned list of student count objects.</returns>
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

   /// <summary>
   /// Make 2 new normal entries for both hours.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="tid">The ID of the teacher in which to make the entries.</param>
   /// <param name="subject">The subject to set in the entry.</param>
   /// <param name="activity">The activity of the user in this entry.</param>
   /// <returns>A list of the 3 returned normal entries.</returns>
   public async Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity)
   {
      var results = new List<Normal>();

      results.Add(await MakeNormalEntryAsync(date, 3, tid, subject, activity));
      results.Add(await MakeNormalEntryAsync(date, 4, tid, subject, activity));

      return results;
   }

   /// <summary>
   /// Make a new normal entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry for. Must be either 3 or 4.</param>
   /// <param name="tid">The ID of the teacher in which to make the entry in.</param>
   /// <param name="subject">The subject to set in the entry.</param>
   /// <param name="activity">The activity of the user in this entry.</param>
   /// <returns>The returned normal entry.</returns>
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

   /// <summary>
   /// Make 2 new absence entries for both hours.A list of the 2 returned absence entries.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <returns>A list of the 2 returned absence entries.</returns>
   public async Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date)
   {
      var results = new List<Absence>();

      results.Add(await MakeAbsenceEntryAsync(date, 3));
      results.Add(await MakeAbsenceEntryAsync(date, 4));

      return results;
   }

   /// <summary>
   /// Make a new absence entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry for. Must be either 4 or 4.</param>
   /// <returns>The returned absence entry.</returns>
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

   /// <summary>
   /// Make 2 new schoolevent entries for both hours instead of just 1.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="tid">The ID of the teacher to make the entry in.</param>
   /// <param name="description">The description of what the user is doing.</param>
   /// <returns>A list of the 2 returned schoolevent entries.</returns>
   public async Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description)
   {
      var results = new List<SchoolEvent>();

      results.Add(await MakeSchoolEventEntryAsync(date, 3, tid, description));
      results.Add(await MakeSchoolEventEntryAsync(date, 4, tid, description));

      return results;
   }

   /// <summary>
   /// Make a new schoolevent entry.
   /// </summary>
   /// <param name="date">The date of the indy day.</param>
   /// <param name="hour">The hour for which to make the entry. Must be either 3 or 4.</param>
   /// <param name="tid">The ID of the teacher to make the entry in.</param>
   /// <param name="description">The description of what the user is doing.</param>
   /// <returns>The returned schoolevent entry.</returns>
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

   /// <summary>
   /// Get all user detailes for the logged in user
   /// </summary>
   /// <returns>List of a single student object (**WHY IS IT A LIST??**)</returns>
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

   /// <summary>
   /// Get all teachers.
   /// </summary>
   /// <returns>The returned list of teachers</returns>
   public async Task<List<Teacher>> GetTeachersAsync()
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

   /// <summary>
   /// Get the status of all indy days in range.
   /// </summary>
   /// <param name="startDate">The start of the range.</param>
   /// <param name="endDate">The end of the range.</param>
   /// <returns>The returned list of statuses.</returns>
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

            throw new HttpRequestException($"Getting statuses failed: {errorJson}");
         }

         List<DayStatus>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<DayStatus>>();

            return result ?? throw new Exception("Getting statuses failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Statuses parsing failed {errorJson} {e}");
         }
      });
   }

   /// <summary>
   /// Get all teacher absences.
   /// </summary>
   /// <returns>THe returned list of the absences.</returns>
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

   /// <summary>
   /// Get all made entries for a specific date.
   /// </summary>
   /// <param name="date">The date of the fetched entries.</param>
   /// <returns>The returned object.</returns>
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

   /// <summary>
   /// Get all made normal entries for the user which is logged in.
   /// </summary>
   /// <returns>The returned list of normal entries.</returns>
   public async Task<List<Normal>> GetAllNormalEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllNormalEntriesAsync(student.First().StudentId);
   }

   /// <summary>
   /// Get all normal entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for.</param>
   /// <returns>The returned list of normal entries.</returns>
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

   /// <summary>
   /// Get all absence entries made for the user logged in
   /// </summary>
   /// <returns>The returned list of absence entries.</returns>
   public async Task<List<Absence>> GetAllAbsenceEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllAbsenceEntriesAsync(student.First().StudentId);
   }

   /// <summary>
   /// Get all absence entries made.
   /// </summary>
   /// <param name="studentId">THe ID of the user to fetch the entries for.</param>
   /// <returns>The returned list of absence entries.</returns>
   public async Task<List<Absence>> GetAllAbsenceEntriesAsync(long studentId)
   {
      return await this.TryRunAuthAsync<List<Absence>>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/absence/" + studentId.ToString());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Entries failed: {response.StatusCode} {errorJson}");
         }

         List<Absence>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<Absence>>();

            return result ?? throw new Exception("Getting Entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries Parsing failed: {errorJson} {e}");
         }
      });
   }

   /// <summary>
   /// Get all schoolevent entries made for the user which is logged in.
   /// </summary>
   /// <returns>The returned list of schoolevent entries.</returns>
   public async Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllSchoolEventEntriesAsync(student.First().StudentId);
   }

   /// <summary>
   /// Get all schoolevent entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for</param>
   /// <returns>THe returned list of schoolevent entries</returns>
   public async Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync(long studentId)
   {
      return await this.TryRunAuthAsync<List<SchoolEvent>>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/schoolevent/" + studentId);
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Entries failed: {response.StatusCode} {errorJson}");
         }

         List<SchoolEvent>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<SchoolEvent>>();

            return result ?? throw new Exception("getting entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });
   }

   /// <summary>
   /// Get all freeroom entries made for the user which is logged in
   /// </summary>
   /// <returns>A List of Object (freeroom record is in progress (i don't have a sample :( ))</returns>
   public async Task<List<Object>> GetAllFreeroomEntriesAsync()
   {
      var student = await GetStudentAsync();
      
      return await GetAllFreeroomEntriesAsync(student.First().StudentId);
   }

   /// <summary>
   /// Get all freeroom entries made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the entries for</param>
   /// <returns>A list of Object (freeroom record is in profress (i don't have a sample :( ))</returns>
   public async Task<List<Object>> GetAllFreeroomEntriesAsync(long studentId)
   {
      return await this.TryRunAuthAsync<List<Object>>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/freeroom/" + studentId);
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Entries failed; {response.StatusCode} {errorJson}");
         }

         List<Object>? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<List<Object>>();

            return result ?? throw new Exception("Getting Entries failed");

         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });
   }

   /// <summary>
   /// Get all missing entris (not?) made for the user which is logged in.
   /// </summary>
   /// <returns>The returned list of missing entries.</returns>
   public async Task<List<Missing>> GetAllMissingEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllMissingEntriesAsync(student.First().StudentId);
   }

   /// <summary>
   /// Get all missing entries (not?) made.
   /// </summary>
   /// <param name="studentId">The ID of the user to fetch the missing entries for.</param>
   /// <returns>The returned List of missing entries.</returns>
   public async Task<List<Missing>> GetAllMissingEntriesAsync(long studentId)
   {
      return await this.TryRunAuthAsync(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "missing/" + studentId.ToString());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting entries failed: {response.StatusCode} {errorJson}");
         }

         List<Missing>? result;
         try
         {
             result = await response.Content.ReadFromJsonAsync<List<Missing>>();

             return result ?? throw new Exception("Getting entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync(); 
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });
   }

   /// <summary>
   /// Get the rank corresponding to the user which is logged in.
   /// </summary>
   /// <returns>The returned absence rank object.</returns>
   public async Task<AbsenceRank> GetAbsenceRankAsync()
   {
      var student = (await GetStudentAsync()).First();

      return await GetAbsenceRankAsync(student.Firstname + " " + student.Lastname);
   }

   /// <summary>
   /// Get the rank corresponding to the name of a student.
   /// </summary>
   /// <param name="name">The name of the student.</param>
   /// <returns>The returned absence rank object.</returns>
   public async Task<AbsenceRank> GetAbsenceRankAsync(string name)
   {
      if (name == null)
         throw new ArgumentNullException("Argument 'name' is null");

      if (!name.Contains(' '))
         throw new ArgumentException("Name must consists of first name and last name");

      return await this.TryRunAuthAsync(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "leaderboard/absences?name=" + name.Trim());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            if (errorJson.Contains("Student"))
               throw new StudentNotFoundException(name);

            throw new HttpRequestException($"Getting rank failed: {response.StatusCode} {errorJson}");
         }

         AbsenceRank? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<AbsenceRank>();

            return result ?? throw new Exception("Getting rank failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Rank parsing failed: {errorJson} {e}");
         }
      });   
   }

   /// <summary>
   /// Get the report of the user corresponding to the token.
   /// </summary>
   /// <returns>The returned report object.</returns>
   public async Task<Report> GetReportAsync()
   {
      var student = await GetStudentAsync();

      return await GetReportAsync(student.First().StudentId);
   }

   /// <summary>
   /// Gets the report of the user.
   /// </summary>
   /// <param name="studentId">The ID of the user.</param>
   /// <returns>The returned report object.</returns>
   public async Task<Report> GetReportAsync(long studentId)
   {
      return await this.TryRunAuthAsync<Report>(async () =>
      {
         var request = new HttpRequestMessage(HttpMethod.Get, "entry/report/" + studentId.ToString());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting Report failed: {response.StatusCode} {errorJson}");
         }

         Report? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<Report>();

            return result ?? throw new Exception("Getting Report failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Report parsing failed: {errorJson} {e}");
         }
      });
   }
}
