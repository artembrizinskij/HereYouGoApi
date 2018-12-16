using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.ViewModels
{
    public class JsonWebTokenViewModel
    {
        public string AccessToken { get; set; }
        public string TokenType { get; set; } = "bearer";
        public int Expire { get; set; }
    }
}
