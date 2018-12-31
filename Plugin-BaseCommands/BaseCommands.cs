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
            this.LoadTranslations("BaseCommands.Plugin");

            this.RegAdminCmd("cancelvote", AdminFlags.Vote, "Cancelvote Description").Executed += this.OnCancelvoteCommandExecuted;
            this.RegAdminCmd("cvar", AdminFlags.Convars, "Cvar Description").Executed += this.OnCvarCommandExecuted;
            this.RegAdminCmd("kick", AdminFlags.Kick, "Kick Description").Executed += this.OnKickCommandExecuted;
            this.RegAdminCmd("rcon", AdminFlags.RCON, "Rcon Description").Executed += this.OnRconCommandExecuted;
            this.RegAdminCmd("reloadadmins", AdminFlags.Ban, "Reloadadmins Description").Executed += this.OnReloadadminsCommandExecuted;
            this.RegAdminCmd("resetcvar", AdminFlags.Convars, "Resetcvar Description").Executed += this.OnResetcvarCommandExecuted;
            this.RegAdminCmd("who", AdminFlags.Generic, "Who Description").Executed += this.OnWhoCommandExecuted;
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
                this.ReplyToCommand(e.Client, "Vote not in progress");
                return;
            }

            VoteManager.CurrentVote.Cancel();
            this.ShowActivity(e.Client, "Cancelled vote");

            if (!this.ShouldReplyToChat(e.Client))
            {
                this.ReplyToCommand(e.Client, "Cancelled vote");
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
                this.ReplyToCommand(e.Client, "Unable to find ConVar", e.Arguments[0]);
                return;
            }

            if (e.Arguments.Count == 1)
            {
                this.ReplyToCommand(e.Client, "Value of ConVar", cvar.Name, cvar.Value.AsString);
            }
            else
            {
                cvar.Value.Value = string.Join(" ", e.Arguments.GetRange(1, e.Arguments.Count - 1).ToArray());
                this.LogAction(e.Client, null, "\"{1:L}\" changed cvar (cvar \"{2:s}\") (value \"{3:s}\")", e.Client, cvar.Name, cvar.Value.AsString);
                this.ReplyToCommand(e.Client, "Changed ConVar", cvar.Name, cvar.Value.AsString);
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

            if (this.ParseTargetString(e.Client, e.Arguments[0], out var targets, out var targetName, out var nameIsPhrase) > 0)
            {
                if (nameIsPhrase)
                {
                    this.ShowActivity(e.Client, "Kicked target", targetName);
                }
                else if (targetName != null)
                {
                    this.ShowActivity(e.Client, "Kicked player", targetName);
                }

                SMClient self = null;
                foreach (var target in targets)
                {
                    this.LogAction(e.Client, target, "\"{1:L}\" kicked \"{2:L}\"", e.Client, target);
                    if (target == e.Client)
                    {
                        self = target;
                    }
                    else
                    {
                        if (targetName == null)
                        {
                            this.ShowActivity(e.Client, "Kicked player", target.PlayerName);
                        }

                        SdtdConsole.Instance.ExecuteSync($"kick {target.PlayerId}", null);
                    }
                }

                if (self != null)
                {
                    if (targetName == null)
                    {
                        this.ShowActivity(e.Client, "Kicked player", self.PlayerName);
                    }

                    SdtdConsole.Instance.ExecuteSync($"kick {self.PlayerId}", null);
                }
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

            var command = string.Join(" ", e.Arguments.ToArray());
            this.LogAction(e.Client, null, "\"{1:L}\" console command (cmdline \"{2:s}\")", e.Client, command);
            this.ServerCommand(command, e.Client);
        }

        /// <summary>
        /// Called when the reloadadmins admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnReloadadminsCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            this.LogAction(e.Client, null, "\"{1:L}\" refreshed the admin cache.", e.Client);
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
                this.ReplyToCommand(e.Client, "Unable to find ConVar", name);
                return;
            }

            cvar.Reset();
            this.LogAction(e.Client, null, "\"{1:L}\" reset cvar (cvar \"{2:s}\") (value \"{3:s}\")", e.Client, cvar.Name, cvar.Value.AsString);
            this.ReplyToCommand(e.Client, "Changed ConVar", cvar.Name, cvar.Value.AsString);
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

            SdtdConsole.Instance.Output(this.GetString("Who Columns", e.Client));
            foreach (var client in GameManager.Instance.World.Players.dict.Values)
            {
                var player = ConnectionManager.Instance.Clients.ForEntityId(client.entityId);
                var admin = AdminManager.GetAdmin(player.playerId);
                var flags = string.Empty;
                if (admin.Flags == 0)
                {
                    flags = this.GetString("none", e.Client);
                }
                else if ((admin.Flags & AdminFlags.Root) == AdminFlags.Root)
                {
                    flags = this.GetString("root", e.Client);
                }
                else
                {
                    foreach (var keyValue in AdminManager.AdminFlagKeys)
                    {
                        if ((admin.Flags & keyValue.Value) == keyValue.Value)
                        {
                            flags += keyValue.Key;
                        }
                    }
                }

                SdtdConsole.Instance.Output($"  {player.playerName, -24} {player.playerId, -18} {flags}");
            }
        }
    }
}
