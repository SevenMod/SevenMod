// <copyright file="AdminCommandEventArgs.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System;
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// Contains arguments for the <see cref="AdminCommand.Executed"/> event.
    /// </summary>
    public class AdminCommandEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdminCommandEventArgs"/> class.
        /// </summary>
        /// <param name="command">The <see cref="AdminCommand"/> object that raised the event.</param>
        /// <param name="arguments">The list of arguments supplied to the admin command.</param>
        /// <param name="client">The <see cref="ClientInfo"/> object representing the client that executed the admin command.</param>
        internal AdminCommandEventArgs(AdminCommand command, List<string> arguments, ClientInfo client)
        {
            this.Command = command;
            this.Arguments = arguments;
            this.Client = (client == null) ? null : new SMClient(client);
        }

        /// <summary>
        /// Gets the <see cref="AdminCommand"/> object representing the admin command that was executed.
        /// </summary>
        public AdminCommand Command { get; }

        /// <summary>
        /// Gets the list of arguments supplied to the admin command.
        /// </summary>
        public List<string> Arguments { get; }

        /// <summary>
        /// Gets the <see cref="SMClient"/> object representing the client that executed the admin command.
        /// </summary>
        public SMClient Client { get; }
    }
}
