// <copyright file="AdminCmdVote.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseVotes
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_vote</para>
    /// <para>Starts a generic vote with a custom question and up to four answer options.</para>
    /// </summary>
    public class AdminCmdVote : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_vote" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "starts a vote";
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

            if (args.Count > 5)
            {
                this.ReplyToCommand(senderInfo, "Too many options");
                return;
            }

            VoteManager.Instance.StartVote(args[0], args.GetRange(1, args.Count - 1), new VoteListener());
        }

        /// <summary>
        /// Represents a generic vote results listener.
        /// </summary>
        public class VoteListener : IVoteResultListener
        {
            /// <inheritdoc/>
            public void OnVoteEnd(string[] options, int[] votes, float[] percents)
            {
                string msg;
                foreach (var client in ConnectionManager.Instance.GetClients())
                {
                    client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, "Voting has ended", "[Vote]", false, "SevenMod", false));
                    for (var i = 0; i < options.Length; i++)
                    {
                        msg = string.Format("{0}: {1:P2} ({2} votes)", options[i], percents[i], votes[i]);
                        client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, msg, "[Result]", false, "SevenMod", false));
                    }
                }
            }
        }
    }
}
