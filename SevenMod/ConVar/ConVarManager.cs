// <copyright file="ConVarManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.ConVar
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Xml;
    using SevenMod.Core;

    /// <summary>
    /// Manages console variables.
    /// </summary>
    public class ConVarManager
    {
        /// <summary>
        /// The list of registered console variables.
        /// </summary>
        private static Dictionary<string, ConVar> conVars = new Dictionary<string, ConVar>();

        /// <summary>
        /// The list of config files to be automatically executed.
        /// </summary>
        private static List<ConfigInfo> configs = new List<ConfigInfo>();

        /// <summary>
        /// Gets a value indicating whether the auto executing configurations have been loaded.
        /// </summary>
        public static bool ConfigsLoaded { get; private set; }

        /// <summary>
        /// Find an existing console variable with the specified name.
        /// </summary>
        /// <param name="name">The name of the console variable to locate.</param>
        /// <returns>The <see cref="ConVar"/> object representing the console variable if found; otherwise <c>null</c>.</returns>
        public static ConVar FindConVar(string name)
        {
            conVars.TryGetValue(name.ToLower().Trim(), out ConVar value);
            return value;
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
        public static ConVar CreateConVar(PluginAbstract plugin, string name, string defaultValue, string description = "", bool hasMin = false, float min = 0.0f, bool hasMax = false, float max = 1.0f)
        {
            name = name.Trim();
            ConVar conVar;
            var key = name.ToLower();
            if (conVars.ContainsKey(key))
            {
                conVar = conVars[key];
            }
            else
            {
                conVar = new ConVar(plugin, name, defaultValue, description, hasMin, min, hasMax, max);
            }

            conVars[key] = conVar;
            return conVar;
        }

        /// <summary>
        /// Adds a configuration file to be automatically loaded.
        /// </summary>
        /// <param name="plugin">The plugin associated with this configuration file.</param>
        /// <param name="autoCreate">A value indicating whether the file should be automatically created if it does not exist.</param>
        /// <param name="name">The name of the configuration file without extension.</param>
        public static void AutoExecConfig(PluginAbstract plugin, bool autoCreate, string name)
        {
            configs.Add(new ConfigInfo(plugin, autoCreate, name.Trim()));
        }

        /// <summary>
        /// Executes all the automatically executed configuration files.
        /// </summary>
        public static void ExecuteConfigs()
        {
            ExecuteConfig(new ConfigInfo(null, true, "Core"));
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
        public static void ExecuteConfigs(PluginAbstract plugin)
        {
            foreach (var config in configs.FindAll((ConfigInfo c) => c.Plugin.Equals(plugin)))
            {
                ExecuteConfig(config);
            }
        }

        /// <summary>
        /// Unload all configuration associated with a plugin.
        /// </summary>
        /// <param name="plugin">The plugin for which to unload configuration.</param>
        public static void UnloadPlugin(PluginAbstract plugin)
        {
            conVars.RemoveAll((ConVar conVar) => plugin.Equals(conVar.Plugin));
            configs.RemoveAll((ConfigInfo config) => plugin.Equals(config.Plugin));
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
                if (config.AutoCreate)
                {
                    GenerateConfig(config);
                }

                return;
            }

            var xml = new XmlDocument();
            try
            {
                xml.Load(path);
            }
            catch (XmlException e)
            {
                Log.Warning($"[SM] Failed reading configuration from {path}: {e.Message}");
                return;
            }

            foreach (XmlElement element in xml.GetElementsByTagName("property"))
            {
                if (element.HasAttribute("name") && element.HasAttribute("value"))
                {
                    var conVar = FindConVar(element.GetAttribute("name"));
                    if (conVar != null)
                    {
                        conVar.Value.Value = element.GetAttribute("value");
                    }
                }
            }
        }

        /// <summary>
        /// Creates a configuration file.
        /// </summary>
        /// <param name="config">The <see cref="ConfigInfo"/> object containing the configuration file metadata.</param>
        private static void GenerateConfig(ConfigInfo config)
        {
            var path = $"{SMPath.Config}{config.Name}.xml";
            if (File.Exists(path))
            {
                return;
            }

            var xml = new XmlDataDocument();
            xml.AppendChild(xml.CreateXmlDeclaration("1.0", "utf-8", null));
            var root = xml.CreateElement(config.Name);
            xml.AppendChild(root);

            foreach (var conVar in conVars.Values)
            {
                if (conVar.Plugin != config.Plugin)
                {
                    continue;
                }

                root.AppendChild(xml.CreateWhitespace("\r\n\r\n  "));

                var description = new StringBuilder().AppendLine();
                if (!string.IsNullOrEmpty(conVar.Description))
                {
                    description.Append("    ").AppendLine(conVar.Description).AppendLine();
                }

                if (conVar.HasMin)
                {
                    description.Append("    Min: ").AppendLine(conVar.MinValue.ToString());
                }

                if (conVar.HasMax)
                {
                    description.Append("    Max: ").AppendLine(conVar.MaxValue.ToString());
                }

                description.Append("    Default: ").AppendLine(conVar.DefaultValue).Append("  ");
                root.AppendChild(xml.CreateComment(description.ToString()));
                root.AppendChild(xml.CreateWhitespace("\r\n  "));

                var prop = xml.CreateElement("property");
                prop.SetAttribute("name", conVar.Name);
                prop.SetAttribute("value", conVar.DefaultValue);
                root.AppendChild(prop);
            }

            root.AppendChild(xml.CreateWhitespace("\r\n\r\n"));
            xml.Save(path);
        }

        /// <summary>
        /// Contains the metadata for a config file.
        /// </summary>
        private struct ConfigInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ConfigInfo"/> struct.
            /// </summary>
            /// <param name="plugin">The plugin associated with this configuration file.</param>
            /// <param name="autoCreate">A value indicating whether the file should be automatically created if it does not exist.</param>
            /// <param name="name">The name of the configuration file without extension.</param>
            public ConfigInfo(PluginAbstract plugin, bool autoCreate, string name)
            {
                this.Plugin = plugin;
                this.Name = name;
                this.AutoCreate = autoCreate;
            }

            /// <summary>
            /// Gets the plugin associated with this configuration file.
            /// </summary>
            public PluginAbstract Plugin { get; }

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
