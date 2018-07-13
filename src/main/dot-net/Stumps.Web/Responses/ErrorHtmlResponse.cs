﻿namespace Stumps.Web.Responses
{
    using Nancy;
    using Nancy.Responses;

    /// <summary>
    ///     A class that provides custom HTML pages for HTTP errors.
    /// </summary>
    public class ErrorHtmlResponse : HtmlResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorHtmlResponse"/> class.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        public ErrorHtmlResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
            this.ContentType = WebResources.ContentTypeHtmlUtf8;

            this.Contents = stream =>
            {
                string page;

                switch (this.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        page = WebResources.PageNotFound;
                        break;

                    case HttpStatusCode.InternalServerError:
                        page = WebResources.PageInternalServerError;
                        break;

                    default:
                        page = string.Empty;
                        break;

                }

                StreamUtility.WriteUtf8StringToStream(page, stream);
            };
        }
    }
}