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
            this.LoadTranslations("AdminHelp.Plugin");

            this.RegAdminCmd("help", 0, "Help Description").Executed += this.OnHelpCommandExecuted;
            this.RegAdminCmd("searchcmd", 0, "Searchcmd Description").Executed += this.OnSearchcmdCommandExecuted;
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

            SdtdConsole.Instance.Output(this.GetString("Command Information", e.Client));
            var start = Math.Max(0, page - 1) * CommandsPerPage;
            var list = AdminCommandManager.Commands.Where((AdminCommand c) => c.HasAccess(e.Client)).OrderBy((AdminCommand c) => c.Command).ToArray();
            if (start >= list.Length)
            {
                SdtdConsole.Instance.Output(this.GetString("No commands available", e.Client));
                return;
            }

            var noDescription = this.GetString("No description available", e.Client);
            var end = Math.Min(start + CommandsPerPage - 1, list.Length - 1);
            for (var i = start; i <= end; i++)
            {
                var desc = string.IsNullOrEmpty(list[i].Description) ? noDescription : this.GetString(list[i].Description, e.Client);
                SdtdConsole.Instance.Output($"[{i + 1:D3}] sm {list[i].Command} - {desc}");
            }

            SdtdConsole.Instance.Output(this.GetString("Entries", e.Client, start + 1, end + 1, page));
            if (end < list.Length - 2)
            {
                SdtdConsole.Instance.Output(this.GetString("See More Commands", e.Client, page + 1));
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
                SdtdConsole.Instance.Output(this.GetString("No matching results found", e.Client));
                return;
            }

            var noDescription = this.GetString("No description available", e.Client);
            for (var i = 0; i < list.Length; i++)
            {
                var desc = string.IsNullOrEmpty(list[i].Description) ? noDescription : this.GetString(list[i].Description, e.Client);
                SdtdConsole.Instance.Output($"[{i + 1:D3}] sm {list[i].Command} - {desc}");
            }
        }
    }
}
