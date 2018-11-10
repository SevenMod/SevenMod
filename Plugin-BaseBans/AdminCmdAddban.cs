// <copyright file="AdminCmdAddban.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Plugin.BaseBans
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// <para>Admin Command: sm_addban</para>
    /// <para>Adds a player ID to the server ban list.</para>
    /// </summary>
    public class AdminCmdAddban : AdminCmdAbstract
    {
        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_addban" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "adds a player ID to the server ban list";
        }

        /// <inheritdoc/>
        public override void Exec(List<string> args, CommandSenderInfo senderInfo)
        {
            if (args.Count < 1)
            {
                this.ReplyToCommand(senderInfo, "Not enough parameters");
                return;
            }

            if (!ConsoleHelper.ParseParamSteamIdValid(args[0]))
            {
                this.ReplyToCommand(senderInfo, "Invalid player ID");
                return;
            }

            SdtdConsole.Instance.ExecuteSync($"ban add {args[0]}", null);
        }
    }
}
