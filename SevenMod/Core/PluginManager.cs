// <copyright file="PluginManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        /// The queue of plugin names to be loaded.
        /// </summary>
        private static Queue<string> loadQueue = new Queue<string>();

        /// <summary>
        /// Gets a value indicating whether loading and unloading plugins is permitted.
        /// </summary>
        public static bool IsLocked { get; internal set; }

        /// <summary>
        /// Gets the currently active plugins.
        /// </summary>
        internal static SortedDictionary<string, PluginContainer> Plugins { get; } = new SortedDictionary<string, PluginContainer>();

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        /// <param name="plugin">Will be set to the <see cref="PluginContainer"/> object containing the metadata for the plugin, or <c>null</c> if the plugin is not loaded.</param>
        /// <returns><c>true</c> if the plugin was found; otherwise <c>false</c>.</returns>
        public static bool GetPlugin(string name, out PluginContainer plugin)
        {
            var key = NameToKey(name);
            return Plugins.TryGetValue(key, out plugin);
        }

        /// <summary>
        /// Gets the metadata for a plugin.
        /// </summary>
        /// <param name="index">The index of the plugin.</param>
        /// <param name="plugin">Will be set to the <see cref="PluginContainer"/> object containing the metadata for the plugin, or <c>null</c> if the plugin is not loaded.</param>
        /// <returns><c>true</c> if the plugin was found; otherwise <c>false</c>.</returns>
        public static bool GetPlugin(int index, out PluginContainer plugin)
        {
            if (!IndexToKey(index, out var key))
            {
                plugin = null;
                return false;
            }

            plugin = Plugins[key];
            return true;
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Load(string name)
        {
            loadQueue.Enqueue(NameToKey(name));
            while (loadQueue.Count > 0)
            {
                Load(loadQueue.Dequeue(), false);
            }
        }

        /// <summary>
        /// Loads the enabled plugins.
        /// </summary>
        public static void Refresh()
        {
            if (IsLocked)
            {
                throw new NotSupportedException();
            }

            var files = Directory.GetFiles(SMPath.Plugins, "*.dll");
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file);
                loadQueue.Enqueue(NameToKey(name));
            }

            while (loadQueue.Count > 0)
            {
                Load(loadQueue.Dequeue(), true);
            }

            if (!ConVarManager.ConfigsLoaded)
            {
                ConVarManager.ExecuteConfigs();
                foreach (var k in Plugins.Keys.ToArray())
                {
                    if (Plugins.TryGetValue(k, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
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
            Load(name);
        }

        /// <summary>
        /// Reloads a plugin.
        /// </summary>
        /// <param name="index">The index of the plugin.</param>
        public static void Reload(int index)
        {
            if (IndexToKey(index, out var key))
            {
                Unload(key);
                Load(key);
            }
        }

        /// <summary>
        /// Unloads a plugin.
        /// </summary>
        /// <param name="name">The name of the plugin.</param>
        public static void Unload(string name)
        {
            if (IsLocked)
            {
                throw new NotSupportedException();
            }

            var key = NameToKey(name);
            if (Plugins.TryGetValue(key, out var plugin))
            {
                plugin.LoadStatus = PluginContainer.Status.Unloaded;
                try
                {
                    IsLocked = true;
                    plugin.Plugin.OnUnloadPlugin();
                    (plugin as IDisposable)?.Dispose();
                    IsLocked = false;
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

                Plugins.Remove(key);
            }
        }

        /// <summary>
        /// Unloads a plugin.
        /// </summary>
        /// <param name="index">The index of the plugin.</param>
        public static void Unload(int index)
        {
            if (IndexToKey(index, out var key))
            {
                Unload(key);
            }
        }

        /// <summary>
        /// Unloads all plugins.
        /// </summary>
        public static void UnloadAll()
        {
            if (IsLocked)
            {
                throw new NotSupportedException();
            }

            IsLocked = true;
            foreach (var key in Plugins.Keys)
            {
                if (Plugins.TryGetValue(key, out var plugin) && plugin.LoadStatus == PluginContainer.Status.Loaded)
                {
                    plugin.LoadStatus = PluginContainer.Status.Unloaded;
                    try
                    {
                        plugin.Plugin.OnUnloadPlugin();
                        (plugin as IDisposable)?.Dispose();
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

            Plugins.Clear();
            IsLocked = false;
            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Loads a plugin.
        /// </summary>
        /// <param name="key">The dictionary key of the plugin.</param>
        /// <param name="refreshing">A value indicating whether the plugin list is being refreshed.</param>
        private static void Load(string key, bool refreshing)
        {
            if (IsLocked)
            {
                throw new NotSupportedException();
            }

            if (Plugins.ContainsKey(key) && Plugins[key].LoadStatus == PluginContainer.Status.Loaded)
            {
                return;
            }

            var file = $"{SMPath.Plugins}{key}.dll";
            if (!File.Exists(file))
            {
                return;
            }

            try
            {
                var dll = Assembly.LoadFile(file);
                var type = dll.GetType($"SevenMod.Plugin.{key}.{key}", true, true);
                var container = new PluginContainer(Path.GetFileName(type.Assembly.Location));
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

                    Plugins[key] = container;
                }
                else
                {
                    throw new Exception($"{type.Name} does not inherit from {PluginParentType.Name}");
                }
            }
            catch (Exception e)
            {
                SMLog.Error($"Failed loading plugin {key}.dll: {e.Message}");
            }
        }

        /// <summary>
        /// Converts a plugin index to its dictionary key.
        /// </summary>
        /// <param name="index">The plugin index.</param>
        /// <param name="key">Variable to be populated by the dictionary key, or <c>null</c> if <paramref name="index"/> is out of range.</param>
        /// <returns><c>true</c> on success; <c>false</c> if <paramref name="index"/> is out of range.</returns>
        private static bool IndexToKey(int index, out string key)
        {
            if (index < 0 || index >= Plugins.Count)
            {
                key = null;
                return false;
            }

            key = new List<string>(Plugins.Keys)[index];
            return true;
        }

        /// <summary>
        /// Converts a name to the format of a plugin dictionary key.
        /// </summary>
        /// <param name="name">The input name.</param>
        /// <returns>The dictionary key.</returns>
        private static string NameToKey(string name)
        {
            name = name.Trim().ToLower();
            if (name.EndsWith(".dll"))
            {
                name = name.Substring(0, name.LastIndexOf('.'));
            }

            return name;
        }
    }
}
