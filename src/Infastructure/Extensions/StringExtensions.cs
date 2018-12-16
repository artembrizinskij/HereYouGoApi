using System;
using System.Collections.Generic;
using System.Text;

namespace Infastructure.Extensions
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
    }
}
