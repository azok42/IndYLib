using Microsoft.Extensions.DependencyInjection;
using IndYLib.Interfaces;
using IndYLib.Services;

namespace IndYLib.Extensions;

public static class ServiceCollectionExtensions
{
   public static IServiceCollection AddIndyAuth(this IServiceCollection services, string? baseUrl = null)
   {
      services.AddHttpClient<IIndyAuth, IndyAuth>(client =>
            {
            client.BaseAddress = new Uri(baseUrl ?? "https://indy.sz-ybbs.ac.at:8443/");
            });

      return services;
   }
}
