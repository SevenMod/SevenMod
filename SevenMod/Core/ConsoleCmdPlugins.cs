// <copyright file="ConsoleCmdPlugins.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Console command for managing SevenMod plugins.
    /// </summary>
    public class ConsoleCmdPlugins : ConsoleCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_plugins" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "manages SevenMod plugins";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (_params.Count < 1)
            {
                SdtdConsole.Instance.Output("SevenMod Plugins Menu:");
                SdtdConsole.Instance.Output("    info              - Information about a plugin");
                SdtdConsole.Instance.Output("    list              - Show loaded plugins");
                SdtdConsole.Instance.Output("    load              - Load a plugin");
                SdtdConsole.Instance.Output("    refresh           - Reloads/refreshes all plugins in the plugins.ini file");
                SdtdConsole.Instance.Output("    reload            - Reload a plugin");
                SdtdConsole.Instance.Output("    unload            - Unload a plugin");
                SdtdConsole.Instance.Output("    unload_all        - Unload all plugins");
                return;
            }

            switch (_params[0])
            {
                case "info":
                    this.Info(_params.Count > 1 ? _params[1] : null);
                    break;
                case "list":
                    this.List();
                    break;
                case "load":
                    this.Load(_params.Count > 1 ? _params[1] : null);
                    break;
                case "refresh":
                    this.Refresh();
                    break;
                case "reload":
                    this.Reload(_params.Count > 1 ? _params[1] : null);
                    break;
                case "unload":
                    this.Unload(_params.Count > 1 ? _params[1] : null);
                    break;
                case "unload_all":
                    this.UnloadAll();
                    break;
            }
        }

        /// <summary>
        /// Handles the info sub-command.
        /// </summary>
        /// <param name="name">The name parameter.</param>
        private void Info(string name)
        {
            if (name == null)
            {
                SdtdConsole.Instance.Output("[SM] Usage: sm_plugins info <#|name>");
                return;
            }

            var isIndex = int.TryParse(name, out var index);
            PluginContainer plugin;
            if (isIndex ? PluginManager.GetPlugin(index - 1, out plugin) : PluginManager.GetPlugin(name, out plugin))
            {
                SdtdConsole.Instance.Output($"  Title: {plugin.PluginInfo.Name}");
                SdtdConsole.Instance.Output($"  Author: {plugin.PluginInfo.Author}");
                SdtdConsole.Instance.Output($"  Version: {plugin.PluginInfo.Version}");
                SdtdConsole.Instance.Output($"  URL: {plugin.PluginInfo.Website}");
            }
            else
            {
                SdtdConsole.Instance.Output($"[SM] Plugin {name} is not loaded.");
            }
        }

        /// <summary>
        /// Handles the list sub-command.
        /// </summary>
        private void List()
        {
            var list = PluginManager.Plugins.Values.ToArray();

            SdtdConsole.Instance.Output($"[SM] Listing {list.Length} plugins:");
            for (var i = 0; i < list.Length; i++)
            {
                var p = list[i];
                var error = p.LoadStatus == PluginContainer.Status.Error ? " <Failed>" : string.Empty;
                SdtdConsole.Instance.Output($"{i + 1, 4:d2}{error} \"{p.PluginInfo.Name}\" ({p.PluginInfo.Version}) by {p.PluginInfo.Author}");
            }

            var errored = list.Where((PluginContainer p) => p.LoadStatus == PluginContainer.Status.Error);
            if (errored.Count() > 0)
            {
                SdtdConsole.Instance.Output("Errors:");
                foreach (var plugin in errored)
                {
                    SdtdConsole.Instance.Output($"{plugin.File} ({plugin.PluginInfo.Name}): {plugin.Error}");
                }
            }
        }

        /// <summary>
        /// Handles the load sub-command.
        /// </summary>
        /// <param name="name">The name parameter.</param>
        private void Load(string name)
        {
            if (name == null)
            {
                SdtdConsole.Instance.Output("[SM] Usage: sm_plugins load <name>");
                return;
            }

            PluginManager.Load(name);
        }

        /// <summary>
        /// Handles the refresh sub-command.
        /// </summary>
        private void Refresh()
        {
            PluginManager.Refresh();
        }

        /// <summary>
        /// Handles the reload sub-command.
        /// </summary>
        /// <param name="name">The name parameter.</param>
        private void Reload(string name)
        {
            if (name == null)
            {
                SdtdConsole.Instance.Output("[SM] Usage: sm_plugins reload <#|name>");
                return;
            }

            if (int.TryParse(name, out var index))
            {
                PluginManager.Reload(index - 1);
            }
            else
            {
                PluginManager.Reload(name);
            }
        }

        /// <summary>
        /// Handles the unload sub-command.
        /// </summary>
        /// <param name="name">The name parameter.</param>
        private void Unload(string name)
        {
            if (name == null)
            {
                SdtdConsole.Instance.Output("[SM] Usage: sm_plugins unload <#|name>");
                return;
            }

            if (int.TryParse(name, out var index))
            {
                PluginManager.Unload(index - 1);
            }
            else
            {
                PluginManager.Unload(name);
            }
        }

        /// <summary>
        /// Handles the unload_all sub-command.
        /// </summary>
        private void UnloadAll()
        {
            PluginManager.UnloadAll();
        }
    }
}
