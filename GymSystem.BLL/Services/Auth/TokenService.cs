using GymSystem.BLL.Interfaces.Auth;
using GymSystem.DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<(string, RefreshToken)> CreateTokenAsync(AppUser user)
        {
            if (user is null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            var authClaims = await BuildAuthClaimsAsync(user);

            var jwtKey = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key cannot be null or empty");

            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var signingCredentials = new SigningCredentials(authKey, SecurityAlgorithms.HmacSha256);

            var tokenHeader = new JwtHeader(signingCredentials);

            var tokenPayload = new JwtPayload(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: authClaims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationInDayes"]))
            );

            var token = new JwtSecurityToken(tokenHeader, tokenPayload);
            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = GenerateRefreshToken();

            user.RefreshTokens.Add(refreshToken);
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new Exception($"Failed to update user with refresh token: {string.Join(", ", result.Errors.Select(e => e.Description))}");

            return (jwtToken, refreshToken);
        }

        public async Task<(string, RefreshToken)> RefreshTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null || !user.RefreshTokens.Any(t => t.Token == refreshToken && t.IsActive))
                throw new UnauthorizedAccessException("Invalid or inactive refresh token");

            var refreshTokenEntity = user.RefreshTokens.Single(t => t.Token == refreshToken);

            refreshTokenEntity.Revoked = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return await CreateTokenAsync(user);
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == refreshToken));

            if (user == null)
                return false;

            var refreshTokenEntity = user.RefreshTokens.SingleOrDefault(t => t.Token == refreshToken);
            if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
                return false;

            refreshTokenEntity.Revoked = DateTime.UtcNow;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomNumber),
                    Expires = DateTime.UtcNow.AddDays(30),
                    Created = DateTime.UtcNow
                };
            }
        }

        private async Task<List<Claim>> BuildAuthClaimsAsync(AppUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? throw new InvalidOperationException("User email cannot be null")),
                new Claim("TrainerId", user.Id.ToString()),
                new Claim("UserId", user.Id.ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);
            if (roles == null || !roles.Any())
                throw new InvalidOperationException("User must have at least one role");

            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            return authClaims;
        }
    }
}