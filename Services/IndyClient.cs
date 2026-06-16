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

   /**
    * @brief gets all possible days for indy
    *
    * @param startDate sets start of range
    * @param endDate sets end of range
    * @return List of possible IndyDays between startDate and endDate
    */
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

   /**
    * @brief gets all subjects
    *
    * @return List of available school subject one can make an entry for
    */
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

   /**
    * @brief gets all possible indy hours
    *
    * @return List of all possible entries one can make
    */
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

   /**
    * @brief gets all special indy's
    *
    * @return List of all SpecialIndy hours 
    */
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

   /**
    * @brief gets the student counts of all possible hours
    *
    * @param date of which day to get the student count
    * @return List of all StudentCount
    */
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

   /**
    * @brief make 2 new normal entry for hour 3 and 4
    *
    * @param date for which to create the entries
    * @param tid TeacherId, for which teacher to create the entries
    * @param subject the subject to set in the entries
    * @param activity what the user is doing in the indy hours
    * @return List of normal entries
    */
   public async Task<List<Normal>> MakeNormalEntryAsync(DateOnly date, string tid, string subject, string activity)
   {
      var results = new List<Normal>();

      results.Add(await MakeNormalEntryAsync(date, 3, tid, subject, activity));
      results.Add(await MakeNormalEntryAsync(date, 4, tid, subject, activity));

      return results;
   }

   /**
    * @brief make a new normal entry
    *
    * @param date for which to create the entry
    * @param 3 or 4, hour for which hour the user wants the entry to be made
    * @param tid TeacherId, for which to create the entry
    * @param subject the subject to set in the entry 
    * @param activity what the user is doing in the indy hour
    * @return a normal entry record
    */
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

   /**
    * @brief make 2 new absence entries
    *
    * @param date for which to create the entries
    * @return List of absence entries
    */
   public async Task<List<Absence>> MakeAbsenceEntryAsync(DateOnly date)
   {
      var results = new List<Absence>();

      results.Add(await MakeAbsenceEntryAsync(date, 3));
      results.Add(await MakeAbsenceEntryAsync(date, 4));

      return results;
   }

   /**
    * @brief make a new absence entry
    *
    * @param date for which to create the enty
    * @param hour for which to create the entry
    * @return a absence entry record
    */
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

   /**
    * @brief make 2 new schoolevent entries
    *
    * @param date for which to create the entries
    * @param tid TeacherId, for which to create the entry with
    * @param description of the schoolevent
    * @return List of made Schoolevent entries
    */
   public async Task<List<SchoolEvent>> MakeSchoolEventEntryAsync(DateOnly date, string tid, string description)
   {
      var results = new List<SchoolEvent>();

      results.Add(await MakeSchoolEventEntryAsync(date, 3, tid, description));
      results.Add(await MakeSchoolEventEntryAsync(date, 4, tid, description));

      return results;
   }

   /**
    * @brief make a new schooevent entry
    *
    * @param date for which to create the entry
    * @param hour for which to create the enrty
    * @param tid TeacherId, for which to create the entry with
    * @param description of the schoolevent
    * @return the newly made schooevent entry record
    */
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

   /**
    * @brief get user details from the token
    *
    * @return List with a single student object
    */
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

   /**
    * @brief get all teachers
    *
    * @return List of all Teacher's
    */
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

   /**
    * @brief get all indy days with status in range
    *
    * @param startDate sets start of range
    * @param endDate sets end of range
    * @return List of indy days with status
    */
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

   /**
    * @brief get all teacher absences
    *
    * @return List of teacher absences
    */
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

   /**
    * @brief get all amde entries for a specific date
    *
    * @param date for which to get the entry
    * @return FullReturned object for the date
    */
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

   /**
    * @brief get all made normal entries using the studentId corresponding to the token
    *
    * @return List of all Normal entries made
    */
   public async Task<List<Normal>> GetAllNormalEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllNormalEntriesAsync(student.First().StudentId);
   }

   /**
    * @brief get all made normal entries
    *
    * @param studentId of the user to fetch the entries
    * @return List all Normal entries made
    */
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

   /**
    * @brief get all absence entries made using the studendId corresponding to the token
    *
    * @return List of all Absence entries made
    */
   public async Task<List<Absence>> GetAllAbsenceEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllAbsenceEntriesAsync(student.First().StudentId);
   }

   /**
    * @brief get all absence entries made
    *
    * @param studentId of the user to fetch the entries for
    * @return List of all Absence entries made
    */
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

   /**
    * @brief get all schoolevent entries made using the studentId correspondig token
    *
    * @return List of all SchoolEvent entries made
    */
   public async Task<List<SchoolEvent>> GetAllSchoolEventEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllSchoolEventEntriesAsync(student.First().StudentId);
   }

   /**
    * @brief get all schoolevent entries made using the studentId corresponding to the token
    *
    * @param studentId of the user to get the entries for
    * @return List of all schoolevent entries made
    */
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

   /**
    * @brief get all freeroom entries made using the studentId corresponding to the token
    *
    * @return List of Object (freeroom record is in progress (i dont have a sample :( ))
    */
   public async Task<List<Object>> GetAllFreeroomEntriesAsync()
   {
      var student = await GetStudentAsync();
      
      return await GetAllFreeroomEntriesAsync(student.First().StudentId);
   }

   /**
    * @brief get all freeroom entries made
    *
    * @param studentId of the user to fetch the freeroom entries for
    * @return List of Object (freeroom record is in progress (i dont have a sample :( ))
    */
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

   /**
    * @brief get all missing entries (not?) made using the studentId corresponding to the token
    *
    * @return List of Missing entries
    */
   public async Task<List<Missing>> GetAllMissingEntriesAsync()
   {
      var student = await GetStudentAsync();

      return await GetAllMissingEntriesAsync(student.First().StudentId);
   }

   /**
    * @brief get all missing entries (not?) made
    *
    * @param studentId of the user to fetch the missing entries for
    * @return List of Missing entries
    */
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

   /**
    * @brief get the rank corresponding to the name of a student
    *
    * @param name of the student
    * @return the returned absence rank object
    */
   public async Task<AbsenceRank> GetAbsenceRankAsync(string name)
   {
      return await this.TryRunAuthAsync(async () =>
      {
         if (name == null)
            throw new ArgumentNullException("Argument 'name' is null");

         if (!name.Contains(' '))
            throw new ArgumentException("Name must consists of first name and last name");

         var request = new HttpRequestMessage(HttpMethod.Get, "leaderboard/absences?name=" + name.Trim());
         request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token.AccessToken);

         var response = await _httpClient.SendAsync(request);
         if (response.StatusCode != System.Net.HttpStatusCode.OK)
         {
            var errorJson = await response.Content.ReadAsStringAsync();

            if (errorJson.Contains("Invalid token"))
               throw new InvalidTokenExcpetion("Parsing failed: Invalid token");

            throw new HttpRequestException($"Getting rank failed: {response.StatusCode} {errorJson}");
         }
         AbsenceRank? result;
         try
         {
            result = await response.Content.ReadFromJsonAsync<AbsenceRank>();

            return result ?? throw new Exception("Getting entries failed");
         }
         catch (JsonException e)
         {
            var errorJson = await response.Content.ReadAsStringAsync();
            throw new JsonException($"Entries parsing failed: {errorJson} {e}");
         }
      });   
   }

   public async Task<Report> GetReportAsync()
   {
      var student = await GetStudentAsync();

      return await GetReportAsync(student.First().StudentId);
   }

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
