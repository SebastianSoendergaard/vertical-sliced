using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WebApi.Test
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task<T?> As<T>(this HttpResponseMessage response)
        {
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<T>(jsonResult, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return result;
        }
    }
}
