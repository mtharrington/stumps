﻿namespace Stumps.Web.Responses
{
    using System;
    using Nancy;
    using Nancy.Responses;
    using Stumps.Web.Models;

    /// <summary>
    ///     A class that provides custom HTML pages for HTTP errors.
    /// </summary>
    public class ErrorJsonResponse : JsonResponse
    {
        private readonly ErrorModel _errorModel;

        /// <summary>
        ///     Prevents a default instance of the <see cref="ErrorJsonResponse"/> class from being created.
        /// </summary>
        /// <param name="error">The <see cref="ErrorModel"/> to return.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/>is <c>null</c>.</exception>
        private ErrorJsonResponse(ErrorModel error) : base(error, new DefaultJsonSerializer())
        {
            _errorModel = error ?? throw new ArgumentNullException(nameof(error));
        }

        /// <summary>
        ///     Gets the error message.
        /// </summary>
        /// <value>
        ///     The error message.
        /// </value>
        public string ErrorMessage
        {
            get => _errorModel.ErrorMessage;
        }

        /// <summary>
        ///     Gets or sets the full exception for the error that occurred.
        /// </summary>
        /// <value>
        ///     The full exception for the error that occured.
        /// </value>
        public string FullException
        {
            get => _errorModel.FullException;
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorJsonResponse" /> from an exception.
        /// </summary>
        /// <param name="ex">The exception used to generate the error.</param>
        /// <returns>A new <see cref="ErrorJsonResponse"/> object.</returns>
        public static ErrorJsonResponse FromException(Exception ex)
        {
            if (ex == null)
            {
                return new ErrorJsonResponse(
                    new ErrorModel
                    {
                        ErrorMessage = null,
                        Errors = null,
                        FullException = null
                    });
            }

            var rootException = ex.GetBaseException();

            var error = new ErrorModel
            {
                ErrorMessage = WebResources.ErrorUnexpected + rootException.Message,
                FullException = rootException.ToString()
            };

            ErrorJsonResponse response = null;

            try
            {
                response = new ErrorJsonResponse(error)
                {
                    StatusCode = HttpStatusCode.InternalServerError
                };
            }
            catch
            {
                if (response != null)
                {
                    response.Dispose();
                }

                throw;
            }

            return response;
        }

        /// <summary>
        ///     Creates a new <see cref="ErrorJsonResponse" /> from an error message.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <returns>A new <see cref="ErrorJsonResponse"/> object.</returns>
        public static ErrorJsonResponse FromMessage(string message)
        {
            var error = new ErrorModel
            {
                ErrorMessage = message
            };

            var response = new ErrorJsonResponse(error);

            return response;
        }
    }
}