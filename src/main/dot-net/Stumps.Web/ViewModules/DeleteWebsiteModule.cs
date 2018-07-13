﻿namespace Stumps.Web.ViewModules
{
    using System;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support for deleting a proxy server through the Stumps website.
    /// </summary>
    public class DeleteWebsiteModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DeleteWebsiteModule"/> class.
        /// </summary>
        /// <param name="stumpsHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stumpsHost"/> is <c>null</c>.</exception>
        public DeleteWebsiteModule(IStumpsHost stumpsHost)
        {
            stumpsHost = stumpsHost ?? throw new ArgumentNullException(nameof(stumpsHost));

            Get["/proxy/{serverId}/delete"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = stumpsHost.FindServer(serverId);

                var model = new
                {
                    ProxyId = server.ServerId,
                    ExternalHostName = server.RemoteServerHostName
                };

                return View["deletewebsite", model];
            };
        }
    }
}