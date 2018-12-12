// <copyright file="VoteManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Manages the voting system.
    /// </summary>
    public class VoteManager
    {
        /// <summary>
        /// Gets a value indicating whether a vote is in progress.
        /// </summary>
        public static bool VoteInProgress { get; private set; }

        /// <summary>
        /// Gets the <see cref="Vote"/> object representing the currently running vote.
        /// </summary>
        public static Vote CurrentVote { get; private set; }

        /// <summary>
        /// Starts a vote if one is not already in progress.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="options">The list of answer options; <c>null</c> for a boolean yes/no vote.</param>
        /// <param name="data">Optional data to associate with the vote.</param>
        /// <returns><c>true</c> if the vote started; <c>false</c> if another vote is already started.</returns>
        public static bool StartVote(string message, List<string> options = null, object data = null)
        {
            if (VoteInProgress)
            {
                return false;
            }

            VoteInProgress = true;
            CurrentVote = new Vote(message, options, data);
            CurrentVote.Ended += VoteEnded;
            CurrentVote.Cancelled += VoteEnded;

            return true;
        }

        /// <summary>
        /// Called by <see cref="Vote"/> objects when voting ends.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">An <see cref="EventArgs"/> object containing the event data.</param>
        private static void VoteEnded(object sender, EventArgs e)
        {
            CurrentVote.Dispose();
            CurrentVote = null;
            VoteInProgress = false;
        }
    }
}
