// <copyright file="AdminCmdVoteKick.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_votekick</para>
    /// <para>Starts a vote to kick a player from the server.</para>
    /// </summary>
    public class AdminCmdVoteKick : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_votekick" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "starts a vote to kick a player from the server";
        }

        /// <inheritdoc/>
        public override void Exec(List<string> args, CommandSenderInfo senderInfo)
        {
            if (VoteManager.Instance.VoteInProgress)
            {
                return;
            }

            if (args.Count < 1)
            {
                this.ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            string playerId;
            ClientInfo target;
            if (ConsoleHelper.ParseParamPartialNameOrId(args[0], out playerId, out target) != 1)
            {
                this.ReplyToCommand(senderInfo, "No valid targets found");
                return;
            }

            if (!AdminManager.CanTarget(senderInfo.RemoteClientInfo, target))
            {
                this.ReplyToCommand(senderInfo, $"Cannot target {target.playerName}");
                return;
            }

            var message = $"A vote has begun to kick {target.playerName} from the server";
            var listener = new VoteKickListener(target);
            VoteManager.Instance.StartVote(message, null, listener);
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

                foreach (var client in ConnectionManager.Instance.GetClients())
                {
                    client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, "[Vote]", false, "SevenMod", false));
                }
            }
        }
    }
}
