﻿namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     A class that represents a recorded HTTP response.
    /// </summary>
    public sealed class RecordedResponse : IRecordedContextPart
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Stumps.Proxy.RecordedResponse"/> class.
        /// </summary>
        public RecordedResponse()
        {
            this.Headers = new List<HttpHeader>();
        }

        /// <summary>
        ///     Gets or sets the byte array representing the body of the context part.
        /// </summary>
        /// <value>
        ///     The byte array representing the body of the context part.
        /// </value>
        public byte[] Body { get; set; }

        /// <summary>
        ///     Gets or sets the content type of the body.
        /// </summary>
        /// <value>
        ///     The content type of the body.
        /// </value>
        public string BodyContentType { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body" /> of the current instance is an image.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body" /> of the current instance is an image; otherwise, <c>false</c>.
        /// </value>
        public bool BodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body" /> of the current instance is text.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the <see cref="P:Stumps.Proxy.IRecordedContextPart.Body" /> of the current instance is text; otherwise, <c>false</c>.
        /// </value>
        public bool BodyIsText { get; set; }

        /// <summary>
        ///     Gets or sets the headers in the context part.
        /// </summary>
        /// <value>
        ///     The headers in the context part.
        /// </value>
        public IList<HttpHeader> Headers { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        /// <value>
        ///     The HTTP status code.
        /// </value>
        public int StatusCode { get; set; }

        /// <summary>
        ///     Gets or sets the description of the HTTP status code.
        /// </summary>
        /// <value>
        ///     The description of the HTTP status code.
        /// </value>
        public string StatusDescription { get; set; }

        /// <summary>
        ///     Finds the header with the specified name.
        /// </summary>
        /// <param name="name">The name of the specified header.</param>
        /// <returns>
        ///     A <see cref="T:Stumps.Proxy.HttpHeader" /> with the specified <paramref name="name" />.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if an <see cref="T:Stumps.Proxy.HttpHeader" /> is not found.
        /// </remarks>
        public HttpHeader FindHeader(string name)
        {

            for (int i = 0; i < this.Headers.Count; i++)
            {
                if (this.Headers[i].Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return this.Headers[i];
                }
            }

            return null;

        }

    }

}