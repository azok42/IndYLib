using System.Text.Json.Serialization;

namespace IndYLib.Models;

/// <summary>
/// The token object returned from /token endpoint.
/// </summary>
/// <param name="AccessToken">Token used to for auth.</param>
/// <param name="RefreshToken">Token used to refresh the <paramref name="AccessToken"/>.</param>
/// <param name="Type">The auth type (should be "Bearer").</param>
public record Token(
      [property: JsonPropertyName("access_token")] string AccessToken,
      [property: JsonPropertyName("refresh_token")] string RefreshToken,
      [property: JsonPropertyName("token_type")] string Type
      );

/// <summary>
/// The access token returned by the server when refreshing a token.
/// </summary>
/// <param name="AccessToken">The refreshed access token.</param>
public record Access(
      [property: JsonPropertyName("access_token")] string AccessToken
      );
