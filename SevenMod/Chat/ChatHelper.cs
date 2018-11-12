// <copyright file="ChatHelper.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Chat
{
    /// <summary>
    /// Chat related utilities.
    /// </summary>
    public class ChatHelper
    {
        /// <summary>
        /// Sends a response to a client in the console or chat depending on which input method the
        /// client used to call the currently executing command.
        /// </summary>
        /// <param name="senderInfo">The calling client information.</param>
        /// <param name="message">The message to send.</param>
        public static void ReplyToCommand(CommandSenderInfo senderInfo, string message)
        {
            if ((senderInfo.RemoteClientInfo != null) && ChatHook.ShouldReplyToChat(senderInfo.RemoteClientInfo))
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
