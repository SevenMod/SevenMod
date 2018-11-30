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
        /// The <see cref="Type"/> object representing the <see cref="IPlugin"/> interface plugins must implement.
        /// </summary>
        private static readonly Type PluginInterface = Type.GetType("SevenMod.Core.IPlugin");

        /// <summary>
        /// Gets the currently active plugins.
        /// </summary>
        internal static Dictionary<string, PluginContainer> Plugins { get; } = new Dictionary<string, PluginContainer>();

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <returns>The <see cref="PluginContainer"/> object containing the metadata for the plugin, or <c>null</c> if the plugin is not loaded.</returns>
        public static PluginContainer GetPluginInfo(string name)
        {
            name = name.Trim().ToLower();
            if (Plugins.ContainsKey(name))
            {
                return Plugins[name];
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
                foreach (var plugin in Plugins.Values)
                {
                    plugin.Plugin.ConfigsExecuted();
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
            if (Plugins.ContainsKey(name))
            {
                Plugins[name].Plugin.UnloadPlugin();
                AdminCommandManager.UnloadPlugin(Plugins[name].Plugin);
                ConVarManager.UnloadPlugin(Plugins[name].Plugin);
                Plugins.Remove(name);
                AdminManager.ReloadAdmins();
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var plugin in Plugins.Values)
            {
                plugin.Plugin.UnloadPlugin();
                AdminCommandManager.UnloadPlugin(plugin.Plugin);
                ConVarManager.UnloadPlugin(plugin.Plugin);
            }

            Plugins.Clear();
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
            if (Plugins.ContainsKey(name) && Plugins[name].LoadStatus == PluginContainer.Status.Loaded)
            {
                return;
            }

            var file = $"{SMPath.Plugins}{name}.dll";
            if (!File.Exists(file))
            {
                return;
            }

            try
            {
                var dll = Assembly.LoadFile(file);
                var type = dll.GetType($"SevenMod.Plugin.{name}.{name}", true, true);
                var container = new PluginContainer(type.Assembly.GetName().FullName);
                Plugins[name] = container;
                if (PluginInterface.IsAssignableFrom(type))
                {
                    try
                    {
                        var plugin = Activator.CreateInstance(type) as IPlugin;
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

                        container.Plugin = plugin;
                        container.PluginInfo = plugin.Info;
                        container.LoadStatus = PluginContainer.Status.Loaded;
                        SMLog.Out($"Added plugin {type.Name}");
                    }
                    catch (Exception e)
                    {
                        container.LoadStatus = PluginContainer.Status.Error;
                        container.Error = e.Message;
                        throw e;
                    }
                }
                else
                {
                    throw new Exception($"{type.Name} does not implement interface {PluginInterface.Name}");
                }
            }
            catch (Exception e)
            {
                SMLog.Error($"Failed loading plugin {name}.dll: {e.Message}");
            }
        }
    }
}
