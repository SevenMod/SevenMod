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
            Version = "0.1.0.0",
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

            var groups = new Dictionary<int, GroupInfo>();
            var results = db.TQuery("SELECT id, name, flags, immunity_level FROM sm_groups");
            foreach (DataRow row in results.Rows)
            {
                if (!int.TryParse(row.ItemArray.GetValue(0).ToString(), out int id))
                {
                    continue;
                }

                var name = row.ItemArray.GetValue(1).ToString();
                if (string.IsNullOrEmpty(name))
                {
                    continue;
                }

                var flags = row.ItemArray.GetValue(2).ToString();

                var immunity = 1;
                int.TryParse(row.ItemArray.GetValue(3).ToString(), out immunity);

                groups.Add(id, new GroupInfo(name, immunity, flags));
            }

            var adminGroups = new Dictionary<int, List<GroupInfo>>();
            results = db.TQuery("SELECT admin_id, group_id FROM sm_admins_groups");
            foreach (DataRow row in results.Rows)
            {
                if (int.TryParse(row.ItemArray.GetValue(0).ToString(), out int adminId) && int.TryParse(row.ItemArray.GetValue(1).ToString(), out int groupId))
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

            results = db.TQuery("SELECT id, identity, flags, immunity FROM sm_admins");
            foreach (DataRow row in results.Rows)
            {
                if (!int.TryParse(row.ItemArray.GetValue(0).ToString(), out int id))
                {
                    continue;
                }

                var identity = row.ItemArray.GetValue(1).ToString();
                var flags = row.ItemArray.GetValue(2).ToString();
                var immunity = 1;
                int.TryParse(row.ItemArray.GetValue(3).ToString(), out immunity);

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
