// <copyright file="PluginManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Manages plugins.
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// The path to the directory containing the plugin files.
        /// </summary>
        public static readonly string PluginPath = $"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}{Path.DirectorySeparatorChar}Plugins{Path.DirectorySeparatorChar}";

        /// <summary>
        /// The currently active plugins.
        /// </summary>
        private static readonly Dictionary<string, PluginAbstract> Plugins = new Dictionary<string, PluginAbstract>();

        /// <summary>
        /// Gets a list of the currently active plugins.
        /// </summary>
        public static List<PluginAbstract> ActivePlugins { get => new List<PluginAbstract>(Plugins.Values); }

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <returns>The <see cref="PluginAbstract.PluginInfo"/> struct containing the metadata for
        /// the plugin, or <c>null</c> if the plugin is not loaded.</returns>
        public static PluginAbstract.PluginInfo? GetPluginInfo(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                return Plugins[name].Info;
            }

            return null;
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Load(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                return;
            }

            var parentType = Type.GetType("SevenMod.Core.PluginAbstract");
            var dll = Assembly.LoadFile($"{PluginPath}{name}.dll");
            try
            {
                var type = dll.GetType($"SevenMod.Plugin.{name}.{name}", true, true);
                if (type.IsSubclassOf(parentType))
                {
                    Log.Out("Added {0}", type.Name);
                    var plugin = Activator.CreateInstance(type) as PluginAbstract;
                    plugin.LoadPlugin();
                    plugin.GameAwake();
                    Plugins.Add(name, plugin);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
        }

        /// <summary>
        /// Loads the enabled plugins defined in the plugins.ini file.
        /// </summary>
        public static void Refresh()
        {
            var list = new List<PluginAbstract>();

            StreamReader file = new StreamReader($"{ConfigManager.ConfigPath}plugins.ini");
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line.TrimStart().StartsWith(";"))
                {
                    continue;
                }

                line = Regex.Replace(line, @"[^a-zA-Z0-9\-_]", string.Empty);
                if (line.Length == 0)
                {
                    continue;
                }

                Load(line);
            }
        }

        /// <summary>
        /// Reloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Reload(string name)
        {
            Unload(name);
            Load(name);
        }

        /// <summary>
        /// Unloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Unload(string name)
        {
            name = name.ToLower();
            if (Plugins.ContainsKey(name))
            {
                Plugins[name].UnloadPlugin();
                Plugins.Remove(name);
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var plugin in Plugins.Values)
            {
                plugin.UnloadPlugin();
            }

            Plugins.Clear();
        }
    }
}
