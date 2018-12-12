// <copyright file="ConsoleCmdPlugins.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;

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
                    if (_params.Count < 2)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins info <name>");
                        return;
                    }

                    var info = PluginManager.GetPluginInfo(_params[1])?.PluginInfo;
                    if (info.HasValue)
                    {
                        SdtdConsole.Instance.Output($"  Title: {info.Value.Name}");
                        SdtdConsole.Instance.Output($"  Author: {info.Value.Author}");
                        SdtdConsole.Instance.Output($"  Version: {info.Value.Version}");
                        SdtdConsole.Instance.Output($"  URL: {info.Value.Website}");
                    }
                    else
                    {
                        SdtdConsole.Instance.Output($"[SM] Plugin {_params[1]} is not loaded.");
                    }

                    break;
                case "list":
                    var list = new List<PluginContainer>(PluginManager.Plugins.Values);

                    var active = list.FindAll((PluginContainer p) => p.LoadStatus == PluginContainer.Status.Loaded);
                    SdtdConsole.Instance.Output($"[SM] Listing {active.Count} plugins:");
                    for (var i = 0; i < active.Count; i++)
                    {
                        var p = active[i].PluginInfo;
                        SdtdConsole.Instance.Output($"{i + 1, 4:d2} \"{p.Name}\" ({p.Version}) by {p.Author}");
                    }

                    var errored = list.FindAll((PluginContainer p) => p.LoadStatus == PluginContainer.Status.Error);
                    if (errored.Count > 0)
                    {
                        SdtdConsole.Instance.Output("Errors:");
                        foreach (var plugin in errored)
                        {
                            SdtdConsole.Instance.Output($"{plugin.File} ({plugin.PluginInfo.Name}): {plugin.Error}");
                        }
                    }

                    break;
                case "load":
                    if (_params.Count < 2)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins load <name>");
                        return;
                    }

                    PluginManager.Load(_params[1]);
                    break;
                case "refresh":
                    PluginManager.Refresh();
                    break;
                case "reload":
                    if (_params.Count < 2)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins reload <name>");
                        return;
                    }

                    PluginManager.Reload(_params[1]);
                    break;
                case "unload":
                    if (_params.Count < 2)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins unload <name>");
                        return;
                    }

                    PluginManager.Unload(_params[1]);
                    break;
                case "unload_all":
                    PluginManager.UnloadAll();
                    break;
            }
        }
    }
}
