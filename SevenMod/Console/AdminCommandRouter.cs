// <copyright file="AdminCommandRouter.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Console
{
    using System.Collections.Generic;

    /// <summary>
    /// Routes the sm console command to the appropriate admin command.
    /// </summary>
    public class AdminCommandRouter : ConsoleCmdAbstract
    {
        /// <inheritdoc/>
        public override int DefaultPermissionLevel => 2000;

        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "accesses SevenMod admin commands";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (_params.Count < 1)
            {
                return;
            }

            AdminCommandManager.ExecuteCommand(_params[0], _params.GetRange(1, _params.Count - 1), _senderInfo);
        }
    }
}
