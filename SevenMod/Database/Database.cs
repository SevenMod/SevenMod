// <copyright file="Database.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Security.Permissions;
    using System.Xml;
    using SevenMod.Core;
    using SevenMod.Logging;

    /// <summary>
    /// Represents a connection to a database.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    public abstract class Database : IDisposable
    {
        /// <summary>
        /// The path to the database configuration file.
        /// </summary>
        private static readonly string ConfigPath = $"{SMPath.Config}Databases.xml";

        /// <summary>
        /// The available database connection configurations.
        /// </summary>
        private static Dictionary<string, ConnectionInfo> connections = new Dictionary<string, ConnectionInfo>();

        /// <summary>
        /// A value indicating whether the database configuration file has been read.
        /// </summary>
        private static bool configLoaded;

        /// <summary>
        /// The watcher for changes to the database configuration file.
        /// </summary>
        private static FileSystemWatcher watcher;

        /// <summary>
        /// The <see cref="ThreadedQuery"/> objects representing the currently active threaded queries.
        /// </summary>
        private static List<ThreadedQuery> queries = new List<ThreadedQuery>();

        /// <summary>
        /// Initializes static members of the <see cref="Database"/> class.
        /// </summary>
        static Database()
        {
            ModEvents.GameUpdate.RegisterHandler(() =>
            {
                queries.RemoveAll((ThreadedQuery query) => query.CheckStatus());
            });
        }

        /// <summary>
        /// Represents a database driver.
        /// </summary>
        protected enum DatabaseDriver
        {
            /// <summary>
            /// Represents the SQLite driver.
            /// </summary>
            SQLite,
        }

        /// <summary>
        /// Creates a new instance of a <see cref="Database"/> class representing a connection to a database defined by the named configuration.
        /// </summary>
        /// <param name="connectionName">The name of the database configuration.</param>
        /// <returns>An instance of a <see cref="Database"/> class representing a connection to the database.</returns>
        public static Database Connect(string connectionName)
        {
            if (!configLoaded)
            {
                ParseConfig();
                configLoaded = true;
            }

            if (connections.ContainsKey(connectionName))
            {
                Database conn;
                switch (connections[connectionName].Driver)
                {
                    case DatabaseDriver.SQLite:
                    default:
                        conn = new SQLiteDatabase();
                        break;
                }

                conn.Setup(connections[connectionName]);
                return conn;
            }

            throw new DatabaseConfigException(connectionName);
        }

        /// <summary>
        /// Opens a SQLite database.
        /// </summary>
        /// <param name="name">The name of the database file without extension.</param>
        /// <returns>An instance of a <see cref="SQLiteDatabase"/> class representing the database.</returns>
        public static SQLiteDatabase OpenSQLiteDatabase(string name)
        {
            var conn = new SQLiteDatabase();
            conn.Setup(new ConnectionInfo(DatabaseDriver.SQLite, name));
            return conn;
        }

        /// <summary>
        /// Check if a database connection configuration exists.
        /// </summary>
        /// <param name="name">The name of the database connection configuration.</param>
        /// <returns><c>true</c> if the configuration exists; otherwise <c>false</c>.</returns>
        public static bool ConfigExists(string name)
        {
            if (!configLoaded)
            {
                ParseConfig();
                configLoaded = true;
            }

            return connections.ContainsKey(name);
        }

        /// <summary>
        /// Executes a query on the database and returns the result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <returns>A <see cref="DataTable"/> object containing the result of the query.</returns>
        public DataTable Query(string sql, Dictionary<string, object> parameters)
        {
            lock (this)
            {
                return this.RunQuery(sql, parameters);
            }
        }

        /// <summary>
        /// Executes a query on the database without returning a result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <returns>The number of affected rows.</returns>
        public int FastQuery(string sql, Dictionary<string, object> parameters)
        {
            lock (this)
            {
                return this.RunFastQuery(sql, parameters);
            }
        }

        /// <summary>
        /// Executes a threaded query on the database and returns the result to a callback function.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <param name="data">Data associated with this query.</param>
        /// <returns>A <see cref="ThreadedQuery"/> object representing the query running in the background.</returns>
        public ThreadedQuery TQuery(string sql, Dictionary<string, object> parameters = null, object data = null)
        {
            var query = ThreadedQuery.Query(sql, parameters, this, data);
            queries.Add(query);

            return query;
        }

        /// <summary>
        /// Executes a query on the database without returning a result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <param name="data">Data associated with this query.</param>
        /// <returns>A <see cref="ThreadedQuery"/> object representing the query running in the background.</returns>
        public ThreadedQuery TFastQuery(string sql, Dictionary<string, object> parameters = null, object data = null)
        {
            var query = ThreadedQuery.FastQuery(sql, parameters, this, data);
            queries.Add(query);

            return query;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Implemented as needed")]
        public virtual void Dispose()
        {
        }

        /// <summary>
        /// Executes a query on the database and returns the result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <returns>A <see cref="DataTable"/> object containing the result of the query.</returns>
        protected internal abstract DataTable RunQuery(string sql, Dictionary<string, object> parameters);

        /// <summary>
        /// Executes a query on the database without returning a result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="parameters">The map of parameter names to values.</param>
        /// <returns>The number of affected rows.</returns>
        protected internal abstract int RunFastQuery(string sql, Dictionary<string, object> parameters);

        /// <summary>
        /// Sets up the connection to the backing database.
        /// </summary>
        /// <param name="connection">The database connection configuration.</param>
        protected abstract void Setup(ConnectionInfo connection);

        /// <summary>
        /// Parses the database configuration file and loads the list of connection configurations.
        /// </summary>
        private static void ParseConfig()
        {
            connections.Clear();

            if (!File.Exists(ConfigPath))
            {
                CreateConfig();
            }

            var xml = new XmlDocument();
            try
            {
                xml.Load(ConfigPath);
            }
            catch (XmlException e)
            {
                SMLog.Error($"Failed reading database configuration from {ConfigPath}: {e.Message}");
                return;
            }

            var defaultDriver = DatabaseDriver.SQLite;

            foreach (XmlElement element in xml.GetElementsByTagName("Connection"))
            {
                if (!element.HasAttribute("Name"))
                {
                    continue;
                }

                var name = element.GetAttribute("Name");
                if (connections.ContainsKey(name))
                {
                    continue;
                }

                var driver = defaultDriver;
                var driverElements = element.GetElementsByTagName("Driver");
                if (driverElements.Count > 0)
                {
                    if ("sqlite".EqualsCaseInsensitive(driverElements[0].InnerText))
                    {
                        driver = DatabaseDriver.SQLite;
                    }
                    else
                    {
                        continue;
                    }
                }

                var databaseElements = element.GetElementsByTagName("Database");
                if (databaseElements.Count == 0)
                {
                    continue;
                }

                var database = databaseElements[0].InnerText;

                var connection = new ConnectionInfo(DatabaseDriver.SQLite, database);
                connections.Add(name, connection);
            }

            if (watcher == null)
            {
                watcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigPath), Path.GetFileName(ConfigPath))
                {
                    NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
                };
                watcher.Changed += OnConfigFileChanged;
                watcher.Deleted += OnConfigFileChanged;
                watcher.Renamed += OnConfigFileChanged;
                watcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="watcher"/> when the database configuration file changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> object containing the event data.</param>
        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            ParseConfig();
        }

        /// <summary>
        /// Creates the default database configuration file.
        /// </summary>
        private static void CreateConfig()
        {
            using (var writer = XmlWriter.Create(ConfigPath))
            {
                writer.WriteWhitespace("\r\n");
                writer.WriteStartElement("Databases");
                writer.WriteWhitespace("\r\n  ");
                writer.WriteElementString("DefaultDriver", "sqlite");
                writer.WriteWhitespace("\r\n\r\n  ");

                writer.WriteStartElement("Connection");
                writer.WriteAttributeString("Name", "storage-local");
                writer.WriteWhitespace("\r\n    ");
                writer.WriteElementString("Driver", "sqlite");
                writer.WriteWhitespace("\r\n    ");
                writer.WriteElementString("Database", "storage-local");
                writer.WriteWhitespace("\r\n  ");
                writer.WriteEndElement();
                writer.WriteWhitespace("\r\n\r\n  ");

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Represents a database connection configuration.
        /// </summary>
        protected struct ConnectionInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionInfo"/> struct.
            /// </summary>
            /// <param name="driver">The <see cref="DatabaseDriver"/> value representing the database driver to use.</param>
            /// <param name="database">The name of the database.</param>
            public ConnectionInfo(DatabaseDriver driver, string database)
                : this(driver, database, string.Empty, string.Empty, string.Empty, 0)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionInfo"/> struct.
            /// </summary>
            /// <param name="driver">The <see cref="DatabaseDriver"/> value representing the database driver to use.</param>
            /// <param name="database">The name of the database.</param>
            /// <param name="host">The address of the database host.</param>
            /// <param name="user">The name of the database user to use.</param>
            /// <param name="pass">The password for the database user.</param>
            /// <param name="port">The port for the database server.</param>
            public ConnectionInfo(DatabaseDriver driver, string database, string host, string user, string pass, uint port = 3306)
            {
                this.Driver = driver;
                this.Database = database;
                this.Host = host;
                this.User = user;
                this.Pass = pass;
                this.Port = port;
            }

            /// <summary>
            /// Gets the <see cref="DatabaseDriver"/> value representing the database driver.
            /// </summary>
            public DatabaseDriver Driver { get; }

            /// <summary>
            /// Gets the name of the database.
            /// </summary>
            public string Database { get; }

            /// <summary>
            /// Gets the address of the database host.
            /// </summary>
            public string Host { get; }

            /// <summary>
            /// Gets the name of the database user.
            /// </summary>
            public string User { get; }

            /// <summary>
            /// Gets the password for the database user.
            /// </summary>
            public string Pass { get; }

            /// <summary>
            /// Gets the port for the database server.
            /// </summary>
            public uint Port { get; }
        }
    }
}
