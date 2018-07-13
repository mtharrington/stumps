﻿namespace Stumps.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Stumps.Server.Utility;

    /// <summary>
    ///     A class that provides an implementation of <see cref="IDataAccess"/>
    ///     that uses JSON files and directory structures to persist information about Stumps servers and Stump instances.
    /// </summary>
    public sealed class DataAccess : IDataAccess
    {
        /// <summary>
        ///     The file extension used to for files that contain the body of the original HTTP request.
        /// </summary>
        public const string OriginalRequestBodyFileExtension = ".orequest.body";

        /// <summary>
        ///     The file extension used to for files that contain the body of the original HTTP response.
        /// </summary>
        public const string OriginalResponseBodyFileExtension = ".oresponse.body";

        /// <summary>
        ///     The file extension used for files that contain the body used to in response to an HTTP request.
        /// </summary>
        public const string ResponseBodyFileExtension = ".response.body";

        /// <summary>
        ///     The file extension used for stumps configuration files. 
        /// </summary>
        public const string StumpFileExtension = ".stump";

        /// <summary>
        ///     The path used to persist stumps for a Stumps server.
        /// </summary>
        public const string StumpsPathName = "stumps";

        /// <summary>
        ///     The file extension used for Stumps server configuration files. 
        /// </summary>
        public const string StumpsServerFileExtension = ".server";

        private readonly string _storagePath;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataAccess"/> class.
        /// </summary>
        /// <param name="storagePath">The data path.</param>
        /// <exception cref="ArgumentNullException"><paramref name="storagePath"/> is <c>null</c>.</exception>
        public DataAccess(string storagePath)
        {
            _storagePath = storagePath ?? throw new ArgumentNullException(nameof(storagePath));
        }

        /// <summary>
        ///     Gets or sets the path used to access the data store.
        /// </summary>
        /// <value>
        ///     The path used to access the data store.
        /// </value>
        public string StoragePath
        {
            get => _storagePath;
        }

        /// <summary>
        ///     Creates an entry for a new stumps server.
        /// </summary>
        /// <param name="server">The <see cref="ServerEntity" /> to create.</param>
        /// <exception cref="ArgumentNullException">server</exception>
        public void ServerCreate(ServerEntity server)
        {
            server = server ?? throw new ArgumentNullException(nameof(server));

            var serverFile = Path.Combine(_storagePath, server.ServerId + DataAccess.StumpsServerFileExtension);
            JsonUtility.SerializeToFile(server, serverFile);

            Directory.CreateDirectory(Path.Combine(_storagePath, server.ServerId));
            Directory.CreateDirectory(Path.Combine(_storagePath, server.ServerId, DataAccess.StumpsPathName));
        }

        /// <summary>
        ///     Deletes an existing <see cref="ServerEntity" />.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="ServerEntity" /> to delete.</param>
        /// <exception cref="ArgumentNullException"><paramref name="serverId"/> is <c>null</c>.</exception>
        public void ServerDelete(string serverId)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            var serverFile = Path.Combine(_storagePath, serverId + DataAccess.StumpsServerFileExtension);
            File.Delete(serverFile);

            var serverPath = Path.Combine(_storagePath, serverId);
            Directory.Delete(serverPath, true);
        }

        /// <summary>
        ///     Finds the persisted <see cref="ServerEntity" /> for a specified <paramref name="serverId" />.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="ServerEntity" /> to find.</param>
        /// <returns>
        ///     The <see cref="ServerEntity" /> with the specified <paramref name="serverId" />.
        /// </returns>
        public ServerEntity ServerFind(string serverId)
        {
            var path = Path.Combine(_storagePath, serverId + DataAccess.StumpsServerFileExtension);
            var entity = JsonUtility.DeserializeFromFile<ServerEntity>(path);
            return entity;
        }

        /// <summary>
        ///     Finds a list of all persisted <see cref="ServerEntity" />.
        /// </summary>
        /// <returns>
        ///     A generic list of <see cref="ServerEntity" /> objects.
        /// </returns>
        public IList<ServerEntity> ServerFindAll()
        {
            var serverEntities = JsonUtility.DeserializeFromDirectory<ServerEntity>(
                _storagePath, "*" + DataAccess.StumpsServerFileExtension, SearchOption.TopDirectoryOnly);

            return serverEntities;
        }

        /// <summary>
        ///     Loads the contents of a resource for a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="resourceName">Name of the file.</param>
        /// <returns>
        ///     A byte array containing the contents of the resource.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="serverId"/> is <c>null</c>.
        /// </exception>
        /// <remarks>
        ///     A <c>null</c> value is returned if the resource cannot be found.
        /// </remarks>
        public byte[] ServerReadResource(string serverId, string resourceName)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            resourceName = resourceName ?? string.Empty;

            byte[] fileBytes = null;

            var path = Path.Combine(_storagePath, serverId, DataAccess.StumpsPathName, resourceName);
            if (File.Exists(path))
            {
                fileBytes = File.ReadAllBytes(path);
            }

            return fileBytes;
        }

        /// <summary>
        ///     Creates a new <see cref="StumpEntity" /> for an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="entity">The <see cref="StumpEntity" /> to persist.</param>
        /// <param name="originalRequestBody">The array of bytes representing the original request's HTTP body.</param>
        /// <param name="originalResponseBody">The array of bytes representing the original response's HTTP body.</param>
        /// <param name="responseBody">The array of bytes returned as the HTTP body in response to the stump.</param>
        /// <returns>
        ///     The created <see cref="StumpEntity" /> object.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="serverId"/> is <c>null</c>.
        /// or
        /// <paramref name="entity"/> is <c>null</c>.
        /// </exception>
        public StumpEntity StumpCreate(string serverId, StumpEntity entity, byte[] originalRequestBody, byte[] originalResponseBody, byte[] responseBody)
        {
            if (string.IsNullOrWhiteSpace(serverId))
            {
                throw new ArgumentNullException(nameof(serverId));
            }

            entity = entity ?? throw new ArgumentNullException(nameof(entity));

            var stumpsPath = Path.Combine(_storagePath, serverId, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, entity.StumpId + DataAccess.StumpFileExtension);
            var originalRequestFileName = entity.StumpId + DataAccess.OriginalRequestBodyFileExtension;
            var originalResponseFileName = entity.StumpId + DataAccess.OriginalResponseBodyFileExtension;
            var responseFileName = entity.StumpId + DataAccess.ResponseBodyFileExtension;

            if (originalRequestBody != null && originalRequestBody.Length > 0)
            {
                entity.OriginalRequest.BodyResourceName = originalRequestFileName;

                var file = Path.Combine(stumpsPath, originalRequestFileName);
                File.WriteAllBytes(file, originalRequestBody);
            }

            if (originalResponseBody != null && originalResponseBody.Length > 0)
            {
                entity.OriginalResponse.BodyResourceName = originalResponseFileName;

                var file = Path.Combine(stumpsPath, originalResponseFileName);
                File.WriteAllBytes(file, originalResponseBody);
            }

            if (responseBody != null && responseBody.Length > 0)
            {
                entity.Response.BodyResourceName = responseFileName;

                var file = Path.Combine(stumpsPath, responseFileName);
                File.WriteAllBytes(file, responseBody);
            }

            JsonUtility.SerializeToFile(entity, stumpFileName);

            return entity;
        }

        /// <summary>
        ///     Deletes an existing stump from a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server the Stump is located in.</param>
        /// <param name="stumpId">The  unique identifier for the stump to delete.</param>
        public void StumpDelete(string serverId, string stumpId)
        {
            var stumpsPath = Path.Combine(_storagePath, serverId, DataAccess.StumpsPathName);

            var stumpFileName = Path.Combine(stumpsPath, stumpId + DataAccess.StumpFileExtension);
            var matchFileName = Path.Combine(stumpsPath, stumpId + DataAccess.OriginalRequestBodyFileExtension);
            var responseFileName = Path.Combine(stumpsPath, stumpId + DataAccess.ResponseBodyFileExtension);

            if (File.Exists(stumpFileName))
            {
                File.Delete(stumpFileName);
            }

            if (File.Exists(matchFileName))
            {
                File.Delete(matchFileName);
            }

            if (File.Exists(responseFileName))
            {
                File.Delete(responseFileName);
            }
        }

        /// <summary>
        ///     Finds all a list of all <see cref="StumpEntity" /> for the specified <paramref name="serverId" />.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>
        ///     A generic list of <see cref="StumpEntity" /> objects.
        /// </returns>
        public IList<StumpEntity> StumpFindAll(string serverId)
        {
            var stumpsPath = Path.Combine(_storagePath, serverId, DataAccess.StumpsPathName);

            var stumpEntities = JsonUtility.DeserializeFromDirectory<StumpEntity>(
                stumpsPath, "*" + DataAccess.StumpFileExtension, SearchOption.TopDirectoryOnly);

            return stumpEntities;
        }
    }
}