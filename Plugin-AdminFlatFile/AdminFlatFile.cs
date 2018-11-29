// <copyright file="AdminFlatFile.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminFlatFile
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
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

        /// <summary>
        /// The watcher for changes to the admin configuration file.
        /// </summary>
        private FileSystemWatcher watcher;

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
                this.LogError($"Failed reading admin configuration from {ConfigPath}: {e.Message}");
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

            if (this.watcher == null)
            {
                this.watcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigPath), Path.GetFileName(ConfigPath));
                this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                this.watcher.Changed += this.OnAdminFileChanged;
                this.watcher.Deleted += this.OnAdminFileChanged;
                this.watcher.Renamed += this.OnAdminFileChanged;
                this.watcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="watcher"/> when the admin configuration file changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> object containing the event data.</param>
        private void OnAdminFileChanged(object sender, FileSystemEventArgs e)
        {
            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Creates the admin configuration file.
        /// </summary>
        private void CreateConfig()
        {
            var flagsComment = new StringBuilder("\r\n")
                .AppendLine("    Immunity:")
                .AppendLine("      Can be any positive integer.")
                .AppendLine("      Admins with a lower immunity cannot target admins with a higher immunity.")
                .AppendLine()
                .AppendLine("    Access flags:")
                .AppendLine("      a - Reserved slots")
                .AppendLine("      b - Generic admin")
                .AppendLine("      c - Kick other players")
                .AppendLine("      d - Ban other players")
                .AppendLine("      e - Remove bans")
                .AppendLine("      f - Damage or kill other players")
                .AppendLine("      g - Map/world related functions")
                .AppendLine("      h - Change server settings")
                .AppendLine("      i - Execute configuration files")
                .AppendLine("      j - Special chat privileges")
                .AppendLine("      k - Start votes")
                .AppendLine("      l - Password the server")
                .AppendLine("      m - Server console access")
                .AppendLine("      n - \"Cheat\" commands")
                .AppendLine()
                .AppendLine("      o - Custom flag")
                .AppendLine("      p - Custom flag")
                .AppendLine("      q - Custom flag")
                .AppendLine("      r - Custom flag")
                .AppendLine("      s - Custom flag")
                .AppendLine("      t - Custom flag")
                .AppendLine()
                .AppendLine("      z - Root access; grants all flags and overrides immunity")
                .Append("  ").ToString();

            var groupComment = new StringBuilder("\r\n      Group attributes:\r\n\r\n")
                .Append("        Name:     ").AppendLine("Unique name for the group")
                .Append("        Immunity: ").AppendLine("Minimum immunity level for members of the group")
                .Append("        Flags:    ").AppendLine("Access flags shared among members of the group")
                .Append("    ").ToString();

            var adminComment = new StringBuilder("\r\n      Admin attributes:\r\n\r\n")
                .Append("        AuthId:   ").AppendLine("SteamID")
                .Append("        Immunity: ").AppendLine("Immunity level")
                .Append("        Flags:    ").AppendLine("Access flags")
                .Append("        Groups:   ").AppendLine("Comma separated list of group names")
                .Append("    ").ToString();

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(ConfigPath, settings))
            {
                writer.WriteStartElement("AdminFlatFile");
                writer.WriteComment(flagsComment);

                writer.WriteStartElement("Groups");
                writer.WriteComment(groupComment);
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("Name", string.Empty);
                writer.WriteAttributeString("Immunity", string.Empty);
                writer.WriteAttributeString("Flags", string.Empty);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteStartElement("Admins");
                writer.WriteComment(adminComment);
                writer.WriteStartElement("Admin");
                writer.WriteAttributeString("AuthId", string.Empty);
                writer.WriteAttributeString("Immunity", string.Empty);
                writer.WriteAttributeString("Flags", string.Empty);
                writer.WriteAttributeString("Groups", string.Empty);
                writer.WriteEndElement();
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
