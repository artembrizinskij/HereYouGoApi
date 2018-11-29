using System;
using System.Collections.Generic;
using System.Text;

namespace Infastructure.Result
{
    public class ResultError
    {
        public ResultError(string type, string message)
        {
            ErrorType = type;
            ErrorMessage = message;
        }

        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
    }
}
