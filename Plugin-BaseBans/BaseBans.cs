// <copyright file="BaseBans.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds the ban, addban, and unban admin commands.
    /// </summary>
    public class BaseBans : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Ban Commands",
            Author = "SevenMod",
            Description = "Adds basic banning commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.RegAdminCmd("addban", AdminFlags.Ban, "Adds a player to the server ban list").Executed += this.OnAddbanCommandExecuted;
            this.RegAdminCmd("ban", AdminFlags.Ban, "Bans a player from the server").Executed += this.OnBanCommandExecuted;
            this.RegAdminCmd("unban", AdminFlags.Unban, "Unbans a player from the server").Executed += this.OnUnbanCommandExecuted;
        }

        /// <summary>
        /// Called when the addban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnAddbanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            if (!ConsoleHelper.ParseParamSteamIdValid(e.Arguments[0]))
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Invalid player ID");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"ban add {e.Arguments[0]}", null);
        }

        /// <summary>
        /// Called when the ban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnBanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 2)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            if (!int.TryParse(e.Arguments[1], out int duration) || duration < 0)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Invaid ban duration");
                return;
            }

            var target = SMConsoleHelper.ParseSingleTargetString(e.SenderInfo, e.Arguments[0]);
            if (target != null)
            {
                var unit = "minutes";
                if (duration == 0)
                {
                    unit = "years";
                    duration = 999999;
                }

                SdtdConsole.Instance.ExecuteSync($"ban add {target.playerId} {duration} {unit}", null);
            }
        }

        /// <summary>
        /// Called when the unban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnUnbanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            if (!ConsoleHelper.ParseParamSteamIdValid(e.Arguments[0]))
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Invalid player ID");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"ban remove {e.Arguments[0]}", null);
        }
    }
}
