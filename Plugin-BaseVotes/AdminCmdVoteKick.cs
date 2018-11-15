// <copyright file="AdminCmdVoteKick.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using System.Collections.Generic;
    using SevenMod.Chat;
    using SevenMod.Core;
    using SevenMod.Voting;

    /// <summary>
    /// Admin command that starts a vote to kick a player from the server.
    /// </summary>
    public class AdminCmdVoteKick : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string Description => "starts a vote to kick a player from the server";

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
                var message = $"A vote has begun to kick {target.playerName} from the server";
                var listener = new VoteKickListener(target);
                VoteManager.Instance.StartVote(message, null, listener);
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
            /// Initializes a new instance of the <see cref="VoteKickListener"/> class.
            /// </summary>
            /// <param name="target">The target client for the kick vote.</param>
            public VoteKickListener(ClientInfo target)
            {
                this.target = target;
            }

            /// <inheritdoc/>
            public void OnVoteEnd(string[] options, int[] votes, float[] percents)
            {
                string message;
                if (percents[0] >= BaseVotesConfig.Instance.VoteKickPercent)
                {
                    message = string.Format("Vote succeeded with {0:P2} of the vote. Kicking {1}...", percents[0], this.target.playerName);
                    SdtdConsole.Instance.ExecuteSync($"kick {this.target.playerId} \"Vote kicked\"", null);
                }
                else
                {
                    message = string.Format("Vote failed with {0:P2} of the vote.", percents[0]);
                }

                ChatHelper.SendToAll(message, "[Vote]");
            }
        }
    }
}
