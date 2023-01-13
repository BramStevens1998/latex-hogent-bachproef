using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace Dlw.Integration.DataAccelerator.Functions.Domain.Helpers
{
    /// <summary>
    /// Represents a helper for HTTP requests.
    /// </summary>
    public static class HttpRequestHelper
    {
        /// <summary>
        /// Reads the body.
        /// </summary>
        /// <param name="request">A <see cref="HttpRequest"/>.</param>
        /// <returns>A string containing the body.</returns>
        public static string ReadBody(this HttpRequest request)
        {
            string body = null;
            using (var reader = new StreamReader(request.Body))
            {
                body = reader.ReadToEnd();
            }

            return body;
        }

        /// <summary>
        /// Gets the parsed value parameter.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <param name="value">The value to check for.</param>
        /// <param name="default">The default int value.</param>
        /// <returns>A parsed 32 bit integer.</returns>
        public static int GetParsedValueParameter(HttpRequest request, string value, int @default)
        {
            var str = GetValueParameter(request, value);
            return Int32.TryParse(str, out var number) ? number : @default;
        }

        /// <summary>
        /// Gets the value parameter.
        /// </summary>
        /// <param name="request">The <see cref="HttpRequest"/>.</param>
        /// <param name="value">The value to check for.</param>
        /// <returns>The value parameter.</returns>
        public static string GetValueParameter(HttpRequest request, string value)
        {
            return request.QueryString.Value.Contains(value) ? request.Query[value].ToString() : String.Empty;
        }


    }
}
