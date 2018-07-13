﻿namespace Stumps.Http
{
    /// <summary>
    ///     A class that represents the response to an HTTP request.
    /// </summary>
    internal sealed class StumpsHttpResponse : BasicHttpResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StumpsHttpResponse"/> class.
        /// </summary>
        public StumpsHttpResponse()
        {
        }

        /// <summary>
        ///     Gets or sets the origin of the HTTP response.
        /// </summary>
        /// <value>
        ///     The origin of the HTTP response.
        /// </value>
        public HttpResponseOrigin Origin
        {
            get;
            set;
        } = HttpResponseOrigin.Unprocessed;

        /// <summary>
        ///     Gets the unique identifier for the Stump that processed the request.
        /// </summary>
        /// <value>
        ///     The unique identifier for the Stump that processed the request.
        /// </value>
        /// <remarks>If a <see cref="Stump"/> was not used to process the request, the value will be <c>null</c>.</remarks>
        public string StumpId
        {
            get;
            set;
        }
    }
}