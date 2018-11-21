using System;
using System.Collections.Generic;
using System.Text;

namespace Infastructure.Helpers
{
    public static class PasswordHelper
    {
        public static string Generate(string password) => BCrypt.Net.BCrypt.HashPassword(password);

        public static bool IsMatch(string password, string hash) => BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
