using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Api.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace Api.E2E.Shared;

// attribution: https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
public static class MockJwtTokensHelper
{
    private static readonly JwtSecurityTokenHandler s_tokenHandler = new();
    private static readonly RandomNumberGenerator s_rng = RandomNumberGenerator.Create();
    private static readonly byte[] s_key = new byte[32];

    static MockJwtTokensHelper()
    {
        s_rng.GetBytes(s_key);
        SecurityKey = new SymmetricSecurityKey(s_key) { KeyId = Guid.NewGuid().ToString() };
        SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
    }

    public static string Audience { get; } = "weather.dev.api";
    public static string Issuer { get; } = "dotnet-user-jwts";
    public static SecurityKey SecurityKey { get; }
    public static SigningCredentials SigningCredentials { get; }

    private static string GenerateJwtToken(params Claim[] claims)
    {
        return s_tokenHandler.WriteToken(new JwtSecurityToken(Issuer, Audience, claims, null,
            DateTime.UtcNow.AddMinutes(20), SigningCredentials));
    }

    public static string GenerateJwtToken(TokenOptions options)
    {
        string[] scopes = [..options.Scopes];
        var claims = scopes.Select(val => new Claim("scope", val)).ToList();
        claims.Add(new Claim("client_id", options.ClientId));
        var jwt = GenerateJwtToken(claims.ToArray());

        return jwt;
    }

    public struct TokenOptions
    {
        public TokenOptions()
        {
        }

        public string ClientId { get; set; } = "client-id";
        public string[] Scopes { get; set; } = [];
    }
}