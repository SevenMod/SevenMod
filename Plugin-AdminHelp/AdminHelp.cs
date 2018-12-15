// <copyright file="AdminHelp.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.AdminHelp
{
    using System;
    using System.Linq;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that displays admin command information.
    /// </summary>
    public class AdminHelp : PluginAbstract
    {
        /// <summary>
        /// The number of commands to display per page.
        /// </summary>
        private static readonly int CommandsPerPage = 10;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Admin Help",
            Author = "SevenMod",
            Description = "Displays admin command information.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.RegAdminCmd("help", 0, "Displays SevenMod commands and descriptions").Executed += this.OnHelpCommandExecuted;
            this.RegAdminCmd("searchcmd", 0, "Searches SevenMod commands").Executed += this.OnSearchcmdCommandExecuted;
        }

        /// <summary>
        /// Called when the help admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnHelpCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, "See console for output");
            }

            var page = 1;
            if (e.Arguments.Count > 0)
            {
                int.TryParse(e.Arguments[0], out page);
            }

            SdtdConsole.Instance.Output("SevenMod Help: Command Information");
            var start = Math.Max(0, page - 1) * CommandsPerPage;
            var list = AdminCommandManager.Commands.Where((AdminCommand c) => c.HasAccess(e.Client)).OrderBy((AdminCommand c) => c.Command).ToArray();
            if (start >= list.Length)
            {
                SdtdConsole.Instance.Output("No commands available");
                return;
            }

            var end = Math.Min(start + CommandsPerPage - 1, list.Length - 1);
            for (var i = start; i <= end; i++)
            {
                var desc = string.IsNullOrEmpty(list[i].Description) ? "No description available" : list[i].Description;
                SdtdConsole.Instance.Output($"[{i + 1:D3}] sm {list[i].Command} - {desc}");
            }

            SdtdConsole.Instance.Output($"Entries {start + 1} - {end + 1} in page {page}");
            if (end < list.Length - 2)
            {
                SdtdConsole.Instance.Output($"Type sm_help {page + 1} to see more commands");
            }
        }

        /// <summary>
        /// Called when the searchcmd admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnSearchcmdCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, "See console for output");
            }

            var search = e.Arguments.Count > 0 ? e.Arguments[0] : string.Empty;
            var list = AdminCommandManager.Commands.Where((AdminCommand c) => c.HasAccess(e.Client) && c.Command.Contains(search)).OrderBy((AdminCommand c) => c.Command).ToArray();
            if (list.Length < 1)
            {
                SdtdConsole.Instance.Output("No matching results found");
                return;
            }

            for (var i = 0; i < list.Length; i++)
            {
                var desc = string.IsNullOrEmpty(list[i].Description) ? "No description available" : list[i].Description;
                SdtdConsole.Instance.Output($"[{i + 1:D3}] sm {list[i].Command} - {desc}");
            }
        }
    }
}
