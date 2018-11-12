// <copyright file="MySqlDatabase.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System.Data;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Represents a connection to a MySQL database.
    /// </summary>
    public class MySqlDatabase : Database
    {
        /// <summary>
        /// The backing MySQL database connection.
        /// </summary>
        private MySqlConnection connection;

        /// <inheritdoc/>
        public override void FastQuery(string sql)
        {
            this.connection.Open();
            var cmd = new MySqlCommand(sql, this.connection);
            cmd.ExecuteNonQuery();
            this.connection.Close();
        }

        /// <inheritdoc/>
        public override DataTable TQuery(string sql)
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
        public override string Escape(string str)
        {
            return MySqlHelper.EscapeString(str);
        }

        /// <inheritdoc/>
        protected override void Setup(ConnectionInfo connection)
        {
            var connString = $"SERVER={connection.Host};PORT={connection.Port};DATABASE={connection.Database};UID={connection.User};PASSWORD={connection.Pass};CHARSET=utf8;";
            this.connection = new MySqlConnection(connString);
        }
    }
}
