namespace Stumps
{
    using System;
    using System.Threading.Tasks;
    using Stumps.Http;

    /// <summary>
    ///     A class implementing the <see cref="IHttpHandler"/> interface that provides a fallback
    ///     response to an incoming HTTP request.
    /// </summary>
    internal class FallbackResponseHandler : IHttpHandler
    {
        private readonly int _statusCode;
        private readonly string _statusCodeDescription;
        private readonly HttpResponseOrigin _origin;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FallbackResponseHandler"/> class.
        /// </summary>
        /// <param name="response">The default response.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="response"/> is <c>null</c>.</exception>
        public FallbackResponseHandler(FallbackResponse response)
        {
            if (!Enum.IsDefined(typeof(FallbackResponse), response))
            {
                throw new ArgumentOutOfRangeException(nameof(response));
            }

            // Fallback to 503 Service Unavailable when undefined
            response = response == FallbackResponse.Undefined ? FallbackResponse.Http503ServiceUnavailable : response;

            _statusCode = (int)response;
            _statusCodeDescription = HttpStatusCodes.GetStatusDescription(_statusCode);

            _origin = response == FallbackResponse.Http404NotFound
                          ? HttpResponseOrigin.NotFoundResponse
                          : HttpResponseOrigin.ServiceUnavailable;
        }

        /// <summary>
        ///     Occurs when an incoming HTTP requst is processed and responded to by the HTTP handler.
        /// </summary>
        public event EventHandler<StumpsContextEventArgs> ContextProcessed;

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        /// A member of the <see cref="ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public async Task<ProcessHandlerResult> ProcessRequest(IStumpsHttpContext context)
        {
            context = context ?? throw new ArgumentNullException(nameof(context));

            context.Response.Headers.Clear();
            context.Response.ClearBody();
            context.Response.StatusCode = _statusCode;
            context.Response.StatusDescription = _statusCodeDescription;

            if (context.Response is StumpsHttpResponse stumpsResponse)
            {
                stumpsResponse.Origin = _origin;
            }

            this.ContextProcessed?.Invoke(this, new StumpsContextEventArgs(context));

            return await Task.FromResult<ProcessHandlerResult>(ProcessHandlerResult.Terminate);
        }
    }
}
