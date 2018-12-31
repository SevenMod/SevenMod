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

            if (this.ParseTargetString(e.Client, e.Arguments[0], out var targets, out var targetName, out var nameIsPhrase) > 0)
            {
                if (nameIsPhrase)
                {
                    this.ShowActivity(e.Client, "Slayed target", targetName);
                }
                else if (targetName != null)
                {
                    this.ShowActivity(e.Client, "Slayed player", targetName);
                }

                foreach (var target in targets)
                {
                    this.LogAction(e.Client, target, "\"{0:L}\" slayed \"{1:L}\"", e.Client, target);
                    if (targetName == null)
                    {
                        this.ShowActivity(e.Client, "Slayed player", target.PlayerName);
                    }

                    SdtdConsole.Instance.ExecuteSync($"kill {target.PlayerId}", null);
                }
            }
        }
    }
}
