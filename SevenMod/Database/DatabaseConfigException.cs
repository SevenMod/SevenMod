// <copyright file="DatabaseConfigException.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Database
{
    using System;

    /// <summary>
    /// Represents an exception relating to a named database configuration.
    /// </summary>
    public class DatabaseConfigException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseConfigException"/> class.
        /// </summary>
        /// <param name="name">The name of the database configuration.</param>
        internal DatabaseConfigException(string name)
            : base($"Failed to load the \"{name}\" database configuration.")
        {
        }
    }
}
