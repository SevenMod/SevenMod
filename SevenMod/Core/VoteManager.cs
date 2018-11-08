// <copyright file="VoteManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages the voting system.
    /// </summary>
    public sealed class VoteManager : IDisposable, IChatHookListener
    {
        /// <summary>
        /// The timer for the currently running vote.
        /// </summary>
        private System.Timers.Timer timer;

        /// <summary>
        /// The options for the currently running vote.
        /// </summary>
        private string[] voteOptions;

        /// <summary>
        /// The value indicating whether the currently running vote is a yes/no vote.
        /// </summary>
        private bool boolVote;

        /// <summary>
        /// The map of eligible voter auth IDs to their selected option item index in the currently
        /// running vote.
        /// </summary>
        private Dictionary<string, int> votingPool = new Dictionary<string, int>();

        /// <summary>
        /// The <see cref="IVoteResultListener"/> for the currently running vote.
        /// </summary>
        private IVoteResultListener voteListener;

        /// <summary>
        /// Gets the singleton instance of the <see cref="VoteManager"/> class.
        /// </summary>
        public static VoteManager Instance { get; } = new VoteManager();

        /// <summary>
        /// Gets a value indicating whether a vote is in progress.
        /// </summary>
        public bool VoteInProgress { get; private set; }

        /// <summary>
        /// Starts a vote if one is not already in progress.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="options">The list of answer options; <c>null</c> for a boolean yes/no
        /// vote.</param>
        /// <param name="listener">The vote result listener.</param>
        public void StartVote(string message, List<string> options, IVoteResultListener listener)
        {
            if (this.VoteInProgress)
            {
                return;
            }

            this.VoteInProgress = true;

            if (options == null || options.Count == 0)
            {
                this.voteOptions = new string[] { "Yes", "No" };
                this.boolVote = true;
            }
            else
            {
                this.voteOptions = options.ToArray();
                this.boolVote = false;
            }

            this.voteListener = listener;
            ChatHook.RegisterChatHook(this);

            this.timer = new System.Timers.Timer(20000);
            this.timer.Elapsed += this.VoteElapsed;
            this.timer.Enabled = true;

            string msg;
            foreach (var client in ConnectionManager.Instance.GetClients())
            {
                this.votingPool.Add(client.playerId, -1);
                client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, "[Vote]", false, "SevenMod", false));
                if (this.boolVote)
                {
                    msg = "Enter /yes or /no into chat to cast your vote";
                    client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, msg, "[Vote]", false, "SevenMod", false));
                }
                else
                {
                    msg = "Enter /# to cast your vote";
                    client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, msg, "[Vote]", false, "SevenMod", false));
                    for (var i = 0; i < this.voteOptions.Length; i++)
                    {
                        msg = $"/{i + 1}: {this.voteOptions[i]}";
                        client.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, msg, "[Option]", false, "SevenMod", false));
                    }
                }
            }
        }

        /// <inheritdoc/>
        public bool OnChatMessage(ClientInfo client, string message)
        {
            if (!this.VoteInProgress || !this.votingPool.ContainsKey(client.playerId))
            {
                return true;
            }

            var index = -1;
            if (this.boolVote)
            {
                switch (message.ToLower())
                {
                    case "/yes":
                        index = 0;
                        break;
                    case "/no":
                        index = 1;
                        break;
                }
            }
            else
            {
                switch (message)
                {
                    case "/1":
                        index = 0;
                        break;
                    case "/2":
                        index = 1;
                        break;
                    case "/3":
                        index = 2;
                        break;
                    case "/4":
                        index = 3;
                        break;
                }
            }

            if ((index > -1) && (index < this.voteOptions.Length))
            {
                this.votingPool[client.playerId] = index;
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            this.EndVote();
        }

        /// <summary>
        /// Called by the <see cref="timer"/> at the end of a vote.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="System.Timers.ElapsedEventArgs"/> object that contains
        /// the event data.</param>
        private void VoteElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            var counts = new int[this.voteOptions.Length];
            foreach (var vote in this.votingPool.Values)
            {
                if ((vote < 0) || (vote >= counts.Length))
                {
                    continue;
                }

                counts[vote]++;
            }

            float total = this.votingPool.Count;
            var percents = new float[this.voteOptions.Length];
            for (var i = 0; i < counts.Length; i++)
            {
                percents[i] = counts[i] / total;
            }

            this.voteListener.OnVoteEnd(this.voteOptions, counts, percents);
            this.EndVote();
        }

        /// <summary>
        /// Ends the current vote if one is in progress.
        /// </summary>
        private void EndVote()
        {
            if (this.timer != null)
            {
                this.timer.Dispose();
                this.timer = null;
            }

            ChatHook.UnregisterChatHook(this);

            this.voteOptions = null;
            this.voteListener = null;
            this.votingPool.Clear();

            this.VoteInProgress = false;
        }
    }
}
