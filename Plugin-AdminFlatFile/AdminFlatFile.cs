// <copyright file="AdminFlatFile.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminFlatFile
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// <para>Plugin: AdminFlatFile</para>
    /// <para>Loads admin users from a local configuration file.</para>
    /// </summary>
    public class AdminFlatFile : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Admin File Reader",
            Author = "SevenMod",
            Description = "Loads admin users from a local configuration file.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();
            this.LoadAdmins();
        }

        /// <summary>
        /// Loads the admin users from the Admins.xml file.
        /// </summary>
        private void LoadAdmins()
        {
            AdminManager.RemoveAllAdmins();
            var xml = new XmlDocument();
            try
            {
                xml.Load($"{ConfigManager.ConfigPath}Admins.xml");
            }
            catch (XmlException)
            {
                Log.Error("[SevenMod] Failed to load Admins.xml");
                return;
            }

            var groups = new Dictionary<string, GroupInfo>();
            var groupsNodes = xml.GetElementsByTagName("Groups");
            if (groupsNodes.Count > 0)
            {
                foreach (XmlElement node in ((XmlElement)groupsNodes[0]).GetElementsByTagName("Group"))
                {
                    if (!node.HasAttribute("Name"))
                    {
                        continue;
                    }

                    var name = node.GetAttribute("Name");
                    if (groups.ContainsKey(name))
                    {
                        continue;
                    }

                    var immunity = 1;
                    if (node.HasAttribute("Immunity"))
                    {
                        int.TryParse(node.GetAttribute("Immunity"), out immunity);
                    }

                    var flags = string.Empty;
                    if (node.HasAttribute("Flags"))
                    {
                        flags = node.GetAttribute("Flags");
                    }

                    groups.Add(name, new GroupInfo(name, immunity, flags));
                }
            }

            var adminsNodes = xml.GetElementsByTagName("Admins");
            if (adminsNodes.Count > 0)
            {
                foreach (XmlElement node in ((XmlElement)adminsNodes[0]).GetElementsByTagName("Admin"))
                {
                    if (!node.HasAttribute("AuthId"))
                    {
                        continue;
                    }

                    var authId = node.GetAttribute("AuthId");

                    var immunity = 1;
                    if (node.HasAttribute("Immunity"))
                    {
                        int.TryParse(node.GetAttribute("Immunity"), out immunity);
                    }

                    var flags = string.Empty;
                    if (node.HasAttribute("Flags"))
                    {
                        flags = node.GetAttribute("Flags");
                    }

                    if (node.HasAttribute("Groups"))
                    {
                        foreach (var groupName in node.GetAttribute("Groups").Split(','))
                        {
                            if (groups.ContainsKey(groupName))
                            {
                                GroupInfo group = groups[groupName];
                                immunity = Math.Max(immunity, group.Immunity);
                                flags += group.Flags;
                            }
                        }
                    }

                    AdminManager.AddAdmin(authId, immunity, flags);
                }
            }
        }
    }
}
