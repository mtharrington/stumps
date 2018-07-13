﻿namespace Stumps.Server
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Stumps.Server.Data;
    using Stumps.Server.Utility;

    /// <summary>
    ///     A class that represents a multitenant host of Stumps servers.
    /// </summary>
    public class StumpsHost : IStumpsHost
    {
        private readonly IServerFactory _serverFactory;
        private readonly IDataAccess _dataAccess;
        private readonly ConcurrentDictionary<string, StumpsServerInstance> _serverInstances;
        private bool _disposed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StumpsHost"/> class.
        /// </summary>
        /// <param name="serverFactory">The factory used to initialize new server instances.</param>
        /// <param name="dataAccess">The data access provider used by the instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serverFactory"/> is <c>null</c>.
        /// or
        /// <paramref name="dataAccess"/> is <c>null</c>.
        /// </exception>
        public StumpsHost(IServerFactory serverFactory, IDataAccess dataAccess)
        {
            _serverFactory = serverFactory ?? throw new ArgumentNullException(nameof(serverFactory)); ;
            _dataAccess = dataAccess ?? throw new ArgumentNullException(nameof(dataAccess));

            _serverInstances = new ConcurrentDictionary<string, StumpsServerInstance>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="StumpsHost"/> class.
        /// </summary>
        ~StumpsHost() => Dispose(false);

        /// <summary>
        ///     Creates a new instance of a Stumps server.
        /// </summary>
        /// <param name="remoteServerHostName">The host name for the remote server by the Stumps server.</param>
        /// <param name="port">The TCP port used to listen for incoming HTTP requests.</param>
        /// <param name="useSsl"><c>true</c> if the remote server requires SSL.</param>
        /// <param name="autoStart"><c>true</c> to automatically start the Stumps server.</param>
        /// <returns>
        ///     A <see cref="StumpsServerInstance" /> represeting the new Stumps server.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="remoteServerHostName"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="port"/> exceeds the allowed TCP port range.</exception>
        /// <exception cref="StumpsNetworkException">The port is already in use.</exception>
        public StumpsServerInstance CreateServerInstance(string remoteServerHostName, int port, bool useSsl, bool autoStart)
        {
            if (string.IsNullOrWhiteSpace(remoteServerHostName))
            {
                throw new ArgumentNullException(nameof(remoteServerHostName));
            }

            if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            if (NetworkInformation.IsPortBeingUsed(port))
            {
                throw new StumpsNetworkException(Resources.PortIsInUseError);
            }

            // Create a new data entity representing the Stumps server
            var serverEntity = new ServerEntity
            {
                AutoStart = autoStart,
                RemoteServerHostName = remoteServerHostName,
                Port = port,
                UseSsl = useSsl,
                ServerId = RandomGenerator.GenerateIdentifier()
            };

            // Persist the server data entity
            _dataAccess.ServerCreate(serverEntity);

            // Create a new service instance from the entity
            UnwrapAndRegisterServer(serverEntity);

            var server = _serverInstances[serverEntity.ServerId];

            if (autoStart)
            {
                server.Start();
            }

            return server;
        }

        /// <summary>
        ///     Deletes an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void DeleteServerInstance(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            if (_serverInstances.ContainsKey(serverId))
            {
                _serverInstances[serverId].Shutdown();
                _serverInstances[serverId].Dispose();

                _serverInstances.TryRemove(serverId, out StumpsServerInstance server);

                _dataAccess.ServerDelete(serverId);
            }
        }

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///     Finds all Stumps servers hosted by the current instance.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="StumpsServerInstance"/> objects.
        /// </returns>
        public IList<StumpsServerInstance> FindAll()
        {
            var pairs = _serverInstances.ToArray();
            return pairs.Select(pair => pair.Value).ToList();
        }

        /// <summary>
        ///     Finds the Stumps server with the specified identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>
        ///     A <see cref="StumpsServerInstance" /> with the specified identifier.
        /// </returns>
        /// <remarks>
        ///     A <c>null</c> value is returned if a Stumps server with the specified <paramref name="serverId"/>
        ///     is not found.
        /// </remarks>
        public StumpsServerInstance FindServer(string serverId)
        {
            _serverInstances.TryGetValue(serverId, out StumpsServerInstance server);
            return server;
        }

        /// <summary>
        ///     Loads all Stumps servers from the data store.
        /// </summary>
        public void Load()
        {
            var serverEntities = _dataAccess.ServerFindAll();

            foreach (var serverEntity in serverEntities)
            {
                UnwrapAndRegisterServer(serverEntity);
            }
        }

        /// <summary>
        ///     Shuts down this instance and all started Stumps servers.
        /// </summary>
        public void Shutdown()
        {
            foreach (var keyPair in _serverInstances)
            {
                keyPair.Value.Shutdown();
            }
        }

        /// <summary>
        ///     Shut down the specified Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void Shutdown(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            _serverInstances.TryGetValue(serverId, out StumpsServerInstance server);

            if (server != null)
            {
                server.Shutdown();
            }
        }

        /// <summary>
        ///     Starts all Stumps servers that are not currently running.
        /// </summary>
        public void Start()
        {
            foreach (var server in _serverInstances)
            {
                if (server.Value.AutoStart)
                {
                    server.Value.Start();
                }
            }
        }

        /// <summary>
        ///     Starts the Stumps server with the specified unique identifier.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void Start(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            _serverInstances.TryGetValue(serverId, out StumpsServerInstance server);

            if (server != null)
            {
                server.Start();
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var keyPair in _serverInstances)
            {
                if (keyPair.Value != null)
                {
                    keyPair.Value.Dispose();
                }
            }

            _serverInstances.Clear();
        }

        /// <summary>
        ///     Creates a new Stumps server from a <see cref="ServerEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ServerEntity"/> used to create the Stumps server.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Object is disposed later.")]
        private void UnwrapAndRegisterServer(ServerEntity entity)
        {
            var server = new StumpsServerInstance(_serverFactory, entity.ServerId, _dataAccess)
            {
                AutoStart = entity.AutoStart,
                ListeningPort = entity.Port,
                RemoteServerHostName = entity.RemoteServerHostName,
                UseHttpsForIncomingConnections = entity.UseHttpsForIncomingConnections,
                UseSsl = entity.UseSsl
            };

            _serverInstances.AddOrUpdate(server.ServerId, server, (key, oldServer) => server);
        }
    }
}