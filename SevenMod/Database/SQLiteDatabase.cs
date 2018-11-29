// <copyright file="SQLiteDatabase.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System.Data;
    using System.Data.SQLite;
    using System.IO;
    using MySql.Data.MySqlClient;
    using SevenMod.Core;

    /// <summary>
    /// Represents a connection to a SQLite database.
    /// </summary>
    public class SQLiteDatabase : Database
    {
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
            var connString = new SQLiteConnectionStringBuilder
            {
                DataSource = $"{SMPath.Databases}{connection.Database}.sq3",
                Version = 3,
            };
            this.connection = new SQLiteConnection(connString.ToString());
        }
    }
}
