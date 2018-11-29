using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Infastructure.Helpers
{
    public static class DeserializeObjectHelper
    {
        public static bool TryDeserializeObject<TResult>(this string json, out TResult result)
        {
            try
            {
                result = JsonConvert.DeserializeObject<TResult>(json);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = default(TResult);
                return false;
            }
        }
    }
}
