// <copyright file="PlayerCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.PlayerCommands
{
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.Core;

    /// <summary>
    /// Plugin that adds miscellaneous player admin commands.
    /// </summary>
    public class PlayerCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Player Commands",
            Author = "SevenMod",
            Description = "Adds miscellaneous player admin commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("PlayerCommands.Plugin");

            this.RegAdminCmd("slay", AdminFlags.Slay, "Slay Description").Executed += this.OnSlayCommandExecuted;
        }

        /// <summary>
        /// Called when the slay admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnSlayCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            foreach (var target in this.ParseTargetString(e.Client, e.Arguments[0]))
            {
                SdtdConsole.Instance.ExecuteSync($"kill {target.PlayerId}", null);
                this.ShowActivity(e.Client, "Slapped target", target.PlayerName);
            }
        }
    }
}
