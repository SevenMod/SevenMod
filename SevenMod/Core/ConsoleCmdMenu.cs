// <copyright file="ConsoleCmdMenu.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System.Collections.Generic;

    /// <summary>
    /// Console command for managing SevenMod features.
    /// </summary>
    public class ConsoleCmdMenu : ConsoleCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_menu" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "manages SevenMod";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (_params.Count < 1)
            {
                SdtdConsole.Instance.Output("SevenMod Menu:");
                SdtdConsole.Instance.Output("Usage: sm <command> [arguments]");
                SdtdConsole.Instance.Output($"    plugins           - Manage plugins");
                return;
            }

            switch (_params[0])
            {
                case "plugins":
                    this.HandlePluginsCommand(_params);
                    break;
            }
        }

        /// <summary>
        /// Handle the plugin management commands.
        /// </summary>
        /// <param name="args">The list of command arguments.</param>
        private void HandlePluginsCommand(List<string> args)
        {
            if (args.Count < 2)
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

            switch (args[1])
            {
                case "info":
                    if (args.Count < 3)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins info <name>");
                        return;
                    }

                    var info = PluginManager.GetPluginInfo(args[2]);
                    if (info.HasValue)
                    {
                        SdtdConsole.Instance.Output($"  Title: {info.Value.Name}");
                        SdtdConsole.Instance.Output($"  Author: {info.Value.Author}");
                        SdtdConsole.Instance.Output($"  Version: {info.Value.Version}");
                        SdtdConsole.Instance.Output($"  URL: {info.Value.Website}");
                    }
                    else
                    {
                        SdtdConsole.Instance.Output($"[SM] Plugin {args[2]} is not loaded.");
                    }

                    break;
                case "list":
                    var list = PluginManager.Plugins;
                    SdtdConsole.Instance.Output($"[SM] Listing {list.Count} plugins:");
                    for (var i = 0; i < list.Count; i++)
                    {
                        var p = list[i].Info;
                        SdtdConsole.Instance.Output($"{i + 1, 4:d2} \"{p.Name}\" ({p.Version}) by {p.Author}");
                    }

                    break;
                case "load":
                    if (args.Count < 3)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins load <name>");
                        return;
                    }

                    PluginManager.Load(args[2]);
                    break;
                case "refresh":
                    PluginManager.Refresh();
                    break;
                case "reload":
                    if (args.Count < 3)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins reload <name>");
                        return;
                    }

                    PluginManager.Reload(args[2]);
                    break;
                case "unload":
                    if (args.Count < 3)
                    {
                        SdtdConsole.Instance.Output("[SM] Usage: sm plugins unload <name>");
                        return;
                    }

                    PluginManager.Unload(args[2]);
                    break;
                case "unload_all":
                    PluginManager.UnloadAll();
                    break;
            }
        }
    }
}
