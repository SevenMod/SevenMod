// <copyright file="AdminFlatFile.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminFlatFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml;
    using SevenMod.Admin;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that loads admin users from a local configuration file.
    /// </summary>
    public class AdminFlatFile : PluginAbstract
    {
        /// <summary>
        /// The path to the plugin configuration file.
        /// </summary>
        private static readonly string ConfigPath = $"{SMPath.Config}Admins.xml";

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Admin File Reader",
            Author = "SevenMod",
            Description = "Loads admin users from a local configuration file.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void ReloadAdmins()
        {
            base.ReloadAdmins();
            this.LoadAdmins();
        }

        /// <summary>
        /// Loads the admin users from the Admins.xml file.
        /// </summary>
        private void LoadAdmins()
        {
            if (!File.Exists(ConfigPath))
            {
                this.CreateConfig();
            }

            var xml = new XmlDocument();
            try
            {
                xml.Load(ConfigPath);
            }
            catch (XmlException e)
            {
                Log.Error($"[SM] Failed reading configuration from {ConfigPath}: {e.Message}");
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

        /// <summary>
        /// Creates the admin configuration file.
        /// </summary>
        private void CreateConfig()
        {
            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(ConfigPath, settings))
            {
                writer.WriteStartElement("AdminFlatFile");

                writer.WriteStartElement("Groups");
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", string.Empty);
                writer.WriteAttributeString("Immunity", string.Empty);
                writer.WriteAttributeString("Flags", string.Empty);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Admins");
                writer.WriteStartElement("Admin");
                writer.WriteAttributeString("AuthId", string.Empty);
                writer.WriteAttributeString("Immunity", string.Empty);
                writer.WriteAttributeString("Flags", string.Empty);
                writer.WriteAttributeString("Groups", string.Empty);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();

                writer.Close();
                writer.Flush();
            }
        }
    }
}
