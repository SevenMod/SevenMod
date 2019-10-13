// <copyright file="AdminSQL.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminSQL
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using SevenMod.Admin;
    using SevenMod.Core;
    using SevenMod.Database;

    /// <summary>
    /// Plugin that loads admin users from a database.
    /// </summary>
    public class AdminSQL : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "SQL Admins",
            Author = "SevenMod",
            Description = "Loads admin users from a database.",
            Version = "0.1.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnReloadAdmins()
        {
            this.LoadAdmins();
        }

        /// <summary>
        /// Loads the admin users from the database.
        /// </summary>
        private void LoadAdmins()
        {
            if (!Database.ConfigExists("admins"))
            {
                this.LogError("The \"admins\" database configuration was not found.");
                return;
            }

            var db = Database.Connect("admins");
            if (db == null)
            {
                this.LogError("Failed to connect to the admins database.");
                return;
            }

            db.TQuery("SELECT id, name, flags, immunity_level FROM sm_groups").QueryCompleted += this.OnGroupsQuery;
        }

        /// <summary>
        /// Called when the query for the list of groups is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="QueryCompletedEventArgs"/> object containing the event data.</param>
        private void OnGroupsQuery(object sender, QueryCompletedEventArgs e)
        {
            var groups = new Dictionary<int, GroupInfo>();
            foreach (DataRow row in e.Results.Rows)
            {
                if (!int.TryParse(row.ItemArray.GetValue(0).ToString(), out var id))
                {
                    continue;
                }

                var name = row.ItemArray.GetValue(1).ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var flags = row.ItemArray.GetValue(2).ToString();
                int.TryParse(row.ItemArray.GetValue(3).ToString(), out var immunity);

                groups.Add(id, new GroupInfo(name, immunity, flags));
            }

            var bundle = new Dictionary<string, object>
            {
                ["groups"] = groups,
            };
            e.Database.TQuery("SELECT admin_id, group_id FROM sm_admins_groups", null, bundle).QueryCompleted += this.OnAdminGroupsQuery;
        }

        /// <summary>
        /// Called when the query for the admin users-to-groups relationships is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="QueryCompletedEventArgs"/> object containing the event data.</param>
        private void OnAdminGroupsQuery(object sender, QueryCompletedEventArgs e)
        {
            var bundle = (Dictionary<string, object>)e.Data;
            var groups = (Dictionary<int, GroupInfo>)bundle["groups"];

            var adminGroups = new Dictionary<int, List<GroupInfo>>();
            foreach (DataRow row in e.Results.Rows)
            {
                if (int.TryParse(row.ItemArray.GetValue(0).ToString(), out var adminId) && int.TryParse(row.ItemArray.GetValue(1).ToString(), out var groupId))
                {
                    if (!groups.ContainsKey(groupId))
                    {
                        continue;
                    }

                    if (!adminGroups.ContainsKey(adminId))
                    {
                        adminGroups.Add(adminId, new List<GroupInfo>());
                    }

                    adminGroups[adminId].Add(groups[groupId]);
                }
            }

            bundle["adminGroups"] = adminGroups;
            e.Database.TQuery("SELECT id, identity, flags, immunity FROM sm_admins", null, bundle).QueryCompleted += this.OnAdminsQuery;
        }

        /// <summary>
        /// Called when the query for the list of admin users is completed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="QueryCompletedEventArgs"/> object containing the event data.</param>
        private void OnAdminsQuery(object sender, QueryCompletedEventArgs e)
        {
            var bundle = (Dictionary<string, object>)e.Data;
            var groups = (Dictionary<int, GroupInfo>)bundle["groups"];
            var adminGroups = (Dictionary<int, List<GroupInfo>>)bundle["adminGroups"];

            foreach (DataRow row in e.Results.Rows)
            {
                if (!int.TryParse(row.ItemArray.GetValue(0).ToString(), out var id))
                {
                    continue;
                }

                var identity = row.ItemArray.GetValue(1).ToString();
                var flags = row.ItemArray.GetValue(2).ToString();
                int.TryParse(row.ItemArray.GetValue(3).ToString(), out var immunity);

                if (adminGroups.ContainsKey(id))
                {
                    foreach (var group in adminGroups[id])
                    {
                        flags += group.Flags;
                        immunity = Math.Max(immunity, group.Immunity);
                    }
                }

                AdminManager.AddAdmin(identity, immunity, flags);
            }
        }
    }
}
