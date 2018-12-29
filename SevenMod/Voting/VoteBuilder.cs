// <copyright file="VoteBuilder.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Voting
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains the data for starting a vote.
    /// </summary>
    public class VoteBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VoteBuilder"/> class.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="args">The arguments for the message.</param>
        internal VoteBuilder(string message, params object[] args)
        {
            this.SetMessage(message, args);
        }

        /// <summary>
        /// Gets the vote prompt.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Gets the arguments for the vote prompt.
        /// </summary>
        public object[] MessageArgs { get; private set; }

        /// <summary>
        /// Gets or sets the answer options for this vote.
        /// </summary>
        public List<string> Options { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the vote.
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Sets the vote prompt.
        /// </summary>
        /// <param name="message">The vote prompt.</param>
        /// <param name="args">The arguments for the message.</param>
        /// <returns>This <see cref="VoteBuilder"/> object for chaining.</returns>
        public VoteBuilder SetMessage(string message, params object[] args)
        {
            this.Message = message;
            this.MessageArgs = args;

            return this;
        }

        /// <summary>
        /// Sets the answer options for this vote.
        /// </summary>
        /// <param name="options">The answer options for this vote.</param>
        /// <returns>This <see cref="VoteBuilder"/> object for chaining.</returns>
        public VoteBuilder SetOptions(List<string> options)
        {
            this.Options = options;

            return this;
        }

        /// <summary>
        /// Sets the data associated with the vote.
        /// </summary>
        /// <param name="data">The data associated with the vote.</param>
        /// <returns>This <see cref="VoteBuilder"/> object for chaining.</returns>
        public VoteBuilder SetData(object data)
        {
            this.Data = data;

            return this;
        }

        /// <summary>
        /// Starts this vote if a vote is not already in progress.
        /// </summary>
        /// <returns><c>true</c> if the vote started; <c>false</c> if another vote is already started.</returns>
        public bool Start()
        {
            return VoteManager.StartVote(this);
        }
    }
}
