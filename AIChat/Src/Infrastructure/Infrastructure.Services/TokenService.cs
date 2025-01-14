using Application.Service.Common;
using Domain.Core.Entities.InvalidatedTokenTemplateAggregate;
using Domain.Core.Entities.UserTemplateAggregate;
using Domain.Core.Entities.UserTokenTemplateAggregate;
using Domain.Core.UnitOfWorkContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SymmetricSecurityKey _signingKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly IInvalidatedTokenRepository _invalidatedTokenRepository;
        private readonly IUserTokenRepository _userTokenRepository;
        private readonly IApplicationDbContextUnitOfWork _unitOfWork;

        public TokenService(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IInvalidatedTokenRepository invalidatedTokenRepository, IUserTokenRepository userTokenRepository, IApplicationDbContextUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            _issuer = _configuration["Jwt:Issuer"];
            _audience = _configuration["Jwt:Audience"];
            _invalidatedTokenRepository = invalidatedTokenRepository;
            _userTokenRepository = userTokenRepository;
            _unitOfWork = unitOfWork;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var creds = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = creds,
                Issuer = _issuer,
                Audience = _audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Store the token in the database
            var userToken = new UserToken
            {
                Id = Guid.NewGuid(),
                ApplicationUserId = user.Id,
                Token = tokenString,
                IssuedAt = DateTime.UtcNow,
                ExpiresAt = tokenDescriptor.Expires.Value
            };

            _userTokenRepository.AddTokenAsync(userToken);
            _unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();

            return tokenString;
        }

        public void InvalidateToken(string token, Guid userId)
        {
            _invalidatedTokenRepository.InvalidateToken(token, userId);
            _unitOfWork.SaveChangesAsync().GetAwaiter().GetResult();
        }

        public bool IsTokenValid(string token)
        {
            if (_invalidatedTokenRepository.GetInvalidatedToken(token) != null)
            {
                return false;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    IssuerSigningKey = _signingKey
                }, out _);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public string GetTokenFromRequest()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        }
    }
}
