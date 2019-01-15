// <copyright file="VoteManager.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    using System;
    using SevenMod.ConVar;

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
        /// Gets the value of the ShowVoteProgress <see cref="ConVar"/>.
        /// </summary>
        internal static ConVarValue ShowVoteProgress { get; private set; }

        /// <summary>
        /// Creates a new instance of the <see cref="VoteBuilder"/> class for starting a vote.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="args">The arguments for the message.</param>
        /// <returns>The <see cref="VoteBuilder"/> object.</returns>
        public static VoteBuilder CreateVote(string message, params object[] args)
        {
            return new VoteBuilder(message, args);
        }

        /// <summary>
        /// Initializes the voting system.
        /// </summary>
        internal static void Init()
        {
            ShowVoteProgress = ConVarManager.CreateConVar(null, "ShowVoteProgress", "True", "Show votes in chat.").Value;
        }

        /// <summary>
        /// Starts a vote if one is not already in progress.
        /// </summary>
        /// <param name="builder">The <see cref="VoteBuilder"/> object with the vote information.</param>
        /// <returns><c>true</c> if the vote started; <c>false</c> if another vote is already started.</returns>
        internal static bool StartVote(VoteBuilder builder)
        {
            if (VoteInProgress)
            {
                return false;
            }

            VoteInProgress = true;
            CurrentVote = new Vote(builder);
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
