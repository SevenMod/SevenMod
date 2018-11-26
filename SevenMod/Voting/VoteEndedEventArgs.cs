// <copyright file="VoteEndedEventArgs.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    using System;

    /// <summary>
    /// Contains the arguments for the <see cref="Vote.Ended"/> event.
    /// </summary>
    public class VoteEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoteEndedEventArgs"/> class.
        /// </summary>
        /// <param name="vote">The <see cref="Voting.Vote"/> object representing the vote that ended.</param>
        /// <param name="options">The answer options for the vote.</param>
        /// <param name="votes">The count of votes for each answer option.</param>
        /// <param name="percents">The percentage of eligible voters that voted for each answer option.</param>
        /// <param name="data">Data associated with this vote.</param>
        internal VoteEndedEventArgs(Vote vote, string[] options, int[] votes, float[] percents, object data)
        {
            this.Vote = vote;
            this.Options = options;
            this.Votes = votes;
            this.Percents = percents;
            this.Data = data;
        }

        /// <summary>
        /// Gets the <see cref="Voting.Vote"/> object representing the vote that ended.
        /// </summary>
        public Vote Vote { get; }

        /// <summary>
        /// Gets the answer options for the vote.
        /// </summary>
        public string[] Options { get; }

        /// <summary>
        /// Gets the count of votes for each answer option.
        /// </summary>
        public int[] Votes { get; }

        /// <summary>
        /// Gets the percentage of eligible voters that voted for each answer option.
        /// </summary>
        public float[] Percents { get; }

        /// <summary>
        /// Gets the data associated with this vote.
        /// </summary>
        public object Data { get; }
    }
}
