using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Irrelephant.DnB.Server.Authentication.Services
{
    public class JwtViaGoogleAuthenticationService : IAuthenticationService
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly byte[] _issuerKey;
        private readonly string _issuer;

        public JwtViaGoogleAuthenticationService(ITokenValidator tokenValidator, byte[] issuerKey, string issuer)
        {
            _tokenValidator = tokenValidator;
            _issuerKey = issuerKey;
            _issuer = issuer;
        }

        public async Task<string> Authenticate(string idToken)
        {
            var tokenValidationResult = await _tokenValidator.ValidateIdToken(idToken);
            if (tokenValidationResult.IsValid)
            {
                return IssueToken(tokenValidationResult.Email, tokenValidationResult.DisplayName);
            }

            return null;
        }

        private string IssueToken(string email, string displayName)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Email, email),
                    new Claim(ClaimTypes.Name, displayName) 
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(_issuerKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
