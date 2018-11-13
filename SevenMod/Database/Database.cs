// <copyright file="Database.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Xml;
    using SevenMod.Core;

    /// <summary>
    /// Represents a connection to a database.
    /// </summary>
    public abstract class Database
    {
        /// <summary>
        /// The available database connection configurations.
        /// </summary>
        private static readonly Dictionary<string, ConnectionInfo> Connections = new Dictionary<string, ConnectionInfo>();

        /// <summary>
        /// Whether the database configuration file has been read.
        /// </summary>
        private static bool configLoaded;

        /// <summary>
        /// Represents a database driver.
        /// </summary>
        protected enum DatabaseDriver
        {
            /// <summary>
            /// Represents the SQLite driver.
            /// </summary>
            SQLite,

            /// <summary>
            /// Represents the MySQL driver.
            /// </summary>
            MySQL,
        }

        /// <summary>
        /// Creates a new instance of a <see cref="Database"/> class representing a connection to a
        /// database defined by the named configuration.
        /// </summary>
        /// <param name="connectionName">The name of the database configuration.</param>
        /// <returns>An instance of a <see cref="Database"/> class representing a connection to the
        /// database.</returns>
        public static Database Connect(string connectionName)
        {
            if (!configLoaded)
            {
                ParseConfig();
                configLoaded = true;
            }

            if (Connections.ContainsKey(connectionName))
            {
                Database conn;
                switch (Connections[connectionName].Driver)
                {
                    case DatabaseDriver.MySQL:
                        conn = new MySqlDatabase();
                        break;
                    case DatabaseDriver.SQLite:
                        conn = new SQLiteDatabase();
                        break;
                    default:
                        return null;
                }

                try
                {
                    conn.Setup(Connections[connectionName]);
                    return conn;
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            return null;
        }

        /// <summary>
        /// Check if a database connection configuration exists.
        /// </summary>
        /// <param name="name">The name of the database connection configuration.</param>
        /// <returns><c>true</c> if the configuration exists; <c>false</c> otherwise.</returns>
        public static bool ConfigExists(string name)
        {
            if (!configLoaded)
            {
                ParseConfig();
                configLoaded = true;
            }

            return Connections.ContainsKey(name);
        }

        /// <summary>
        /// Executes a query on the database without returning a result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        public abstract void FastQuery(string sql);

        /// <summary>
        /// Executes a query on the database and returns the result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <returns>A <see cref="DataTable"/> object containing the result of the query.</returns>
        public abstract DataTable TQuery(string sql);

        /// <summary>
        /// Escapes a string to be safely used in a query.
        /// </summary>
        /// <param name="str">The original unescaped string.</param>
        /// <returns>The escaped string.</returns>
        public abstract string Escape(string str);

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
            var xml = new XmlDocument();
            try
            {
                xml.Load($"{SMPath.Config}Databases.xml");
            }
            catch (XmlException)
            {
                Log.Error("[SevenMod] Failed to load Datebases.xml");
                return;
            }

            var defaultDriver = DatabaseDriver.SQLite;
            var defaultDriverElements = xml.GetElementsByTagName("DefaultDriver");
            if ((defaultDriverElements.Count > 0) && "mysql".EqualsCaseInsensitive(defaultDriverElements[0].InnerText))
            {
                defaultDriver = DatabaseDriver.MySQL;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("Connection"))
            {
                if (!element.HasAttribute("Name"))
                {
                    continue;
                }

                var name = element.GetAttribute("Name");
                if (Connections.ContainsKey(name))
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
                    else if ("mysql".EqualsCaseInsensitive(driverElements[0].InnerText))
                    {
                        driver = DatabaseDriver.MySQL;
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

                if (driver == DatabaseDriver.MySQL)
                {
                    var hostElements = element.GetElementsByTagName("Host");
                    if (hostElements.Count == 0)
                    {
                        continue;
                    }

                    var host = hostElements[0].InnerText;

                    var userElements = element.GetElementsByTagName("User");
                    if (userElements.Count == 0)
                    {
                        continue;
                    }

                    var user = hostElements[0].InnerText;

                    var pass = string.Empty;
                    var passElements = element.GetElementsByTagName("Pass");
                    if (passElements.Count > 0)
                    {
                        pass = passElements[0].InnerText;
                    }

                    var port = 3306;
                    var portElements = element.GetElementsByTagName("Port");
                    if (passElements.Count > 0)
                    {
                        int.TryParse(portElements[0].InnerText, out port);
                    }

                    var connection = new ConnectionInfo(DatabaseDriver.MySQL, database, host, user, pass, port);
                    Connections.Add(name, connection);
                }
                else
                {
                    var connection = new ConnectionInfo(DatabaseDriver.SQLite, database);
                    Connections.Add(name, connection);
                }
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
            /// <param name="driver">The <see cref="DatabaseDriver"/> value representing the
            /// database driver to use.</param>
            /// <param name="database">The name of the database.</param>
            public ConnectionInfo(DatabaseDriver driver, string database)
                : this(driver, database, string.Empty, string.Empty, string.Empty, 0)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="ConnectionInfo"/> struct.
            /// </summary>
            /// <param name="driver">The <see cref="DatabaseDriver"/> value representing the
            /// database driver to use.</param>
            /// <param name="database">The name of the database.</param>
            /// <param name="host">The address of the database host.</param>
            /// <param name="user">The name of the database user to use.</param>
            /// <param name="pass">The password for the database user.</param>
            /// <param name="port">The port for the database server.</param>
            public ConnectionInfo(DatabaseDriver driver, string database, string host, string user, string pass, int port = 3306)
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
            public int Port { get; }
        }
    }
}
