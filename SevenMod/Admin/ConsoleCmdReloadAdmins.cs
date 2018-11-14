// <copyright file="AdminCmdReloadAdmins.cs" company="Steve Guidetti">
// Copyright (c) Steve Guidetti. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace SevenMod.Admin
{
    using System.Collections.Generic;
    using SevenMod.Core;

    /// <summary>
    /// Console command to reload the admin list.
    /// </summary>
    public class ConsoleCmdReloadAdmins : ConsoleCmdAbstract
    {
        /// <inheritdoc/>
        public override int DefaultPermissionLevel => 2000;

        /// <inheritdoc/>
        public override string[] GetCommands()
        {
            return new string[] { "sm_reloadadmins" };
        }

        /// <inheritdoc/>
        public override string GetDescription()
        {
            return "reloads the admin list";
        }

        /// <inheritdoc/>
        public override void Execute(List<string> _params, CommandSenderInfo _senderInfo)
        {
            if (!AdminManager.CheckAccess(_senderInfo.RemoteClientInfo, AdminFlags.Ban))
            {
                SdtdConsole.Instance.Output("[SM] You do not have access to that command");
                return;
            }

            AdminManager.RemoveAllAdmins();
            foreach (var plugin in PluginManager.ActivePlugins)
            {
                plugin.ReloadAdmins();
            }
        }
    }
}
