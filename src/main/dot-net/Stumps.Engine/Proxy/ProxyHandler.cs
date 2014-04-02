﻿namespace Stumps.Proxy
{

    using System;
    using System.Collections.Generic;
    using System.Net;
    using Stumps.Http;
    using Stumps.Logging;
    using Stumps.Utility;

    /// <summary>
    ///     A class implementing the <see cref="T:Stumps.Http.IHttpHandler"/> interface that proxies requests to an external host.
    /// </summary>
    internal class ProxyHandler : IHttpHandler
    {

        private readonly ProxyEnvironment _environment;
        private readonly Uri _externalHostUri;
        private readonly ILogger _logger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Stumps.Proxy.ProxyHandler"/> class.
        /// </summary>
        /// <param name="environment">The environment for the proxy server.</param>
        /// <param name="logger">The logger used by the instance.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="environment"/> is <c>null</c>.
        /// or
        /// <paramref name="logger"/> is <c>null</c>.
        /// </exception>
        public ProxyHandler(ProxyEnvironment environment, ILogger logger)
        {

            if (environment == null)
            {
                throw new ArgumentNullException("environment");
            }

            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            _environment = environment;
            _logger = logger;

            _externalHostUri = CreateUriFromEnvironment();

        }

        /// <summary>
        ///     Processes an incoming HTTP request.
        /// </summary>
        /// <param name="context">The <see cref="T:Stumps.Http.IStumpsHttpContext" /> representing both the incoming request and the response.</param>
        /// <returns>
        ///     A member of the <see cref="T:Stumps.Http.ProcessHandlerResult" /> enumeration.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="context"/> is <c>null</c>.</exception>
        public ProcessHandlerResult ProcessRequest(IStumpsHttpContext context)
        {

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var remoteUrl = BuildRemoteUrlFromContext(context);

            // Create a new HTTP web request
            var remoteWebRequest = (HttpWebRequest)WebRequest.Create(remoteUrl);
            remoteWebRequest.AllowAutoRedirect = false;

            // Populate the headers for the new HTTP request from the incoming HTTP request
            PopulateRemoteHeadersFromContext(context, remoteWebRequest);

            // Setup a repsponse used by the class
            HttpWebResponse response = null;

            // Populate the HTTP body for the request
            var continueProcess = PopulateRemoteBodyFromContext(context, remoteWebRequest, ref response);

            // Execute the remote web request
            if (continueProcess)
            {
                continueProcess = ExecuteRemoteWebRequest(remoteWebRequest, ref response);
            }

            if (!continueProcess)
            {

                // Return a response to the client that the service is unavailable at this time.
                context.Response.StatusCode = HttpStatusCodes.HttpServiceUnavailable;
                context.Response.StatusDescription =
                    HttpStatusCodes.GetStatusDescription(HttpStatusCodes.HttpServiceUnavailable);

            }
            else if (response != null)
            {

                // Write the headers and the body of the response from the remote HTTP request
                // to the incoming HTTP context.
                WriteContextHeadersFromResponse(context, response);
                WriteContextBodyFromRemoteResponse(context, response);
                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.StatusDescription = response.StatusDescription;

                var disposable = response as IDisposable;
                disposable.Dispose();

            }

            return ProcessHandlerResult.Continue;

        }

        /// <summary>
        ///     Builds the remote URL from context.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <returns>
        ///     A string representing the URL for the remote server.
        /// </returns>
        private string BuildRemoteUrlFromContext(IStumpsHttpContext incommingHttpContext)
        {

            var urlPath = incommingHttpContext.Request.RawUrl;

            var urlHost = _externalHostUri.AbsoluteUri;

            urlPath = urlPath.StartsWith("/", StringComparison.Ordinal) ? urlPath : urlPath + "/";

            var url = urlHost + urlPath;

            return url;

        }

        /// <summary>
        ///     Creates the URI from environment environment configuration.
        /// </summary>
        /// <returns></returns>
        private Uri CreateUriFromEnvironment()
        {

            var schema = _environment.UseSsl ? "https://" : "http://";

            var externalHostUri = new Uri(schema + _environment.ExternalHostName + "/");

            return externalHostUri;

        }

        /// <summary>
        ///     Executes the remote web request.
        /// </summary>
        /// <param name="remoteWebRequest">The remote web request.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        /// <returns>
        ///     <c>true</c> if the remote request executed successfully; otherwise, <c>false</c>.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Errors are logged.")]
        private bool ExecuteRemoteWebRequest(HttpWebRequest remoteWebRequest, ref HttpWebResponse remoteWebResponse)
        {

            var success = true;

            try
            {
                remoteWebResponse = (HttpWebResponse)remoteWebRequest.GetResponse();
            }
            catch (WebException wex)
            {

                if (wex.Response != null)
                {
                    remoteWebResponse = (HttpWebResponse)wex.Response;
                }
                else
                {
                    _logger.LogException(Resources.LogProxyRemoteError, wex);
                    success = false;
                }

            }
            catch (Exception ex)
            {
                _logger.LogException(Resources.LogProxyRemoteError, ex);
                success = false;
            }

            return success;

        }

        /// <summary>
        ///     Gets the value of a header.
        /// </summary>
        /// <param name="headers">The dictionary of possible headers.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="defaultValue">The default value to use if the header is not found.</param>
        /// <returns>
        ///     A <see cref="T:System.String"/> representing the value of the header.
        /// </returns>
        private string GetHeaderValue(Dictionary<string, string> headers, string headerName, string defaultValue)
        {

            var headerValue = headers.ContainsKey(headerName) ? headers[headerName] : defaultValue;
            return headerValue;

        }

        /// <summary>
        ///     Populates the remote body from context.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="remoteWebRequest">The remote web request.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        /// <returns>
        ///     <c>true</c> if the remote body was populated successfully; otherwise, <c>false</c>.
        /// </returns>
        private bool PopulateRemoteBodyFromContext(
            IStumpsHttpContext incommingHttpContext, 
            HttpWebRequest remoteWebRequest,
            ref HttpWebResponse remoteWebResponse)
        {

            var success = true;

            try
            {

                incommingHttpContext.Request.InputStream.Position = 0;

                var body = StreamUtility.ConvertStreamToByteArray(incommingHttpContext.Request.InputStream);

                incommingHttpContext.Request.InputStream.Position = 0;

                if (body != null && body.Length > 0)
                {
                    remoteWebRequest.ContentLength = body.Length;
                    var requestStream = remoteWebRequest.GetRequestStream();
                    requestStream.Write(body, 0, body.Length);
                }

            }
            catch (WebException wex)
            {
                if (wex.Response != null)
                {
                    remoteWebResponse = (HttpWebResponse)wex.Response;
                }
                else
                {
                    success = false;
                }
            }

            return success;

        }

        /// <summary>
        ///     Populates the remote headers from context.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="remoteWebRequest">The remote web request.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Errors are logged.")]
        private void PopulateRemoteHeadersFromContext(
            IStumpsHttpContext incommingHttpContext, HttpWebRequest remoteWebRequest)
        {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in incommingHttpContext.Request.Headers.AllKeys)
            {
                headers.Add(key, incommingHttpContext.Request.Headers[key]);
            }

            remoteWebRequest.Method = incommingHttpContext.Request.HttpMethod;
            remoteWebRequest.Accept = GetHeaderValue(headers, "accept", null);
            remoteWebRequest.ContentType = GetHeaderValue(
                headers, "content-type", incommingHttpContext.Request.ContentType);
            remoteWebRequest.Referer = GetHeaderValue(headers, "referer", incommingHttpContext.Request.Referer);
            remoteWebRequest.TransferEncoding = GetHeaderValue(headers, "transfer-encoding", null);
            remoteWebRequest.UserAgent = GetHeaderValue(headers, "user-agent", incommingHttpContext.Request.UserAgent);

            headers.Remove("accept");
            headers.Remove("connection");
            headers.Remove("content-length");
            headers.Remove("content-type");
            headers.Remove("expect");
            headers.Remove("date");
            headers.Remove("host");
            headers.Remove("if-modified-since");
            headers.Remove("range");
            headers.Remove("referer");
            headers.Remove("transfer-encoding");
            headers.Remove("user-agent");

            if (headers.Count <= 0)
            {
                return;
            }

            foreach (var key in headers.Keys)
            {
                try
                {
                    remoteWebRequest.Headers.Add(key, headers[key]);
                }
                catch (Exception ex)
                {
                    // The header could fail to add because it is being referenced
                    // as a property - this is OK.
                    this._logger.LogException(Resources.LogProxyHeaderError, ex);
                }
            }

        }

        /// <summary>
        /// Writes the context body from the remote response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        private void WriteContextBodyFromRemoteResponse(
            IStumpsHttpContext incommingHttpContext, HttpWebResponse remoteWebResponse)
        {

            if (remoteWebResponse.ContentLength != 0)
            {
                var responseStream = remoteWebResponse.GetResponseStream();

                StreamUtility.CopyStream(responseStream, incommingHttpContext.Response.OutputStream);

            }

        }

        /// <summary>
        /// Writes the context headers from the remote response.
        /// </summary>
        /// <param name="incommingHttpContext">The incomming HTTP context.</param>
        /// <param name="remoteWebResponse">The remote web response.</param>
        private void WriteContextHeadersFromResponse(
            IStumpsHttpContext incommingHttpContext, HttpWebResponse remoteWebResponse)
        {

            var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var key in remoteWebResponse.Headers.AllKeys)
            {
                headers.Add(key, remoteWebResponse.Headers[key]);
            }

            incommingHttpContext.Response.Headers.Clear();
            incommingHttpContext.Response.ContentType = remoteWebResponse.ContentType;

            if (headers.ContainsKey("transfer-encoding") &&
                headers["transfer-encoding"].Equals("chunked", StringComparison.OrdinalIgnoreCase))
            {
                incommingHttpContext.Response.SendChunked = true;
            }

            headers.Remove("content-length");
            headers.Remove("content-type");

            // The following headers should not be necessary - re-enable them if we see a need in the future.

            ////headers.Remove("accept");
            ////headers.Remove("connection");
            ////headers.Remove("expect");
            ////headers.Remove("date");
            ////headers.Remove("host");
            ////headers.Remove("if-modified-since");
            ////headers.Remove("range");
            ////headers.Remove("referer");
            headers.Remove("transfer-encoding");
            headers.Remove("keep-alive");
            ////headers.Remove("user-agent");

            foreach (var key in headers.Keys)
            {
                incommingHttpContext.Response.Headers.Add(key, headers[key]);
            }

        }

    }

}