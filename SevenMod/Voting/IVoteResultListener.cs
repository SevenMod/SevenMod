// <copyright file="IVoteResultListener.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    /// <summary>
    /// Represents a vote result listener.
    /// </summary>
    public interface IVoteResultListener
    {
        /// <summary>
        /// Called when the associated vote ends.
        /// </summary>
        /// <param name="options">The answer options for the vote.</param>
        /// <param name="votes">The count of votes for each option.</param>
        /// <param name="percents">The percentage of votes for each option.</param>
        void OnVoteEnd(string[] options, int[] votes, float[] percents);
    }
}
