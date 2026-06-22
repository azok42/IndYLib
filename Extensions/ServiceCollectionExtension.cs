using Microsoft.Extensions.DependencyInjection;
using IndYLib.Interfaces;
using IndYLib.Services;

namespace IndYLib.Extensions;

/// <summary>
/// Provides methods for dependecy inhection
/// </summary>
public static class ServiceCollectionExtensions
{
   /// <summary>
   /// Add a new indy auth instance to the project.
   /// </summary>
   /// <param name="IServiceCollection"></param>
   /// <param name="baseUrl">The base url for the http client.</param>
   public static IServiceCollection AddIndyAuth(this IServiceCollection services, string? baseUrl = null)
   {
      services.AddHttpClient<IIndyAuth, IndyAuth>(client =>
            {
            client.BaseAddress = new Uri(baseUrl ?? "https://indy.sz-ybbs.ac.at:8443/");
            });

      return services;
   }
}
