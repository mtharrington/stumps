﻿namespace Stumps.Http
{

    using System.Collections.Generic;

    /// <summary>
    ///     A class that contains standard HTTP status codes.
    /// </summary>
    internal static class HttpStatusCodes
    {

        /// <summary>
        ///     The HTTP 404 Not Found status code.
        /// </summary>
        public const int HttpNotFound = 404;

        /// <summary>
        ///     The HTTP 200 OK status code.
        /// </summary>
        public const int HttpOk = 200;

        /// <summary>
        ///     The HTTP Error 503 Service Unavailable status code.
        /// </summary>
        public const int HttpServiceUnavailable = 503;

        private static readonly Dictionary<int, string> Descriptions = new Dictionary<int, string>
        {
            {
                HttpStatusCodes.HttpOk, "OK"
            },
            {
                HttpStatusCodes.HttpNotFound, "Not Found"  
            },
            {
                HttpStatusCodes.HttpServiceUnavailable, "Service Unavailable"
            }
        };

        /// <summary>
        ///     Gets the description for a specified HTTP status code.
        /// </summary>
        /// <param name="httpStatusCode">The HTTP status code.</param>
        /// <returns>
        ///     A <see cref="T:System.String" /> that represents the description for the <paramref name="httpStatusCode" />.
        /// </returns>
        public static string GetStatusDescription(int httpStatusCode)
        {

            string description = null;

            if (Descriptions.ContainsKey(httpStatusCode))
            {
                description = Descriptions[httpStatusCode];
            }

            return description;

        }

    }

}