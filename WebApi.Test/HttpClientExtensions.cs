using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Test
{
    internal static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> PostAsync(this HttpClient client, [StringSyntax(StringSyntaxAttribute.Uri)] string? requestUri)
        {
            return await client.PostAsync(requestUri, null);
        }
    }
}
