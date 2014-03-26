namespace Stumps.Server
{

    /// <summary>
    ///     A class that represents an HTTP response in the context of a <see cref="T:Stumps.Server.StumpContract"/>.
    /// </summary>
    public class ContractHttpResponse : BasicHttpResponse
    {

        /// <summary>
        ///     Gets or sets a value indicating whether the HTTP body is an image.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the HTTP body is an image; otherwise, <c>false</c>.
        /// </value>
        public bool BodyIsImage { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the HTTP body is text.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the HTTP body is text; otherwise, <c>false</c>.
        /// </value>
        public bool BodyIsText { get; set; }

    }

}
