// <copyright file="BaseVotes.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using SevenMod.Admin;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.ConVar;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Plugin that adds the vote, votekick, and voteban admin commands.
    /// </summary>
    public class BaseVotes : PluginAbstract
    {
        /// <inheritdoc/>
        public override PluginInfo Info => new PluginInfo
        {
            Name = "Basic Votes",
            Author = "SevenMod",
            Description = "Adds basic voting commands.",
            Version = "0.1.0.0",
            Website = "https://github.com/stevotvr/sevenmod"
        };

        /// <inheritdoc/>
        public override void LoadPlugin()
        {
            base.LoadPlugin();

            this.CreateConVar("VoteBanPercent", "0.60", "The percentage of players that must vote yes for a successful ban vote.", true, 0, true, 1);
            this.CreateConVar("VoteKickPercent", "0.60", "The percentage of players that must vote yes for a successful kick vote.", true, 0, true, 1);

            this.AutoExecConfig(true, "BaseVotes");

            this.RegAdminCmd("vote", AdminFlags.Vote, "Starts a vote").Executed += this.VoteExecuted;
            this.RegAdminCmd("voteban", AdminFlags.Vote, "Starts a vote to temporarily ban a player from the server").Executed += this.VoteBanExecuted;
            this.RegAdminCmd("votekick", AdminFlags.Vote, "Starts a vote to kick a player from the server").Executed += this.VoteKickExecuted;
        }

        /// <summary>
        /// Called when the vote admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void VoteExecuted(object sender, AdminCommandEventArgs e)
        {
            if (VoteManager.Instance.VoteInProgress)
            {
                return;
            }

            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            if (e.Arguments.Count > 5)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Too many options");
                return;
            }

            VoteManager.Instance.StartVote(e.Arguments[0], e.Arguments.GetRange(1, e.Arguments.Count - 1), new VoteListener());
        }

        /// <summary>
        /// Called when the voteban admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void VoteBanExecuted(object sender, AdminCommandEventArgs e)
        {
            if (VoteManager.Instance.VoteInProgress)
            {
                return;
            }

            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            var target = SMConsoleHelper.ParseSingleTargetString(e.SenderInfo, e.Arguments[0]);
            if (target != null)
            {
                var message = $"A vote has begun to ban {target.playerName} from the server";
                var listener = new VoteBanListener(target);
                VoteManager.Instance.StartVote(message, null, listener);
            }
        }

        /// <summary>
        /// Called when the votekick admin command is executed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="AdminCommandEventArgs"/> object containing the event data.</param>
        private void VoteKickExecuted(object sender, AdminCommandEventArgs e)
        {
            if (VoteManager.Instance.VoteInProgress)
            {
                return;
            }

            if (e.Arguments.Count < 1)
            {
                ChatHelper.ReplyToCommand(e.SenderInfo, "Not enough parameters");
                return;
            }

            var target = SMConsoleHelper.ParseSingleTargetString(e.SenderInfo, e.Arguments[0]);
            if (target != null)
            {
                var message = $"A vote has begun to kick {target.playerName} from the server";
                var listener = new VoteKickListener(target);
                VoteManager.Instance.StartVote(message, null, listener);
            }
        }

        /// <summary>
        /// Represents a generic vote results listener.
        /// </summary>
        public class VoteListener : IVoteResultListener
        {
            /// <inheritdoc/>
            public void OnVoteEnd(string[] options, int[] votes, float[] percents)
            {
                ChatHelper.SendToAll("Voting has ended", "Vote");
                for (var i = 0; i < options.Length; i++)
                {
                    ChatHelper.SendToAll(string.Format("{0}: {1:P2} ({2} votes)", options[i], percents[i], votes[i]), "Result");
                }
            }
        }

        /// <summary>
        /// Represents a ban vote result listener.
        /// </summary>
        public class VoteBanListener : IVoteResultListener
        {
            /// <summary>
            /// The target client for the ban vote.
            /// </summary>
            private ClientInfo target;

            /// <summary>
            /// The value of the VoteBanPercent <see cref="ConVar"/>.
            /// </summary>
            private ConVarValue percent;

            /// <summary>
            /// Initializes a new instance of the <see cref="VoteBanListener"/> class.
            /// </summary>
            /// <param name="target">The target client for the ban vote.</param>
            public VoteBanListener(ClientInfo target)
            {
                this.target = target;
                this.percent = ConVarManager.FindConVar("VoteBanPercent").Value;
            }

            /// <inheritdoc/>
            public void OnVoteEnd(string[] options, int[] votes, float[] percents)
            {
                string message;
                if (percents[0] >= this.percent.AsFloat)
                {
                    message = string.Format("Vote succeeded with {0:P2} of the vote. Banning {1}...", percents[0], this.target.playerName);
                    SdtdConsole.Instance.ExecuteSync($"ban add {this.target.playerId} 30 minutes \"Vote banned\"", null);
                }
                else
                {
                    message = string.Format("Vote failed with {0:P2} of the vote.", percents[0]);
                }

                ChatHelper.SendToAll(message, "Vote");
            }
        }

        /// <summary>
        /// Represents a kick vote result listener.
        /// </summary>
        public class VoteKickListener : IVoteResultListener
        {
            /// <summary>
            /// The target client for the kick vote.
            /// </summary>
            private ClientInfo target;

            /// <summary>
            /// The value of the VoteKickPercent <see cref="ConVar"/>.
            /// </summary>
            private ConVarValue percent;

            /// <summary>
            /// Initializes a new instance of the <see cref="VoteKickListener"/> class.
            /// </summary>
            /// <param name="target">The target client for the kick vote.</param>
            public VoteKickListener(ClientInfo target)
            {
                this.target = target;
                this.percent = ConVarManager.FindConVar("VoteKickPercent").Value;
            }

            /// <inheritdoc/>
            public void OnVoteEnd(string[] options, int[] votes, float[] percents)
            {
                string message;
                if (percents[0] >= this.percent.AsFloat)
                {
                    message = string.Format("Vote succeeded with {0:P2} of the vote. Kicking {1}...", percents[0], this.target.playerName);
                    SdtdConsole.Instance.ExecuteSync($"kick {this.target.playerId} \"Vote kicked\"", null);
                }
                else
                {
                    message = string.Format("Vote failed with {0:P2} of the vote.", percents[0]);
                }

                ChatHelper.SendToAll(message, "Vote");
            }
        }
    }
}
