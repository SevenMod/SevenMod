// <copyright file="Vote.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    using System;
    using System.Collections.Generic;
    using System.Timers;
    using SevenMod.Chat;
    using SevenMod.Core;

    /// <summary>
    /// Represents a vote.
    /// </summary>
    public class Vote : IDisposable
    {
        /// <summary>
        /// The answer options for this vote.
        /// </summary>
        private readonly string[] voteOptions;

        /// <summary>
        /// The data associated with this vote.
        /// </summary>
        private readonly object data;

        /// <summary>
        /// A value indicating whether this is a yes/no vote.
        /// </summary>
        private readonly bool boolVote;

        /// <summary>
        /// The map of eligible voter auth IDs to their selected answer option index.
        /// </summary>
        private readonly Dictionary<string, int> votingPool = new Dictionary<string, int>();

        /// <summary>
        /// The timer for this vote.
        /// </summary>
        private readonly Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vote"/> class.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="options">The list of answer options; <c>null</c> for a boolean yes/no vote.</param>
        /// <param name="data">Data to associate with this vote.</param>
        internal Vote(string message, List<string> options, object data)
        {
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

            this.data = data;

            ChatHook.ChatMessage += this.OnChatMessage;

            this.timer = new Timer(20000);
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.Enabled = true;

            string msg;
            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                this.votingPool.Add(client.playerId, -1);
                ChatHelper.SendTo(client, message, "Vote");
                if (this.boolVote)
                {
                    msg = "Enter /yes or /no into chat to cast your vote";
                    ChatHelper.SendTo(client, msg, "Vote");
                }
                else
                {
                    msg = "Enter /# to cast your vote";
                    ChatHelper.SendTo(client, msg, "Vote");
                    for (var i = 0; i < this.voteOptions.Length; i++)
                    {
                        msg = $"/{i + 1}: {this.voteOptions[i]}";
                        ChatHelper.SendTo(client, msg, "Option");
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the vote ends.
        /// </summary>
        public event EventHandler<VoteEndedEventArgs> Ended;

        /// <inheritdoc/>
        public void Dispose()
        {
            this.EndVote();
        }

        /// <summary>
        /// Called when a chat message is received from a client.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">A <see cref="ChatMessageEventArgs"/> object containing the event data.</param>
        private void OnChatMessage(object sender, ChatMessageEventArgs e)
        {
            if (!this.votingPool.ContainsKey(e.Client.playerId))
            {
                return;
            }

            var index = -1;
            if (this.boolVote)
            {
                switch (e.Message.ToLower())
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
                switch (e.Message)
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
                this.votingPool[e.Client.playerId] = index;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Called by the <see cref="timer"/> at the end of a vote.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="ElapsedEventArgs"/> object that contains the event data.</param>
        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
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

            this.OnVoteEnded(counts, percents);
            this.EndVote();
        }

        /// <summary>
        /// Raises the <see cref="Ended"/> event.
        /// </summary>
        /// <param name="counts">The count of votes for each answer option.</param>
        /// <param name="percents">The percentage of eligible voters that voted for each answer option.</param>
        private void OnVoteEnded(int[] counts, float[] percents)
        {
            if (this.Ended != null)
            {
                var args = new VoteEndedEventArgs(this, this.voteOptions, counts, percents, this.data);
                foreach (EventHandler<VoteEndedEventArgs> d in this.Ended.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(this, args);
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        if (d.Target is IPlugin)
                        {
                            (d.Target as IPlugin).Container.SetFailState(e.Message);
                        }
                        else
                        {
                            SMLog.Error(e.Message);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ends the current vote if one is in progress.
        /// </summary>
        private void EndVote()
        {
            this.timer.Dispose();
            ChatHook.ChatMessage -= this.OnChatMessage;
        }
    }
}
