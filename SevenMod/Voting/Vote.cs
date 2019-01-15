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
    using SevenMod.Lang;
    using SevenMod.Logging;

    /// <summary>
    /// Represents a vote.
    /// </summary>
    public sealed class Vote : IDisposable
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
        /// <param name="builder">The <see cref="VoteBuilder"/> object with the vote information.</param>
        internal Vote(VoteBuilder builder)
        {
            if (builder.Options == null || builder.Options.Count == 0)
            {
                this.voteOptions = new string[] { "Yes", "No" };
                this.boolVote = true;
            }
            else
            {
                this.voteOptions = builder.Options.ToArray();
                this.boolVote = false;
            }

            this.data = builder.Data;

            ChatHook.ChatMessage += this.OnChatMessage;

            this.timer = new Timer(20000);
            this.timer.Elapsed += this.OnTimerElapsed;
            this.timer.Enabled = true;

            foreach (var client in ConnectionManager.Instance.Clients.List)
            {
                this.votingPool.Add(client.playerId, -1);
                ChatHelper.SendTo(client, "Vote question", Language.GetString(builder.Message, client, builder.MessageArgs));
                if (this.boolVote)
                {
                    ChatHelper.SendTo(client, "Vote Yes Or No");
                }
                else
                {
                    ChatHelper.SendTo(client, "Vote Number");
                    for (var i = 0; i < this.voteOptions.Length; i++)
                    {
                        ChatHelper.SendTo(client, "Vote option", i + 1, this.voteOptions[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Occurs when the vote ends.
        /// </summary>
        public event EventHandler<VoteEndedEventArgs> Ended;

        /// <summary>
        /// Occurs when the vote is cancelled.
        /// </summary>
        public event EventHandler Cancelled;

        /// <summary>
        /// Cancels the vote.
        /// </summary>
        public void Cancel()
        {
            this.OnVoteCancelled();
            this.EndVote();
        }

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
            if (!this.votingPool.ContainsKey(e.Client.PlayerId))
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
                if (VoteManager.ShowVoteProgress.AsBool)
                {
                    if (this.votingPool[e.Client.PlayerId] == -1)
                    {
                        ChatHelper.SendToAll("Voted For", e.Client, this.voteOptions[index]);
                    }
                    else if (this.votingPool[e.Client.PlayerId] != index)
                    {
                        ChatHelper.SendToAll("Changed Vote", e.Client, this.voteOptions[index]);
                    }
                }
                else
                {
                    ChatHelper.SendTo(e.Client.ClientInfo, "You Voted", this.voteOptions[index]);
                }

                this.votingPool[e.Client.PlayerId] = index;
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
                        if (d.Target is IPlugin p)
                        {
                            p.Container.SetFailState(e);
                        }
                        else
                        {
                            SMLog.Error(e);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raises the <see cref="Cancelled"/> event.
        /// </summary>
        private void OnVoteCancelled()
        {
            if (this.Cancelled != null)
            {
                foreach (EventHandler d in this.Cancelled.GetInvocationList())
                {
                    try
                    {
                        d.Invoke(this, EventArgs.Empty);
                    }
                    catch (HaltPluginException)
                    {
                    }
                    catch (Exception e)
                    {
                        if (d.Target is IPlugin p)
                        {
                            p.Container.SetFailState(e);
                        }
                        else
                        {
                            SMLog.Error(e);
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
