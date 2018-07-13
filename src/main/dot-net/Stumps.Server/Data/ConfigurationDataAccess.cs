﻿namespace Stumps.Server.Data
{
    using System;
    using System.IO;
    using Stumps.Server.Utility;

    /// <summary>
    ///     A class that provides an implementation of <see cref="IConfigurationDataAccess"/>
    ///     that uses a JSON file to configuration information.
    /// </summary>
    public class ConfigurationDataAccess : IConfigurationDataAccess
    {
        private readonly string _configurationFile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDataAccess"/> class.
        /// </summary>
        /// <param name="configurationFile">The path to the file containing configuration information in JSON format.</param>
        /// <exception cref="ArgumentNullException"><paramref name="configurationFile"/> is <c>null</c>.</exception>
        public ConfigurationDataAccess(string configurationFile)
        {
            _configurationFile = configurationFile ?? throw new ArgumentNullException(nameof(configurationFile));
        }

        /// <summary>
        ///     Ensures the configuration is correctly initialized.
        /// </summary>
        /// <param name="configureDefaultsAction">The action to execute after preparing the data access.</param>
        public void EnsureConfigurationIsInitialized(Action configureDefaultsAction)
        {
            if (File.Exists(_configurationFile))
            {
                return;
            }

            var configurationFileDirectory = Path.GetDirectoryName(this._configurationFile);

            configurationFileDirectory = string.IsNullOrEmpty(configurationFileDirectory)
                                             ? "."
                                             : configurationFileDirectory;

            if (!Directory.Exists(configurationFileDirectory))
            {
                Directory.CreateDirectory(configurationFileDirectory);
            }

            configureDefaultsAction?.Invoke();
        }

        /// <summary>
        /// Loads the <see cref="ConfigurationEntity" /> from the data store.
        /// </summary>
        /// <returns>
        /// A <see cref="ConfigurationEntity" /> containing the configuration information for the application.
        /// </returns>
        public ConfigurationEntity LoadConfiguration()
        {
            var loadedConfiguration = JsonUtility.DeserializeFromFile<ConfigurationEntity>(_configurationFile);
            return loadedConfiguration;
        }

        /// <summary>
        /// Persists the specified <see cref="ConfigurationEntity" /> to the data store.
        /// </summary>
        /// <param name="value">The <see cref="ConfigurationEntity" /> to persist in the store.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/> is <c>null</c>.</exception>
        public void SaveConfiguration(ConfigurationEntity value)
        {
            value = value ?? throw new ArgumentNullException(nameof(value));

            JsonUtility.SerializeToFile(value, _configurationFile);
        }
    }
}
