// <copyright file="ThreadedQuery.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;
    using System.Data;
    using System.Threading;
    using SevenMod.Core;
    using SevenMod.Logging;

    /// <summary>
    /// Represents a threaded database query.
    /// </summary>
    public class ThreadedQuery
    {
        /// <summary>
        /// The <see cref="Database"/> object that created this query.
        /// </summary>
        private readonly Database database;

        /// <summary>
        /// The data associated with this query.
        /// </summary>
        private readonly object data;

        /// <summary>
        /// The number of rows affected by this query.
        /// </summary>
        private int affectedRows;

        /// <summary>
        /// The <see cref="DataTable"/> object containing the results returned by this query.
        /// </summary>
        private DataTable results;

        /// <summary>
        /// The exception thrown by the query.
        /// </summary>
        private Exception exception;

        /// <summary>
        /// The current status of this query.
        /// </summary>
        private Status status = Status.Pending;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedQuery"/> class.
        /// </summary>
        /// <param name="database">The <see cref="Database"/> object creating this query.</param>
        /// <param name="data">The data associated with this query.</param>
        private ThreadedQuery(Database database, object data)
        {
            this.database = database;
            this.data = data;
        }

        /// <summary>
        /// Occurs when this query has completed.
        /// </summary>
        public event EventHandler<QueryCompletedEventArgs> QueryCompleted;

        /// <summary>
        /// Enumeration of query statuses.
        /// </summary>
        private enum Status
        {
            /// <summary>
            /// The status for a query that has not yet started.
            /// </summary>
            Pending,

            /// <summary>
            /// The status for a query that is currently running in the background.
            /// </summary>
            Running,

            /// <summary>
            /// The status for a query that has completed and is waiting for its results to be published.
            /// </summary>
            Completed,

            /// <summary>
            /// The status for a query whose results have been published.
            /// </summary>
            Published,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedQuery"/> class.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="database">The <see cref="Database"/> object creating this query.</param>
        /// <param name="data">The data associated with this query.</param>
        /// <returns>A new instance of the <see cref="ThreadedQuery"/> class.</returns>
        internal static ThreadedQuery Query(string sql, Database database, object data)
        {
            var query = new ThreadedQuery(database, data);
            new Thread(() =>
            {
                lock (database)
                {
                    query.status = Status.Running;
                    try
                    {
                        query.results = database.RunQuery(sql);
                    }
                    catch (Exception e)
                    {
                        query.exception = e;
                    }

                    query.status = Status.Completed;
                }
            }).Start();

            return query;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadedQuery"/> class for a query that does not return a result.
        /// </summary>
        /// <param name="sql">The SQL string to execute.</param>
        /// <param name="database">The <see cref="Database"/> object creating this query.</param>
        /// <param name="data">The data associated with this query.</param>
        /// <returns>A new instance of the <see cref="ThreadedQuery"/> class.</returns>
        internal static ThreadedQuery FastQuery(string sql, Database database, object data)
        {
            var query = new ThreadedQuery(database, data);
            new Thread(() =>
            {
                lock (database)
                {
                    query.status = Status.Running;
                    try
                    {
                        query.affectedRows = database.RunFastQuery(sql);
                    }
                    catch (Exception e)
                    {
                        query.exception = e;
                    }

                    query.status = Status.Completed;
                }
            }).Start();

            return query;
        }

        /// <summary>
        /// Checks the status of this query and publishes the result if it has completed.
        /// </summary>
        /// <returns><c>true</c> if this query has completed; otherwise <c>false</c>.</returns>
        internal bool CheckStatus()
        {
            if (this.status == Status.Completed)
            {
                if (this.QueryCompleted != null)
                {
                    var args = new QueryCompletedEventArgs(this.database, this.affectedRows, this.results, this.data, this.exception);
                    foreach (EventHandler<QueryCompletedEventArgs> d in this.QueryCompleted.GetInvocationList())
                    {
                        try
                        {
                            d.Invoke(this, args);
                        }
                        catch (HaltPluginException)
                        {
                        }
                        catch (Exception e)
                        {
                            if (d.Target is IPlugin p)
                            {
                                p.Container.SetFailState(e);
                            }
                            else
                            {
                                SMLog.Error(e);
                            }
                        }
                    }
                }

                this.status = Status.Published;
            }

            return this.status == Status.Published;
        }
    }
}
