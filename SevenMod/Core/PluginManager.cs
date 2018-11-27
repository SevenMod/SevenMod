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
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.ConVar;

    /// <summary>
    /// Manages plugins.
    /// </summary>
    public class PluginManager
    {
        /// <summary>
        /// The <see cref="Type"/> object representing the <see cref="IPluginAPI"/> interface plugins must implement.
        /// </summary>
        private static readonly Type PluginInterface = Type.GetType("SevenMod.Core.IPluginAPI");

        /// <summary>
        /// The currently active plugins.
        /// </summary>
        private static Dictionary<string, IPluginAPI> plugins = new Dictionary<string, IPluginAPI>();

        /// <summary>
        /// Gets a list of the currently active plugins.
        /// </summary>
        public static List<IPluginAPI> Plugins { get => new List<IPluginAPI>(plugins.Values); }

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <returns>The <see cref="PluginInfo"/> object containing the metadata for the plugin, or <c>null</c> if the plugin is not loaded.</returns>
        public static PluginInfo? GetPluginInfo(string name)
        {
            name = name.Trim().ToLower();
            if (plugins.ContainsKey(name))
            {
                return plugins[name].Info;
            }

            return null;
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Load(string name)
        {
            Load(name, false);
        }

        /// <summary>
        /// Loads the enabled plugins.
        /// </summary>
        public static void Refresh()
        {
            var files = Directory.GetFiles(SMPath.Plugins, "*.dll");
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                Load(name, true);
            }

            if (!ConVarManager.ConfigsLoaded)
            {
                ConVarManager.ExecuteConfigs();
                foreach (var plugin in plugins.Values)
                {
                    plugin.ConfigsExecuted();
                }
            }

            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Reloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Reload(string name)
        {
            Unload(name);
            Load(name, false);
        }

        /// <summary>
        /// Unloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Unload(string name)
        {
            name = name.Trim().ToLower();
            if (plugins.ContainsKey(name))
            {
                plugins[name].UnloadPlugin();
                AdminCommandManager.UnloadPlugin(plugins[name]);
                ConVarManager.UnloadPlugin(plugins[name]);
                plugins.Remove(name);
                AdminManager.ReloadAdmins();
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var plugin in plugins.Values)
            {
                plugin.UnloadPlugin();
                AdminCommandManager.UnloadPlugin(plugin);
                ConVarManager.UnloadPlugin(plugin);
            }

            plugins.Clear();
            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="refreshing">A value indicating whether the plugin list is being refreshed.</param>
        private static void Load(string name, bool refreshing)
        {
            name = name.Trim().ToLower();
            if (plugins.ContainsKey(name))
            {
                return;
            }

            var dll = Assembly.LoadFile($"{SMPath.Plugins}{name}.dll");
            try
            {
                var type = dll.GetType($"SevenMod.Plugin.{name}.{name}", true, true);
                if (PluginInterface.IsAssignableFrom(type))
                {
                    var plugin = Activator.CreateInstance(type) as IPluginAPI;
                    plugin.LoadPlugin();
                    if (API.IsGameAwake)
                    {
                        plugin.GameAwake();
                    }

                    if (API.IsGameStartDone)
                    {
                        plugin.GameStartDone();
                    }

                    if (ConVarManager.ConfigsLoaded)
                    {
                        ConVarManager.ExecuteConfigs(plugin);
                        plugin.ConfigsExecuted();
                    }

                    if (!refreshing)
                    {
                        AdminManager.ReloadAdmins();
                    }

                    plugins.Add(name, plugin);
                    Log.Out($"[SM] Added plugin {type.Name}");
                }
            }
            catch (Exception e)
            {
                Log.Error($"[SM] Failed loading plugin {name}.dll: {e.Message}");
            }
        }
    }
}
