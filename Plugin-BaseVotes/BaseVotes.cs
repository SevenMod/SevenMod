// <copyright file="BaseVotes.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using SevenMod.Admin;
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Plugin that adds the vote, votekick, and voteban admin commands.
    /// </summary>
    public class BaseVotes : PluginAbstract
    {
        /// <summary>
        /// The value of the VoteBanPercent <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue voteBanPercent;

        /// <summary>
        /// The value of the VoteKickPercent <see cref="ConVar"/>.
        /// </summary>
        private ConVarValue voteKickPercent;

        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Votes",
            Author = "SevenMod",
            Description = "Adds basic voting commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/SevenMod/SevenMod"
        };

        /// <inheritdoc/>
        public override void OnLoadPlugin()
        {
            this.LoadTranslations("BaseVotes.Plugin");

            this.voteBanPercent = this.CreateConVar("VoteBanPercent", "0.60", "The percentage of players that must vote yes for a successful ban vote.", true, 0, true, 1).Value;
            this.voteKickPercent = this.CreateConVar("VoteKickPercent", "0.60", "The percentage of players that must vote yes for a successful kick vote.", true, 0, true, 1).Value;

            this.AutoExecConfig(true, "BaseVotes");

            this.RegAdminCmd("vote", AdminFlags.Vote, "Vote Description").Executed += this.OnVoteCommandExecuted;
            this.RegAdminCmd("voteban", AdminFlags.Vote, "Voteban Description").Executed += this.OnVotebanCommandExecuted;
            this.RegAdminCmd("votekick", AdminFlags.Vote, "Votekick Description").Executed += this.OnVotekickCommandExecuted;
        }

        /// <summary>
        /// Called when the vote admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnVoteCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (e.Arguments.Count > 5)
            {
                this.ReplyToCommand(e.Client, "Too many options");
                return;
            }

            if (VoteManager.CreateVote(e.Arguments[0]).SetOptions(e.Arguments.GetRange(1, e.Arguments.Count - 1)).Start())
            {
                this.ShowActivity(e.Client, "Initiated Vote", e.Arguments[0]);
                VoteManager.CurrentVote.Ended += this.OnVoteEnded;
            }
        }

        /// <summary>
        /// Called when a generic vote ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="VoteEndedEventArgs"/> object containing the event data.</param>
        private void OnVoteEnded(object sender, VoteEndedEventArgs e)
        {
            this.PrintToChatAll("Voting has ended");
            for (var i = 0; i < e.Options.Length; i++)
            {
                this.PrintToChatAll("Vote Results", e.Options[i], e.Percents[i], e.Votes[i]);
            }
        }

        /// <summary>
        /// Called when the voteban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnVotebanCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (this.ParseSingleTargetString(e.Client, e.Arguments[0], out var target))
            {
                if (VoteManager.CreateVote("Voteban Started", target.PlayerName).SetData(target).Start())
                {
                    this.ShowActivity(e.Client, "Initiated Vote Ban", target.PlayerName);
                    VoteManager.CurrentVote.Ended += this.OnBanVoteEnded;
                }
            }
        }

        /// <summary>
        /// Called when a ban vote ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="VoteEndedEventArgs"/> object containing the event data.</param>
        private void OnBanVoteEnded(object sender, VoteEndedEventArgs e)
        {
            if (e.Percents[0] >= this.voteBanPercent.AsFloat)
            {
                var target = e.Data as SMClient;
                this.PrintToChatAll("Voteban Succeeded", e.Percents[0], target.PlayerName);
                SdtdConsole.Instance.ExecuteSync(this.GetString("Vote banned", target, target.PlayerId), null);
            }
            else
            {
                this.PrintToChatAll("Vote Failed", e.Percents[0]);
            }
        }

        /// <summary>
        /// Called when the votekick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void OnVotekickCommandExecuted(object sender, AdminCommandEventArgs e)
        {
            if (e.Arguments.Count < 1)
            {
                this.ReplyToCommand(e.Client, "Not enough parameters");
                return;
            }

            if (this.ParseSingleTargetString(e.Client, e.Arguments[0], out var target))
            {
                if (VoteManager.CreateVote("Votekick Started", target.PlayerName).SetData(target).Start())
                {
                    this.ShowActivity(e.Client, "Initiated Vote Kick", target.PlayerName);
                    VoteManager.CurrentVote.Ended += this.OnKickVoteEnded;
                }
            }
        }

        /// <summary>
        /// Called when a kick vote ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="VoteEndedEventArgs"/> object containing the event data.</param>
        private void OnKickVoteEnded(object sender, VoteEndedEventArgs e)
        {
            if (e.Percents[0] >= this.voteKickPercent.AsFloat)
            {
                var target = e.Data as SMClient;
                this.PrintToChatAll("Votekick Succeeded", e.Percents[0], target.PlayerName);
                SdtdConsole.Instance.ExecuteSync(this.GetString("Vote kicked", target, target.PlayerId), null);
            }
            else
            {
                this.PrintToChatAll("Vote Failed", e.Percents[0]);
            }
        }
    }
}
