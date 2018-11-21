using System;
using Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace Security.Providers
{
    public interface ITokenProvider
    {
        string CreateToken(Account user, DateTime expiry);
        TokenValidationParameters GetValidationParameters();
    }
}