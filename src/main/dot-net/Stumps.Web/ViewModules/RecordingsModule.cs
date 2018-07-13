﻿namespace Stumps.Web.ViewModules
{
    using System;
    using System.Collections;
    using System.Globalization;
    using Nancy;
    using Stumps.Server;

    /// <summary>
    ///     A class that provides support the recordings overview webpage of the Stumps website.
    /// </summary>
    public class RecordingsModule : NancyModule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RecordingsModule"/> class.
        /// </summary>
        /// <param name="serverHost">The <see cref="IStumpsHost"/> used by the instance.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serverHost"/> is <c>null</c>.</exception>
        public RecordingsModule(IStumpsHost serverHost)
        {
            serverHost = serverHost ?? throw new ArgumentNullException(nameof(serverHost));

            Get["/proxy/{serverId}/recordings"] = _ =>
            {
                var serverId = (string)_.serverId;
                var server = serverHost.FindServer(serverId);

                var recordingModelArray = new ArrayList();

                var lastIndex = -1;

                var recordingList = server.Recordings.Find(-1);
                for (var i = 0; i < recordingList.Count; i++)
                {
                    var recordingModel = new
                    {
                        Index = i,
                        Method = recordingList[i].Request.HttpMethod,
                        RawUrl = recordingList[i].Request.RawUrl,
                        StatusCode = recordingList[i].Response.StatusCode
                    };

                    recordingModelArray.Add(recordingModel);
                    lastIndex = i;
                }

                var model = new
                {
                    ProxyId = server.ServerId,
                    ExternalHostName = server.UseSsl ? server.RemoteServerHostName + " (SSL)" : server.RemoteServerHostName,
                    LocalWebsite = $"http://localhost:{server.ListeningPort}/",
                    IsRecording = server.RecordTraffic,
                    LastIndex = lastIndex,
                    Recordings = recordingModelArray
                };

                return View["recordings", model];
            };
        }
    }
}