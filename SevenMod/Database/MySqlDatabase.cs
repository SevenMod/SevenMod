// <copyright file="MySqlDatabase.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;
    using System.Data;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Represents a connection to a MySQL database.
    /// </summary>
    public sealed class MySqlDatabase : Database
    {
        /// <summary>
        /// The backing MySQL database connection.
        /// </summary>
        private MySqlConnection connection;

        /// <inheritdoc/>
        public override string Escape(string str)
        {
            return MySqlHelper.EscapeString(str);
        }

        /// <inheritdoc/>
        public sealed override void Dispose()
        {
            ((IDisposable)this.connection).Dispose();
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Sanitized by client")]
        protected internal override DataTable RunQuery(string sql)
        {
            var dataTable = new DataTable();

            this.connection.Open();
            var cmd = new MySqlCommand(sql, this.connection);
            var reader = cmd.ExecuteReader();
            dataTable.Load(reader);
            reader.Close();
            this.connection.Close();

            return dataTable;
        }

        /// <inheritdoc/>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Sanitized by client")]
        protected internal override int RunFastQuery(string sql)
        {
            this.connection.Open();
            var cmd = new MySqlCommand(sql, this.connection);
            var affectedRows = cmd.ExecuteNonQuery();
            this.connection.Close();

            return affectedRows;
        }

        /// <inheritdoc/>
        protected override void Setup(ConnectionInfo connection)
        {
            var connString = new MySqlConnectionStringBuilder
            {
                Server = connection.Host,
                Port = connection.Port,
                Database = connection.Database,
                UserID = connection.User,
                Password = connection.Pass,
                CharacterSet = "utf8mb4",
            };
            this.connection = new MySqlConnection(connString.ToString());
        }
    }
}
