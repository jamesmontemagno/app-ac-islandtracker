using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace TurnipTracker.Functions.Helpers
{
    public static class Utils
    {
        public static string ParseToken(HttpRequest request)
        {
            var header = AuthenticationHeaderValue.Parse(request.Headers["Authorization"]);
            var authHeader = header.Parameter;

            if (!string.IsNullOrWhiteSpace(authHeader))
            {
                var encoding = Encoding.GetEncoding("iso-8859-1");
#if DEBUG
                return authHeader;
#endif
                return encoding.GetString(Convert.FromBase64String(authHeader));
            }

            return null;
        }
    }
}
