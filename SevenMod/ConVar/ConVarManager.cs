// <copyright file="ConVarManager.cs" company="StevoTVR">
// Copyright (c) StevoTVR. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.ConVar
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Permissions;
    using System.Text;
    using System.Xml;
    using SevenMod.Core;
    using SevenMod.Logging;

    /// <summary>
    /// Manages console variables.
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    internal class ConVarManager
    {
        /// <summary>
        /// The list of registered console variables.
        /// </summary>
        private static Dictionary<string, ConVar> conVars = new Dictionary<string, ConVar>();

        /// <summary>
        /// Lists of plugins referencing each console variable.
        /// </summary>
        private static Dictionary<string, HashSet<IPlugin>> pluginReferences = new Dictionary<string, HashSet<IPlugin>>();

        /// <summary>
        /// The list of config files to be automatically executed.
        /// </summary>
        private static List<ConfigInfo> configs = new List<ConfigInfo>();

        /// <summary>
        /// The watches for changes in the configuration directory.
        /// </summary>
        private static FileSystemWatcher watcher;

        /// <summary>
        /// Gets a value indicating whether the auto executing configurations have been loaded.
        /// </summary>
        public static bool ConfigsLoaded { get; private set; }

        /// <summary>
        /// Find an existing console variable with the specified name.
        /// </summary>
        /// <param name="plugin">The plugin requesting the variable.</param>
        /// <param name="name">The name of the console variable to locate.</param>
        /// <returns>The <see cref="ConVar"/> object representing the console variable if found; otherwise <c>null</c>.</returns>
        public static ConVar FindConVar(IPlugin plugin, string name)
        {
            var key = name.Trim().ToLower();
            if (!conVars.ContainsKey(key))
            {
                return null;
            }

            if (plugin != null)
            {
                pluginReferences[key].Add(plugin);
            }

            return conVars[key];
        }

        /// <summary>
        /// Creates a new <see cref="ConVar"/> or returns the existing one if one with the same name already exists.
        /// </summary>
        /// <param name="plugin">The plugin creating the variable.</param>
        /// <param name="name">The name of the variable.</param>
        /// <param name="defaultValue">The default value of the variable as a string.</param>
        /// <param name="description">Optional description for the variable.</param>
        /// <param name="hasMin">Optional value indicating whether the variable has a minimum value.</param>
        /// <param name="min">The minimum value of the variable if <paramref name="hasMin"/> is <c>true</c>.</param>
        /// <param name="hasMax">Optional value indicating whether the variable has a maximum value.</param>
        /// <param name="max">The maximum value of the variable if <paramref name="hasMax"/> is <c>true</c>.</param>
        /// <returns>The <see cref="ConVar"/> object representing the console variable.</returns>
        public static ConVar CreateConVar(IPlugin plugin, string name, string defaultValue, string description = "", bool hasMin = false, float min = 0.0f, bool hasMax = false, float max = 1.0f)
        {
            name = name.Trim();
            var key = name.ToLower();
            if (!conVars.ContainsKey(key))
            {
                conVars[key] = new ConVar(plugin, name, defaultValue, description, hasMin, min, hasMax, max);
                pluginReferences[key] = new HashSet<IPlugin>();
            }

            if (plugin != null)
            {
                pluginReferences[key].Add(plugin);
            }

            return conVars[key];
        }

        /// <summary>
        /// Adds a configuration file to be automatically loaded.
        /// </summary>
        /// <param name="plugin">The plugin associated with this configuration file.</param>
        /// <param name="autoCreate">A value indicating whether the file should be automatically created if it does not exist.</param>
        /// <param name="name">The name of the configuration file without extension.</param>
        public static void AutoExecConfig(IPlugin plugin, bool autoCreate, string name)
        {
            configs.Add(new ConfigInfo(plugin, autoCreate, name.Trim()));
        }

        /// <summary>
        /// Executes all the automatically executed configuration files.
        /// </summary>
        public static void ExecuteConfigs()
        {
            foreach (var config in configs)
            {
                ExecuteConfig(config);
            }

            ConfigsLoaded = true;
        }

        /// <summary>
        /// Executes all the automatically executed configuration files for a plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which to load configs.</param>
        public static void ExecuteConfigs(IPlugin plugin)
        {
            foreach (var config in configs.FindAll((ConfigInfo c) => plugin.Equals(c.Plugin)))
            {
                ExecuteConfig(config);
            }
        }

        /// <summary>
        /// Unload all configuration associated with a plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which to unload configuration.</param>
        public static void UnloadPlugin(IPlugin plugin)
        {
            foreach (var plugins in pluginReferences)
            {
                if (plugins.Value.Remove(plugin) && plugins.Value.Count == 0)
                {
                    conVars.Remove(plugins.Key);
                }
            }

            pluginReferences.RemoveAll((HashSet<IPlugin> hs) => hs.Count == 0);
            configs.RemoveAll((ConfigInfo config) => plugin == config.Plugin);
        }

        /// <summary>
        /// Executes a single automatically executed configuration file.
        /// </summary>
        /// <param name="config">The <see cref="ConfigInfo"/> object containing the configuration file metadata.</param>
        private static void ExecuteConfig(ConfigInfo config)
        {
            var path = $"{SMPath.Config}{config.Name}.xml";
            if (!File.Exists(path))
            {
                if (!config.AutoCreate)
                {
                    return;
                }

                GenerateConfig(config);
            }

            var xml = new XmlDocument();
            try
            {
                xml.Load(path);
            }
            catch (XmlException e)
            {
                SMLog.Error($"Failed reading configuration from {path}: {e.Message}");
                return;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("property"))
            {
                if (element.HasAttribute("name") && element.HasAttribute("value"))
                {
                    if (conVars.TryGetValue(element.GetAttribute("name").Trim().ToLower(), out var conVar))
                    {
                        conVar.Value.Value = element.GetAttribute("value");
                    }
                }
            }

            if (watcher == null)
            {
                watcher = new FileSystemWatcher(SMPath.Config, "*.xml");
                watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName;
                watcher.Changed += OnConfigFileChanged;
                watcher.Deleted += OnConfigFileChanged;
                watcher.Renamed += OnConfigFileChanged;
                watcher.EnableRaisingEvents = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="watcher"/> when a change is detected in the configuration directory.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="FileSystemEventArgs"/> object containing the event data.</param>
        private static void OnConfigFileChanged(object sender, FileSystemEventArgs e)
        {
            var file = Path.GetFileNameWithoutExtension(e.FullPath);
            var config = configs.Find((ConfigInfo c) => c.Name.Equals(file));
            if (config != null)
            {
                ExecuteConfig(config);
            }
        }

        /// <summary>
        /// Creates a configuration file.
        /// </summary>
        /// <param name="config">The <see cref="ConfigInfo"/> object containing the configuration file metadata.</param>
        private static void GenerateConfig(ConfigInfo config)
        {
            var path = $"{SMPath.Config}{config.Name}.xml";

            using (var writer = XmlWriter.Create(path))
            {
                writer.WriteWhitespace("\r\n");
                writer.WriteStartElement(config.Name);

                foreach (var conVar in conVars.Values)
                {
                    if (conVar.Plugin != config.Plugin)
                    {
                        continue;
                    }

                    var description = new StringBuilder();
                    if (!string.IsNullOrEmpty(conVar.Description))
                    {
                        foreach (var line in conVar.Description.Trim().Split('\n'))
                        {
                            description.AppendLine().Append("    ");
                            var words = new Queue<string>(line.Trim().Split(' '));
                            var index = 0;
                            while (words.Count > 0)
                            {
                                var word = words.Dequeue();
                                if (index + word.Length > 96)
                                {
                                    description.AppendLine().Append("    ");
                                    index = 0;
                                }

                                if (index > 0)
                                {
                                    description.Append(' ');
                                }

                                description.Append(word);
                                index += word.Length + 1;
                            }
                        }

                        description.AppendLine().Append("    -");
                    }

                    description.AppendLine().Append("    Default: ").Append('"').Append(conVar.DefaultValue).Append('"').AppendLine();

                    if (conVar.HasMin)
                    {
                        description.Append("    Minimum: ").Append('"').Append(conVar.MinValue.ToString("F6")).Append('"').AppendLine();
                    }

                    if (conVar.HasMax)
                    {
                        description.Append("    Maximum: ").Append('"').Append(conVar.MaxValue.ToString("F6")).Append('"').AppendLine();
                    }

                    description.Append("  ");

                    writer.WriteWhitespace("\r\n\r\n  ");
                    writer.WriteComment(description.ToString());
                    writer.WriteWhitespace("\r\n  ");
                    writer.WriteStartElement("property");
                    writer.WriteAttributeString("name", conVar.Name);
                    writer.WriteAttributeString("value", conVar.DefaultValue);
                    writer.WriteEndElement();
                }

                writer.WriteWhitespace("\r\n\r\n");
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Contains the metadata for a config file.
        /// </summary>
        private class ConfigInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigInfo"/> class.
            /// </summary>
            /// <param name="plugin">The plugin associated with this configuration file.</param>
            /// <param name="autoCreate">A value indicating whether the file should be automatically created if it does not exist.</param>
            /// <param name="name">The name of the configuration file without extension.</param>
            public ConfigInfo(IPlugin plugin, bool autoCreate, string name)
            {
                this.Plugin = plugin;
                this.Name = name;
                this.AutoCreate = autoCreate;
            }

            /// <summary>
            /// Gets the plugin associated with this configuration file.
            /// </summary>
            public IPlugin Plugin { get; }

            /// <summary>
            /// Gets a value indicating whether the file should be automatically created if it does not exist.
            /// </summary>
            public bool AutoCreate { get; }

            /// <summary>
            /// Gets the name of the configuration file without extension.
            /// </summary>
            public string Name { get; }
        }
    }
}
