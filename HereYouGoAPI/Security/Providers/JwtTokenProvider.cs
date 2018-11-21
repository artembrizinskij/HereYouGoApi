using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Security.Providers
{
    public class JwtTokenProvider : ITokenProvider
    {
        private readonly RsaSecurityKey _key;
        private readonly string _algorithm;
        private readonly string _issuer;
        private readonly string _audience;

        public JwtTokenProvider(string issuer, string audience)
        {
            RSA rsa = RSA.Create(2048);
            _key = new RsaSecurityKey(rsa);
            _algorithm = SecurityAlgorithms.RsaSha256Signature;
            _issuer = issuer;
            _audience = audience;
        }

        public string CreateToken(Account user, DateTime expiry)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(new GenericIdentity(user.Login, "jwt"));
            identity.AddClaim(new Claim("Id", user.Id.ToString()));

            SecurityToken token = tokenHandler.CreateJwtSecurityToken(new SecurityTokenDescriptor
            {
                Audience = _audience,
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(_key, _algorithm),
                Expires = expiry.ToUniversalTime(),
                Subject = identity
            });

            return tokenHandler.WriteToken(token);
        }

        public TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                IssuerSigningKey = _key,
                ValidAudience = _audience,
                ValidIssuer = _issuer,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(0)
            };
        }
    }
}
