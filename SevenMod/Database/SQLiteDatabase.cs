// <copyright file="SQLiteDatabase.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using System.Reflection;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Represents a connection to a SQLite database.
    /// </summary>
    public class SQLiteDatabase : Database
    {
        /// <summary>
        /// The path to the directory where database files are stored.
        /// </summary>
        private static readonly string ConfigPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Data{Path.DirectorySeparatorChar}Databases{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The backing SQLite database connection.
        /// </summary>
        private SQLiteConnection connection;

        /// <inheritdoc/>
        public override void FastQuery(string sql)
        {
            this.connection.Open();
            var cmd = new SQLiteCommand(sql, this.connection);
            cmd.ExecuteNonQuery();
            this.connection.Close();
        }

        /// <inheritdoc/>
        public override DataTable TQuery(string sql)
        {
            var dataTable = new DataTable();

            this.connection.Open();
            var cmd = new SQLiteCommand(sql, this.connection);
            var reader = cmd.ExecuteReader();
            dataTable.Load(reader);
            reader.Close();
            this.connection.Close();

            return dataTable;
        }

        /// <inheritdoc/>
        public override string Escape(string str)
        {
            return MySqlHelper.EscapeString(str);
        }

        /// <inheritdoc/>
        protected override void Setup(ConnectionInfo connection)
        {
            var filePath = $"{ConfigPath}{connection.Database}";
            var connString = $"Data Source={filePath};Version=3;New={!File.Exists(filePath)};Compress=True;";
            this.connection = new SQLiteConnection(connString);
        }
    }
}
