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
        /// The <see cref="Type"/> object representing the <see cref="PluginAbstract"/> class plugins must inherit from.
        /// </summary>
        private static readonly Type PluginParentType = Type.GetType("SevenMod.Core.PluginAbstract");

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
            Plugins.TryGetValue(name, out PluginContainer plugin);
            return plugin;
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
                foreach (var k in Plugins.Keys)
                {
                    if (Plugins.TryGetValue(k, out PluginContainer plugin))
                    {
                        try
                        {
                            plugin.Plugin.OnConfigsExecuted();
                        }
                        catch (HaltPluginException)
                        {
                        }
                        catch (Exception e)
                        {
                            plugin.SetFailState(e);
                        }
                    }
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
            if (Plugins.TryGetValue(name, out PluginContainer plugin))
            {
                plugin.LoadStatus = PluginContainer.Status.Unloaded;
                try
                {
                    plugin.Plugin.OnUnloadPlugin();
                }
                catch (HaltPluginException)
                {
                }
                catch (Exception e)
                {
                    plugin.SetFailState(e);
                }

                AdminCommandManager.UnloadPlugin(plugin.Plugin);
                ConVarManager.UnloadPlugin(plugin.Plugin);
                plugin.Plugin = null;
                AdminManager.ReloadAdmins();
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            foreach (var k in Plugins.Keys)
            {
                if (Plugins.TryGetValue(k, out PluginContainer plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    plugin.LoadStatus = PluginContainer.Status.Unloaded;
                    try
                    {
                        plugin.Plugin.OnUnloadPlugin();
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        plugin.SetFailState(e);
                    }

                    AdminCommandManager.UnloadPlugin(plugin.Plugin);
                    ConVarManager.UnloadPlugin(plugin.Plugin);
                    plugin.Plugin = null;
                }
            }

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
                var container = new PluginContainer(Path.GetFileName(type.Assembly.Location));
                Plugins[name] = container;
                if (type.IsSubclassOf(PluginParentType))
                {
                    try
                    {
                        var plugin = Activator.CreateInstance(type) as PluginAbstract;
                        plugin.Container = container;
                        plugin.OnLoadPlugin();
                        if (API.IsGameAwake)
                        {
                            plugin.OnGameAwake();
                        }

                        if (API.IsGameStartDone)
                        {
                            plugin.OnGameStartDone();
                        }

                        if (ConVarManager.ConfigsLoaded)
                        {
                            ConVarManager.ExecuteConfigs(plugin);
                            plugin.OnConfigsExecuted();
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
                    catch (HaltPluginException)
                    {
                        throw;
                    }
                    catch (Exception e)
                    {
                        container.LoadStatus = PluginContainer.Status.Error;
                        container.Error = e.Message;
                        throw;
                    }
                }
                else
                {
                    throw new Exception($"{type.Name} does not inherit from {PluginParentType.Name}");
                }
            }
            catch (Exception e)
            {
                SMLog.Error($"Failed loading plugin {name}.dll: {e.Message}");
            }
        }
    }
}
