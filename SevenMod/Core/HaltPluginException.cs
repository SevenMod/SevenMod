// <copyright file="HaltPluginException.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;

    /// <summary>
    /// Thrown by plugins entering a fail state.
    /// </summary>
    [Serializable]
    internal class HaltPluginException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HaltPluginException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public HaltPluginException(string message)
            : base(message)
        {
        }
    }
}
