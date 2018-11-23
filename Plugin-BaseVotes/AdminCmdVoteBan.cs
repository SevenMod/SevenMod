// <copyright file="AdminCmdVoteBan.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using System.Collections.Generic;
    using SevenMod.Chat;
    using SevenMod.Console;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Admin command that starts a vote to temporarily ban a player from the server.
    /// </summary>
    public class AdminCmdVoteBan : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "starts a vote to temporarily ban a player from the server";

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (VoteManager.Instance.VoteInProgress)
            {
                return;
            }

            if (args.Count < 1)
            {
                ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            var target = this.ParseSingleTargetString(senderInfo, args[0]);
            if (target != null)
            {
                var message = $"A vote has begun to ban {target.playerName} from the server";
                var listener = new VoteBanListener(target);
                VoteManager.Instance.StartVote(message, null, listener);
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
            /// The value of the VoteBanPercent console value.
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
    }
}
