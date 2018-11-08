// <copyright file="AdminCmdAbstract.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Core
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents an admin command, a special console command that is managed by the mod, with
    /// built in permission checking.
    /// </summary>
    public abstract class AdminCmdAbstract : ConsoleCmdAbstract
    {
        /// <summary>
        /// The map of registered admin commands to their required <see cref="AdminFlags"/>.
        /// </summary>
        internal static readonly Dictionary<Type, AdminFlags> Registry = new Dictionary<Type, AdminFlags>();

        /// <inheritdoc/>
        public override int DefaultPermissionLevel => 2000;

        /// <summary>
        /// Executes the logic of the admin command. This is called after checking the calling
        /// client's permission.
        /// </summary>
        /// <param name="args">The list of arguments supplied by the client.</param>
        /// <param name="senderInfo">Information about the calling client.</param>
        public abstract void Exec(List<string> args, CommandSenderInfo senderInfo);

        /// <inheritdoc/>
        public override void Execute(List<string> args, CommandSenderInfo senderInfo)
        {
            if (!Registry.ContainsKey(this.GetType()))
            {
                this.ReplyToCommand(senderInfo, "Unknown command");
                return;
            }

            AdminFlags flags = Registry[this.GetType()];
            if ((flags == 0) || !AdminManager.CheckAccess(senderInfo.RemoteClientInfo, flags))
            {
                this.ReplyToCommand(senderInfo, "You do not have access to that command");
                return;
            }

            this.Exec(args, senderInfo);
        }

        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the
        /// client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="message">The message to send.</param>
        protected void ReplyToCommand(CommandSenderInfo senderInfo, string message)
        {
            if (ChatHook.ShouldReplyToChat(senderInfo.RemoteClientInfo))
            {
                senderInfo.RemoteClientInfo.SendPackage(new NetPackageGameMessage(EnumGameMessages.Chat, message, "[SM]", false, "SevenMod", false));
            }
            else
            {
                SdtdConsole.Instance.Output(message);
            }
        }
    }
}
