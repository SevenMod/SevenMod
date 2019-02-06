// <copyright file="AdminCommandManager.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Core;
    using SevenMod.Logging;

    /// <summary>
    /// Manages admin commands.
    /// </summary>
    public class AdminCommandManager
    {
        /// <summary>
        /// The path to the command overrides configuration file.
        /// </summary>
        private static readonly string ConfigPath = $"{SMPath.Config}AdminCommandOverrides.xml";

        /// <summary>
        /// The list of registered admin commands.
        /// </summary>
        private static Dictionary<string, AdminCommand> commands = new Dictionary<string, AdminCommand>();

        /// <summary>
        /// Lists of plugins referencing each command.
        /// </summary>
        private static Dictionary<string, HashSet<IPlugin>> pluginReferences = new Dictionary<string, HashSet<IPlugin>>();

        /// <summary>
        /// The map of command access overrides.
        /// </summary>
        private static Dictionary<string, AdminFlags> overrides = new Dictionary<string, AdminFlags>();

        /// <summary>
        /// The watcher for changes to the admin command overrides configuration file.
        /// </summary>
        private static FileSystemWatcher overrideWatcher;

        /// <summary>
        /// Initializes static members of the <see cref="AdminCommandManager"/> class.
        /// </summary>
        static AdminCommandManager()
        {
            LoadOverrides();
        }

        /// <summary>
        /// Gets the list of currently registered admin commands.
        /// </summary>
        public static ReadOnlyCollection<AdminCommand> Commands { get => new List<AdminCommand>(commands.Values).AsReadOnly(); }

        /// <summary>
        /// Find an existing admin command with the specified name.
        /// </summary>
        /// <param name="plugin">The plugin requesting the command.</param>
        /// <param name="command">The name of the admin command to locate.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command if found; otherwise <c>null</c>.</returns>
        internal static AdminCommand FindCommand(IPlugin plugin, string command)
        {
            var key = command.Trim().ToLower();
            if (!commands.ContainsKey(key))
            {
                return null;
            }

            if (plugin != null)
            {
                pluginReferences[key].Add(plugin);
            }

            return commands[key];
        }

        /// <summary>
        /// Creates a new <see cref="AdminCommand"/> or returns the existing one if one with the same name already exists.
        /// </summary>
        /// <param name="plugin">The plugin creating the admin command.</param>
        /// <param name="command">The name of the admin command.</param>
        /// <param name="accessFlags">The <see cref="AdminFlags"/> value required to execute the admin command.</param>
        /// <param name="description">An optional description for the admin command.</param>
        /// <returns>The <see cref="AdminCommand"/> object representing the admin command.</returns>
        internal static AdminCommand CreateCommand(IPlugin plugin, string command, AdminFlags accessFlags, string description = "")
        {
            command = command.Trim();
            var key = command.ToLower();
            if (!commands.ContainsKey(key))
            {
                commands[key] = new AdminCommand(command, description, accessFlags);
                pluginReferences[key] = new HashSet<IPlugin>();
            }

            if (plugin != null)
            {
                pluginReferences[key].Add(plugin);
            }

            return commands[key];
        }

        /// <summary>
        /// Removes an <see cref="AdminCommand"/>.
        /// </summary>
        /// <param name="plugin">The plugin that created the admin command.</param>
        /// <param name="command">The name of the admin command.</param>
        internal static void RemoveCommand(IPlugin plugin, string command)
        {
            var key = command.Trim().ToLower();
            if (!pluginReferences.ContainsKey(key))
            {
                return;
            }

            if (pluginReferences[key].Remove(plugin) && pluginReferences[key].Count == 0)
            {
                commands.Remove(key);
            }
        }

        /// <summary>
        /// Executes an admin command.
        /// </summary>
        /// <param name="command">The name of the admin command to execute.</param>
        /// <param name="arguments">The arguments for the admin command.</param>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client executing the command.</param>
        internal static void ExecuteCommand(string command, List<string> arguments, ClientInfo client)
        {
            var key = command.Trim().ToLower();
            if (!commands.ContainsKey(key))
            {
                ChatHelper.ReplyToCommand(client, "Unknown command");
                return;
            }

            if (!HasAccess(client, commands[key]))
            {
                ChatHelper.ReplyToCommand(client, "You do not have access to that command");
                return;
            }

            commands[key].OnExecute(arguments, client);
        }

        /// <summary>
        /// Checks whether a specified client has access to a command.
        /// </summary>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client to check.</param>
        /// <param name="command">The <see cref="AdminCommand"/> representing the command.</param>
        /// <returns><c>true</c> if <paramref name="client"/> has access to <paramref name="command"/>; otherwise <c>false</c>.</returns>
        internal static bool HasAccess(ClientInfo client, AdminCommand command)
        {
            if (client != null)
            {
                var flags = overrides.ContainsKey(command.Command) ? overrides[command.Command] : command.AccessFlags;
                if ((flags != 0) && !AdminManager.CheckAccess(client, flags))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Unloads all admin commands associated with a plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which to unload admin commands.</param>
        internal static void UnloadPlugin(IPlugin plugin)
        {
            foreach (var plugins in pluginReferences)
            {
                if (plugins.Value.Remove(plugin) && plugins.Value.Count == 0)
                {
                    commands.Remove(plugins.Key);
                }
            }

            pluginReferences.RemoveAll((HashSet<IPlugin> hs) => hs.Count == 0);
        }

        /// <summary>
        /// Loads the admin command overrides from the config file.
        /// </summary>
        private static void LoadOverrides()
        {
            overrides.Clear();

            if (!File.Exists(ConfigPath))
            {
                CreateOverrideFile();
            }

            var xml = new XmlDocument();
            try
            {
                xml.Load(ConfigPath);
            }
            catch (XmlException e)
            {
                SMLog.Error($"Failed reading admin command overrides from {ConfigPath}: {e.Message}");
                return;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("Override"))
            {
                if (!element.HasAttribute("Command") || !element.HasAttribute("Flag"))
                {
                    continue;
                }

                var command = element.GetAttribute("Command").Trim().ToLower();
                if (string.IsNullOrEmpty(command))
                {
                    continue;
                }

                var flags = element.GetAttribute("Flag").ToLower().ToCharArray();
                if (flags.Length == 0)
                {
                    overrides[command] = 0;
                }
                else if (AdminManager.AdminFlagKeys.TryGetValue(flags[0], out var flag))
                {
                    overrides[command] = flag;
                }
            }

            if (overrideWatcher == null)
            {
                overrideWatcher = new FileSystemWatcher(Path.GetDirectoryName(ConfigPath), Path.GetFileName(ConfigPath));
                overrideWatcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite;
                overrideWatcher.Changed += OnOverrideFileChanged;
                overrideWatcher.Deleted += OnOverrideFileChanged;
                overrideWatcher.Renamed += OnOverrideFileChanged;
                overrideWatcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="overrideWatcher"/> when the admin command overrides configuration file changes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> object containing the event data.</param>
        private static void OnOverrideFileChanged(object sender, FileSystemEventArgs e)
        {
            LoadOverrides();
        }

        /// <summary>
        /// Creates the admin command overrides configuration file.
        /// </summary>
        private static void CreateOverrideFile()
        {
            var comment = new StringBuilder("\r\n")
                .AppendLine("    Use this file to override the default access flag assigned to admin commands.")
                .AppendLine()
                .AppendLine("    Override attributes:")
                .Append("      Command: ").AppendLine("The command to override")
                .Append("      Flag:    ").AppendLine("The new access flag, or leave empty to remove access restriction")
                .Append("  ").ToString();

            var settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(ConfigPath, settings))
            {
                writer.WriteStartElement("AdminCommandOverrides");
                writer.WriteComment(comment);

                writer.WriteStartElement("Override");
                writer.WriteAttributeString("Command", string.Empty);
                writer.WriteAttributeString("Flag", string.Empty);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }
    }
}
