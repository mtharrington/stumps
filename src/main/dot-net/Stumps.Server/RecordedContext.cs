﻿namespace Stumps.Server
{
    using System;

    /// <summary>
    ///     A class representing a recorded HTTP request and response.
    /// </summary>
    public class RecordedContext : IStumpsHttpContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordedContext" /> class.
        /// </summary>
        /// <param name="context">The <see cref="IStumpsHttpContext"/> used to initialize the instance.</param>
        /// <param name="decoderHandling">The <see cref="ContentDecoderHandling" /> requirements for the HTTP body.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public RecordedContext(IStumpsHttpContext context, ContentDecoderHandling decoderHandling)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            this.Request = new RecordedRequest(context.Request, decoderHandling);
            this.Response = new RecordedResponse(context.Response, decoderHandling);

            this.ReceivedDate = context.ReceivedDate;
            this.UniqueIdentifier = context.UniqueIdentifier;
        }

        /// <summary>
        /// Gets the received date and time the request was received.
        /// </summary>
        /// <value>
        /// The date and time the request was received.
        /// </value>
        public DateTime ReceivedDate
        {
            get;
        }

        /// <summary>
        ///     Gets the <see cref="RecordedRequest" /> object for the HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="RecordedRequest" /> object for the HTTP request.
        /// </value>
        public RecordedRequest Request
        {
            get;
        }

        /// <summary>
        ///     Gets the <see cref="IStumpsHttpRequest" /> object for the HTTP request.
        /// </summary>
        /// <value>
        ///     The <see cref="IStumpsHttpRequest" /> object for the HTTP request.
        /// </value>
        IStumpsHttpRequest IStumpsHttpContext.Request
        {
            get => this.Request;
        }

        /// <summary>
        ///     Gets the <see cref="RecordedResponse" /> object for the HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="RecordedResponse" /> object for the HTTP response.
        /// </value>
        public RecordedResponse Response
        {
            get;
        }

        /// <summary>
        ///     Gets the <see cref="IStumpsHttpResponse" /> object for the HTTP response.
        /// </summary>
        /// <value>
        ///     The <see cref="IStumpsHttpResponse" /> object for the HTTP response.
        /// </value>
        IStumpsHttpResponse IStumpsHttpContext.Response
        {
            get => this.Response;
        }

        /// <summary>
        ///     Gets the unique identifier for the recorded HTTP context.
        /// </summary>
        /// <value>
        ///     The unique identifier for the recorded HTTP context.
        /// </value>
        public Guid UniqueIdentifier
        {
            get;
        }
    }
}