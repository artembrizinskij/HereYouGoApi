using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infastructure.Result
{
    public class RequestResult<TData>
    {
        public TData Data { get; set; }
        public IEnumerable<ResultError> Errors { get; set; } = new List<ResultError>();
        public bool Success => !Errors.Any();

        public RequestResult<TData> AddError(string errorMessage, string type = "validation")
        {
            Errors.Append(new ResultError(errorMessage, type));
            return this;
        }

        public RequestResult<TData> SetData(TData data)
        {
            Data = data;
            return this;
        }
    }
}
