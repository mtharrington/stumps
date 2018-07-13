﻿namespace Stumps.Server.Data
{
    using System.Collections.Generic;

    /// <summary>
    ///     An interface that represents access to a data store used to persist information about Stumps servers and stump instances.
    /// </summary>
    public interface IDataAccess
    {
        /// <summary>
        ///     Creates an entry for a new stumps server.
        /// </summary>
        /// <param name="server">The <see cref="ServerEntity" /> to create.</param>
        void ServerCreate(ServerEntity server);

        /// <summary>
        ///     Deletes an existing <see cref="ServerEntity" />.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="ServerEntity" /> to delete.</param>
        void ServerDelete(string serverId);

        /// <summary>
        ///     Finds the persisted <see cref="ServerEntity" /> for a specified <paramref name="serverId"/>.
        /// </summary>
        /// <param name="serverId">The unique identifier for the <see cref="ServerEntity" /> to find.</param>
        /// <returns>The <see cref="ServerEntity"/> with the specified <paramref name="serverId"/>.</returns>
        ServerEntity ServerFind(string serverId);

        /// <summary>
        ///     Finds a list of all persisted <see cref="ServerEntity" />.
        /// </summary>
        /// <returns>A generic list of <see cref="ServerEntity" /> objects.</returns>
        IList<ServerEntity> ServerFindAll();

        /// <summary>
        ///     Loads the contents of a resource for a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="resourceName">Name of the file.</param>
        /// <returns>A byte array containing the contents of the resource.</returns>
        /// <remarks>A <c>null</c> value is returned if the resource cannot be found.</remarks>
        byte[] ServerReadResource(string serverId, string resourceName);
 
        /// <summary>
        ///     Creates a new <see cref="StumpEntity"/> for an existing Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <param name="entity">The <see cref="StumpEntity"/> to persist.</param>
        /// <param name="originalRequestBody">The array of bytes representing the original request's HTTP body.</param>
        /// <param name="originalResponseBody">The array of bytes representing the original response's HTTP body.</param>
        /// <param name="responseBody">The array of bytes returned as the HTTP body in response to the stump.</param>
        /// <returns>The created <see cref="StumpEntity"/> object.</returns>
        StumpEntity StumpCreate(string serverId, StumpEntity entity, byte[] originalRequestBody, byte[] originalResponseBody, byte[] responseBody);

        /// <summary>
        ///     Deletes an existing stump from a Stumps server.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server the Stump is located in.</param>
        /// <param name="stumpId">The  unique identifier for the stump to delete.</param>
        void StumpDelete(string serverId, string stumpId);

        /// <summary>
        ///     Finds all a list of all <see cref="StumpEntity"/> for the specified <paramref name="serverId"/>.
        /// </summary>
        /// <param name="serverId">The unique identifier for the Stumps server.</param>
        /// <returns>A generic list of <see cref="StumpEntity"/> objects.</returns>
        IList<StumpEntity> StumpFindAll(string serverId);
    }
}