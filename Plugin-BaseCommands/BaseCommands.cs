// <copyright file="BaseCommands.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseCommands
{
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Plugin that adds basic admin commands.
    /// </summary>
    public class BaseCommands : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Commands",
            Author = "SevenMod",
            Description = "Adds basic admin commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.RegAdminCmd("cancelvote", AdminFlags.Vote, "Cancels a vote in progress").Executed += this.OnCancelvoteCommandExecuted;
            this.RegAdminCmd("cvar", AdminFlags.Convars, "Chages a SevenMod console variable.").Executed += this.OnCvarCommandExecuted;
            this.RegAdminCmd("kick", AdminFlags.Kick, "Kicks a player from the server").Executed += this.OnKickCommandExecuted;
            this.RegAdminCmd("rcon", AdminFlags.RCON, "Executes a command on the server console").Executed += this.OnRconCommandExecuted;
            this.RegAdminCmd("reloadadmins", AdminFlags.Ban, "Reloads the admin list").Executed += this.OnReloadadminsCommandExecuted;
            this.RegAdminCmd("resetcvar", AdminFlags.Convars, "Resets a SevenMod console variable to its default value").Executed += this.OnResetcvarCommandExecuted;
            this.RegAdminCmd("who", AdminFlags.Generic, "Lists connected clients and their access flags").Executed += this.OnWhoCommandExecuted;
        }

        /// <summary>
        /// Called when the cancelvote admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnCancelvoteCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (!VoteManager.VoteInProgress)
            {
                this.ReplyToCommand(e.Client, "A vote is not currently in progress");
                return;
            }

            VoteManager.CurrentVote.Cancel();
            this.PrintToChatAll("The vote was cancelled");

            if (!this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, "The vote was cancelled");
            }
        }

        /// <summary>
        /// Called when the kick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnCvarCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            var cvar = this.FindConVar(e.Arguments[0]);
            if (cvar == null)
            {
                this.ReplyToCommand(e.Client, $"Unable to find ConVar \"{e.Arguments[0]}\"");
                return;
            }

            if (e.Arguments.Count == 1)
            {
                this.ReplyToCommand(e.Client, $"Value of \"{cvar.Name}\": {cvar.Value.AsString}");
            }
            else
            {
                cvar.Value.Value = string.Join(" ", e.Arguments.GetRange(1, e.Arguments.Count - 1).ToArray());
                this.ReplyToCommand(e.Client, $"Changed ConVar \"{cvar.Name}\" to \"{cvar.Value.AsString}\"");
            }
        }

        /// <summary>
        /// Called when the kick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnKickCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            foreach (var target in this.ParseTargetString(e.Client, e.Arguments[0]))
            {
                SdtdConsole.Instance.ExecuteSync($"kick {target.PlayerId}", null);
            }
        }

        /// <summary>
        /// Called when the rcon admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnRconCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            this.ServerCommand(string.Join(" ", e.Arguments.ToArray()), e.Client);
        }

        /// <summary>
        /// Called when the reloadadmins admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnReloadadminsCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            AdminManager.ReloadAdmins();
        }

        /// <summary>
        /// Called when the kick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnResetcvarCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            var name = string.Join(" ", e.Arguments.ToArray());
            var cvar = this.FindConVar(name);
            if (cvar == null)
            {
                this.ReplyToCommand(e.Client, $"Unable to find ConVar \"{name}\"");
                return;
            }

            cvar.Reset();
            this.ReplyToCommand(e.Client, $"Changed ConVar \"{cvar.Name}\" to \"{cvar.Value.AsString}\"");
        }

        /// <summary>
        /// Called when the who admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnWhoCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, "See console for output");
            }

            SdtdConsole.Instance.Output($"  {"Name", -24} {"Username", -18} {"Admin access"}");
            foreach (var client in GameManager.Instance.World.Players.dict.Values)
            {
                var player = ConnectionManager.Instance.Clients.ForEntityId(client.entityId);
                var admin = AdminManager.GetAdmin(player.playerId);
                if (admin != null)
                {
                    var flags = string.Empty;
                    foreach (var keyValue in AdminManager.AdminFlagKeys)
                    {
                        if ((admin.Flags & keyValue.Value) == keyValue.Value)
                        {
                            flags += keyValue.Key;
                        }
                    }

                    SdtdConsole.Instance.Output($"  {player.playerName, -24} {player.playerId, -18} {flags}");
                }
            }
        }
    }
}
